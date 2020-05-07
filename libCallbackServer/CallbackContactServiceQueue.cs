using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    public class CallbackContactServiceQueue
    {
        private bool _CallbackEnabled = false;
        private String _Name = String.Empty;

        private CallbackContactServiceQueueSettingsProfile _Profile = null;

        public bool CallbackEnabled
        {
            get { return _CallbackEnabled; }
            set { _CallbackEnabled = value; }
        }

        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public CallbackContactServiceQueueSettingsProfile Profile
        {
            get { return _Profile; }
            set { _Profile = value; }
        }

        public CallbackContactServiceQueue()
        {
            _CallbackEnabled = false;
            _Name = String.Empty;
            _Profile = null;
        }
    }
}
