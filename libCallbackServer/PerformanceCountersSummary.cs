using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    [Serializable]
    public class PerformanceCountersSummary
    {
        int _TotalRequests = 0;
        int _TotalRequestsHandled = 0;
        int _TotalRequestsFailed = 0;
        int _AvgProcessingTime = 0;
        int _CurrentRequests = 0;

        public PerformanceCountersSummary()
        {
            _TotalRequests = 0;
            _TotalRequestsHandled = 0;
            _TotalRequestsFailed = 0;
            _AvgProcessingTime = 0;
        }

        public int TotalRequests
        {
            get { return _TotalRequests; }
        }

        public int TotalRequestsHandled
        {
            get { return _TotalRequestsHandled; }
        }

        public int TotalRequestsFailed
        {
            get { return _TotalRequestsFailed; }
        }

        public int AvgProcessingTime
        {
            get { return _AvgProcessingTime; }
        }

        public int CurrentRequests
        {
            get { return _CurrentRequests; }
        }

        public PerformanceCountersSummary(int TotalRequests, int TotalRequestsHandled, int TotalRequestsFailed, int AvgProcessingTime, int CurrentRequests)
        {
            _TotalRequests = TotalRequests;
            _TotalRequestsHandled = TotalRequestsHandled;
            _TotalRequestsFailed = TotalRequestsFailed;
            _AvgProcessingTime = AvgProcessingTime;
            _CurrentRequests = CurrentRequests;
        }
    }

}
