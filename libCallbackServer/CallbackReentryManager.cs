using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class CallbackReentryManager
    {
        public event EventHandler Started;
        public event EventHandler Stopped;

        const int TICKINTERVAL = 5000;

        ContactRealtimeDataClient _RealtimeDataClient = null;

        CallbackRecordManager _recordManager = null;
        SettingsManager _settingsManager = null;

        System.Threading.Timer _tmrTick = null;

        DateTime _LastTick = DateTime.Now;

        private object objLock = null;
        private object objLockReentry = null;

        public CallbackRecordManager RecordManager
        {
            get { return _recordManager; }
        }

        public SettingsManager Settings
        {
            get { return _settingsManager; }
        }

        public CallbackReentryManager()
        {
            _recordManager = null;
            _settingsManager = null;

            objLock = new object();
            objLockReentry = new object();

            _LastTick = DateTime.ParseExact("01/01/1900 00:00:00", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            _tmrTick = new System.Threading.Timer(_tmrTick_Tick);
        }

        public CallbackReentryManager(CallbackRecordManager RecordManager, ContactRealtimeDataClient RealtimeDataClient, SettingsManager SettingsManager)
        {
            _recordManager = RecordManager;
            _RealtimeDataClient = RealtimeDataClient;
            _settingsManager = SettingsManager;

            objLock = new object();
            objLockReentry = new object();

            _LastTick = DateTime.ParseExact("01/01/1900 00:00:00", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            _tmrTick = new System.Threading.Timer(_tmrTick_Tick);
        }

        public bool Start()
        {
            Trace.TraceInformation("Enter.");

            _LastTick = DateTime.ParseExact("01/01/1900 00:00:00", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);

            if (Started != null)
            {
                Started(this, new EventArgs());
            }

            return true;
        }

        public bool Stop()
        {
            Trace.TraceInformation("Enter.");

            _tmrTick.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            if (Stopped != null)
            {
                Stopped(this, new EventArgs());
            }

            return true;
        }

        void _tmrTick_Tick(object State)
        {
            Trace.TraceInformation("Enter.");

            lock (objLock)
            {   
                _tmrTick.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                DateTime dtBegin = DateTime.Now;

                if (_settingsManager == null)
                {
                    Trace.TraceWarning("_settingsManager is null.");
                    _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);
                    return;
                }

                if (_settingsManager.ApplicationSettings == null)
                {
                    Trace.TraceWarning("_settingsManager.ApplicationSettings is null.");
                    _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);
                    return;
                }

                if (_settingsManager.CallbackSettings == null)
                {
                    Trace.TraceWarning("_settingsManager.CallbackSettings is null.");
                    _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);
                    return;
                }

                if (!_settingsManager.UCCXMasterNodeDetected)
                {
                    Trace.TraceWarning("_settingsManager.UCCXMasterNodeDetected is false; no UCCX Master Node has been detected.");
                    _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);
                    return;
                }

                if (_recordManager == null)
                {
                    Trace.TraceWarning("_recordManager is null.");
                    _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);
                    return;
                }

                if (_recordManager.GetRecordCount() == 0)
                {
                    _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);
                    return;
                }

                try
                {
                    //Task #1: Detect day boundary crossing, check end of day purge and flag records

                    DateTime _CurrentTick = DateTime.Now;

                    bool _DayBoundaryCrossed = false;

                    if (!(_CurrentTick.Year == _LastTick.Year && _CurrentTick.Month == _LastTick.Month && _CurrentTick.Day == _LastTick.Day))
                    {
                        Trace.TraceInformation("Day bounderay was crossed.");
                         _DayBoundaryCrossed = true;
                    }

                    _LastTick = _CurrentTick;

                    foreach (CallbackRecord record in _recordManager.Records)
                    {
                        String sCSQ = record.TargetCSQ;

                        CallbackContactServiceQueue _queue = null;

                        foreach (CallbackContactServiceQueue queue in _settingsManager.CallbackSettings.Queues)
                        {
                            if (sCSQ == queue.Name)
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
                                if (_DayBoundaryCrossed)
                                {
                                    record.CallbackProcessingTimeframeEndCrossed = false;

                                    Trace.TraceInformation("CallbackProcessingTimeframeEndCrossed set to false for record " + record.ID);
                                }
                                else
                                {
                                    //Trace.TraceInformation("CallbackProcessingTimeframeEndCrossed was not set for record " + record.ID);
                                }

                                DateTime dtCallbackProcessingTimeframeEnd = DateTime.ParseExact(_Profile.CallbackProcessingTimeframeEnd, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                                if (!record.CallbackProcessingTimeframeEndCrossed)
                                {
                                    //Trace.TraceInformation("CallbackProcessingTimeframeEndCrossed is false for record " + record.ID);

                                    if (DateTime.Now.Subtract(dtCallbackProcessingTimeframeEnd).TotalMilliseconds >= 0)
                                    {
                                        Trace.TraceInformation("Now is after dtCallbackProcessingTimeframeEnd for record " + record.ID);

                                        record.CallbackProcessingTimeframeEndCrossed = true;
                                        record.ReportOn = true;

                                        //Check record age; mark purge if older than maximum number of days
                                        if (DateTime.Now.Subtract(record.RequestDate).TotalDays > _settingsManager.ApplicationSettings.MaximumNumberOfDays)
                                        {
                                            record.Purge = true;
                                            record.PurgeDueToAge = true;
                                            Trace.TraceInformation("Record " + record.ID + " is " + record.Status + " and was marked for end of day purging due to record age.");
                                        }

                                        if (record.Status == Constants.RecordStatus.INVALID
                                            || record.Status == Constants.RecordStatus.INACTIVE
                                            || record.Status == Constants.RecordStatus.EXCEEDEDNUMBEROFATTEMPTS
                                            || record.Status == Constants.RecordStatus.COMPLETED)
                                        {
                                            record.Purge = true;
                                            Trace.TraceInformation("Record " + record.ID + " is " + record.Status + " and was marked for end of day purging due to status");
                                        }

                                        if (record.Status == Constants.RecordStatus.NEW
                                                || record.Status == Constants.RecordStatus.RETRY
                                                || record.Status == Constants.RecordStatus.FORCERETRY)
                                        {
                                            if (_Profile.EndOfDayPurgeCallbackRequests)
                                            {
                                                record.Purge = true;
                                                Trace.TraceInformation("Record " + record.ID + " is " + record.Status + " and was marked for end of day purging due to status");
                                            }
                                            else
                                            {
                                                record.Status = Constants.RecordStatus.PURGED;

                                                Trace.TraceInformation("Record " + record.ID + " was marked " + record.Status);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Trace.TraceInformation("Now is not after dtCallbackProcessingTimeframeEnd for record " + record.ID);
                                    }
                                }
                                else
                                {
                                    Trace.TraceInformation("CallbackProcessingTimeframeEndCrossed is true for record " + record.ID);
                                }

                            }//if (_Profile != null)

                        }//if (_queue != null)

                    }//foreach (CallbackRecord record in _recordManager.Records)

                    //Task #2: Remove records where purge is true from _recordManager.Records

                    if (_recordManager.Purge())
                    {
                        Trace.TraceInformation("_recordManager.Purge() returned true.");
                    }
                    else
                    {
                        Trace.TraceWarning("_recordManager.Purge() returned false.");
                    }

                    //This code is temporary, until a inactive cache is built
                    if (_recordManager.RemoveRecordsInFinalState())
                    {
                        Trace.TraceInformation("_recordManager.RemoveRecordsInFinalState() returned true.");
                    }
                    else
                    {
                        Trace.TraceWarning("_recordManager.RemoveRecordsInFinalState() returned false.");
                    }
                    //This code is temporary, until a inactive cache is built

                    if (_recordManager.StatusReport())
                    {
                        Trace.TraceInformation("_recordManager.StatusReport() returned true.");
                    }
                    else
                    {
                        Trace.TraceWarning("_recordManager.StatusReport() returned false.");
                    }

                    if (_recordManager.NumberOfActionableRecords() == 0)
                    {
                        Trace.TraceWarning("_recordManager.NumberOfActionableRecords() is 0; ");
                        _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);
                        return;
                    }

                    if (DateTime.Now.Subtract(_RealtimeDataClient.LastRealtimeDataCollectedAt).TotalMilliseconds > Constants.CONTACT_REALTIMEDATE_REFRESH)
                    {
                        Trace.TraceWarning("Last RealtimeData refresh was at : " + _RealtimeDataClient.LastRealtimeDataCollectedAt);
                    }

                    //Task #3:

                    String sErrorDescription = String.Empty;

                    foreach (CallbackRecord record in _recordManager.Records)
                    {
                        double dInactiveTime = 0;

                        switch (record.Status)
                        {
                            case Constants.RecordStatus.NEW:

                                break;

                            case Constants.RecordStatus.PURGED:

                                break;

                            case Constants.RecordStatus.RETRY:

                                break;

                            case Constants.RecordStatus.INVALID:

                                continue;

                            case Constants.RecordStatus.INACTIVE:

                                continue;

                            case Constants.RecordStatus.COMPLETED:

                                continue;

                            case Constants.RecordStatus.EXCEEDEDNUMBEROFATTEMPTS:

                                continue;

                            case Constants.RecordStatus.PROCESSING:

                                dInactiveTime = DateTime.Now.Subtract(record.StatusLastUpdated).TotalSeconds;

                                if (dInactiveTime >= Constants.STATUSUPDATEINACTIVITY)
                                {
                                    sErrorDescription = String.Empty;

                                    if (_recordManager.Update(record.ID, String.Empty, String.Empty, Constants.RecordStatus.RETRY, out sErrorDescription))
                                    {
                                        Trace.TraceInformation("Record " + record.ID + " went from PROCESSING to " + record.Status  + " because it was inactive for " + dInactiveTime.ToString());
                                    }
                                    else
                                    {
                                        Trace.TraceWarning("Record " + record.ID + " did not go to RETRY. Current status:" + record.Status);
                                    }
                                }

                                continue;

                            case Constants.RecordStatus.REQUESTED:

                                dInactiveTime = DateTime.Now.Subtract(record.StatusLastUpdated).TotalSeconds;

                                if (dInactiveTime >= Constants.STATUSUPDATEINACTIVITY)
                                {
                                    sErrorDescription = String.Empty;

                                    if (_recordManager.Update(record.ID, String.Empty, String.Empty, Constants.RecordStatus.RETRY, out sErrorDescription))
                                    {
                                        Trace.TraceInformation("Record " + record.ID + " went from REQUESTED to RETRY because it was inactive for " + dInactiveTime.ToString());
                                    }
                                    else
                                    {
                                        Trace.TraceWarning("Record " + record.ID + " did not go to RETRY. Current status:" + record.Status);
                                    }
                                }

                                continue;

                            case Constants.RecordStatus.QUEUED:

                                dInactiveTime = DateTime.Now.Subtract(record.StatusLastUpdated).TotalSeconds;

                                if (dInactiveTime >= Constants.STATUSUPDATEINACTIVITY)
                                {
                                    sErrorDescription = String.Empty;

                                    if (_recordManager.Update(record.ID, String.Empty, String.Empty, Constants.RecordStatus.RETRY, out sErrorDescription))
                                    {
                                        Trace.TraceInformation("Record " + record.ID + " went from " + record.Status + " to RETRY because it was inactive for " + dInactiveTime.ToString());
                                    }
                                    else
                                    {
                                        Trace.TraceWarning("Record " + record.ID + " did not go to RETRY. Current status:" + record.Status);
                                    }
                                }

                                continue;

                            case Constants.RecordStatus.DIALINGTARGET:

                                sErrorDescription = String.Empty;

                                if (_recordManager.Update(record.ID, String.Empty, String.Empty, Constants.RecordStatus.COMPLETED, out sErrorDescription))
                                {
                                    Trace.TraceInformation("Record " + record.ID + " went from DIALINGTARGET to " + record.Status);
                                }
                                else
                                {
                                    Trace.TraceWarning("Record " + record.ID + " did not go to COMPLETED. Current status:" + record.Status);
                                }

                               continue;

                            case Constants.RecordStatus.AGENTACKNOWLEDGED:

                                dInactiveTime = DateTime.Now.Subtract(record.StatusLastUpdated).TotalSeconds;

                                if (dInactiveTime >= Constants.STATUSUPDATEINACTIVITY)
                                {
                                    sErrorDescription = String.Empty;

                                    if (_recordManager.Update(record.ID, String.Empty, String.Empty, Constants.RecordStatus.RETRY, out sErrorDescription))
                                    {
                                        Trace.TraceInformation("Record " + record.ID + " went from AGENTACKNOWLEDGED to " + record.Status  + " because it was inactive for " + dInactiveTime.ToString());
                                    }
                                    else
                                    {
                                        Trace.TraceWarning("Record " + record.ID + " did not go to RETRY. Current status:" + record.Status);
                                    }
                                }

                               continue;

                            case Constants.RecordStatus.AGENTABANDONED:

                                sErrorDescription = String.Empty;

                                if (_recordManager.Update(record.ID, String.Empty, String.Empty, Constants.RecordStatus.RETRY, out sErrorDescription))
                                {
                                    Trace.TraceInformation("Record " + record.ID + " went from AGENTABANDONED to " + record.Status  + " because it was inactive for " + dInactiveTime.ToString());
                                }
                                else
                                {
                                    Trace.TraceWarning("Record " + record.ID + " did not go to RETRY. Current status:" + record.Status);
                                }

                               continue;

                            case Constants.RecordStatus.IVR_FAILURE:

                                sErrorDescription = String.Empty;

                                if (_recordManager.Update(record.ID, String.Empty, String.Empty, Constants.RecordStatus.RETRY, out sErrorDescription))
                                {
                                    Trace.TraceInformation("Record " + record.ID + " went from IVR_FAILURE to " + record.Status);
                                }
                                else
                                {
                                    Trace.TraceWarning("Record " + record.ID + " did not go to RETRY. Current status:" + record.Status);
                                }

                                break;

                        }//switch (record.Status)

                        if (record.Status == Constants.RecordStatus.NEW
                            || record.Status == Constants.RecordStatus.RETRY
                            || record.Status == Constants.RecordStatus.PURGED)
                        {
                            if (record.NumberOfAttempts >= _settingsManager.ApplicationSettings.MaximumNumberOfAttempts)
                            {
                                Trace.TraceInformation("Record " + record.ID + " has reached the maximum number of attempts:" + record.NumberOfAttempts + " !!!Code added to try to remedy multiple records with same id with Requeue gadget!!!");

                                sErrorDescription = String.Empty;

                                _recordManager.Update(record.ID, String.Empty, String.Empty, Constants.RecordStatus.EXCEEDEDNUMBEROFATTEMPTS, out sErrorDescription);

                                continue;
                            }

                            //Assert NumberOFAttempts
                            if (record.Status == Constants.RecordStatus.RETRY)
                            {
                                if (record.NumberOfAttempts >= _settingsManager.ApplicationSettings.MaximumNumberOfAttempts)
                                {
                                    Trace.TraceInformation("Record " + record.ID + " has reached the maximum number of attempts:" + record.NumberOfAttempts);

                                    sErrorDescription = String.Empty;

                                    _recordManager.Update(record.ID, String.Empty, String.Empty, Constants.RecordStatus.EXCEEDEDNUMBEROFATTEMPTS, out sErrorDescription);

                                    continue;
                                }

                                if (DateTime.Now.Subtract(record.StatusLastUpdated).TotalMinutes <= _settingsManager.ApplicationSettings.MinimumIntervalBetweenRetries)
                                {
                                    Trace.TraceInformation("Record  " + record.ID + " is in status:" + record.Status + " and has not yet waited " + _settingsManager.ApplicationSettings.MinimumIntervalBetweenRetries + " mins to be attempted again.");

                                    continue;
                                }
                            }

                            String sCSQ = record.TargetCSQ;

                            CallbackContactServiceQueue _queue = null;

                            foreach (CallbackContactServiceQueue queue in _settingsManager.CallbackSettings.Queues)
                            {
                                if (sCSQ == queue.Name)
                                {
                                    _queue = queue;
                                    break;
                                }

                            }//foreach (CallbackContactServiceQueue queue in _settingsManager.CallbackSettings.Queues)

                            if (_queue != null)
                            {
                                Trace.TraceInformation("Record " + record.ID + " Settings for CSQ " + sCSQ + " were found.");

                                CallbackContactServiceQueueSettingsProfile _Profile = null;

                                if (_queue.Profile == null)
                                {
                                    Trace.TraceInformation("Record " + record.ID + " Profile for CSQ " + sCSQ + " is null; look for default profile.");

                                    foreach (CallbackContactServiceQueue queue in _settingsManager.CallbackSettings.Queues)
                                    {
                                        if (queue.Name == "Default")
                                        {
                                            _Profile = queue.Profile;
                                            break;
                                        }

                                    }//foreach (CallbackContactServiceQueue queue in _settingsManager.CallbackSettings.Queues)

                                    if (_Profile == null)
                                    {
                                        Trace.TraceWarning("Record " + record.ID + " Profile for CSQ " + sCSQ + " was not found. Flag record as invalid.");
                                        sErrorDescription = String.Empty;
                                        _recordManager.Update(record.ID, String.Empty, String.Empty, Constants.RecordStatus.INVALID, out sErrorDescription);

                                        continue;
                                    }
                                    else
                                    {
                                        Trace.TraceInformation("Record " + record.ID + " Default Profile for CSQ " + sCSQ + " was found.");
                                    }
                                }
                                else
                                {
                                    Trace.TraceInformation("Record " + record.ID + " Profile for CSQ " + sCSQ + " is not null.");
                                    _Profile = _queue.Profile;
                                }

                                //Check profile settings: 
                                if (_queue.CallbackEnabled)
                                {
                                    if (_Profile == null)
                                    {
                                        Trace.TraceWarning("Record " + record.ID + " _Profile is null.");
                                    }

                                    if (_Profile.CallbackProcessingTimeframeBegin == null)
                                    {
                                        Trace.TraceWarning("Record " + record.ID + " _Profile.CallbackProcessingTimeframeBegin is null.");
                                    }

                                    DateTime dtCallbackProcessingTimeframeBegin = DateTime.ParseExact(_Profile.CallbackProcessingTimeframeBegin, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                    DateTime dtCallbackProcessingTimeframeEnd = DateTime.ParseExact(_Profile.CallbackProcessingTimeframeEnd, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                                    Trace.TraceInformation("Record " + record.ID + " dtCallbackProcessingTimeframeBegin = " + dtCallbackProcessingTimeframeBegin.ToString() + " Now = " + DateTime.Now.ToString());

                                    if (DateTime.Now.Subtract(dtCallbackProcessingTimeframeBegin).TotalMilliseconds >= 0)
                                    {
                                        Trace.TraceInformation("Record " + record.ID + " CallbackProcessingTimeframe has opened.");

                                        if (dtCallbackProcessingTimeframeEnd.Subtract(DateTime.Now).TotalMilliseconds >= 0)
                                        {
                                            Trace.TraceInformation("Record " + record.ID + " CallbackProcessingTimeframe has not yet closed.");

                                            double dNumberOfIVRPorts = double.Parse(_settingsManager.ApplicationSettings.UCCXNumberOfIVRPorts);
                                            double dMaxIVRPortUsagePercent = double.Parse(_settingsManager.ApplicationSettings.UCCXMaxIVRPortUsagePercent);

                                            double bMaxIVRPortsAvailable = Math.Floor(dNumberOfIVRPorts * (dMaxIVRPortUsagePercent / 100.0));

                                            int iNumberOfContactsInIVR = _RealtimeDataClient.NumberOfContactsInIVR;

                                            if (bMaxIVRPortsAvailable > iNumberOfContactsInIVR)
                                            {
                                                Trace.TraceInformation("Record id " + record.ID + " Number of contacts in IVR: " + iNumberOfContactsInIVR.ToString() + " IVR Ports available to Callback Server: " + bMaxIVRPortsAvailable.ToString());

                                                if(_settingsManager.ApplicationSettings.BasicInsertionThrottling_Enabled)
                                                {
                                                    Trace.TraceInformation("Record id " + record.ID + " BasicInsertionThrottling: Enabled");

                                                    int iCallbacksInIVR = _recordManager.RecordsCurrentlyInIVR;

                                                    if (iCallbacksInIVR < _settingsManager.ApplicationSettings.BasicInsertionThrottling_MaximumRecordsAtATime)
                                                    {
                                                        Trace.TraceInformation("Record id " + record.ID + " Number of Callbacks in IVR: " + iCallbacksInIVR.ToString() + " BasicInsertionThrottling_MaximumRecordsAtATime: " + _settingsManager.ApplicationSettings.BasicInsertionThrottling_MaximumRecordsAtATime);
                                                    }
                                                    else //if(iCallbacksInIVR < _settingsManager.ApplicationSettings.BasicInsertionThrottling_MaximumRecordsAtATime)
                                                    {
                                                        Trace.TraceWarning("Record " + record.ID + " cannot be reentered: " + " Number of Callbacks in IVR: " + iCallbacksInIVR.ToString() + " BasicInsertionThrottling_MaximumRecordsAtATime: " + _settingsManager.ApplicationSettings.BasicInsertionThrottling_MaximumRecordsAtATime);

                                                        continue;

                                                    }//if(iCallbacksInIVR < _settingsManager.ApplicationSettings.BasicInsertionThrottling_MaximumRecordsAtATime)
                                                }
                                                else
                                                {
                                                    Trace.TraceInformation("Record id " + record.ID + " BasicInsertionThrottling: Disabled");
                                                }

                                                if (_RealtimeDataClient.GetContactsQueuedFor(record.TargetCSQ))
                                                {
                                                    Trace.TraceInformation("Record " + record.ID + " contactRealtimeData.GetContactsQueuedFor() returned true.");

                                                    if (_RealtimeDataClient.OrderContactsQueuedFor())
                                                    {
                                                        Trace.TraceInformation("Record " + record.ID + " contactRealtimeData.OrderContactsQueuedFor() returned true.");

                                                        int iResult = _RealtimeDataClient.GetNumberOfContactAheadOf(record.ContactID, record.ID, record.QueueStartTime);

                                                        if (iResult == -1)
                                                        {
                                                            Trace.TraceWarning("_RealtimeDataClient.GetNumberOfContactAheadOf() of recordID " + record.ID + " returned -1. Not sure what to do if false!!!!");

                                                            continue;
                                                        }
                                                        else if (iResult == -2)
                                                        {
                                                            Trace.TraceInformation("_RealtimeDataClient.GetNumberOfContactAheadOf() of recordID " + record.ID + " Not to be reentered yet; scheduled.");

                                                            continue;
                                                        }
                                                        else if (iResult == 0)
                                                        {
                                                            Trace.TraceInformation("_RealtimeDataClient.GetNumberOfContactAheadOf() of recordID " + record.ID + " :" + iResult);

                                                            Trace.TraceInformation("Record id " + record.ID + " with contactID " + record.ContactID + " can be reentered");

                                                            System.Threading.Thread thr = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(_SendReentryRequest));
                                                            thr.Start(record.ID);
                                                            thr = null;

                                                            break;
                                                        }
                                                    }
                                                    else//if (_RealtimeDataClient.OrderContactsQueuedFor())
                                                    {
                                                        Trace.TraceWarning("Record " + record.ID + " contactRealtimeData.OrderContactsQueuedFor() returned false.");

                                                    }//if (_RealtimeDataClient.OrderContactsQueuedFor())

                                                }
                                                else //if (_RealtimeDataClient.GetContactsQueuedFor(record.TargetCSQ))
                                                {
                                                    Trace.TraceWarning("Record " + record.ID + " contactRealtimeData.GetContactsQueuedFor() returned false.");

                                                }//if (_RealtimeDataClient.GetContactsQueuedFor(record.TargetCSQ))

                                            }
                                            else //if (bMaxIVRPortsAvailable > iNumberOfContactsInIVR)
                                            {
                                                Trace.TraceWarning("Record " + record.ID + " cannot be reentered: " + " Number of contacts in IVR: " + iNumberOfContactsInIVR.ToString() + " IVR Ports available to Callback Server: " + bMaxIVRPortsAvailable.ToString());

                                                continue;

                                            }//if (bMaxIVRPortsAvailable > iNumberOfContactsInIVR)                                            

                                            //Try reentry based on default algorithm
                                        }
                                        else//if (dtCallbackProcessingTimeframeEnd.Subtract(DateTime.Now).TotalMilliseconds >= 0)
                                        {
                                            Trace.TraceInformation("Record " + record.ID + " CallbackProcessingTimeframe has closed.");

                                        }//if (dtCallbackProcessingTimeframeEnd.Subtract(DateTime.Now).TotalMilliseconds >= 0)
                                    }
                                    else//if (DateTime.Now.Subtract(dtCallbackProcessingTimeframeBegin).TotalMilliseconds >= 0)
                                    {
                                        Trace.TraceInformation("Record " + record.ID + " CallbackProcessingTimeframe has not yet opened.");

                                    }//if (DateTime.Now.Subtract(dtCallbackProcessingTimeframeBegin).TotalMilliseconds >= 0)
                                }
                                else//if (_queue.CallbackEnabled)
                                {
                                    Trace.TraceWarning("Record " + record.ID + " Callback in not enabled for CSQ " + sCSQ + "; Flag record as invalid.");
                                    sErrorDescription = String.Empty;
                                    _recordManager.Update(record.ID, String.Empty, String.Empty, Constants.RecordStatus.INVALID,out sErrorDescription);
                                    continue;

                                }//if (_queue.CallbackEnabled)
                            }
                            else //if (_queue != null)
                            {
                                Trace.TraceWarning("Record " + record.ID + " Settings for CSQ " + sCSQ + " were not found. Flag record as invalid.");
                                sErrorDescription = String.Empty;
                                _recordManager.Update(record.ID, String.Empty, String.Empty, Constants.RecordStatus.INVALID,out sErrorDescription);
                                continue;

                            }//if (_queue != null)
                        }
                        else//if (record.Status != Constants.RecordStatus.INVALID)
                        {
                            Trace.TraceWarning("Record " + record.ID + " is not NEW or RETRY.");

                        }//if (record.Status != Constants.RecordStatus.INVALID)

                    }//foreach (CallbackRecord record in _recordManager.Records)
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                }

                Trace.TraceInformation("Cycle through " + _recordManager.Records.Count + " records took " + DateTime.Now.Subtract(dtBegin).TotalMilliseconds + " ms.");

                _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);

            }//lock (objLock)

            Trace.TraceInformation("Exit.");
        }

        void _SendReentryRequest(object state)
        {
            lock (objLockReentry)
            {
                try
                {
                    Trace.TraceInformation("Enter.");

                    if (state == null)
                    {
                        Trace.TraceWarning("state is null");
                        return;
                    }

                    String sRecordID = (String)state;

                    Trace.TraceInformation("Record " + sRecordID + "  Attempt reentry");

                    CallbackRecord record = _recordManager.GetRecordByID(sRecordID);

                    if (record == null)
                    {
                        Trace.TraceWarning("Could not get record " + sRecordID + " from recordManager.");
                        return;
                    }

                    Trace.TraceInformation("Record " + record.ID + " is in status " + record.Status.ToString());

                    if (record.Status != Constants.RecordStatus.NEW
                        && record.Status != Constants.RecordStatus.RETRY
                        && record.Status != Constants.RecordStatus.PURGED)
                    {
                        Trace.TraceInformation("Record " + record.ID + " is in a status that cannot be reentered.");
                        return;
                    }

                    try
                    {
                        String sRequestID = Guid.NewGuid().ToString();

                        String sURL = "http://" + _settingsManager.UCCXMasterNode + ":" + _settingsManager.ApplicationSettings.UCCXApplicationPort + "/" + _settingsManager.ApplicationSettings.UCCXCallbackPrefix + "?";

                        sURL = sURL + "id=" + System.Web.HttpUtility.UrlEncode(record.ID);
                        sURL = sURL + "&dnis=" + System.Web.HttpUtility.UrlEncode(record.DNIS);
                        sURL = sURL + "&targetcsq=" + System.Web.HttpUtility.UrlEncode(record.TargetCSQ);
                        sURL = sURL + "&prompt=" + System.Web.HttpUtility.UrlEncode(record.Prompt);
                        sURL = sURL + "&sessionid=" + System.Web.HttpUtility.UrlEncode(record.SessionID);
                        sURL = sURL + "&implid=" + System.Web.HttpUtility.UrlEncode(record.ContactImplementationID);
                        sURL = sURL + "&contactid=" + System.Web.HttpUtility.UrlEncode(record.ContactID);
                        sURL = sURL + "&language=" + System.Web.HttpUtility.UrlEncode(record.Language);
                        sURL = sURL + "&customvar1=" + System.Web.HttpUtility.UrlEncode(record.CustomVar1);
                        sURL = sURL + "&customvar2=" + System.Web.HttpUtility.UrlEncode(record.CustomVar2);
                        sURL = sURL + "&customvar3=" + System.Web.HttpUtility.UrlEncode(record.CustomVar3);
                        sURL = sURL + "&customvar4=" + System.Web.HttpUtility.UrlEncode(record.CustomVar4);
                        sURL = sURL + "&customvar5=" + System.Web.HttpUtility.UrlEncode(record.CustomVar5);
                        sURL = sURL + "&requeuecode=" + System.Web.HttpUtility.UrlEncode(record.RequeueCode);
                        sURL = sURL + "&requeuecounter=" + System.Web.HttpUtility.UrlEncode(record.RequeueCounter);
                        sURL = sURL + "&reqid=" + System.Web.HttpUtility.UrlEncode(sRequestID);

                        Trace.TraceInformation("sURL -> " + sURL);

                        String sErrorDescription = String.Empty;

                        _recordManager.Update(record.ID, String.Empty, sRequestID, Constants.RecordStatus.PROCESSING, out sErrorDescription);

                        if (SendRequestToServer(sURL, record.ID))
                        {
                            Trace.TraceInformation("SendRequestToServer() returned true for record " + record.ID);
                        }
                        else
                        {
                            Trace.TraceInformation("SendRequestToServer() returned false for record " + record.ID);

                            _recordManager.Update(record.ID, String.Empty, String.Empty, Constants.RecordStatus.RETRY, out sErrorDescription);
                        }

                        Trace.TraceInformation("Record " + record.ID + " was set to " + record.Status.ToString());
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning("Record " + record.ID + " got Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                }

            }//lock (objLockReentry)
        }

        private bool SendRequestToServer(String sURL, String RecordID)
        {
            try
            {
                Trace.TraceInformation("Enter.");

                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(sURL);

                request.Timeout = Constants.CONTACT_REENTRY_TIMEOUT;

                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();

                Trace.TraceInformation("Request for record " + RecordID + " received status code {0}", response.StatusCode);

                response.Close();
                response = null;
                request = null;

                return true;
            }
            catch (System.Net.WebException webEx)
            {
                Trace.TraceWarning("Request for record " + RecordID + " got WebException: " + webEx.Message + Environment.NewLine + "Status:" + webEx.Status);

                if (webEx.Status == System.Net.WebExceptionStatus.Timeout)
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Request for record " + RecordID + " got Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);

                return false;
            }
        }

        private bool GetResponseFromServer(String sURL, out String sResponse)
        {
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(sURL);

                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();

                Trace.TraceInformation("Content type is {0} and length is {1}", response.ContentType, response.ContentLength);

                System.IO.Stream stream = response.GetResponseStream();

                // Pipes the stream to a higher level stream reader with the required encoding format. 
                System.IO.StreamReader streamReader = new System.IO.StreamReader(stream, Encoding.UTF8);

                sResponse = streamReader.ReadToEnd();

                streamReader.Close();
                streamReader.Dispose();
                streamReader = null;

                stream.Close();
                stream.Dispose();
                stream = null;

                response.Close();
                response = null;
                request = null;

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                sResponse = String.Empty;
                return false;
            }
        }

        private bool ParseResponse(out int iCode, String sResponse)
        {
            try
            {
                System.Xml.XmlDocument xmlDoc = null;

                xmlDoc = new System.Xml.XmlDocument();

                xmlDoc.LoadXml(sResponse);

                System.Xml.XmlNode nodCode = xmlDoc.SelectSingleNode("//Response/Code");

                if (nodCode != null)
                {
                    try
                    {
                        iCode = int.Parse(nodCode.InnerText);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning("Exception casting Code to integer; Code defaulted to -1.");
                        iCode = -1;
                    }
                }
                else
                {
                    Trace.TraceWarning("//Response/Code element not found. Code defaulted to -1");
                    iCode = -1;
                }

                nodCode = null;
                xmlDoc = null;

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                iCode = -1;
                return false;
            }
        }
    }
}
