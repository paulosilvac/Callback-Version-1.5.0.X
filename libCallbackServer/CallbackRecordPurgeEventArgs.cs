using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    public class CallbackRecordPurgeEventArgs:EventArgs
    {
        List<CallbackRecord> _PurgedRecords = null;

        public List<CallbackRecord> PurgedRecords
        {
            get { return _PurgedRecords; }
            set { _PurgedRecords = value; }
        }

        public CallbackRecordPurgeEventArgs()
        {
            _PurgedRecords = null;
        }

        public CallbackRecordPurgeEventArgs(List<CallbackRecord> PurgedRecords)
        {
            _PurgedRecords = PurgedRecords;
        }
    }
}
