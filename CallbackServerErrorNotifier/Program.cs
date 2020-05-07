using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                ApplicationSettings _Settings = null;

                Notifier _Notifier = null;

                try
                {
                    _Settings = new ApplicationSettings("C:\\ProgramData\\Workflow Concepts\\Callback Server");

                    if (_Settings.Load() == ApplicationTypes.ApplicationSettingsReturn.SUCCESS)
                    {
                        _Notifier = new Notifier(_Settings);

                        if (_Notifier.Email(Environment.MachineName + " - " + Constants.SERVICEDISPLAYNAME + " - Service failure", "The Callback Server Windows service appears to have failed. Please collect the logs immediately.", ApplicationTypes.NotificationType.FAILURE))
                        {
                            Trace.TraceInformation("_Notifier.Email() returned true.");
                        }
                        else
                        {
                            Trace.TraceWarning("_Notifier.Email() returned false.");
                        }
                    }
                    else
                    {
                        Trace.TraceWarning("_Settings.Load() did not return SUCCESS.");

                        _Notifier = new Notifier();

                        if (_Notifier.WriteToEventLog(Constants.SERVICEDISPLAYNAME, "CallbackServerErrorMonitor could not load C:\\ProgramData\\Workflow Concepts\\Callback Server\\ApplicationSettings.xml file.", EventLogEntryType.Error))
                        {
                            Trace.TraceInformation("notifier.WriteToEventLog() returned true.");
                        }
                        else
                        {
                            Trace.TraceWarning("notifier.WriteToEventLog() returned false.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                    _Notifier = new Notifier();

                    if (_Notifier.WriteToEventLog(Constants.SERVICEDISPLAYNAME, "Exception in CallbackServerErrorMonitor: " + ex.Message, EventLogEntryType.Error))
                    {
                        Trace.TraceInformation("notifier.WriteToEventLog() returned true.");
                    }
                    else
                    {
                        Trace.TraceWarning("notifier.WriteToEventLog() returned false.");
                    }
                }

                _Settings = null;
                _Notifier = null;
            }
            catch (Exception oex)
            {
            }
        }
    }
}
