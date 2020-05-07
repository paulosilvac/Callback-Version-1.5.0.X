using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public partial class ucRealtimeProcessInformation : UserControl
    {
        public delegate MemoryCounters GetMemoryCounters();
        public delegate PerformanceCountersSummary GetPerformanceCountersSummary();

        private GetMemoryCounters _GetMemoryCounters = null;
        private GetPerformanceCountersSummary _GetPerformanceCountersSummary = null;

        public ucRealtimeProcessInformation()
        {
            InitializeComponent();
        }

        private void ucRealtimeProcessInformation_Load(object sender, EventArgs e)
        {
            ResetMemoryCounterLabels();
        }

        public void SetCounterDelegates(GetMemoryCounters GetMemoryCounters, GetPerformanceCountersSummary GetPerformanceCountersSummary)
        {
            _GetMemoryCounters = GetMemoryCounters;
            _GetPerformanceCountersSummary = GetPerformanceCountersSummary;
        }

        public bool ResetMemoryCounterLabels()
        {
            lblStartTime.Text = "";
            lblTotalProcessorTime.Text = "0 secs";
            lblThreads.Text = "0";
            lblMemoryUsed.Text = "0 MB";
            lblVirtualMemory.Text = "0 MB";
            lblWorkingSet.Text = "0 MB";

            lblTotalRequests.Text = "0";
            lblRequestsHandled.Text = "0";
            lblRequestsFailed.Text = "0";
            lblAvgProcessingTime.Text = "0 ms";
            lblRequestQueueSize.Text = "0";

            return true;
        }

        private void tmrRefreshMemoryLabels_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_GetMemoryCounters == null && _GetPerformanceCountersSummary == null)
                {
                    Trace.TraceWarning("_GetMemoryCounters and _GetPerformanceCountersSummary delegates are null");
                    return;
                }

                if (_GetMemoryCounters != null)
                {
                    MemoryCounters mc = _GetMemoryCounters.Invoke();

                    if (mc != null)
                    {
                        try
                        {
                            lblStartTime.Text = mc.StartTime.ToString();
                        }
                        catch
                        {
                            lblStartTime.Text = string.Empty;
                        }

                        try
                        {
                            lblTotalProcessorTime.Text = String.Format("{0:0.00}",mc.TotalProcessorTime.TotalSeconds) + " secs";
                        }
                        catch
                        {
                            lblTotalProcessorTime.Text = "0 secs";
                        }

                        try
                        {
                            lblThreads.Text = mc.Threads.ToString();
                        }
                        catch
                        {
                            lblThreads.Text = "0";
                        }

                        try
                        {
                            lblMemoryUsed.Text = (mc.MemoryUsed / (1024 * 1024)).ToString() + " MB";
                        }
                        catch
                        {
                            lblMemoryUsed.Text = "0 MB";
                        }

                        try
                        {
                            lblVirtualMemory.Text = (mc.VirtualMemory / (1024 * 1024)).ToString() + " MB";
                        }
                        catch
                        {
                            lblVirtualMemory.Text = "0 MB";
                        }

                        try
                        {
                            lblWorkingSet.Text = (mc.WorkingSet / (1024 * 1024)).ToString() + " MB";
                        }
                        catch
                        {
                            lblWorkingSet.Text = "0 MB";
                        }
                    }
                    else
                    {
                        Trace.TraceWarning("_MemoryCounters is null");
                    }
                }
                else
                {
                    Trace.TraceWarning("_GetMemoryCounters delegate is null");
                }                

                if (_GetPerformanceCountersSummary != null)
                {
                    PerformanceCountersSummary _PerformanceCountersSummary = _GetPerformanceCountersSummary.Invoke();

                    if (_PerformanceCountersSummary != null)
                    {
                        try
                        {
                            lblTotalRequests.Text = _PerformanceCountersSummary.TotalRequests.ToString();
                        }
                        catch
                        {
                            lblTotalRequests.Text = "0";
                        }

                        try
                        {
                            lblRequestsHandled.Text = _PerformanceCountersSummary.TotalRequestsHandled.ToString();
                        }
                        catch
                        {
                            lblRequestsHandled.Text = "0";
                        }

                        try
                        {
                            lblRequestsFailed.Text = _PerformanceCountersSummary.TotalRequestsFailed.ToString();
                        }
                        catch
                        {
                            lblRequestsFailed.Text = "0";
                        }

                        try
                        {
                            lblAvgProcessingTime.Text = String.Format("{0:0}", _PerformanceCountersSummary.AvgProcessingTime) + " ms";
                        }
                        catch
                        {
                            lblAvgProcessingTime.Text = "0 ms";
                        }

                        try
                        {
                            lblRequestQueueSize.Text = String.Format("{0:0}", _PerformanceCountersSummary.CurrentRequests);
                        }
                        catch
                        {
                            lblRequestQueueSize.Text = "0";
                        }
                    }
                    else
                    {
                        Trace.TraceWarning("_PerformanceCountersSummary is null");
                    }
                }
                else
                {
                    Trace.TraceWarning("_GetPerformanceCountersSummary delegate is null");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                ResetMemoryCounterLabels();
            }
        }
    }
}
