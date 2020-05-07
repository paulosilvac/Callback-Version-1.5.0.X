using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class PerformanceCounters
    {
        int _TotalRequests = 0;
        int _TotalRequestsHandled = 0;
        int _TotalRequestsFailed = 0;
        double _TotalProcessingTime = 0;
        int _CurrentRequests = 0;

        private int TotalRequests
        {
            get
            {
                lock (this)
                {
                    return _TotalRequests;
                }
            }
        }

        private int TotalRequestsHandled
        {
            get
            {
                lock (this)
                {
                    return _TotalRequestsHandled;
                }
            }
        }

        private int TotalRequestsFailed
        {
            get
            {
                lock (this)
                {
                    return _TotalRequestsFailed;
                }
            }
        }

        private double TotalProcessingTime
        {
            get
            {
                lock (this)
                {
                    return _TotalProcessingTime;
                }
            }
        }

        private int CurrentRequests
        {
            get
            {
                lock (this)
                {
                    return _CurrentRequests;
                }
            }
        }

        public PerformanceCounters()
        {
            _TotalRequests = 0;
            _TotalRequestsHandled = 0;
            _TotalRequestsFailed = 0;
            _TotalProcessingTime = 0;
            _CurrentRequests = 0;
        }

        public void AddRequest()
        {
            lock (this)
            {
                if (_TotalRequests == (int.MaxValue - 100) || _TotalRequestsHandled == (int.MaxValue - 100) || _TotalRequestsFailed == (int.MaxValue - 100) || _TotalProcessingTime == (double.MaxValue - 1000))
                {
                    _TotalRequests = 0;
                    _TotalRequestsHandled = 0;
                    _TotalRequestsFailed = 0;
                    _TotalProcessingTime = 0;
                }

                _TotalRequests++;
                _CurrentRequests++;
            }
        }

        public void AddRequestHandled(double ProcessingTime)
        {
            lock (this)
            {
                if (_TotalRequests == (int.MaxValue - 100) || _TotalRequestsHandled == (int.MaxValue - 100) || _TotalRequestsFailed == (int.MaxValue - 100) || _TotalProcessingTime == (double.MaxValue - 1000))
                {
                    _TotalRequests = 0;
                    _TotalRequestsHandled = 0;
                    _TotalRequestsFailed = 0;
                    _TotalProcessingTime = 0;
                }

                _TotalRequestsHandled++;
                _CurrentRequests--;
                _TotalProcessingTime = _TotalProcessingTime + ProcessingTime;
            }
        }

        public void AddRequestFailed(double ProcessingTime)
        {
            lock (this)
            {
                //if (_TotalRequests == (int.MaxValue - 100) || _TotalRequestsHandled == (int.MaxValue - 100) || _TotalRequestsFailed == (int.MaxValue - 100) || _TotalProcessingTime == (double.MaxValue - 1000))
                if (_TotalRequests == (int.MaxValue - 100) || _TotalRequestsHandled == (int.MaxValue - 100) || _TotalRequestsFailed == (int.MaxValue - 100))
                {
                    _TotalRequests = 0;
                    _TotalRequestsHandled = 0;
                    _TotalRequestsFailed = 0;
                    //_TotalProcessingTime = 0;
                }

                _TotalRequestsFailed++;
                _CurrentRequests--;
                //_TotalProcessingTime = _TotalProcessingTime + ProcessingTime;
            }
        }

        public PerformanceCountersSummary GetSummary()
        {
            try
            {
                int __TotalRequests = TotalRequests;
                int __TotalRequestsHandled = TotalRequestsHandled;
                int __TotalRequestsFailed = TotalRequestsFailed;
                double __TotalProcessingTime = TotalProcessingTime;
                int __AvgProcessingTime = 0;
                int __CurrentRequests = CurrentRequests;

                try
                {
                    if (__TotalRequestsHandled > 0)
                    {
                        __AvgProcessingTime = (int)(__TotalProcessingTime / __TotalRequestsHandled);
                    }
                    else
                    {
                        __AvgProcessingTime = 0;
                    }
                }
                catch
                {
                    __AvgProcessingTime = 0;
                }

                return new PerformanceCountersSummary(__TotalRequests, __TotalRequestsHandled, __TotalRequestsFailed, __AvgProcessingTime, __CurrentRequests);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return new PerformanceCountersSummary();
            }
        }
    }
}
