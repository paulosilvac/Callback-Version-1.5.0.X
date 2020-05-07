using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    [Serializable]
    public class MemoryCounters
    {
        long _MemoryUsed = 0;
        long _WorkingSet = 0;
        long _PeakWorkingSet = 0;
        long _VirtualMemory = 0;
        long _PeakVirtualMemory = 0;
        long _PagedMemory = 0;
        long _PeakPagedMemory = 0;
        long _Threads = 0;
        DateTime _StartTime = DateTime.MinValue;
        TimeSpan _TotalProcessorTime = new TimeSpan(0);

        public long MemoryUsed
        {
            get { return _MemoryUsed; }
        }

        public long WorkingSet
        {
            get { return _WorkingSet; }
        }

        public long PeakWorkingSet
        {
            get { return PeakWorkingSet; }
        }

        public long VirtualMemory
        {
            get { return _VirtualMemory; }
        }

        public long PeakVirtualMemory
        {
            get { return _PeakVirtualMemory; }
        }

        public long PagedMemory
        {
            get { return _PagedMemory; }
        }

        public long PeakPagedMemory
        {
            get { return PeakPagedMemory; }
        }

        public long Threads
        {
            get { return _Threads; }
        }

        public DateTime StartTime
        {
            get { return _StartTime; }
        }

        public TimeSpan TotalProcessorTime
        {
            get { return _TotalProcessorTime; }
        }

        public MemoryCounters()
        {
            try
            {
                _MemoryUsed = System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64; ;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                _MemoryUsed = 0;
            }

            try
            {
                _WorkingSet = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                _WorkingSet = 0;
            }

            try
            {
                _PeakWorkingSet = System.Diagnostics.Process.GetCurrentProcess().PeakWorkingSet64;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                _PeakWorkingSet = 0;
            }

            try
            {
                _VirtualMemory = System.Diagnostics.Process.GetCurrentProcess().VirtualMemorySize64;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                _VirtualMemory = 0;
            }

            try
            {
                _PeakVirtualMemory = System.Diagnostics.Process.GetCurrentProcess().PeakVirtualMemorySize64;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                _PeakVirtualMemory = 0;
            }

            try
            {
                _PagedMemory = System.Diagnostics.Process.GetCurrentProcess().PagedMemorySize64;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                _PagedMemory = 0;
            }

            try
            {
                _PeakPagedMemory = System.Diagnostics.Process.GetCurrentProcess().PeakPagedMemorySize64;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                _PeakPagedMemory = 0;
            }

            try
            {
                _Threads = System.Diagnostics.Process.GetCurrentProcess().Threads.Count;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                _Threads = 0;
            }

            try
            {
                _StartTime = System.Diagnostics.Process.GetCurrentProcess().StartTime;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                _StartTime = DateTime.MinValue;
            }

            try
            {
                _TotalProcessorTime = System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                _TotalProcessorTime = new TimeSpan(0);
            }
        }
    }
}
