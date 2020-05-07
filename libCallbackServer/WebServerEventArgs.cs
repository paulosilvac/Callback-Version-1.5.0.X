using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    public class WebServerEventArgs : EventArgs
    {
        private string _Message = string.Empty;

        public string Message
        {
            get { return _Message; }
        }

        public WebServerEventArgs(string Message)
        {
            _Message = Message;
        }
    }
}
