using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class HTTPRequestHandler:ApplicationTypes.iRequestHandler
    {
        public enum HTTPRequestHandlerEventCodes { NONE,OVERMAXIVRPORTUSAGE};

        public EventHandler<HTTPRequestHandlerEventArgs> HTTPRequestHandlerEvent;

        CallbackRecordManager _recordManager = null;
        SettingsManager _settingsManager = null;

        ContactRealtimeDataClient _RealtimeDataClient = null;
        ContactServiceQueueRealtimeDataClient _CSQRealtimeDataClient = null;

        object lock_CheckCSQStatus = new object();

        public HTTPRequestHandler(CallbackRecordManager RecordManager, SettingsManager SettingsManager, ContactRealtimeDataClient RealtimeDataClient, ContactServiceQueueRealtimeDataClient CSQRealtimeDataClient)
        {
            _recordManager = RecordManager;
            _settingsManager = SettingsManager;
            _RealtimeDataClient = RealtimeDataClient;
            _CSQRealtimeDataClient = CSQRealtimeDataClient;
        }

        public bool Handle(System.Net.HttpListenerContext Context, Guid ReqID)
        {
            Trace.TraceInformation("Enter.");

            bool _result = true;

            System.Net.HttpListenerRequest request = null;
            System.Net.HttpListenerResponse response = null;

            System.Text.StringBuilder Response = null;

            try
            {
                request = Context.Request;
                response = Context.Response;

                String sRecordID = String.Empty;
                String sOriginCSQ = String.Empty;
                String sTargetCSQ = String.Empty;
                String sDNIS = String.Empty;
                String sPrompt = String.Empty;
                String sToken = String.Empty;
                String sLanguage = String.Empty;
                String sContactImplementationID = String.Empty;
                String sContactID = String.Empty;
                String sSessionID = String.Empty;
                String sQueuedStartTime = String.Empty;
                String sReentryDelay = String.Empty;
                String sCustomVar1 = String.Empty;
                String sCustomVar2 = String.Empty;
                String sCustomVar3 = String.Empty;
                String sCustomVar4 = String.Empty;
                String sCustomVar5 = String.Empty;
                String sRequeueCode = String.Empty;
                String sRequeueCounter = String.Empty;

                DateTime dtBeginWriteResponse = DateTime.MinValue;

                Constants.Operations Operation = Constants.Operations.NONE;

                Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  Request URL -> " + request.Url.ToString());

                String sOperation = request.QueryString["operation"];

                
                if (sOperation == null || sOperation == string.Empty)
                {
                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  No operation was defined for this request. Assume web UI request.");

                    String sLocation = AppDomain.CurrentDomain.BaseDirectory;

                    String sURL = request.Url.ToString();

                    String sFileRequested = sURL.Substring(sURL.LastIndexOf("/") + 1);

                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  File requested: " + sFileRequested);

                    String sFileExtension = sFileRequested.Substring(sFileRequested.LastIndexOf(".") + 1);

                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  File extension: " + sFileExtension);

                    byte[] _Response = null;

                    String sTargetFile = sLocation + "web\\" + sFileRequested;

                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  Target file ->" + sTargetFile);

                    if (System.IO.File.Exists(sTargetFile))
                    {
                        Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  Requested file was found...");

                        if (sFileExtension.ToLower() == "gif")
                        {
                            response.ContentType = "image/gif";

                            _Response = System.IO.File.ReadAllBytes(sTargetFile);
                        }
                        else if (sFileExtension.ToLower() == "html")
                        {
                            response.ContentType = "text/html";
                            response.ContentEncoding = Encoding.UTF8;

                            String sRequestedFile = System.IO.File.ReadAllText(sTargetFile);

                            sRequestedFile = sRequestedFile.Replace("@@CALBACKSERVERENDPOINT@@", _settingsManager.ApplicationSettings.WebServerIP + ":" + _settingsManager.ApplicationSettings.WebServerPort);

                            _Response = Encoding.ASCII.GetBytes(sRequestedFile);
                        }
                        else if (sFileExtension.ToLower() == "css")
                        {
                            response.ContentType = "text/css";
                            response.ContentEncoding = Encoding.UTF8;

                            String sRequestedFile = System.IO.File.ReadAllText(sTargetFile);

                            sRequestedFile = sRequestedFile.Replace("@@CALBACKSERVERENDPOINT@@", _settingsManager.ApplicationSettings.WebServerIP + ":" + _settingsManager.ApplicationSettings.WebServerPort);

                            _Response = Encoding.ASCII.GetBytes(sRequestedFile);
                        }
                        else
                        {
                            response.ContentType = "text/plain";
                            response.ContentEncoding = Encoding.UTF8;

                            String sRequestedFile = System.IO.File.ReadAllText(sTargetFile);

                            sRequestedFile = sRequestedFile.Replace("@@CALLBACKSERVERENDPOINT@@", _settingsManager.ApplicationSettings.WebServerIP + ":" + _settingsManager.ApplicationSettings.WebServerPort);

                            _Response = Encoding.ASCII.GetBytes(sRequestedFile);
                        }

                        System.IO.Stream sw = response.OutputStream;

                        sw.Write(_Response,0,_Response.Length);

                        sw.Flush();
                        sw.Close();
                        sw = null;

                        _Response = null;

                        _result = true;
                    }
                    else
                    {
                        Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  Requested file was not found...");

                        String sResponse = String.Empty;
                        response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
                        response.StatusDescription = "404 Not Found";
                        response.ContentType = "text/plain";

                        if (SendMessage(response, ReqID.ToString(), sResponse, false))
                        {
                            Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                            _result = true;
                        }
                        else
                        {
                            Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                            _result = false;
                        }
                    }
                }
                else //if (sOperation == null || sOperation == string.Empty)
                {
                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  Requested operation -> " + sOperation);

                    //Task #1:
                    try
                    {
                        Operation = (Constants.Operations)Enum.Parse(typeof(Constants.Operations), sOperation.ToUpper());
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("REQID{" + ReqID.ToString() + "}  Exception casting service:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                        Operation = Constants.Operations.NONE;

                        Response = new StringBuilder();

                        Response.Append("<Description>Unknown operation.</Description>");
                        Response.Append("<Code>-1</Code>");

                        if (SendMessage(response, ReqID.ToString(), Response.ToString(),true))
                        {
                            Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                            Response = null;
                            //TO DO: Close context
                            return true;
                        }
                        else
                        {
                            Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                            Response = null;
                            //TO DO: Close context
                            return false;
                        }                     
                    }

                    switch (Operation)
                    {
                        case Constants.Operations.NONE:

                            Response = new StringBuilder();

                            Response.Append("<Description>Operation NONE was requested.</Description>");
                            Response.Append("<Code>-1</Code>");
                            
                            if (SendMessage(response, ReqID.ToString(), Response.ToString(),true))
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                _result = true;
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                _result = false;
                            }

                            Response = null;                            

                            break;

                        case Constants.Operations.ADDRECORD:

                            Response = new StringBuilder();

                            sRecordID = request.QueryString["id"];
                            sDNIS = request.QueryString["dnis"];
                            sOriginCSQ = request.QueryString["origincsq"];
                            sTargetCSQ = request.QueryString["targetcsq"];
                            sPrompt = request.QueryString["prompt"];
                            sContactImplementationID = request.QueryString["implid"];
                            sContactID = request.QueryString["contactid"];
                            sSessionID = request.QueryString["sessionid"];
                            sQueuedStartTime = request.QueryString["queuestarttime"];
                            sReentryDelay = request.QueryString["delay"];
                            sLanguage = request.QueryString["language"];
                            sCustomVar1 = request.QueryString["customvar1"];
                            sCustomVar2 = request.QueryString["customvar2"];
                            sCustomVar3 = request.QueryString["customvar3"];
                            sCustomVar4 = request.QueryString["customvar4"];
                            sCustomVar5 = request.QueryString["customvar5"];
                            sRequeueCode = request.QueryString["requeuecode"];
                            sRequeueCounter = request.QueryString["requeuecounter"];

                            if (String.IsNullOrEmpty(sRecordID))
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  sRecordID is either null or empty.");

                                Response.Append("<Description>Record ID is empty/null.</Description>");
                                Response.Append("<Code>-1</Code>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                    _result = false;
                                }

                                return _result;
                            }

                            if (String.IsNullOrEmpty(sDNIS))
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  sDNIS is either null or empty.");

                                Response.Append("<Description>Record DNIS is empty/null.</Description>");
                                Response.Append("<Code>-1</Code>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                    _result = false;
                                }

                                return _result;
                            }

                            if (String.IsNullOrEmpty(sOriginCSQ))
                            {
                                sOriginCSQ = "NOT_IN_USE";
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  sOriginCSQ is either null or empty; default it to " + sOriginCSQ);
                            }

                            if (sTargetCSQ == null || sTargetCSQ == String.Empty)
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  sTargetCSQ is either null or empty.");

                                Response.Append("<Description>Record sTargetCSQ is empty/null.</Description>");
                                Response.Append("<Code>-1</Code>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                    _result = false;
                                }

                                return _result;
                            }

                            if (!_recordManager.AssertBelowSystemLimit())
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "} Maximum number of requests has been reached: " + Constants.MAXIMUM_NUMBER_OF_REQUESTS);

                                Response.Append("<Description>Maximum number of requests has been reached: " + Constants.MAXIMUM_NUMBER_OF_REQUESTS + "</Description>");
                                Response.Append("<Code>-1</Code>");
                                Response.Append("<AcceptingCallbacks>false</AcceptingCallbacks>");
                                Response.Append("<SettingsLastUpdate></SettingsLastUpdate>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                    _result = false;
                                }

                                return _result;
                            }

                            if (_recordManager.GetRecordByID(sRecordID) != null)
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "} A record with this id already exists; record cannot be added.");

                                Response.Append("<Description>Record id already exists.</Description>");
                                Response.Append("<Code>-1</Code>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "} SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "} SendMessage() returned false.");
                                    _result = false;
                                }

                                return _result;
                            }

                            if (_recordManager.GetRecordByDNIS(sDNIS) != null)
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "} A record with this DNIS already exists; record cannot be added.");

                                Response.Append("<Description>Record with same DNIS exists.</Description>");
                                Response.Append("<Code>10</Code>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "} SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "} SendMessage() returned false.");
                                    _result = false;
                                }

                                return _result;
                            }

                            if(String.IsNullOrEmpty(sReentryDelay))
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "} delay was null or empty; assume 0.");
                                sReentryDelay = "0";
                            }
                            else
                            {
                                if(!sReentryDelay.All(char.IsDigit))
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "} delay was not numeric; assume 0.");
                                    sReentryDelay = "0";
                                }
                            }

                            if (String.IsNullOrEmpty(sQueuedStartTime))
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "} Invalid value for sQueuedStartTime; record cannot be added.");

                                Response.Append("<Description>Invalid value for sQueuedStartTime</Description>");
                                Response.Append("<Code>10</Code>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                    _result = false;
                                }

                                return _result;
                            }

                            try
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "} sQueuedStartTime (Before adding Delay):" + sQueuedStartTime);

                                long lQueuedStartTime = long.Parse(sQueuedStartTime);
                                long lReentryDelay = long.Parse(sReentryDelay) * 60 * 1000;

                                sQueuedStartTime = (lQueuedStartTime + lReentryDelay).ToString();

                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "} sQueuedStartTime (After adding Delay):" + sQueuedStartTime);
                            }
                            catch(Exception)
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "} Error shifting QueuedStartTime by Delay.");
                            }

                            if (sLanguage == null)
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "} language parameter is null; assume EN_US.");
                                sLanguage = "EN_US";
                            }

                            if (String.IsNullOrEmpty(sCustomVar1))
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "} customvar1 parameter is null; assume empty.");
                                sCustomVar1 = String.Empty;
                            }

                            if (String.IsNullOrEmpty(sCustomVar2))
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "} customvar2 parameter is null; assume empty.");
                                sCustomVar2 = String.Empty;
                            }

                            if (String.IsNullOrEmpty(sCustomVar3))
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "} customvar3 parameter is null; assume empty.");
                                sCustomVar3 = String.Empty;
                            }

                            if (String.IsNullOrEmpty(sCustomVar4))
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "} customvar4 parameter is null; assume empty.");
                                sCustomVar4 = String.Empty;
                            }

                            if (String.IsNullOrEmpty(sCustomVar5))
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "} customvar5 parameter is null; assume empty.");
                                sCustomVar5 = String.Empty;
                            }

                            if(String.IsNullOrEmpty(sRequeueCode))
                            {
                                sRequeueCode = String.Empty;
                            }

                            if (String.IsNullOrEmpty(sRequeueCounter))
                            {
                                sRequeueCounter = "0";
                            }

                            int iRequeueCounter = 0;

                            if(int.TryParse(sRequeueCounter, out iRequeueCounter))
                            {
                                if(iRequeueCounter > Constants.MAXIMUM_NUMBER_OF_REQUEUES)
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "} Contact has already been requeued the maximum number of times: " + Constants.MAXIMUM_NUMBER_OF_REQUEUES);

                                    Response.Append("<Description>Contact has already been requeued the maximum number of times</Description>");
                                    Response.Append("<Code>-1</Code>");

                                    if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                    {
                                        Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                        _result = true;
                                    }
                                    else
                                    {
                                        Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                        _result = false;
                                    }

                                    return _result;
                                }
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "} RequeueCounter is not numeric:" + sRequeueCounter);

                                Response.Append("<Description>RequeueCounter is not numeric</Description>");
                                Response.Append("<Code>-1</Code>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                    _result = false;
                                }

                                return _result;
                            }

                            if (_settingsManager != null)
                            {
                                if (_settingsManager.CallbackSettings != null)
                                {
                                    CallbackContactServiceQueue _queue = null;

                                    foreach (CallbackContactServiceQueue queue in _settingsManager.CallbackSettings.Queues)
                                    {
                                        if (queue.Name == sTargetCSQ)
                                        {
                                            _queue = queue;
                                            break;
                                        }

                                    }//foreach (CallbackContactServiceQueue queue in _settingsManager.CallbackSettings.Queues)

                                    if (_queue != null)
                                    {
                                        CallbackContactServiceQueueSettingsProfile _Profile = null;

                                        if (_queue.Profile == null)
                                        {
                                            Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  Profile for TargetCSQ " + sTargetCSQ + " is null; look for default profile.");

                                            foreach (CallbackContactServiceQueue queue in _settingsManager.CallbackSettings.Queues)
                                            {
                                                if (queue.Name == "Default")
                                                {
                                                    _Profile = queue.Profile;
                                                    break;
                                                }

                                            }//foreach (CallbackContactServiceQueue queue in _settingsManager.CallbackSettings.Queues)
                                        }
                                        else
                                        {
                                            _Profile = _queue.Profile;
                                        }

                                        if (_Profile != null)
                                        {
                                            DateTime dtAcceptCallbacksTimeframeBegin = DateTime.ParseExact(_Profile.AcceptCallbacksTimeframeBegin, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                            DateTime dtAcceptCallbacksTimeframeEnd = DateTime.ParseExact(_Profile.AcceptCallbacksTimeframeEnd, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                                            if (DateTime.Now.Subtract(dtAcceptCallbacksTimeframeBegin).TotalMilliseconds >= 0)
                                            {
                                                if (dtAcceptCallbacksTimeframeEnd.Subtract(DateTime.Now).TotalMilliseconds >= 0)
                                                {
                                                    DateTime dtCallbackProcessingTimeframeEnd = DateTime.ParseExact(_Profile.CallbackProcessingTimeframeEnd, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                                                    if (dtCallbackProcessingTimeframeEnd.Subtract(DateTime.Now).TotalMilliseconds >= 0)
                                                    {
                                                        DateTime dReentryDate = DateTime.Now.AddMinutes(2.0);

                                                        CallbackRecord _record = new CallbackRecord(sRecordID, String.Empty, sDNIS, sOriginCSQ, sTargetCSQ, sPrompt, Constants.RecordStatus.NEW, DateTime.Now, sQueuedStartTime, sContactImplementationID, sContactID, sSessionID, dReentryDate, sLanguage, sCustomVar1, sCustomVar2, sCustomVar3, sCustomVar4, sCustomVar5, sRequeueCode, sRequeueCounter);

                                                        _record.CallbackProcessingTimeframeEndCrossed = false;

                                                        if (_recordManager.Add(_record))
                                                        {
                                                            Response.Append("<Description>Record with id " + sRecordID + "  was added to the list.</Description>");
                                                            Response.Append("<Code>0</Code>");
                                                        }
                                                        else
                                                        {
                                                            Response.Append("<Description>Record with id " + sRecordID + "  was not added to the list.</Description>");
                                                            Response.Append("<Code>-1</Code>");
                                                        }

                                                        _record = null;
                                                    }
                                                    else
                                                    {
                                                        Response.Append("<Description>CallbackProcessingTimeframe for CSQ " + sTargetCSQ + " has ended; cannot accept callback request.</Description>");
                                                        Response.Append("<Code>-1</Code>");
                                                    }
                                                }
                                                else
                                                {
                                                    Response.Append("<Description>AcceptCallbacksTimeframe for CSQ " + sTargetCSQ + " has ended; cannot accept callback request.</Description>");
                                                    Response.Append("<Code>-1</Code>");
                                                }
                                            }
                                            else
                                            {
                                                Response.Append("<Description>AcceptCallbacksTimeframe for CSQ " + sTargetCSQ + " has not began; cannot accept callback request.</Description>");
                                                Response.Append("<Code>-1</Code>");
                                            }
                                        }
                                        else
                                        {
                                            Response.Append("<Description>NO valid profile was found for CSQ " + sTargetCSQ + "; cannot accept callback request.</Description>");
                                            Response.Append("<Code>-1</Code>");
                                        }
                                    }
                                    else
                                    {
                                        Response.Append("<Description>CSQ " + sTargetCSQ + " was not found in settings; cannot accept callback request.</Description>");
                                        Response.Append("<Code>-1</Code>");
                                    }
                                }
                                else
                                {
                                    Response.Append("<Description>_settingsManager.CallbackSettings is null; cannot accept callback requests.</Description>");
                                    Response.Append("<Code>-1</Code>");
                                }
                            }
                            else
                            {
                                Response.Append("<Description>_settingsManager is null; cannot accept callback requests.</Description>");
                                Response.Append("<Code>-1</Code>");
                            }

                            if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                _result = true;
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                _result = false;
                            }

                            Response = null;

                            break;

                        case Constants.Operations.REMOVERECORD:

                            Response = new StringBuilder();

                            sRecordID = request.QueryString["id"];

                            if (sRecordID == null || sRecordID == String.Empty)
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  sRecordID is either null or empty.");

                                Response.Append("<Description>Record ID is empty/null.</Description>");
                                Response.Append("<Code>-1</Code>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                    _result = false;
                                }

                                return _result;
                            }

                            if (_recordManager.GetRecordByID(sRecordID) == null)
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  A record with this id does not exist.");

                                Response.Append("<Description>Record with id does not exist.</Description>");
                                Response.Append("<Code>-1</Code>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                    _result = false;
                                }

                                return _result;
                            }

                            if (_recordManager.Remove(sRecordID))
                            {
                                Response.Append("<Description>Record with id " + sRecordID + "  was removed from the list.</Description>");
                                Response.Append("<Code>0</Code>");
                            }
                            else
                            {
                                Response.Append("<Description>Record with id " + sRecordID + "  was not removed from the list.</Description>");
                                Response.Append("<Code>-1</Code>");
                            }

                            if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                _result = true;
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                _result = false;
                            }

                            Response = null;

                            break;

                        case Constants.Operations.UPDATERECORD:

                            Response = new StringBuilder();

                            sRecordID = request.QueryString["id"];
                            String sStatus = request.QueryString["status"];
                            String sAgentID = request.QueryString["agentid"];
                            String sRequestID = request.QueryString["reqid"];

                            if (sRecordID == null || sRecordID == String.Empty)
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  sRecordID is either null or empty.");

                                Response.Append("<Description>Record ID is empty/null.</Description>");
                                Response.Append("<Code>-1</Code>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                    _result = false;
                                }

                                return _result;
                            }

                            if (sStatus == null || sStatus == String.Empty)
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  sStatus is either null or empty.");

                                Response.Append("<Description>Status is empty/null.</Description>");
                                Response.Append("<Code>-1</Code>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                    _result = false;
                                }

                                return _result;
                            }

                            if (sAgentID == null)
                            {
                                sAgentID = String.Empty;
                            }

                            Trace.TraceInformation("REQID{" + ReqID.ToString() + "} sAgentID >" + sAgentID + "<");

                            if (String.IsNullOrEmpty(sRequestID))
                            {
                                sRequestID = String.Empty;
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  sRequestID was either null or empty; assumed empty.");
                            }

                            if (_recordManager.GetRecordByID(sRecordID) == null)
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  A record with this id does not exist.");

                                Response.Append("<Description>Record with id does not exist.</Description>");
                                Response.Append("<Code>-1</Code>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                    _result = false;
                                }

                                return _result;
                            }

                            Constants.RecordStatus _Status = Constants.RecordStatus.NEW;

                            try
                            {
                                _Status = (Constants.RecordStatus)Enum.Parse(typeof(Constants.RecordStatus), sStatus.ToUpper());
                            }
                            catch
                            {
                                _Status = Constants.RecordStatus.INVALID;
                            }

                            if (_Status != Constants.RecordStatus.INVALID)
                            {
                                String sErrorDescription = String.Empty;

                                if (_recordManager.Update(sRecordID, sAgentID, sRequestID, _Status,out sErrorDescription))
                                {
                                    Response.Append("<Description>Record with id " + sRecordID + " was updated to status " + sStatus + "</Description>");
                                    Response.Append("<Code>0</Code>");
                                }
                                else
                                {
                                    Response.Append("<Description>Record with id " + sRecordID + "  was not updated: " + sErrorDescription + "</Description>");
                                    Response.Append("<Code>-1</Code>");
                                }
                            }
                            else
                            {
                                Response.Append("<Description>Invalid value for status " + sStatus + "</Description>");
                                Response.Append("<Code>-1</Code>");
                            }

                            if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                _result = true;
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                _result = false;
                            }

                            break;

                        case Constants.Operations.CHECKDNISINUSE:

                            sDNIS = request.QueryString["dnis"];

                            Response = new StringBuilder();

                            if (sDNIS == null || sDNIS == String.Empty)
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  sDNIS is either null or empty.");

                                Response.Append("<Description>Record DNIS is empty/null.</Description>");
                                Response.Append("<DNISInUse>false</DNISInUse>");
                                Response.Append("<Code>-1</Code>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                    _result = false;
                                }

                                return _result;
                            }

                            if (_recordManager != null)
                            {
                                if (_recordManager.GetRecordByDNIS(sDNIS) != null)
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  A record with this DNIS already exists; record cannot be added.");

                                    Response.Append("<Description>Record with same DNIS exists.</Description>");
                                    Response.Append("<Code>0</Code>");
                                    Response.Append("<DNISInUse>true</DNISInUse>");
                                }
                                else
                                {
                                    Response.Append("<Description>Record with same DNIS exists.</Description>");
                                    Response.Append("<Code>0</Code>");
                                    Response.Append("<DNISInUse>false</DNISInUse>");
                                }
                            }
                            else
                            {
                                Response.Append("<Description>_settingsManager is null; cannot accept callback requests.</Description>");
                                Response.Append("<DNISInUse>false</DNISInUse>");
                                Response.Append("<Code>-1</Code>");
                            }

                            if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                _result = true;
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                _result = false;
                            }

                            Response = null;

                            return _result;

                            break;

                        case Constants.Operations.GETRECORDS:

                            Response = new StringBuilder();

                            String _AllRecordsAsXML = _recordManager.GetRecordListingAsXML();

                            if (_AllRecordsAsXML == String.Empty)
                            {
                                Response.Append("<Description>Error occurred when listing records.</Description>");
                                Response.Append("<Code>-1</Code>");
                            }
                            else
                            {
                                Response.Append("<Description></Description>");
                                Response.Append("<Code>0</Code>");
                                Response.Append(_AllRecordsAsXML);
                            }

                            dtBeginWriteResponse = DateTime.Now;

                            if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "} SendMessage() returned true. Write operation took " + DateTime.Now.Subtract(dtBeginWriteResponse).TotalMilliseconds + " ms");
                                _result = true;
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                _result = false;
                            }

                            return _result;

                        case Constants.Operations.GETRECORDSBYCSQ:

                            Response = new StringBuilder();

                            String _Response = _recordManager.GetRecordsByCSQAsXML();

                            if (_Response == String.Empty)
                            {
                                Response.Append("<Description>Error occurred when listing records.</Description>");
                                Response.Append("<Code>-1</Code>");
                            }
                            else
                            {
                                Response.Append("<Description></Description>");
                                Response.Append("<Code>0</Code>");
                                Response.Append(_Response);
                            }

                            dtBeginWriteResponse = DateTime.Now;

                            if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "} SendMessage() returned true. Write operation took " + DateTime.Now.Subtract(dtBeginWriteResponse).TotalMilliseconds + " ms");
                                _result = true;
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                _result = false;
                            }

                            return _result;

                        case Constants.Operations.CHECKSERVERSTATUS:

                            Response = new StringBuilder();

                            Response.Append("<Description></Description>");
                            Response.Append("<Code>0</Code>");
                            Response.Append("<CallbackRequestsInMemory>" + _recordManager.GetRecordCount().ToString() + "</CallbackRequestsInMemory>");
                            Response.Append("<MaximumNumberCallbackRequests>" + Constants.MAXIMUM_NUMBER_OF_REQUESTS.ToString() + "</MaximumNumberCallbackRequests>");
                            Response.Append("<SystemTime>" + DateTime.Now.ToString() + "</SystemTime>");
                            Response.Append("<ServiceLastStarted>" + _settingsManager.ServiceStartedAt.ToString() + "</ServiceLastStarted>");
                            Response.Append("<ProductVersion>" + System.Windows.Forms.Application.ProductVersion.ToString() + "</ProductVersion>");
                            Response.Append("<SettingsLastUpdate>" + _settingsManager.SettingsLastChangedAt.ToString() + "</SettingsLastUpdate>");
                            Response.Append("<UCCXMasterNode>" + _settingsManager.UCCXMasterNode + "</UCCXMasterNode>");

                            if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                _result = true;
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                _result = false;
                            }

                            return _result;

                        case Constants.Operations.CHECKCSQSTATUS:

                            Response = new StringBuilder();

                            sTargetCSQ = request.QueryString["csq"];

                            if (sTargetCSQ == null || sTargetCSQ == String.Empty)
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  sCSQ is either null or empty.");

                                Response.Append("<Description>Invalid value for the csq parameter</Description>");
                                Response.Append("<Code>-1</Code>");
                            }
                            else
                            {   
                                //if (!_RealtimeDataClient.GetRealtimeData())
                                //{ 
                                //    Trace.TraceWarning("REQID{" + ReqID.ToString() + "} - _RealtimeDataClient.GetRealtimeData() returned false.");

                                //    Response.Append("<Description>_RealtimeDataClient.GetRealtimeData() returned false</Description>");
                                //    Response.Append("<Code>-1</Code>");
                                //    Response.Append("<AcceptingCallbacks>false</AcceptingCallbacks>");
                                //    Response.Append("<SettingsLastUpdate></SettingsLastUpdate>");

                                //    if (SendMessage(response, Response.ToString(), true))
                                //    {
                                //        Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                //        _result = true;
                                //    }
                                //    else
                                //    {
                                //        Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                //        _result = false;
                                //    }

                                //    return _result;
                                //}

                                if (!_recordManager.AssertBelowSystemLimit())
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "} Maximum number of requests has been reached: " + Constants.MAXIMUM_NUMBER_OF_REQUESTS);

                                    Response.Append("<Description>Maximum number of requests has been reached: " + Constants.MAXIMUM_NUMBER_OF_REQUESTS + "</Description>");
                                    Response.Append($"<Code>{Constants.OperationErrorCodes.SYSTEM_LIMIT_REACHED}</Code>");
                                    Response.Append("<AcceptingCallbacks>false</AcceptingCallbacks>");
                                    Response.Append("<SettingsLastUpdate></SettingsLastUpdate>");

                                    if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                    {
                                        Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                        _result = true;
                                    }
                                    else
                                    {
                                        Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                        _result = false;
                                    }

                                    return _result;
                                }

                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "} After _recordManager.AssertBelowSystemLimit()");

                                if (DateTime.Now.Subtract(_RealtimeDataClient.LastRealtimeDataCollectedAt).TotalMilliseconds > (3 * Constants.CONTACT_REALTIMEDATE_REFRESH))
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "} - Realtime Data was last collected at  " + _RealtimeDataClient.LastRealtimeDataCollectedAt + "; it is now " + DateTime.Now.Subtract(_RealtimeDataClient.LastRealtimeDataCollectedAt).TotalMilliseconds + " ms old.");

                                    Response.Append("<Description>Contact Realtime Data is older than " + (3 * Constants.CONTACT_REALTIMEDATE_REFRESH) + "</Description>");
                                    Response.Append($"<Code>{Constants.OperationErrorCodes.REALTIME_DATA_STALE}</Code>");
                                    Response.Append("<AcceptingCallbacks>false</AcceptingCallbacks>");
                                    Response.Append("<SettingsLastUpdate></SettingsLastUpdate>");

                                    if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                    {
                                        Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                        _result = true;
                                    }
                                    else
                                    {
                                        Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                        _result = false;
                                    }

                                    return _result;
                                }

                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "} After making sure RR data is less than 15 secs old");

                                double dNumberOfIVRPorts = double.Parse(_settingsManager.ApplicationSettings.UCCXNumberOfIVRPorts);
                                double dMaxIVRPortUsagePercent = double.Parse(_settingsManager.ApplicationSettings.UCCXMaxIVRPortUsagePercent);

                                double bMaxIVRPortsAvailable = Math.Floor(dNumberOfIVRPorts * (dMaxIVRPortUsagePercent / 100.0));
                                
                                if (bMaxIVRPortsAvailable <= _RealtimeDataClient.NumberOfContactsInIVR)
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "} - " + "Max IVR Ports available:" + bMaxIVRPortsAvailable.ToString() + " IVR Ports in use:" + _RealtimeDataClient.NumberOfContactsInIVR);

                                    Response.Append("<Description>Over Max IVR Ports in use.</Description>");
                                    Response.Append($"<Code>{Constants.OperationErrorCodes.IVR_PORT_USAGE_EXCEEDED}</Code>");
                                    Response.Append("<AcceptingCallbacks>false</AcceptingCallbacks>");
                                    Response.Append("<SettingsLastUpdate></SettingsLastUpdate>");

                                    if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                    {
                                        Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                        _result = true;
                                    }
                                    else
                                    {
                                        Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                        _result = false;
                                    }

                                    String sMessage = String.Empty;

                                    if (HTTPRequestHandlerEvent != null)
                                    {
                                        HTTPRequestHandlerEvent(this, new HTTPRequestHandlerEventArgs("",HTTPRequestHandlerEventCodes.OVERMAXIVRPORTUSAGE));
                                    }

                                    return _result;
                                }

                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "} _RealtimeDataClient.NumberOfContactsInIVR");

                                if (_settingsManager != null)
                                {
                                    if (_settingsManager.CallbackSettings != null)
                                    {   
                                        CallbackContactServiceQueue _queue = null;

                                        foreach (CallbackContactServiceQueue queue in _settingsManager.CallbackSettings.Queues)
                                        {
                                            if (queue.Name == sTargetCSQ)
                                            {
                                                _queue = queue;
                                                break;
                                            }

                                        }//foreach (CallbackContactServiceQueue queue in _settingsManager.CallbackSettings.Queues)

                                        if (_queue != null)
                                        {
                                            if (_queue.CallbackEnabled)
                                            {
                                                CallbackContactServiceQueueSettingsProfile _Profile = null;

                                                if (_queue.Profile == null)
                                                {
                                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  Profile for CSQ " + sTargetCSQ + " is null; look for default profile.");

                                                    foreach (CallbackContactServiceQueue queue in _settingsManager.CallbackSettings.Queues)
                                                    {
                                                        if (queue.Name == "Default")
                                                        {
                                                            _Profile = queue.Profile;
                                                            break;
                                                        }

                                                    }//foreach (CallbackContactServiceQueue queue in _settingsManager.CallbackSettings.Queues)
                                                }
                                                else
                                                {
                                                    _Profile = _queue.Profile;
                                                }

                                                if (_Profile != null)
                                                {
                                                    DateTime dtAcceptCallbacksTimeframeBegin = DateTime.MinValue;
                                                    DateTime dtAcceptCallbacksTimeframeEnd = DateTime.MinValue;

                                                    try
                                                    {
                                                        dtAcceptCallbacksTimeframeBegin = DateTime.ParseExact(_Profile.AcceptCallbacksTimeframeBegin, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                                        dtAcceptCallbacksTimeframeEnd = DateTime.ParseExact(_Profile.AcceptCallbacksTimeframeEnd, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                                                        if (DateTime.Now.Subtract(dtAcceptCallbacksTimeframeBegin).TotalMilliseconds >= 0)
                                                        {
                                                            if (dtAcceptCallbacksTimeframeEnd.Subtract(DateTime.Now).TotalMilliseconds >= 0)
                                                            {
                                                                //Response.Append("<Description>CSQ " + sTargetCSQ + " is accepting callback requests.</Description>");
                                                                //Response.Append("<Code>0</Code>");
                                                                //Response.Append("<AcceptingCallbacks>true</AcceptingCallbacks>");
                                                                //Response.Append("<SettingsLastUpdate>" + _settingsManager.SettingsLastChangedAt.ToString("dd/MM/yyyy HH:mm:ss") + "</SettingsLastUpdate>");

                                                                lock (lock_CheckCSQStatus)
                                                                {
                                                                    if (_CSQRealtimeDataClient == null)
                                                                    {
                                                                        _CSQRealtimeDataClient = new ContactServiceQueueRealtimeDataClient(_settingsManager.ApplicationSettings);
                                                                        Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  _CSQRealtimeDataClient was instantiated.");
                                                                    }

                                                                    //Replace
                                                                    if (DateTime.Now.Subtract(_CSQRealtimeDataClient.LastRealtimeDataCollectedAt).TotalMilliseconds <= (3 * Constants.CONTACT_REALTIMEDATE_REFRESH))
                                                                    //if (_CSQRealtimeDataClient.GetRealtimeData())
                                                                    {
                                                                        CallbackOfferedAlgorithm offeredAlgorithm = new CallbackOfferedAlgorithm();

                                                                        if (offeredAlgorithm == null)
                                                                        {
                                                                            Trace.TraceInformation("REQID{" + ReqID.ToString() + "} offeredAlgorithm is null.");
                                                                        }

                                                                        if (offeredAlgorithm.Analyse(_CSQRealtimeDataClient.CSQInfo, sTargetCSQ, _Profile, _recordManager))
                                                                        {
                                                                            Response.Append("<Description>CSQ " + sTargetCSQ + " is accepting callback requests.</Description>");
                                                                            Response.Append("<Code>0</Code>");
                                                                            Response.Append("<AcceptingCallbacks>true</AcceptingCallbacks>");
                                                                            Response.Append("<SettingsLastUpdate>" + _settingsManager.SettingsLastChangedAtUTC.ToString("dd/MM/yyyy HH:mm:ss") + "</SettingsLastUpdate>");
                                                                        }
                                                                        else
                                                                        {
                                                                            Response.Append("<Description>" + offeredAlgorithm.Description + "</Description>");
                                                                            Response.Append("<Code>-1</Code>");
                                                                            Response.Append("<AcceptingCallbacks>false</AcceptingCallbacks>");
                                                                            Response.Append("<SettingsLastUpdate>" + _settingsManager.SettingsLastChangedAtUTC.ToString("dd/MM/yyyy HH:mm:ss") + "</SettingsLastUpdate>");
                                                                        }

                                                                        offeredAlgorithm = null;
                                                                    }
                                                                    else
                                                                    {
                                                                        Trace.TraceWarning("REQID{" + ReqID.ToString() + "} _CSQRealtimeDataClient was last refreshed on " + _CSQRealtimeDataClient.LastRealtimeDataCollectedAt);

                                                                        //Response.Append("<Description>Failed to get realtime stats from data collection service.</Description>");
                                                                        Response.Append("<Description>CSQ Realtime Data is older than " + (3 * Constants.CONTACT_REALTIMEDATE_REFRESH) + "</Description>");
                                                                        Response.Append("<Code>-1</Code>");
                                                                        Response.Append("<AcceptingCallbacks>false</AcceptingCallbacks>");
                                                                        Response.Append("<SettingsLastUpdate>" + _settingsManager.SettingsLastChangedAtUTC.ToString("dd/MM/yyyy HH:mm:ss") + "</SettingsLastUpdate>");
                                                                    }

                                                                }//lock (lock_CheckCSQStatus)
                                                            }
                                                            else
                                                            {
                                                                Response.Append("<Description>AcceptCallbacksTimeframe for CSQ " + sTargetCSQ + " has ended.</Description>");
                                                                Response.Append("<Code>0</Code>");
                                                                Response.Append("<AcceptingCallbacks>false</AcceptingCallbacks>");
                                                                Response.Append("<SettingsLastUpdate>" + _settingsManager.SettingsLastChangedAtUTC.ToString("dd/MM/yyyy HH:mm:ss") + "</SettingsLastUpdate>");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Response.Append("<Description>AcceptCallbacksTimeframe for CSQ " + sTargetCSQ + " has not began.</Description>");
                                                            Response.Append("<Code>0</Code>");
                                                            Response.Append("<AcceptingCallbacks>false</AcceptingCallbacks>");
                                                            Response.Append("<SettingsLastUpdate>" + _settingsManager.SettingsLastChangedAtUTC.ToString("dd/MM/yyyy HH:mm:ss") + "</SettingsLastUpdate>");
                                                        }

                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  Exception:" + ex.Message);

                                                        Response.Append("<Description>Exception while handling settings for this CSQ</Description>");
                                                        Response.Append("<Code>-1</Code>");
                                                        Response.Append("<AcceptingCallbacks>false</AcceptingCallbacks>");
                                                        Response.Append("<SettingsLastUpdate>" + _settingsManager.SettingsLastChangedAtUTC.ToString("dd/MM/yyyy HH:mm:ss") + "</SettingsLastUpdate>");
                                                    }
                                                }
                                                else
                                                {
                                                    Response.Append("<Description>No valid profile was found for CSQ " + sTargetCSQ + ".</Description>");
                                                    Response.Append("<Code>-1</Code>");
                                                    Response.Append("<AcceptingCallbacks>false</AcceptingCallbacks>");
                                                    Response.Append("<SettingsLastUpdate>" + _settingsManager.SettingsLastChangedAtUTC.ToString("dd/MM/yyyy HH:mm:ss") + "</SettingsLastUpdate>");
                                                }
                                            }
                                            else
                                            {
                                                Response.Append("<Description>CSQ " + sTargetCSQ + " does not have callback enabled.</Description>");
                                                Response.Append("<Code>0</Code>");
                                                Response.Append("<AcceptingCallbacks>false</AcceptingCallbacks>");
                                                Response.Append("<SettingsLastUpdate>" + _settingsManager.SettingsLastChangedAtUTC.ToString("dd/MM/yyyy HH:mm:ss") + "</SettingsLastUpdate>");
                                            }
                                        }
                                        else
                                        {
                                            Response.Append("<Description>CSQ " + sTargetCSQ + " was not found in settings.</Description>");
                                            Response.Append("<Code>-1</Code>");
                                            Response.Append("<AcceptingCallbacks>false</AcceptingCallbacks>");
                                            Response.Append("<SettingsLastUpdate>" + _settingsManager.SettingsLastChangedAtUTC.ToString("dd/MM/yyyy HH:mm:ss") + "</SettingsLastUpdate>");
                                        }
                                    }
                                    else
                                    {
                                        Response.Append("<Description>_settingsManager.CallbackSettings is null.</Description>");
                                        Response.Append("<Code>-1</Code>");
                                        Response.Append("<AcceptingCallbacks>false</AcceptingCallbacks>");
                                        Response.Append("<SettingsLastUpdate></SettingsLastUpdate>");
                                    }
                                }
                                else
                                {
                                    Response.Append("<Description>_settingsManager is null.</Description>");
                                    Response.Append("<Code>-1</Code>");
                                    Response.Append("<AcceptingCallbacks>false</AcceptingCallbacks>");
                                    Response.Append("<SettingsLastUpdate></SettingsLastUpdate>");
                                }
                            }

                            Trace.TraceInformation("REQID{" + ReqID.ToString() + "} After checking settings");

                            dtBeginWriteResponse = DateTime.Now;

                            if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "} SendMessage() returned true. Write took " + DateTime.Now.Subtract(dtBeginWriteResponse).TotalMilliseconds + " ms.");
                                _result = true;
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                _result = false;
                            }

                            return _result;

                        case Constants.Operations.AUTHENTICATEUSER:

                            Response = new StringBuilder();

                            sToken = request.QueryString["token"];

                            String sUsername = String.Empty;
                            String sPassword = String.Empty;

                            if (sToken != null && sToken != String.Empty)
                            {
                                try
                                {
                                    byte[] base64EncodedBytes = System.Convert.FromBase64String(sToken);
                                    String sDecodedToken = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                                    sUsername = sDecodedToken.Substring(0, sDecodedToken.IndexOf(":"));
                                    sPassword = sDecodedToken.Substring(sDecodedToken.IndexOf(":") + 1);
                                }
                                catch(Exception ex)
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  Exception:" + ex.Message);

                                    sUsername = String.Empty;
                                    sPassword = String.Empty;
                                }
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  sToken is either null or empty.");

                                sUsername = String.Empty;
                                sPassword = String.Empty;
                            }

                            Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  sUsername >" + sUsername + "< sPassword >" + sPassword + "<");

                            if (sUsername == null || sUsername == String.Empty)
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  sUsername is either null or empty.");

                                Response.Append("<Description>Invalid value for the username parameter</Description>");
                                Response.Append("<Code>-1</Code>");
                                Response.Append("<Authenticated>false</Authenticated>");
                            }
                            else
                            {
                                if (sPassword == null || sPassword == String.Empty)
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  sPassword is either null or empty.");

                                    Response.Append("<Description>Invalid value for the password parameter</Description>");
                                    Response.Append("<Code>-1</Code>");
                                    Response.Append("<Authenticated>false</Authenticated>");
                                }
                                else
                                {
                                    if (_settingsManager == null)
                                    {
                                        Response.Append("<Description>_settingsManager is null</Description>");
                                        Response.Append("<Code>-1</Code>");
                                        Response.Append("<Authenticated>false</Authenticated>");
                                    }
                                    else
                                    {
                                        if (_settingsManager.ApplicationSettings == null)
                                        {
                                            Response.Append("<Description>_settingsManager.ApplicationSettings is null</Description>");
                                            Response.Append("<Code>-1</Code>");
                                            Response.Append("<Authenticated>false</Authenticated>");
                                        }
                                        else
                                        {
                                            if (_settingsManager.ApplicationSettings.UCCXApplicationPort == String.Empty || _settingsManager.ApplicationSettings.UCCXAuthorizationPrefix == String.Empty)
                                            {
                                                Response.Append("<Description>Either UCCXPort and/or UCCXAuthorizationPrefix is empty</Description>");
                                                Response.Append("<Code>-1</Code>");
                                                Response.Append("<Authenticated>false</Authenticated>");
                                            }
                                            else
                                            {
                                                if (_settingsManager.UCCXMasterNodeDetected)
                                                {
                                                    try
                                                    {
                                                        String sAuthenticationResponse = String.Empty;

                                                        SendAuthenticationRequest(_settingsManager.UCCXMasterNode, _settingsManager.ApplicationSettings.UCCXApplicationPort, _settingsManager.ApplicationSettings.UCCXAuthorizationPrefix, sUsername, sPassword, out sAuthenticationResponse);

                                                        if (sAuthenticationResponse != String.Empty)
                                                        {
                                                            int iCode = 0;
                                                            bool bUserAuthenticated = false;

                                                            ParseAuthenticationResponse(sAuthenticationResponse, out iCode, out bUserAuthenticated);

                                                            Response.Append("<Description></Description>");
                                                            Response.Append("<Code>" + iCode.ToString() + "</Code>");
                                                            Response.Append("<Authenticated>" + bUserAuthenticated.ToString().ToLower() + "</Authenticated>");
                                                        }
                                                        else
                                                        {
                                                            Response.Append("<Description>Invalid response received for authentication request</Description>");
                                                            Response.Append("<Code>-1</Code>");
                                                            Response.Append("<Authenticated>false</Authenticated>");
                                                        }

                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Trace.TraceError("REQID{" + ReqID.ToString() + "}  Exception Authorizing user:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                                                        Response.Append("<Description>" + ex.Message + "</Description>");
                                                        Response.Append("<Code>-1</Code>");
                                                        Response.Append("<Authenticated>false</Authenticated>");
                                                    }
                                                }
                                                else
                                                {
                                                    Response.Append("<Description>UCCX Master Node has not been detected yet.</Description>");
                                                    Response.Append("<Code>-2</Code>");
                                                    Response.Append("<Authenticated>false</Authenticated>");
                                                }
                                                
                                            }
                                        }
                                    }
                                }
                            }

                            if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                _result = true;
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                _result = false;
                            }

                            return _result;

                        case Constants.Operations.GETSETTINGS:

                            Response = new StringBuilder();

                            if (_settingsManager == null)
                            {
                                Response.Append("<Description>_settingsManager is null</Description>");
                                Response.Append("<Code>-1</Code>");
                            }
                            else
                            {
                                if (_settingsManager.CallbackSettings == null)
                                {
                                    Response.Append("<Description>_settingsManager.CallbackSettings is null</Description>");
                                    Response.Append("<Code>-1</Code>");
                                }
                                else
                                {
                                    if (_settingsManager.ApplicationSettings.UCCXApplicationPort == String.Empty || _settingsManager.ApplicationSettings.WebServerPrefix == String.Empty)
                                    {
                                        Response.Append("<Description>Either UCCXApplicationPort and/or WebServerPrefix is empty</Description>");
                                        Response.Append("<Code>-1</Code>");
                                        Response.Append("<Authenticated>false</Authenticated>");
                                    }
                                    else
                                    {
                                        if (_settingsManager.UCCXMasterNodeDetected)
                                        {
                                            Response.Append("<Description></Description>");
                                            Response.Append("<Code>0</Code>");
                                            Response.Append("<Settings>");
                                            Response.Append(_settingsManager.GetSettings());
                                            Response.Append("</Settings>");
                                        }
                                        else
                                        {
                                            Response.Append("<Description>UCCX Master Node not detected</Description>");
                                            Response.Append("<Code>-1</Code>");
                                        }
                                    }
                                }
                            }

                            if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                _result = true;
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                _result = false;
                            }

                            return _result;

                        case Constants.Operations.SETSETTINGS:

                            Response = new StringBuilder();

                            String sDocumentContents = String.Empty;

                            using (System.IO.Stream receiveStream = request.InputStream)
                            {
                                using (System.IO.StreamReader readStream = new System.IO.StreamReader(receiveStream, Encoding.UTF8))
                                {
                                    sDocumentContents = readStream.ReadToEnd();
                                }
                            }

                            if (_settingsManager.SetSettings(sDocumentContents))
                            {
                                Response.Append("<Description></Description>");
                                Response.Append("<Code>0</Code>");
                            }
                            else
                            {
                                Response.Append("<Description>Operation failed.</Description>");
                                Response.Append("<Code>-1</Code>");
                            }

                            if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                _result = true;
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                _result = false;
                            }

                            return _result;

                        case Constants.Operations.GETCSQLIST:

                            Response = new StringBuilder();

                            if (_settingsManager == null)
                            {
                                Response.Append("<Description>_settingsManager is null</Description>");
                                Response.Append("<Code>-1</Code>");
                            }
                            else
                            {
                                if (_settingsManager.UCCXMasterNodeDetected)
                                {
                                    UCCXAdminAPIClient apiclient = new UCCXAdminAPIClient(_settingsManager.UCCXMasterNode, _settingsManager.ApplicationSettings.UCCXAdminUser, _settingsManager.ApplicationSettings.UCCXAdminPassword);

                                    String sCSQList = apiclient.GetVoiceCSQList();

                                    if (sCSQList != null)
                                    {
                                        Response.Append("<Description></Description>");
                                        Response.Append("<Code>0</Code>");
                                        Response.Append(sCSQList);
                                    }
                                    else
                                    {
                                        Response.Append("<Description>Problem getting CSQ list from UCCX API.</Description>");
                                        Response.Append("<Code>-1</Code>");
                                    }

                                    apiclient = null;
                                }
                                else
                                {
                                    Response.Append("<Description>UCCX Master Node not detected</Description>");
                                    Response.Append("<Code>-1</Code>");
                                }
                            }

                            if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                _result = true;
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                _result = false;
                            }

                            return _result;

                        case Constants.Operations.GETCONTACTPOSITIONOFFSET:

                             Response = new StringBuilder();

                            sTargetCSQ = request.QueryString["targetcsq"];
                            sContactID = request.QueryString["contactid"];

                            if (String.IsNullOrEmpty(sTargetCSQ))
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  sTargetCSQ is either null or empty.");

                                Response.Append("<Description>Target CSQ is empty/null.</Description>");
                                Response.Append("<Code>-1</Code>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                    _result = false;
                                }
                                
                                return _result;
                            }

                            if (String.IsNullOrEmpty(sContactID))
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  sContactID is either null or empty.");

                                Response.Append("<Description>Contact ID is empty/null.</Description>");
                                Response.Append("<Code>-1</Code>");

                                if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                                {
                                    Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                    _result = true;
                                }
                                else
                                {
                                    Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                    _result = false;
                                }

                                return _result;
                            }

                            int iContactPositionOffset = _recordManager.GetContactPositionOffset(sContactID, sTargetCSQ);

                            if (iContactPositionOffset != -1)
                            {
                                Response.Append("<Description></Description>");
                                Response.Append("<Code>0</Code>");
                                Response.Append("<ContactPositionOffset>" + iContactPositionOffset + "</ContactPositionOffset>");
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "} iContactPositionOffset == -1");

                                Response.Append("<Description>Contact ID is empty/null.</Description>");
                                Response.Append("<Code>-1</Code>");
                            }

                            if (SendMessage(response, ReqID.ToString(), Response.ToString(), true))
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                _result = true;
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                _result = false;
                            }

                            return _result;

                        default:

                            Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  Unexpected/Unknown operation requested. This code should not be executing.");

                            Response = new StringBuilder();

                            Response.Append("<Description>Unexpected/Unknown operation requested. This code should not be executing.</Description>");
                            Response.Append("<Code>-1</Code>");

                            if (SendMessage(response, ReqID.ToString(), Response.ToString(),true))
                            {
                                Trace.TraceInformation("REQID{" + ReqID.ToString() + "}  SendMessage() returned true.");
                                _result = true;
                            }
                            else
                            {
                                Trace.TraceWarning("REQID{" + ReqID.ToString() + "}  SendMessage() returned false.");
                                _result = false;
                            }

                            Response = null;                            

                            break;

                    }//switch (Source)

                }//(SourceName == null || SourceName == string.Empty)
            }
            catch (Exception ex)
            {
                Trace.TraceError("REQID{" + ReqID.ToString() + "}  Outer Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                _result = false;
            }
            finally
            {
                //Close Request and Response
                Response = null;
            }

            return _result;
        }

        private bool SendMessage(System.Net.HttpListenerResponse Response,String ReqID, String Message,bool EmbedInXML)
        {
            try
            {
                Trace.TraceInformation("REQID{" + ReqID.ToString() + "} Response sent back to client: " + Message);
                
                string _Message = string.Empty;

                if (EmbedInXML)
                {
                    _Message = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                    _Message = _Message + "<Response>";
                    _Message = _Message + Message;
                    _Message = _Message + "</Response>";
                }
                else
                {
                    _Message = Message;
                }

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(_Message);
                // Get a response stream and write the response to it.
                Response.ContentLength64 = buffer.Length;
                System.IO.Stream output = Response.OutputStream;
                output.Write(buffer, 0, buffer.Length);

                // You must close the output stream.
                output.Close();
                output.Dispose();
                output = null;

                Trace.TraceInformation("REQID{" + ReqID.ToString() + "} Number of bytes sent back to client: " + buffer.Length);

                buffer = null;

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return false;
            }
        }

        private bool SendMessage(System.Net.HttpListenerResponse Response, byte[] Message)
        {
            try
            {
                Response.ContentLength64 = Message.Length;
                Response.OutputStream.Write(Message, 0, Message.Length);
                Response.OutputStream.Flush();

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return false;
            }
        }

        private void SendAuthenticationRequest(String IP, String Port, String Prefix, String Username, String Password, out String AuthenticationResponse)
        {
            Trace.TraceInformation("Enter.");

            try
            {
                String sURL = "http://" + IP + ":" + Port + "/" + Prefix + "?operation=authenticateuser&username=" + Username + "&password=" + Password;

                Trace.TraceInformation("URL -> " + sURL);

                System.Net.WebRequest request = System.Net.WebRequest.Create(sURL);
                request.Method = "POST";
                request.Timeout = 2000;

                System.Net.WebResponse response = request.GetResponse();

                if (((System.Net.HttpWebResponse)response).StatusCode == System.Net.HttpStatusCode.OK)
                {

                    System.IO.Stream dataStream = response.GetResponseStream();

                    System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();

                    AuthenticationResponse = responseFromServer;

                    reader.Close();
                    reader.Dispose();
                    reader = null;

                    dataStream.Close();
                    dataStream.Dispose();
                    dataStream = null;
                }
                else
                {
                    Trace.TraceWarning("StatusCode is not Ok");

                    AuthenticationResponse = String.Empty;
                }

                response.Close();
                response = null;

                request = null;
            }
            catch (System.Net.WebException ex)
            {
                System.Net.HttpWebResponse res = (System.Net.HttpWebResponse)ex.Response;

                Trace.TraceWarning("WebException: " + ex.Message + Environment.NewLine + "StackTrace: " + ex.StackTrace);

                //using (var stream = ex.Response.GetResponseStream())
                //{
                //    using (var reader = new System.IO.StreamReader(stream))
                //    {
                //        Trace.TraceWarning("Status Description: (" +  res.StatusDescription + ") WebException: " + reader.ReadToEnd());
                //    }
                //}

                AuthenticationResponse = String.Empty;

                return;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "StackTrace: " + ex.StackTrace);

                AuthenticationResponse = String.Empty;

                return;
            }
        }

        private void ParseAuthenticationResponse(String AuthenticationResponse, out int Code, out bool bUserAuthenticated)
        {
            if (AuthenticationResponse == null)
            {
                Trace.TraceWarning("AuthenticationResponse is null.");
                Code = -1;
                bUserAuthenticated = false;
                return;
            }

            if (AuthenticationResponse == String.Empty)
            {
                Trace.TraceWarning("AuthenticationResponse is empty.");
                Code = -1;
                bUserAuthenticated = false;
                return;
            }

            try
            {
                System.Xml.XmlDocument xmlDoc = null;

                xmlDoc = new System.Xml.XmlDocument();

                xmlDoc.LoadXml(AuthenticationResponse);

                System.Xml.XmlNode nodCode = xmlDoc.SelectSingleNode("//Response/Code");

                if (nodCode != null)
                {
                    try
                    {
                        Code = int.Parse(nodCode.InnerText);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning("Exception casting Code to integer; Code defaulted to -1.");
                        Code = -1;
                    }
                }
                else
                {
                    Trace.TraceWarning("//Response/Code element not found. Code defaulted to -1");
                    Code = -1;
                }

                nodCode = null;

                System.Xml.XmlNode nodAuthenticated = xmlDoc.SelectSingleNode("//Response/Authenticated");

                if (nodAuthenticated != null)
                {
                    try
                    {
                        bUserAuthenticated = bool.Parse(nodAuthenticated.InnerText);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning("Exception casting Authenticated to boolean; Code defaulted to -1 and bUserAuthenticated to false.");
                        Code = -1;
                        bUserAuthenticated = false;
                    }
                }
                else
                {
                    Trace.TraceWarning("//Response/Authenticated element not found. Code defaulted to -1 and bUserAuthenticated to false.");
                    Code = -1;
                    bUserAuthenticated = false;
                }

                nodAuthenticated = null;

                xmlDoc = null;

            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "StackTrace: " + ex.StackTrace);
                Code = -1;
                bUserAuthenticated = false;
            }
        }
    }
}
