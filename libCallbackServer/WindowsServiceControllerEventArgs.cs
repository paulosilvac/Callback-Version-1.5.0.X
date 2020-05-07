using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    public class WindowsServiceControllerEventArgs : EventArgs
    {
        private System.ServiceProcess.ServiceControllerStatus _CurrentStatus = System.ServiceProcess.ServiceControllerStatus.Stopped;

        public System.ServiceProcess.ServiceControllerStatus CurrentStatus
        {
            get { return _CurrentStatus; }
        }

        public WindowsServiceControllerEventArgs(System.ServiceProcess.ServiceControllerStatus CurrentStatus)
        {
            _CurrentStatus = CurrentStatus;
        }
    }
}
