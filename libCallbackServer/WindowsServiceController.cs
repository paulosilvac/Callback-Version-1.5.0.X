using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class WindowsServiceController
    {
        private ServiceController _ctrl;
        private string _errorMsg = "";
        private System.Threading.Timer thrRefresh = null;
        private const int REFRESH_INTERVAL = 1000;

        public string ErrorMessage
        {
            get { return _errorMsg; }
            set { _errorMsg = value; }
        }

        public WindowsServiceController(string ServiceName)
        {
            try
            {
                foreach (ServiceController sc in ServiceController.GetServices())
                {
                    if (sc.DisplayName == ServiceName)
                    {
                        _ctrl = new ServiceController(ServiceName);

                        thrRefresh = new System.Threading.Timer(new System.Threading.TimerCallback(thrRefresh_Tick));
                        thrRefresh.Change(REFRESH_INTERVAL, REFRESH_INTERVAL);

                        return;
                    }
                }

                throw new Exception("Service not found.");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "Stack Trace:" + ex.StackTrace);

                _ctrl = null;

                throw ex;
            }
        }

        public void Dispose()
        {
            //Not implemented
        }

        public ServiceControllerStatus Status()
        {
            return _ctrl.Status;
        }

        public bool Start()
        {
            Trace.TraceInformation("Enter.");

            return this._Start(string.Empty, string.Empty, string.Empty, null);
        }

        public bool Start(string Domain, string Username, string Password)
        {
            Trace.TraceInformation("Enter.");

            return this._Start(Domain, Username, Password, null);
        }

        public bool Start(string Domain, string Username, string Password, string Parameters)
        {
            Trace.TraceInformation("Enter.");

            string[] args = Parameters.Split(' ');

            return this._Start(Domain, Username, Password, args);
        }

        private bool _Start(string Domain, string Username, string Password, string[] Args)
        {
            Trace.TraceInformation("Enter.");

            try
            {
                if (Args != null)
                {
                    _ctrl.Start(Args);
                    _ctrl.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(60));
                }
                else
                {
                    _ctrl.Start();
                    _ctrl.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(60));
                }

                return true;
            }
            catch (Exception ex)
            {
                _errorMsg = ex.Message;

                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                return false;
            }
        }

        public bool Stop()
        {
            Trace.TraceInformation("Enter.");

            return this._Stop(string.Empty, string.Empty, string.Empty);
        }

        public bool Stop(string Domain, string Username, string Password)
        {
            Trace.TraceInformation("Enter.");

            return this._Stop(Domain, Username, Password);
        }

        private bool _Stop(string Domain, string Username, string Password)
        {
            Trace.TraceInformation("Enter.");

            try
            {
                if (_ctrl.CanStop)
                {
                    _ctrl.Stop();

                    _ctrl.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(60));

                    return true;
                }
                else
                {
                    Trace.TraceWarning("This service can not be stopped.");

                    return false;
                }
            }
            catch (Exception ex)
            {
                _errorMsg = ex.Message;

                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                return false;
            }
        }

        private void thrRefresh_Tick(object state)
        {
            lock (this)
            {
                if (_ctrl != null)
                {
                    _ctrl.Refresh();
                }
                else
                {
                    Trace.TraceInformation("Refreshing....null");
                }
            }
        }
    }
}
