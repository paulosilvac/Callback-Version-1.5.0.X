using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class CallbackOfferedAlgorithm
    {
        String _Description = String.Empty;

        public String Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public CallbackOfferedAlgorithm()
        {
            _Description = String.Empty;
        }

        public bool Analyse(ContactServiceQueueInformation Information, CallbackContactServiceQueueSettingsProfile Profile, CallbackRecordManager RecordManager)
        {
            Trace.TraceInformation("Enter.");

            try
            {
                if (Information == null)
                {
                    _Description = "Information is null.";
                    Trace.TraceWarning("Information is null.");
                    return false;
                }

                if (Profile == null)
                {
                    _Description = "Profile is null.";
                    Trace.TraceWarning("Profile is null.");
                    return false;
                }

                if (RecordManager == null)
                {
                    _Description = "RecordManager is null.";
                    Trace.TraceWarning("RecordManager is null.");
                    return false;
                }

                bool bResult = true;

                foreach (CallbackAlgorithmFilter filter in Profile.OfferedAlgorithmFilters)
                {
                    if (filter.Enabled)
                    {
                        try
                        {
                            Constants.FilterOperations Operation = Constants.FilterOperations.BIGGERTHANOREQUALTO;

                            Operation = (Constants.FilterOperations)Enum.Parse(typeof(Constants.FilterOperations), filter.Operation.ToUpper());

                            switch (filter.Name)
                            {
                                case "AgentsLoggedIn":

                                    switch (Operation)
                                    {
                                        case Constants.FilterOperations.BIGGERTHANOREQUALTO:

                                            if (Information.AgentsLoggedIn >= filter.Value)
                                            {
                                                bResult = bResult && true;
                                            }
                                            else
                                            {
                                                _Description = "CSQ " + Information.Name + " has less than " + filter.Value + " agents logged in.";
                                                bResult = bResult && false;
                                            }

                                            break;

                                        case Constants.FilterOperations.SMALLERTHANOREQUALTO:

                                            if (Information.AgentsLoggedIn <= filter.Value)
                                            {
                                                bResult = bResult && true;
                                            }
                                            else
                                            {
                                                _Description = "CSQ " + Information.Name + " has more than " + filter.Value + " agents logged in.";
                                                bResult = bResult && false;
                                            }

                                            break;
                                    }

                                    break;

                                case "CallsWaiting":

                                    switch (Operation)
                                    {
                                        case Constants.FilterOperations.BIGGERTHANOREQUALTO:

                                            if (Information.ContactsWaiting >= filter.Value)
                                            {
                                                bResult = bResult && true;
                                            }
                                            else
                                            {
                                                _Description = "CSQ " + Information.Name + " has less than " + filter.Value + " calls waiting.";
                                                bResult = bResult && false;
                                            }

                                            break;

                                        case Constants.FilterOperations.SMALLERTHANOREQUALTO:

                                            if (Information.ContactsWaiting <= filter.Value)
                                            {
                                                bResult = bResult && true;
                                            }
                                            else
                                            {
                                                _Description = "CSQ " + Information.Name + " has more than " + filter.Value + " calls waiting.";
                                                bResult = bResult && false;
                                            }

                                            break;
                                    }

                                    break;

                                case "LongestQueueTime":

                                    switch (Operation)
                                    {
                                        case Constants.FilterOperations.BIGGERTHANOREQUALTO:

                                            if (Information.LongestWaitingContact >= filter.Value)
                                            {
                                                bResult = bResult && true;
                                            }
                                            else
                                            {
                                                _Description = "CSQ " + Information.Name + " has longest queue time less than " + filter.Value;
                                                bResult = bResult && false;
                                            }

                                            break;

                                        case Constants.FilterOperations.SMALLERTHANOREQUALTO:

                                            if (Information.LongestWaitingContact <= filter.Value)
                                            {
                                                bResult = bResult && true;
                                            }
                                            else
                                            {
                                                _Description = "CSQ " + Information.Name + " has longest queue time more than " + filter.Value;
                                                bResult = bResult && false;
                                            }

                                            break;
                                    }

                                    break;

                                case "CallbackRequests":

                                    switch (Operation)
                                    {
                                        case Constants.FilterOperations.BIGGERTHANOREQUALTO:

                                            if (RecordManager.NumberOfRecordsForQueue(Information.Name) >= filter.Value)
                                            {
                                                bResult = bResult && true;
                                            }
                                            else
                                            {
                                                _Description = "CSQ " + Information.Name + " has fewer than " + filter.Value + " callback requests.";
                                                bResult = bResult && false;
                                            }

                                            break;

                                        case Constants.FilterOperations.SMALLERTHANOREQUALTO:

                                            if (RecordManager.NumberOfRecordsForQueue(Information.Name) <= filter.Value)
                                            {
                                                bResult = bResult && true;
                                            }
                                            else
                                            {
                                                _Description = "CSQ " + Information.Name + " has more than " + filter.Value + " callback requests:" + RecordManager.NumberOfRecordsForQueue(Information.Name);
                                                bResult = bResult && false;
                                            }

                                            break;
                                    }

                                    break;

                                default:

                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            _Description = "Exception performing algorithm comparisons.";
                            Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                            bResult = false;
                        }
                    }
                }

                return bResult;
            }
            catch (Exception ex)
            {
                _Description = "Exception running callback offer algorithm.";
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                return false;
            }
        }

        public bool Analyse(List<ContactServiceQueueInformation> Information, String TargetCSQ, CallbackContactServiceQueueSettingsProfile Profile, CallbackRecordManager RecordManager)
        {
            Trace.TraceInformation("Enter.");

            try
            {
                if (Information == null)
                {
                    _Description = "Information is null.";
                    Trace.TraceWarning("Information is null.");
                    return false;
                }

                if (TargetCSQ == null)
                {
                    _Description = "TargetCSQ is null.";
                    Trace.TraceWarning("TargetCSQ is null.");
                    return false;
                }

                if (TargetCSQ == String.Empty)
                {
                    _Description = "TargetCSQ is empty.";
                    Trace.TraceWarning("TargetCSQ is empty.");
                    return false;
                }

                if (Profile == null)
                {
                    _Description = "Profile is null.";
                    Trace.TraceWarning("Profile is null.");
                    return false;
                }

                if (RecordManager == null)
                {
                    _Description = "RecordManager is null.";
                    Trace.TraceWarning("RecordManager is null.");
                    return false;
                }

                bool bResult = true;

                ContactServiceQueueInformation _CSQInfo = null;

                foreach (ContactServiceQueueInformation CSQInfo in Information)
                {
                    if (CSQInfo.Name == TargetCSQ)
                    {
                        _CSQInfo = CSQInfo;
                        break;
                    }
                }

                if (_CSQInfo != null)
                {
                    foreach (CallbackAlgorithmFilter filter in Profile.OfferedAlgorithmFilters)
                    {
                        if (filter.Enabled)
                        {
                            try
                            {
                                Constants.FilterOperations Operation = Constants.FilterOperations.BIGGERTHANOREQUALTO;

                                Operation = (Constants.FilterOperations)Enum.Parse(typeof(Constants.FilterOperations), filter.Operation.ToUpper());

                                switch (filter.Name)
                                {
                                    case "AgentsLoggedIn":

                                        switch (Operation)
                                        {
                                            case Constants.FilterOperations.BIGGERTHANOREQUALTO:

                                                if (_CSQInfo.AgentsLoggedIn >= filter.Value)
                                                {
                                                    bResult = bResult && true;
                                                }
                                                else
                                                {
                                                    _Description = "CSQ " + _CSQInfo.Name + " has less than " + filter.Value + " agents logged in.";
                                                    bResult = bResult && false;
                                                }

                                                break;

                                            case Constants.FilterOperations.SMALLERTHANOREQUALTO:

                                                if (_CSQInfo.AgentsLoggedIn <= filter.Value)
                                                {
                                                    bResult = bResult && true;
                                                }
                                                else
                                                {
                                                    _Description = "CSQ " + _CSQInfo.Name + " has more than " + filter.Value + " agents logged in.";
                                                    bResult = bResult && false;
                                                }

                                                break;
                                        }

                                        break;

                                    case "CallsWaiting":

                                        switch (Operation)
                                        {
                                            case Constants.FilterOperations.BIGGERTHANOREQUALTO:

                                                if (_CSQInfo.ContactsWaiting >= filter.Value)
                                                {
                                                    bResult = bResult && true;
                                                }
                                                else
                                                {
                                                    _Description = "CSQ " + _CSQInfo.Name + " has less than " + filter.Value + " calls waiting.";
                                                    bResult = bResult && false;
                                                }

                                                break;

                                            case Constants.FilterOperations.SMALLERTHANOREQUALTO:

                                                if (_CSQInfo.ContactsWaiting <= filter.Value)
                                                {
                                                    bResult = bResult && true;
                                                }
                                                else
                                                {
                                                    _Description = "CSQ " + _CSQInfo.Name + " has more than " + filter.Value + " calls waiting.";
                                                    bResult = bResult && false;
                                                }

                                                break;
                                        }

                                        break;

                                    case "LongestQueueTime":

                                        switch (Operation)
                                        {
                                            case Constants.FilterOperations.BIGGERTHANOREQUALTO:

                                                if (_CSQInfo.LongestWaitingContact >= filter.Value)
                                                {
                                                    bResult = bResult && true;
                                                }
                                                else
                                                {
                                                    _Description = "CSQ " + _CSQInfo.Name + " has longest queue time less than " + filter.Value;
                                                    bResult = bResult && false;
                                                }

                                                break;

                                            case Constants.FilterOperations.SMALLERTHANOREQUALTO:

                                                if (_CSQInfo.LongestWaitingContact <= filter.Value)
                                                {
                                                    bResult = bResult && true;
                                                }
                                                else
                                                {
                                                    _Description = "CSQ " + _CSQInfo.Name + " has longest queue time more than " + filter.Value;
                                                    bResult = bResult && false;
                                                }

                                                break;
                                        }

                                        break;

                                    case "CallbackRequests":

                                        switch (Operation)
                                        {
                                            case Constants.FilterOperations.BIGGERTHANOREQUALTO:

                                                if (RecordManager.NumberOfRecordsForQueue(_CSQInfo.Name) >= filter.Value)
                                                {
                                                    bResult = bResult && true;
                                                }
                                                else
                                                {
                                                    _Description = "CSQ " + _CSQInfo.Name + " has fewer than " + filter.Value + " callback requests.";
                                                    bResult = bResult && false;
                                                }

                                                break;

                                            case Constants.FilterOperations.SMALLERTHANOREQUALTO:

                                                if (RecordManager.NumberOfRecordsForQueue(_CSQInfo.Name) <= filter.Value)
                                                {
                                                    bResult = bResult && true;
                                                }
                                                else
                                                {
                                                    _Description = "CSQ " + _CSQInfo.Name + " has more than " + filter.Value + " callback requests:" + RecordManager.NumberOfRecordsForQueue(_CSQInfo.Name);
                                                    bResult = bResult && false;
                                                }

                                                break;
                                        }

                                        break;

                                    default:

                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                _Description = "Exception performing algorithm comparisons.";
                                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                                bResult = false;
                            }

                        }//if (filter.Enabled)

                    }//foreach (CallbackAlgorithmFilter filter in Profile.OfferedAlgorithmFilters)

                    return bResult;
                }
                else
                {
                    _Description = "No realtime information found for CSQ " + TargetCSQ;
                    Trace.TraceWarning("No realtime information found for CSQ " + TargetCSQ);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _Description = "Exception running callback offer algorithm.";
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                return false;
            }
        }
    }
}
