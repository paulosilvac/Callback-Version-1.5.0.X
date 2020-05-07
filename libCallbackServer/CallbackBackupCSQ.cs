using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    public class CallbackBackupCSQ
    {
        String _Name = String.Empty;
        int _OverflowTime = 0;

        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public int OverflowTime
        {
            get { return _OverflowTime; }
            set { _OverflowTime = value; }
        }

        public CallbackBackupCSQ()
        {
            _Name = String.Empty;
            _OverflowTime = 0;
        }
    }
}
