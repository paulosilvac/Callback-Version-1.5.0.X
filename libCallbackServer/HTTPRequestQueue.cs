using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class HTTPRequestQueue
    {
        public event EventHandler NewRequest;

        Queue<System.Net.HttpListenerContext> _Requests = null;

        object _lockRequestQueue = new object();

        public int Size
        {
            get
            {
                lock (_lockRequestQueue)
                {
                    return _Requests.Count;
                }
            }
        }

        public HTTPRequestQueue()
        {
            _Requests = new Queue<System.Net.HttpListenerContext>();
        }

        public void WebServerCallback(IAsyncResult result)
        {
            Trace.TraceInformation("Enter.");

            try
            {
                System.Net.HttpListener listener = (System.Net.HttpListener)result.AsyncState;

                // Call EndGetContext to complete the asynchronous operation.
                System.Net.HttpListenerContext context = listener.EndGetContext(result);

                EnqueueRequest(context);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
            }

            Trace.TraceInformation("Current thread ID: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
        }

        public System.Net.HttpListenerContext GetNextRequest()
        {
            //Trace.TraceInformation("Enter.");

            lock (_lockRequestQueue)
            {
                try
                {
                    if (_Requests != null)
                    {
                        if (_Requests.Count != 0)
                        {
                            //Trace.TraceInformation("Request dequeued.");
                            return _Requests.Dequeue();
                        }
                        else
                        {
                            //Trace.TraceWarning("_Requests queue is empty.");
                            return null;
                        }
                    }
                    else
                    {
                        //Trace.TraceWarning("_Requests queue is null.");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                    return null;
                }
            }
        }

        private void EnqueueRequest(System.Net.HttpListenerContext Context)
        {
            lock (_lockRequestQueue)
            {
                _Requests.Enqueue(Context);

                Trace.TraceInformation("Request queue size: " + _Requests.Count.ToString());

                if (NewRequest != null)
                {
                    NewRequest(this, new EventArgs());
                }
            }
        }
    }
}
