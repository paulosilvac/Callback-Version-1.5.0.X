using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class RealtimeReportsRecordsByCSQ
    {
        private int iContactsWaiting = 0;

        private double dLongestQueueTime = 0;
        private double dTotalQueueTime = 0;

        public int ContactsWaiting
        {
            get { return iContactsWaiting; }
        }

        public double LongestQueueTime
        {
            get { return Math.Round(dLongestQueueTime); }
        }

        public double AvgQueueTime
        {
            get
            {
                if(iContactsWaiting == 0)
                {
                    return 0;
                }
                else
                {
                    return Math.Round(dTotalQueueTime / iContactsWaiting);
                }
            }
        }

        public RealtimeReportsRecordsByCSQ(CallbackRecord record)
        {
            iContactsWaiting = 1;
            dLongestQueueTime = DateTime.Now.Subtract(record.RequestDate).TotalSeconds;
            dTotalQueueTime = dLongestQueueTime;
        }

        public void AddContact(CallbackRecord record)
        {
            if(record == null)
            {
                return;
            }

            iContactsWaiting++;

            double lQueueTime = DateTime.Now.Subtract(record.RequestDate).TotalSeconds;

            if(lQueueTime > dLongestQueueTime)
            {
                dLongestQueueTime = lQueueTime;
            }

            dTotalQueueTime = dTotalQueueTime + lQueueTime;
        }
    }
}
