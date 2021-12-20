using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;

namespace com.workflowconcepts.applications.uccx
{
    public class CallbackRecordManager
    {
        public event EventHandler<CallbackRecordStatusUpdateEventArgs> RecordStatusUpdated;
        public event EventHandler<CallbackRecordPurgeEventArgs> RecordsPurged;
        public event EventHandler<CallbackRecordStatusEndOfDayUpdateEventArgs> EndOfDayStatusUpdate;

        private object objLock = null;
        private System.Collections.Generic.List<CallbackRecord> _Records = null;

        private int iRecordCount = 0;

        private int iRecordsCurrentlyInIVR = 0;

        public System.Collections.Generic.List<CallbackRecord> Records
        {
            get { return _Records; }
        }

        public int RecordsCurrentlyInIVR
        {
            get { return iRecordsCurrentlyInIVR; }
        }

        public CallbackRecordManager()
        {
            objLock = new object();

            _Records = new List<CallbackRecord>();

            iRecordsCurrentlyInIVR = 0;
            iRecordCount = 0;
        }

        public bool GetRecordByID(string RecordID, out int Count, out CallbackRecord FirstMatchingRecord)
        {
            Trace.TraceInformation("Enter.");

            lock (objLock)
            {
                Count = 0;
                FirstMatchingRecord = null;
                
                if (string.IsNullOrEmpty(RecordID))
                {
                    Trace.TraceWarning("RecordID is either null/empty.");
                    return false;
                }

                List<CallbackRecord> records =  _Records.FindAll(r => r.ID == RecordID);

                if(records != null)
                {
                    Trace.TraceWarning($"Number of records found for RecordID:{RecordID} {records.Count}");

                    Count = records.Count;

                    if (records.Count == 0)
                    {
                    
                    }
                    else if(records.Count == 1)
                    {                        
                        FirstMatchingRecord = records[0];
                    }
                    else
                    {
                    
                    }
                }
                else
                {
                    Trace.TraceWarning($"No records found for RecordID:{RecordID}");
                }

                return true;

                //foreach (CallbackRecord record in _Records)
                //{
                //    if (record.ID == RecordID)
                //    {
                //        if(record.Status != Constants.RecordStatus.COMPLETED && record.Status != Constants.RecordStatus.DIALINGTARGET)
                //        {
                //            return record;
                //        }
                //        else
                //        {
                //            Trace.TraceWarning("A record with ID " + RecordID + " already exists but is in Status " + record.Status + "; insertion will be allowed. !!!This exception was added because of the Requeue gadget -> problematic and needs to be reviewed!!!");
                //        }
                //    }
                //}

                //Trace.TraceWarning("RecordID " + RecordID + " was not found.");

                //return null;

            }//lock (objLock)
        }

        public bool GetCountOfActiveRecordsForDNIS(string DNIS, out int Count)
        {
            Trace.TraceInformation("Enter.");

            lock (objLock)
            {
                Count = 0;

                if (string.IsNullOrEmpty(DNIS))
                {
                    Trace.TraceWarning("RecordID is either null/empty.");
                    return false;
                }

                List<CallbackRecord> records = _Records.FindAll(r => r.DNIS == DNIS);

                if (records != null)
                {
                    Trace.TraceWarning($"Number of records found for DNIS:{DNIS} {records.Count}");

                    if (records.Count == 0)
                    {
                        records = null;
                    }
                    else
                    {
                        List<CallbackRecord> recordsnotactive = records.FindAll(r => r.Status == Constants.RecordStatus.DIALINGTARGET || r.Status == Constants.RecordStatus.COMPLETED);

                        if(recordsnotactive == null)
                        {
                            Count = records.Count;
                            records = null;
                        }
                        else
                        {
                            Count = records.Count - recordsnotactive.Count;
                            records = null;
                        }
                    }
                }
                else
                {
                    Trace.TraceWarning($"No records found for DNIS:{DNIS}");
                }

                return true;

                //foreach (CallbackRecord record in _Records)
                //{
                //    if (record.DNIS == DNIS)
                //    {
                //        if (record.Status != Constants.RecordStatus.COMPLETED
                //            && record.Status != Constants.RecordStatus.EXCEEDEDNUMBEROFATTEMPTS)
                //        {
                //            return record;
                //        }
                //        else
                //        {
                //            Trace.TraceWarning("A record with DNIS " + DNIS + " already exists but is in Status " + record.Status + "; insertion will be allowed. !!!This exception was added because of the Requeue gadget -> problematic and needs to be reviewed!!!");
                //        }
                //    }
                //}

                //Trace.TraceWarning("DNIS " + DNIS + " was not found.");

                //return null;

            }//lock (objLock)
        }

