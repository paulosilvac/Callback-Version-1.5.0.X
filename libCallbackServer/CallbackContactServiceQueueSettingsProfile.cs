using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    public class CallbackContactServiceQueueSettingsProfile
    {
        bool _CallerRecording = false;
        int _RetentionPeriod = 0;
        bool _EmailAlerts = false;
        String _AppServerURLPrefix = String.Empty;
        String _AdminEmail = String.Empty;
        bool _CallerIDVerify = false;
        bool _AbandonCallback = false;
        int _AbandonCBMinQTime = 0;
        int _AbandonCBMinInterCallTime = 0;

        List<CallbackBackupCSQ> _BackupCSQs = null;

        String _AcceptCallbacksTimeframeBegin = String.Empty;
        String _AcceptCallbacksTimeframeEnd = String.Empty;

        List<CallbackAlgorithmFilter> _ReentryAlgorithmFilters = null;
        List<CallbackAlgorithmFilter> _OfferedAlgorithmFilters = null;

        String _CallbackProcessingTimeframeBegin = String.Empty;
        String _CallbackProcessingTimeframeEnd = String.Empty;

        bool _EndOfDayPurgeCallbackRequests = false;

        public bool CallerRecording
        {
            get { return _CallerRecording; }
            set { _CallerRecording = value; }
        }

        public int RetentionPeriod
        {
            get { return _RetentionPeriod; }
            set { _RetentionPeriod = value; }
        }

        public bool EmailAlerts
        {
            get { return _EmailAlerts; }
            set { _EmailAlerts = value; }
        }

        public String AdminEmail
        {
            get { return _AdminEmail; }
            set { _AdminEmail = value; }
        }

        public String AppServerURLPrefix
        {
            get { return _AppServerURLPrefix; }
            set { _AppServerURLPrefix = value; }
        }

        public bool CallerIDVerify
        {
            get { return _CallerIDVerify; }
            set { _CallerIDVerify = value; }
        }

        public bool AbandonCallback
        {
            get { return _AbandonCallback; }
            set { _AbandonCallback = value; }
        }

        public int AbandonCBMinQTime
        {
            get { return _AbandonCBMinQTime; }
            set { _AbandonCBMinQTime = value; }
        }

        public int AbandonCBMinInterCallTime
        {
            get { return _AbandonCBMinInterCallTime; }
            set { _AbandonCBMinInterCallTime = value; }
        }

        public List<CallbackBackupCSQ> BackupCSQs
        {
            get { return _BackupCSQs; }
            set { _BackupCSQs = value; }
        }

        public String AcceptCallbacksTimeframeBegin
        {
            get { return _AcceptCallbacksTimeframeBegin; }
            set { _AcceptCallbacksTimeframeBegin = value; }
        }

        public String AcceptCallbacksTimeframeEnd
        {
            get { return _AcceptCallbacksTimeframeEnd; }
            set { _AcceptCallbacksTimeframeEnd = value; }
        }

        public List<CallbackAlgorithmFilter> ReentryAlgorithmFilters
        {
            get { return _ReentryAlgorithmFilters; }
            set { _ReentryAlgorithmFilters = value; }
        }

        public List<CallbackAlgorithmFilter> OfferedAlgorithmFilters
        {
            get { return _OfferedAlgorithmFilters; }
            set { _OfferedAlgorithmFilters = value; }
        }

        public String CallbackProcessingTimeframeBegin
        {
            get { return _CallbackProcessingTimeframeBegin; }
            set { _CallbackProcessingTimeframeBegin = value; }
        }

        public String CallbackProcessingTimeframeEnd
        {
            get { return _CallbackProcessingTimeframeEnd; }
            set { _CallbackProcessingTimeframeEnd = value; }
        }

        public bool EndOfDayPurgeCallbackRequests
        {
            get { return _EndOfDayPurgeCallbackRequests; }
            set { _EndOfDayPurgeCallbackRequests = value; }
        }

        public CallbackContactServiceQueueSettingsProfile()
        {
            _CallerRecording = false;
            _RetentionPeriod = 0;
            _EmailAlerts = false;
            _AppServerURLPrefix = String.Empty;
            _AdminEmail = String.Empty;
            _CallerIDVerify = false;
            _AbandonCallback = false;
            _AbandonCBMinQTime = 0;
            _AbandonCBMinInterCallTime = 0;
            _BackupCSQs = new List<CallbackBackupCSQ>();
            _AcceptCallbacksTimeframeBegin = String.Empty;
            _AcceptCallbacksTimeframeEnd = String.Empty;
            _CallbackProcessingTimeframeBegin = String.Empty;
            _CallbackProcessingTimeframeEnd = String.Empty;
            _ReentryAlgorithmFilters = new List<CallbackAlgorithmFilter>();
            _OfferedAlgorithmFilters = new List<CallbackAlgorithmFilter>();
        }
    }
}
