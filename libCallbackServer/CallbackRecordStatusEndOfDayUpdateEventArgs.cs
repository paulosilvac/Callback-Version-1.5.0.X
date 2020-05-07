using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    public class CallbackRecordStatusEndOfDayUpdateEventArgs:EventArgs
    {
        List<CallbackRecord> _Records = null;

        public List<CallbackRecord> Records
        {
            get { return _Records; }
            set { _Records = value; }
        }

        public CallbackRecordStatusEndOfDayUpdateEventArgs()
        {
            _Records = null;
        }

        public CallbackRecordStatusEndOfDayUpdateEventArgs(List<CallbackRecord> Records)
        {
            _Records = Records;
        }
    }
}
