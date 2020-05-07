using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class WebServer
    {
        public event EventHandler Started;
        public event EventHandler Stopped;
        public event EventHandler Aborted;
        public event EventHandler<WebServerEventArgs> Error;

        private string _WebServerIP = string.Empty;
        private string _WebServerPort = string.Empty;
        private string _WebServerURIPrefix = string.Empty;
        private AsyncCallback _Callback = null;

        private System.Threading.Thread _thrServer = null;

        private System.Threading.AutoResetEvent _Abort = new System.Threading.AutoResetEvent(false);

        public string WebServerIP
        {
            get { return _WebServerIP; }
            set { _WebServerIP = value; }
        }

        public string WebServerPort
        {
            get { return _WebServerPort; }
            set { _WebServerPort = value; }
        }

        public string WebServerURIPrefix
        {
            get { return _WebServerURIPrefix; }
            set { _WebServerURIPrefix = value; }
        }

        public AsyncCallback Callback
        {
            get { return _Callback; }
            set { _Callback = value; }
        }

        public bool IsRunning
        {
            get
            {
                if (_thrServer == null)
                {
                    return false;
                }

                if (_thrServer.ThreadState != System.Threading.ThreadState.Stopped ||
                    _thrServer.ThreadState != System.Threading.ThreadState.StopRequested ||
                    _thrServer.ThreadState != System.Threading.ThreadState.Unstarted)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Start()
        {
            Trace.TraceInformation("Enter.");

            if (_thrServer != null)
            {
                Trace.TraceWarning("Server is already running.");
                return false;
            }

            if (_WebServerIP == string.Empty)
            {
                Trace.TraceWarning("_WebServerIP is empty.");
                return false;
            }

            if (_WebServerPort == string.Empty)
            {
                Trace.TraceWarning("_WebServerPort is empty.");
                return false;
            }

            if (_WebServerURIPrefix == string.Empty)
            {
                Trace.TraceWarning("_WebServerURIPrefix is empty.");
                return false;
            }

            if (_Callback == null)
            {
                Trace.TraceWarning("_Callback has not been set.");
                return false;
            }

            _thrServer = new System.Threading.Thread(_Server);
            _thrServer.Start();

            Trace.TraceInformation("Server started.");

            return true;
        }

        public bool Stop()
        {
            Trace.TraceInformation("Enter.");

            if (_thrServer == null)
            {
                Trace.TraceWarning("Server is already stopped.");
                return false;
            }

            Trace.TraceInformation("Web Server thread state = " + _thrServer.ThreadState.ToString());

            if (_thrServer.ThreadState != System.Threading.ThreadState.Stopped ||
                _thrServer.ThreadState != System.Threading.ThreadState.StopRequested ||
                _thrServer.ThreadState != System.Threading.ThreadState.Unstarted)
            {
                _thrServer.Abort();
                Trace.TraceInformation("Abort was called on thread...");
                _Abort.WaitOne();
                Trace.TraceInformation("Thread has aborted.");
                _thrServer = null;
                return true;
            }
            else
            {
                Trace.TraceWarning("Thread is in an unexpected state; abort cannot be called.");
                return false;
            }
        }

        private void _Server()
        {
            Trace.TraceInformation("Enter.");

            System.Net.HttpListener _Listener = null;
            string sResponse = string.Empty;

            try
            {
                _Listener = new System.Net.HttpListener();
                _Listener.Prefixes.Add("http://" + _WebServerIP + ":" + _WebServerPort + "/" + _WebServerURIPrefix + "/");
                _Listener.Start();

                Trace.TraceInformation("Listener started for prefix " + "http://" + _WebServerIP + ":" + _WebServerPort + "/" + _WebServerURIPrefix + "/");

                if (Started != null)
                {
                    Started(this, new EventArgs());
                }

                do
                {
                    Trace.TraceInformation("Listening for request to be processed asyncronously.");

                    IAsyncResult result = _Listener.BeginGetContext(_Callback, _Listener);

                    Trace.TraceInformation("Waiting for request to be processed asyncronously.");

                    result.AsyncWaitHandle.WaitOne();

                    Trace.TraceInformation("Request processed asyncronously.");

                } while (true);
            }
            catch (System.Threading.ThreadAbortException abortEx)
            {
                Trace.TraceError("ThreadAbortException:" + abortEx.Message + Environment.NewLine + "StackTrace:" + abortEx.StackTrace);

                System.Threading.Thread.ResetAbort();

                _Abort.Set();

                if (Aborted != null)
                {
                    Aborted(this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                _Abort.Set();

                if (Error != null)
                {
                    Error(this, new WebServerEventArgs("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace));
                }
            }
            finally
            {
                if (_Listener != null)
                {
                    if (_Listener.IsListening)
                    {
                        _Listener.Stop();
                    }
                }

                _Listener = null;
            }

            if (Stopped != null)
            {
                Stopped(this, new EventArgs());
            }
        }
    }
}