        public bool Add(CallbackRecord record)
        {
            Trace.TraceInformation("Enter.");

            lock (objLock)
            {
                if (record != null)
                {
                    Trace.TraceInformation("Record list size (before):" + _Records.Count.ToString());

                    _Records.Add(record);

                    Trace.TraceInformation("Record with ID = " + record.ID + " was added to the list.");
                    Trace.TraceInformation("Record list size (after):" + _Records.Count.ToString());

                    iRecordCount = _Records.Count;

                    UpdateNumberOfRecordsCurrentlyInIVR();

                    WriteToDisk();

                    return true;
                }
                else
                {
                    Trace.TraceWarning("record is null");
                    return false;
                }

            }//lock (objLock)
        }

        public bool Remove(String RecordID)
        {
            Trace.TraceInformation("Enter.");

            lock (objLock)
            {
                if (RecordID != null && RecordID != String.Empty)
                {
                    CallbackRecord ToBeRemoved = null;

                    foreach (CallbackRecord record in _Records)
                    {
                        if (record.ID == RecordID)
                        {
                            ToBeRemoved = record;
                            break;
                        }
                    }

                    if (ToBeRemoved != null)
                    {
                        Trace.TraceInformation("Record list size (before):" + _Records.Count.ToString());
                        _Records.Remove(ToBeRemoved);
                        Trace.TraceInformation("Record with ID = " + RecordID + " was removed from the list.");
                        Trace.TraceInformation("Record list size (after):" + _Records.Count.ToString());

                        iRecordCount = _Records.Count;

                        UpdateNumberOfRecordsCurrentlyInIVR();

                        WriteToDisk();

                        return true;
                    }
                    else
                    {
                        Trace.TraceWarning("No record with id " + RecordID + " was found.");
                        return false;
                    }
                }
                else

                {
                    Trace.TraceWarning("RecordID is either null or empty.");
                    return false;
                }
            }
        }

