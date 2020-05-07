using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class HTTPRequestQueueManager
    {
        const int INTERVALMONITORHANDLERS = 5000;

        HTTPRequestQueue _Queue = null;
        System.Threading.AutoResetEvent _NewRequest = new System.Threading.AutoResetEvent(false);
        System.Collections.Generic.List<System.Threading.Thread> _Handlers = null;
        ApplicationTypes.iRequestHandler _RequestHandler;
        System.Threading.Timer _tmrMonitorHandlers = null;
        PerformanceCounters _PerformanceCounters = null;
        int _HandlerThreadsSleep = 0;

        object lock_Queue = new object();
        object lock_ThreadStats = new object();
        System.Collections.Generic.Dictionary<String, bool> _thrStats = null;

        public HTTPRequestQueueManager(HTTPRequestQueue Queue, ApplicationTypes.iRequestHandler Handler, PerformanceCounters PerformanceCounters)
        {
            _Queue = Queue;
            _RequestHandler = Handler;

            _PerformanceCounters = PerformanceCounters;

            _Queue.NewRequest += new EventHandler(_Queue_NewRequest);

            _Handlers = new List<System.Threading.Thread>();

            _thrStats = new Dictionary<string, bool>();

            _tmrMonitorHandlers = new System.Threading.Timer(_tmrMonitorHandlers_Tick);
        }

        public void Start(int NumberOfHandlerThreads, int HandlerThreadsSleep)
        {
            Trace.TraceInformation("Enter.");

            _HandlerThreadsSleep = HandlerThreadsSleep;

            for (int i = 0; i < NumberOfHandlerThreads; i++)
            {
                _Handlers.Add(new System.Threading.Thread(_Handler));
            }

            Trace.TraceInformation("Total Handlers: " + _Handlers.Count.ToString());

            int counter = 0;

            foreach (System.Threading.Thread thr in _Handlers)
            {
                thr.Name = "handler_thread_" + counter.ToString();


                if(!_thrStats.ContainsKey(thr.Name))
                {
                    _thrStats.Add(thr.Name, false);
                }

                counter++;
                thr.Start();
                System.Threading.Thread.Sleep(250);
            }

            _tmrMonitorHandlers.Change(INTERVALMONITORHANDLERS, INTERVALMONITORHANDLERS);

            Trace.TraceInformation("Exit.");
        }

        public void Stop()
        {
            Trace.TraceInformation("Enter.");

            _tmrMonitorHandlers.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            foreach (System.Threading.Thread thr in _Handlers)
            {
                thr.Abort();
                System.Threading.Thread.Sleep(100);
            }

            _Handlers.Clear();

            Trace.TraceInformation("Exit.");
        }

        public PerformanceCountersSummary GetPerformanceCounters()
        {
            return null;
        }

        private void _Handler()
        {
            Trace.TraceInformation("Enter.");
            Trace.TraceInformation(System.Threading.Thread.CurrentThread.Name + " started.");

            DateTime processingBeganAt = DateTime.MinValue;
            double processingTime = 0;

            try
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(_HandlerThreadsSleep);

                    System.Net.HttpListenerContext request = _Queue.GetNextRequest();
                    
                    if (request != null)
                    {
                        UpdateThreadState(System.Threading.Thread.CurrentThread.Name, true);

                        //lock (lock_Queue)
                        //{
                            Guid RequestID = Guid.NewGuid();

                            processingBeganAt = DateTime.Now;

                            if (_RequestHandler.Handle(request, RequestID))
                            {
                                processingTime = DateTime.Now.Subtract(processingBeganAt).TotalMilliseconds;

                                _PerformanceCounters.AddRequestHandled(processingTime);

                                Trace.TraceInformation("Request {" + RequestID.ToString() + "} succeeded; handled by " + System.Threading.Thread.CurrentThread.Name + " in " + processingTime.ToString() + " ms.");
                            }
                            else
                            {
                                processingTime = DateTime.Now.Subtract(processingBeganAt).TotalMilliseconds;

                                _PerformanceCounters.AddRequestFailed(processingTime);
                                Trace.TraceWarning("Request {" + RequestID.ToString() + "} failed; handled by " + System.Threading.Thread.CurrentThread.Name + " in " + processingTime.ToString() + " ms.");
                            }

                        //}//lock (lock_Queue)
                    }
                    else
                    {
                        UpdateThreadState(System.Threading.Thread.CurrentThread.Name, false);

                        //Trace.TraceInformation("No requests to be processed at this time...");
                    }

                }//while (true)
            }
            catch (System.Threading.ThreadAbortException)
            {
                Trace.TraceInformation(System.Threading.Thread.CurrentThread.Name + " stopped.");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
            }

            Trace.TraceInformation("Exit.");
        }

        void _Queue_NewRequest(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            //lock (lock_Queue)
            //{
            _PerformanceCounters.AddRequest();

            //    _NewRequest.Set();
            //}
        }

        void _tmrMonitorHandlers_Tick(object State)
        {
            _tmrMonitorHandlers.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            try
            {
                if (_Handlers != null)
                {
                    bool allHandlersRunning = true;

                    int iNumberOfThreadsBusy = 0;
                    int iNumberOfThreadsIdle = 0;

                    for (int i = 0; i < _Handlers.Count; i++)
                    {  
                        if (_Handlers[i].ThreadState == System.Threading.ThreadState.Stopped)
                        {
                            allHandlersRunning = allHandlersRunning && false;

                            Trace.TraceWarning(_Handlers[i].Name + " was found stopped. Application will sttempt to restart it.");

                            _Handlers[i] = null;

                            _Handlers[i] = new System.Threading.Thread(_Handler);
                            _Handlers[i].Start();
                        }

                        if(GetThreadStats(_Handlers[i].Name))
                        {
                            iNumberOfThreadsBusy++;
                        }
                        else
                        {
                            iNumberOfThreadsIdle++;
                        }
                    }

                    Trace.TraceInformation("API Thread Summary: Busy:" + iNumberOfThreadsBusy + " Idle:" + iNumberOfThreadsIdle);

                    if (allHandlersRunning)
                    {
                        //Trace.TraceInformation("All handlers found running.");
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
            }

            _tmrMonitorHandlers.Change(INTERVALMONITORHANDLERS, INTERVALMONITORHANDLERS);
        }

        void UpdateThreadState(String Name, bool IsBusy)
        {
            try
            {
                lock(lock_ThreadStats)
                {
                    if(_thrStats.ContainsKey(Name))
                    {
                        _thrStats[Name] = IsBusy;
                    }
                }
            }
            catch
            {

            }
        }

        bool GetThreadStats(String Name)
        {
            try
            {
                lock (lock_ThreadStats)
                {
                    return _thrStats[Name];
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
