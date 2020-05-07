using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    public class HTTPRequestHandlerEventArgs:EventArgs
    {
        private string _Message = string.Empty;
        private HTTPRequestHandler.HTTPRequestHandlerEventCodes _HTTPRequestHandlerEventCode = HTTPRequestHandler.HTTPRequestHandlerEventCodes.NONE;

        public string Message
        {
            get { return _Message; }
        }

        public HTTPRequestHandler.HTTPRequestHandlerEventCodes HTTPRequestHandlerEventCode
        {
            get { return _HTTPRequestHandlerEventCode; }
        }

        public HTTPRequestHandlerEventArgs(string Message, HTTPRequestHandler.HTTPRequestHandlerEventCodes HTTPRequestHandlerEventCode)
        {
            _Message = Message;
            _HTTPRequestHandlerEventCode = HTTPRequestHandlerEventCode;
        }
    }
}
