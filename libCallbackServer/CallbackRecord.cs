using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    [Serializable]
    public class CallbackRecord
    {
        private String _ID = String.Empty;
        private String _DNIS = String.Empty;
        private String _TargetCSQ = String.Empty;
        private String _OriginCSQ = String.Empty;
        private String _Prompt = String.Empty;
        private Constants.RecordStatus _Status = Constants.RecordStatus.NEW;
        private DateTime _RequestDate = DateTime.Now;
        private String _ContactImplementationID = string.Empty;
        private String _ContactID = string.Empty;
        private String _SessionID = string.Empty;
        private DateTime _ReentryDate = DateTime.Now;
        private DateTime _StatusLastUpdated = DateTime.Now;
        private DateTime _QueuedAt = DateTime.Now;
        private long _QueueStartTime = 0L;
        private DateTime _AgentAcknowledgedAt = DateTime.Now;
        private DateTime _TargetDialedAt = DateTime.Now;
        private String _AgentID = String.Empty;
        private String _Language = String.Empty;
        private String _CustomVar1 = String.Empty;
        private String _CustomVar2 = String.Empty;
        private String _CustomVar3 = String.Empty;
        private String _CustomVar4 = String.Empty;
        private String _CustomVar5 = String.Empty;
        private String _ReqID = String.Empty;
        private String _RequeueCode = String.Empty;
        private String _RequeueCounter = String.Empty;

        private int _NumberOfAttempts = 0;

        private bool bCallbackProcessingTimeframeBeginCrossed = false;
        private bool bCallbackProcessingTimeframeEndCrossed = false;

        private bool bPurge = false;

        private bool bPurgeDueToAge = false;

        private bool bReportOn = false;

        public String ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public String DNIS
        {
            get { return _DNIS; }
            set { _DNIS = value; }
        }

        public String TargetCSQ
        {
            get { return _TargetCSQ; }
            set { _TargetCSQ = value; }
        }

        public String OriginCSQ
        {
            get { return _OriginCSQ; }
            set { _OriginCSQ = value; }
        }

        public String Prompt
        {
            get { return _Prompt; }
            set { _Prompt = value; }
        }

        public Constants.RecordStatus Status
        {
            get { return _Status; }
            set 
            { 
                _Status = value;
                _StatusLastUpdated = DateTime.Now;
            }
        }

        public DateTime RequestDate
        {
            get { return _RequestDate; }
            set { _RequestDate = value; }
        }

        public DateTime QueuedAt
        {
            //get { return _QueuedAt; }
            get 
            {
                try
                {
                    return (new DateTime(1970, 1, 1)).AddMilliseconds(_QueueStartTime);
                }
                catch
                {
                    Trace.TraceWarning("Contact ID:" + ID + " Exception going from epoch to DateTime; _QueuedAt assumed.");
                    return _QueuedAt; 
                }
            }
            set { _QueuedAt = value; }
        }

        public long QueueStartTime
        {
            get { return _QueueStartTime; }
            set { _QueueStartTime = value; }
        }

        public DateTime AgentAcknowledgedAt
        {
            get { return _AgentAcknowledgedAt; }
            set { _AgentAcknowledgedAt = value; }
        }

        public DateTime TargetDialedAt
        {
            get { return _TargetDialedAt; }
            set { _TargetDialedAt = value; }
        }

        public DateTime StatusLastUpdated
        {
            get { return _StatusLastUpdated; }
            set { _StatusLastUpdated = value; }
        }

        public String ContactImplementationID
        {
            get { return _ContactImplementationID; }
            set { _ContactImplementationID = value; }
        }

        public String ContactID
        {
            get { return _ContactID; }
            set { _ContactID = value; }
        }

        public String SessionID
        {
            get { return _SessionID; }
            set { _SessionID = value; }
        }

        public DateTime ReentryDate
        {
            get { return _ReentryDate; }
            set { _ReentryDate = value; }
        }

        public String AgentID
        {
            get { return _AgentID; }
            set { _AgentID = value; }
        }

        public String Language
        {
            get { return _Language; }
            set { _Language = value; }
        }

        public String CustomVar1
        {
            get { return _CustomVar1; }
            set { _CustomVar1 = value; }
        }

        public String CustomVar2
        {
            get { return _CustomVar2; }
            set { _CustomVar2 = value; }
        }

        public String CustomVar3
        {
            get { return _CustomVar3; }
            set { _CustomVar3 = value; }
        }

        public String CustomVar4
        {
            get { return _CustomVar4; }
            set { _CustomVar4 = value; }
        }

        public String CustomVar5
        {
            get { return _CustomVar5; }
            set { _CustomVar5 = value; }
        }

        public String RequeueCode
        {
            get { return _RequeueCode; }
            set { _RequeueCode = value; }
        }

        public String RequeueCounter
        {
            get { return _RequeueCounter; }
            set { _RequeueCounter = value; }
        }

        public int NumberOfAttempts
        {
            get { return _NumberOfAttempts; }
            set { _NumberOfAttempts = value; }
        }

        public bool CallbackProcessingTimeframeBeginCrossed
        {
            get { return bCallbackProcessingTimeframeBeginCrossed; }
            set { bCallbackProcessingTimeframeBeginCrossed = value; }
        }

        public bool CallbackProcessingTimeframeEndCrossed
        {
            get { return bCallbackProcessingTimeframeEndCrossed; }
            set { bCallbackProcessingTimeframeEndCrossed = value; }
        }

        public bool Purge
        {
            get { return bPurge; }
            set { bPurge = value; }
        }

        public bool PurgeDueToAge
        {
            get { return bPurgeDueToAge; }
            set { bPurgeDueToAge = value; }
        }

        public bool ReportOn
        {
            get { return bReportOn; }
            set { bReportOn = value; }
        }

        public String ReqID
        {
            get { return _ReqID; }
            set { _ReqID = value; }
        }

        public CallbackRecord()
        {
            _ID = String.Empty;
            _ReqID = String.Empty;
            _DNIS = String.Empty;
            _TargetCSQ = String.Empty;
            _OriginCSQ = String.Empty;
            _Prompt = String.Empty;
            _Status = Constants.RecordStatus.NEW;
            _AgentID = String.Empty;
            _RequestDate = DateTime.Now;
            _StatusLastUpdated = DateTime.Now;
            _ContactImplementationID = String.Empty;
            _ContactID = String.Empty;
            _SessionID = String.Empty;
            _ReentryDate = DateTime.Now;
            _QueuedAt = DateTime.MinValue;
            _QueueStartTime = 0L;
            _AgentAcknowledgedAt = DateTime.MinValue;
            _TargetDialedAt = DateTime.MinValue;
            _NumberOfAttempts = 0;
            bCallbackProcessingTimeframeBeginCrossed = true;
            bCallbackProcessingTimeframeEndCrossed = true;
            bPurge = false;
            bPurgeDueToAge = false;
            bReportOn = false;
            _Language = String.Empty;
            _CustomVar1 = String.Empty;
            _CustomVar2 = String.Empty;
            _CustomVar3 = String.Empty;
            _CustomVar4 = String.Empty;
            _CustomVar5 = String.Empty;
            _RequeueCode = String.Empty;
            _RequeueCounter = "0";
        }

        public CallbackRecord(String ID, String ReqID, String DNIS, String OriginCSQ, String TargetCSQ, String Prompt, Constants.RecordStatus Status, DateTime RequestDate, String QueueStartTime, String ContactImplementationID, String ContactID, String SessionID, DateTime ReentryDate, String Language, String CustomVar1, String CustomVar2, String CustomVar3, String CustomVar4, String CustomVar5, String RequeueCode, String RequeueCounter)
        {
            _ID = ID;
            _ReqID = ReqID;
            _DNIS = DNIS;
            _OriginCSQ = OriginCSQ;
            _TargetCSQ = TargetCSQ;
            _Prompt = Prompt;
            _Status = Status;
            _RequestDate = RequestDate;
            _StatusLastUpdated = DateTime.Now;
            _AgentID = String.Empty;
            _ContactImplementationID = ContactImplementationID;
            _ContactID = ContactID;
            _SessionID = SessionID;
            _ReentryDate = ReentryDate;
            _QueuedAt = DateTime.MinValue;

            try
            {
                long lMillisecondsSinceEpoch = long.Parse(QueueStartTime);

                _QueueStartTime = lMillisecondsSinceEpoch;
            }
            catch
            {
                Trace.TraceWarning("Contact ID:" + ID + " Exception casting QueueStartTime:" + QueueStartTime + "; zero assumed.");
                _QueueStartTime = 0L;
            }

            _AgentAcknowledgedAt = DateTime.MinValue;
            _TargetDialedAt = DateTime.MinValue;
            _NumberOfAttempts = 0;
            bCallbackProcessingTimeframeBeginCrossed = true;
            bCallbackProcessingTimeframeEndCrossed = true;
            bPurge = false;
            bPurgeDueToAge = false;
            bReportOn = false;
            _Language = Language;
            _CustomVar1 = CustomVar1;
            _CustomVar2 = CustomVar2;
            _CustomVar3 = CustomVar3;
            _CustomVar4 = CustomVar4;
            _CustomVar5 = CustomVar5;
            _RequeueCode = RequeueCode;
            _RequeueCounter = RequeueCounter;
        }
    }
}