        public bool Update(String RecordID, String AgentID, String RequestID, Constants.RecordStatus Status, out String ErrorDescription)
        {
            Trace.TraceInformation("Enter.");

            lock (objLock)
            {
                if (RecordID != null && RecordID != String.Empty)
                {
                    foreach (CallbackRecord record in _Records)
                    {
                        if (record.ID == RecordID)
                        {
                            if (Status == Constants.RecordStatus.REQUESTED
                                || Status == Constants.RecordStatus.QUEUED
                                || Status == Constants.RecordStatus.AGENTACKNOWLEDGED
                                || Status == Constants.RecordStatus.AGENTABANDONED
                                || Status == Constants.RecordStatus.DIALINGTARGET
                                || Status == Constants.RecordStatus.IVR_FAILURE)
                            {
                                if (record.ReqID != RequestID)
                                {
                                    Trace.TraceWarning("Record " + record.ID + " request id " + record.ReqID + " does not match " + RequestID);
                                    continue;
                                }
                            }

                            switch (Status) //Requested status
                            {
                                case Constants.RecordStatus.PURGED:

                                    if (record.Status != Constants.RecordStatus.NEW)
                                    {
                                        Trace.TraceWarning("Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString());
                                        ErrorDescription = "Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString();
                                        return false;
                                    }

                                    break;

                                case Constants.RecordStatus.REQUESTED:

                                    if (record.Status != Constants.RecordStatus.PROCESSING)
                                    {
                                        Trace.TraceWarning("Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString());
                                        ErrorDescription = "Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString();
                                        return false;
                                    }

                                    break;

                                case Constants.RecordStatus.QUEUED:

                                    //if (record.Status != Constants.RecordStatus.REQUESTED)
                                    //{
                                    //    Trace.TraceWarning("Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString());
                                    //    ErrorDescription = "Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString();
                                    //    return false;
                                    //}

                                    break;

                                case Constants.RecordStatus.AGENTACKNOWLEDGED:

                                    if (record.Status != Constants.RecordStatus.PROCESSING 
                                        && record.Status != Constants.RecordStatus.QUEUED)
                                    {
                                        Trace.TraceWarning("Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString());
                                        ErrorDescription = "Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString();
                                        return false;
                                    }

                                    break;

                                case Constants.RecordStatus.DIALINGTARGET:

                                    if (record.Status != Constants.RecordStatus.PROCESSING 
                                        && record.Status != Constants.RecordStatus.QUEUED 
                                        && record.Status != Constants.RecordStatus.AGENTACKNOWLEDGED)
                                    {
                                        Trace.TraceWarning("Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString());
                                        ErrorDescription = "Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString();
                                        return false;
                                    }

                                    break;

                                case Constants.RecordStatus.AGENTABANDONED:

                                    if (record.Status != Constants.RecordStatus.PROCESSING 
                                        && record.Status != Constants.RecordStatus.QUEUED 
                                        && record.Status != Constants.RecordStatus.AGENTACKNOWLEDGED)
                                    {
                                        Trace.TraceWarning("Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString());
                                        ErrorDescription = "Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString();
                                        return false;
                                    }

                                    break;

                                case Constants.RecordStatus.IVR_FAILURE:

                                    if (record.Status != Constants.RecordStatus.REQUESTED
                                        && record.Status != Constants.RecordStatus.QUEUED
                                        && record.Status != Constants.RecordStatus.AGENTACKNOWLEDGED
                                        && record.Status != Constants.RecordStatus.AGENTABANDONED
                                        && record.Status != Constants.RecordStatus.DIALINGTARGET)
                                    {
                                        Trace.TraceWarning("Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString());
                                        ErrorDescription = "Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString();
                                        return false;
                                    }

                                    break;

                                case Constants.RecordStatus.RETRY:

                                    if (record.Status != Constants.RecordStatus.REQUESTED 
                                        && record.Status != Constants.RecordStatus.PROCESSING
                                        && record.Status != Constants.RecordStatus.QUEUED
                                        && record.Status != Constants.RecordStatus.AGENTABANDONED
                                        && record.Status != Constants.RecordStatus.AGENTACKNOWLEDGED
                                        && record.Status != Constants.RecordStatus.EXCEEDEDNUMBEROFATTEMPTS
                                        && record.Status != Constants.RecordStatus.COMPLETED)
                                    {
                                        Trace.TraceWarning("Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString());
                                        ErrorDescription = "Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString();
                                        return false;
                                    }

                                    break;

                                case Constants.RecordStatus.FORCERETRY:

                                    if (record.Status != Constants.RecordStatus.COMPLETED
                                        && record.Status != Constants.RecordStatus.EXCEEDEDNUMBEROFATTEMPTS
                                        && record.Status != Constants.RecordStatus.INVALID)
                                    {
                                        Trace.TraceWarning("Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString());
                                        ErrorDescription = "Record " + record.ID + " cannot go to " + Status.ToString() + " status from " + record.Status.ToString();
                                        return false;
                                    }

                                    break;
                            }

                            switch (Status)
                            {
                                case Constants.RecordStatus.PROCESSING:

                                    record.Status = Status;
                                    record.StatusLastUpdated = DateTime.Now;
                                    record.ReqID = RequestID;
                                    record.NumberOfAttempts++;

                                    break;

                                case Constants.RecordStatus.RETRY:

                                    record.Status = Status;
                                    record.StatusLastUpdated = DateTime.Now;
                                    record.ReqID = String.Empty;

                                    break;

                                case Constants.RecordStatus.FORCERETRY:

                                    record.Status = Constants.RecordStatus.RETRY;
                                    record.StatusLastUpdated = DateTime.Now;
                                    record.ReqID = String.Empty;
                                    record.NumberOfAttempts = 0;

                                    break;

                                case Constants.RecordStatus.AGENTACKNOWLEDGED:

                                    record.Status = Status;
                                    record.StatusLastUpdated = DateTime.Now;

                                    record.AgentID = AgentID;
                                    record.AgentAcknowledgedAt = DateTime.Now;

                                    break;

                                case Constants.RecordStatus.QUEUED:

                                    if (record.Status == Constants.RecordStatus.REQUESTED)
                                    {
                                        record.Status = Status;
                                        record.StatusLastUpdated = DateTime.Now;

                                        record.QueuedAt = DateTime.Now;
                                    }
                                    else if (record.Status == Constants.RecordStatus.QUEUED)
                                    {
                                        record.StatusLastUpdated = DateTime.Now;
                                    }
                                    else
                                    {
                                        record.Status = Status;
                                        record.StatusLastUpdated = DateTime.Now;

                                        record.QueuedAt = DateTime.Now;
                                    }

                                    break;

                                case Constants.RecordStatus.DIALINGTARGET:

                                    record.Status = Status;
                                    record.StatusLastUpdated = DateTime.Now;

                                    record.TargetDialedAt = DateTime.Now;

                                    break;

                                case Constants.RecordStatus.IVR_FAILURE:

                                    record.Status = Status;
                                    record.StatusLastUpdated = DateTime.Now;
                                    record.ReqID = String.Empty;

                                    break;

                                case Constants.RecordStatus.INVALID:

                                    record.Status = Status;
                                    record.StatusLastUpdated = DateTime.Now;
                                    record.ReqID = String.Empty;

                                    break;

                                case Constants.RecordStatus.INACTIVE:

                                    record.Status = Status;
                                    record.StatusLastUpdated = DateTime.Now;
                                    record.ReqID = String.Empty;

                                    break;

                                case Constants.RecordStatus.REQUESTED:

                                    record.Status = Status;
                                    record.StatusLastUpdated = DateTime.Now;

                                    break;

                                case Constants.RecordStatus.AGENTABANDONED:

                                    record.Status = Status;
                                    record.StatusLastUpdated = DateTime.Now;

                                    record.AgentID = AgentID;

                                    break;

                                case Constants.RecordStatus.EXCEEDEDNUMBEROFATTEMPTS:

                                    record.Status = Status;
                                    record.StatusLastUpdated = DateTime.Now;
                                    record.ReqID = String.Empty;

                                    break;

                                case Constants.RecordStatus.COMPLETED:

                                    record.Status = Status;
                                    record.StatusLastUpdated = DateTime.Now;
                                    record.ReqID = String.Empty;

                                    break;

                                case Constants.RecordStatus.PURGED:

                                    record.Status = Status;
                                    record.StatusLastUpdated = DateTime.Now;
                                    record.ReqID = String.Empty;

                                    break;
                            }

                            if (Status == Constants.RecordStatus.INVALID)
                            {
                                if (RecordStatusUpdated != null)
                                {
                                    RecordStatusUpdated(this, new CallbackRecordStatusUpdateEventArgs(record));
                                }
                            }

                            UpdateNumberOfRecordsCurrentlyInIVR();

                            WriteToDisk();

                            ErrorDescription = String.Empty;

                            return true;

                        }//if (record.ID == RecordID)

                    }//foreach (CallbackRecord record in _Records)

                    ErrorDescription = "No record was found with RecordID";

                    return false;
                }
                else//if (RecordID != null && RecordID != String.Empty)
                {
                    Trace.TraceWarning("RecordID is either null or empty.");
                    ErrorDescription = "RecordID is either null or empty";
                    return false;

                }//if (RecordID != null && RecordID != String.Empty)

            }//lock (objLock)
        }

        public String GetRecordListingAsXML()
        {
            Trace.TraceInformation("Enter.");

            if (_Records == null)
            {
                Trace.TraceWarning("_Records is null.");

                return String.Empty;
            }

            try
            {

                StringBuilder sb = new StringBuilder();

                sb.Append("<callbackrecords>");

                foreach (CallbackRecord record in _Records)
                {
                    sb.Append("<record>");

                    sb.Append("<id>" + record.ID + "</id>");
                    sb.Append("<contactid>" + record.ContactID + "</contactid>");
                    sb.Append("<implid>" + record.ContactImplementationID + "</implid>");
                    sb.Append("<sessionid>" + record.SessionID + "</sessionid>");
                    sb.Append("<dnis>" + record.DNIS + "</dnis>");
                    sb.Append("<origincsq>" + record.OriginCSQ + "</origincsq>");
                    sb.Append("<targetcsq>" + record.TargetCSQ + "</targetcsq>");
                    sb.Append("<prompt>" + record.Prompt + "</prompt>");
                    sb.Append("<language>" + record.Language + "</language>");
                    sb.Append("<customvar1>" + record.CustomVar1.Replace("&","%3d") + "</customvar1>");
                    sb.Append("<customvar2>" + record.CustomVar2.Replace("&", "%3d") + "</customvar2>");
                    sb.Append("<customvar3>" + record.CustomVar3.Replace("&", "%3d") + "</customvar3>");
                    sb.Append("<customvar4>" + record.CustomVar4.Replace("&", "%3d") + "</customvar4>");
                    sb.Append("<customvar5>" + record.CustomVar5.Replace("&", "%3d") + "</customvar5>");
                    sb.Append("<requestdate>" + record.RequestDate.ToString() + "</requestdate>");
                    sb.Append("<statuslastupdated>" + record.StatusLastUpdated.ToString() + "</statuslastupdated>");
                    sb.Append("<numberofattempts>" + record.NumberOfAttempts.ToString() + "</numberofattempts>");
                    sb.Append("<status>" + record.Status.ToString() + "</status>");
                    sb.Append("<agentid>" + record.AgentID + "</agentid>");
                    sb.Append("<queuedat>" + record.QueuedAt.ToString() + "</queuedat>");
                    sb.Append("<agentacknowledgedat>" + record.AgentAcknowledgedAt.ToString() + "</agentacknowledgedat>");
                    sb.Append("<targetdialedat>" + record.TargetDialedAt.ToString() + "</targetdialedat>");
                    sb.Append("<requeuecode>" + record.RequeueCode + "</requeuecode>");
                    sb.Append("<requeuecounter>" + record.RequeueCounter + "</requeuecounter>");

                    sb.Append("</record>");

                }//foreach (CallbackRecord record in _Records)

                sb.Append("</callbackrecords>");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return String.Empty;
            }
        }

        public String GetRecordsByCSQAsXML()
        {
            Trace.TraceInformation("Enter.");

            if (_Records == null)
            {
                Trace.TraceWarning("_Records is null.");

                return String.Empty;
            }

            try
            {
                System.Collections.Hashtable _summary = new System.Collections.Hashtable();
                
                foreach (CallbackRecord record in _Records)
                {
                    if(record.Status != Constants.RecordStatus.INVALID
                        && record.Status != Constants.RecordStatus.INACTIVE
                        && record.Status != Constants.RecordStatus.COMPLETED
                        && record.Status != Constants.RecordStatus.PURGED
                        && record.Status != Constants.RecordStatus.EXCEEDEDNUMBEROFATTEMPTS)
                    {
                        if (!_summary.ContainsKey(record.TargetCSQ))
                        {
                            _summary.Add(record.TargetCSQ, new RealtimeReportsRecordsByCSQ(record));
                        }
                        else
                        {
                            ((RealtimeReportsRecordsByCSQ)_summary[record.TargetCSQ]).AddContact(record);
                        }
                    }

                }//foreach (CallbackRecord record in _Records)

                StringBuilder sb = new StringBuilder();

                sb.Append("<callbackrecords>");

                foreach(String csq in _summary.Keys)
                {
                    
                    sb.Append("<record>");

                    sb.Append("<csqname>" + csq + "</csqname>");
                    sb.Append("<contactswaiting>" + ((RealtimeReportsRecordsByCSQ)_summary[csq]).ContactsWaiting + "</contactswaiting>");
                    sb.Append("<avgcontactwaiting>" + ((RealtimeReportsRecordsByCSQ)_summary[csq]).AvgQueueTime + "</avgcontactwaiting>");
                    sb.Append("<oldestcontactwaiting>" + ((RealtimeReportsRecordsByCSQ)_summary[csq]).LongestQueueTime + "</oldestcontactwaiting>");

                    sb.Append("</record>");
                }

                sb.Append("</callbackrecords>");

                _summary = null;

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return String.Empty;
            }
        }

        public void WriteToDisk()
        {
            Trace.TraceInformation("Enter.");

            if (_Records == null)
            {
                Trace.TraceWarning("_Records is null.");

                return;
            }

            try
            {
                using (System.IO.Stream stream = System.IO.File.Open(Constants.ApplicationSettingsFilePath + "\\Cache.bin", System.IO.FileMode.Create))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    bformatter.Serialize(stream, _Records);
                }

                Trace.TraceInformation("Number of records serialized to file: " + _Records.Count);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
            }
        }

        public bool ReadFromDisk()
        {
            Trace.TraceInformation("Enter.");

            try
            {
                using (System.IO.Stream stream = System.IO.File.Open(Constants.ApplicationSettingsFilePath + "\\Cache.bin", System.IO.FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    _Records = (List<CallbackRecord>)bformatter.Deserialize(stream);
                }

                if (_Records == null)
                {
                    Trace.TraceInformation("No records were deserialized from file");
                }
                else
                {
                    Trace.TraceInformation("Number of records deserialed: " + _Records.Count);
                }

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return false;
            }
        }

        public int GetRecordCount()
        {
            Trace.TraceInformation("Enter.");

            if (_Records == null)
            {
                return 0;
            }
            else
            {
                return _Records.Count;
            }
        }

        public int NumberOfActionableRecords()
        {
            Trace.TraceInformation("Enter.");

            lock (objLock)
            {
                if (_Records == null)
                {
                    return 0;
                }
                else//if (_Records == null)
                {
                    int iCount = 0;

                    foreach (CallbackRecord record in _Records)
                    {
                        if (record.Status != Constants.RecordStatus.INVALID 
                            && record.Status != Constants.RecordStatus.COMPLETED
                            && record.Status != Constants.RecordStatus.EXCEEDEDNUMBEROFATTEMPTS)
                        {
                            iCount++;
                        }

                    }//foreach (CallbackRecord record in _Records)

                    return iCount;

                }//if (_Records == null)

            }//lock (objLock)
        }

        public int NumberOfRecordsForQueue(String QueueName)
        {
            Trace.TraceInformation("Enter.");

            lock (objLock)
            {
                if (QueueName == null)
                {
                    Trace.TraceWarning("QueueName is null.");
                    return 0;
                }

                if (QueueName == String.Empty)
                {
                    Trace.TraceWarning("QueueName is empty.");
                    return 0;
                }

                if (_Records == null)
                {
                    return 0;
                }
                else
                {
                    int iCount = 0;

                    foreach (CallbackRecord record in _Records)
                    {
                        if (record.TargetCSQ == QueueName)
                        {
                            if (record.Status != Constants.RecordStatus.COMPLETED
                                && record.Status != Constants.RecordStatus.INACTIVE
                                && record.Status != Constants.RecordStatus.INVALID)
                            {
                                iCount++;
                            }
                        }
                    }

                    return iCount;
                }
            }
        }

        public int GetContactPositionOffset(String ContactID, String TargetCSQ)
        {
            Trace.TraceInformation("Enter.");

            if (_Records == null)
            {
                Trace.TraceWarning("_Records is null.");
                return -1;
            }

            if (_Records.Count == 0)
            {
                return 0;
            }

            try
            {
                int iCount = 0;

                foreach (CallbackRecord record in _Records)
                {
                    if (record.Status == Constants.RecordStatus.NEW
                        || record.Status == Constants.RecordStatus.RETRY
                        || record.Status == Constants.RecordStatus.PURGED)
                    {

                        if (record.TargetCSQ.Equals(TargetCSQ))
                        {
                            if (ContactID.CompareTo(record.ContactID) >= 0)
                            {
                                iCount++;
                            }

                        }//if (record.TargetCSQ.Equals(TargetCSQ))

                    }

                }//foreach (CallbackRecord record in _Records)

                return iCount;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return -1;
            }
        }

        public int GetNumberOfContactAheadOf(CallbackRecord record)
        {
            try
            {
                if (_Records == null)
                {
                    Trace.TraceWarning("_Records is null");
                    return -1;
                }

                if (record == null)
                {
                    Trace.TraceWarning("record is null");
                    return -1;
                }

                int iNumberOfRecordsAhead = 0;

                lock (objLock)
                {
                    if (_Records.Count == 0)
                    {
                        return 0;
                    }

                    foreach (CallbackRecord r in _Records)
                    {
                        if (r.ID != record.ID)
                        {
                            if (r.QueueStartTime <= record.QueueStartTime)
                            {
                                iNumberOfRecordsAhead++;
                            }

                        }//if (r.ID != record.ID)

                    }//foreach (CallbackRecord r in _Records)

                }//lock (objLock)

                return iNumberOfRecordsAhead;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return -1;
            }
        }

        public bool Purge()
        {
            Trace.TraceInformation("Enter.");

            try
            {
                if (_Records == null)
                {
                    Trace.TraceWarning("_Records == null");
                    return false;
                }

                if (_Records.Count == 0)
                {
                    Trace.TraceWarning("_Records == empty");
                    return true;
                }

                System.Collections.Generic.List<CallbackRecord> ToBeRemoved = new List<CallbackRecord>();

                foreach (CallbackRecord record in _Records)
                {
                    if (record.Purge)
                    {
                        ToBeRemoved.Add(record);

                        Trace.TraceInformation("Record with ID = " + record.ID + " was added to the list to be removed.");
                    }

                }//foreach (CallbackRecord record in _Records)

                if (ToBeRemoved.Count > 0)
                {
                    foreach (CallbackRecord record in ToBeRemoved)
                    {
                        _Records.Remove(record);

                        Trace.TraceInformation("Record with ID = " + record.ID + " was removed.");
                    }

                    WriteToDisk();

                    if (RecordsPurged != null)
                    {
                        RecordsPurged(this, new CallbackRecordPurgeEventArgs(ToBeRemoved));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return false;
            }
        }

        public bool RemoveRecordsInFinalState()
        {
            Trace.TraceInformation("Enter.");

            try
            {
                if (_Records == null)
                {
                    Trace.TraceWarning("_Records == null");
                    return false;
                }

                if (_Records.Count == 0)
                {
                    Trace.TraceWarning("_Records == empty");
                    return true;
                }

                System.Collections.Generic.List<CallbackRecord> ToBeRemoved = new List<CallbackRecord>();

                foreach (CallbackRecord record in _Records)
                {
                    if (record.Status == Constants.RecordStatus.COMPLETED)
                    {
                        ToBeRemoved.Add(record);

                        Trace.TraceInformation("Record with ID = " + record.ID + " was added to the list to be removed.");
                    }

                }//foreach (CallbackRecord record in _Records)

                if (ToBeRemoved.Count > 0)
                {
                    foreach (CallbackRecord record in ToBeRemoved)
                    {
                        _Records.Remove(record);

                        Trace.TraceInformation("Record with ID = " + record.ID + " was removed.");
                    }

                    WriteToDisk();

                    iRecordCount = _Records.Count;
                }

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return false;
            }
        }

        public bool StatusReport()
        {
            Trace.TraceInformation("Enter.");

            try
            {
                if (_Records == null)
                {
                    Trace.TraceWarning("_Records == null");
                    return false;
                }

                if (_Records.Count == 0)
                {
                    Trace.TraceWarning("_Records == empty");
                    return true;
                }

                System.Collections.Generic.List<CallbackRecord> ToBeReportedOn = new List<CallbackRecord>();

                foreach (CallbackRecord record in _Records)
                {
                    if (record.ReportOn)
                    {
                        record.ReportOn = false;
                        ToBeReportedOn.Add(record);
                    }

                }//foreach (CallbackRecord record in _Records)

                if (ToBeReportedOn.Count > 0)
                {
                    if (EndOfDayStatusUpdate != null)
                    {
                        EndOfDayStatusUpdate(this, new CallbackRecordStatusEndOfDayUpdateEventArgs(ToBeReportedOn));
                    }
                }

                ToBeReportedOn = null;

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return false;
            }
        }

        public bool AssertBelowSystemLimit()
        {
            Trace.TraceInformation("Enter.");

            //lock (objLock)
            //{
            //    try
            //    {
            //        if (_Records.Count < Constants.MAXIMUM_NUMBER_OF_REQUESTS)
            //        {
            //            return true;
            //        }
            //        else
            //        {
            //            Trace.TraceWarning("Number of requests in the system:" + _Records.Count + " System limit:" + Constants.MAXIMUM_NUMBER_OF_REQUESTS);
            //            return false;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
            //        return false;
            //    }

            if(iRecordCount <= Constants.MAXIMUM_NUMBER_OF_REQUESTS)
            {
                return true;
            }
            else
            {
                return false;
            }

            //}//lock (objLock)
        }

        private void UpdateNumberOfRecordsCurrentlyInIVR()
        {
            try
            {
                if (_Records == null)
                {
                    Trace.TraceWarning("_Records == null");
                    iRecordsCurrentlyInIVR = 0;
                    return;
                }

                if (_Records.Count == 0)
                {
                    Trace.TraceWarning("_Records == empty");
                    iRecordsCurrentlyInIVR = 0;
                    return;
                }

                iRecordsCurrentlyInIVR = _Records.Where(
                                                    x => x.Status == Constants.RecordStatus.PROCESSING
                                                    || x.Status == Constants.RecordStatus.REQUESTED
                                                    || x.Status == Constants.RecordStatus.QUEUED
                                                    || x.Status == Constants.RecordStatus.AGENTACKNOWLEDGED
                                                    || x.Status == Constants.RecordStatus.AGENTABANDONED
                                                    || x.Status == Constants.RecordStatus.DIALINGTARGET).Count();

                return;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                iRecordsCurrentlyInIVR = 0;
                return;
            }
        }
    }
}
