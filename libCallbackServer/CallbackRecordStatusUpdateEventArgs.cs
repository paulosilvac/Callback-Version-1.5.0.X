using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    public class CallbackRecordStatusUpdateEventArgs:EventArgs
    {
        CallbackRecord _Record = null;

        public CallbackRecord Record
        {
            get { return _Record; }
            set { _Record = value; }
        }

        public CallbackRecordStatusUpdateEventArgs()
        {
            _Record = null;
        }

        public CallbackRecordStatusUpdateEventArgs(CallbackRecord Record)
        {
            _Record = Record;
        }
    }
}
