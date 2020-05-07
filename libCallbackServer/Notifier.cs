using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class Notifier
    {
        private string _EmailFrom = string.Empty;
        private string _EmailTo = string.Empty;
        private string _SMTPServer = string.Empty;
        private string _SMTPPort = string.Empty;
        private string _SMTPUserName = string.Empty;
        private string _SMTPPassword = string.Empty;

        private bool _EmailOnFailure = false;
        private bool _EmailOnSuccess = false;

        public String EmailFrom
        {
            get { return _EmailFrom; }
            set { _EmailFrom = value; }
        }

        public String EmailTo
        {
            get { return _EmailTo; }
            set { _EmailTo = value; }
        }

        public String SMTPServer
        {
            get { return _SMTPServer; }
            set { _SMTPServer = value; }
        }

        public String SMTPPort
        {
            get { return _SMTPPort; }
            set { _SMTPPort = value; }
        }

        public String SMTPUserName
        {
            get { return _SMTPUserName; }
            set { _SMTPUserName = value; }
        }

        public String SMTPPassword
        {
            get { return _SMTPPassword; }
            set { _SMTPPassword = value; }
        }

        public Notifier()
        {
            _EmailFrom = string.Empty;
            _EmailTo = string.Empty;
            _SMTPServer = string.Empty;
            _SMTPPort = string.Empty;
            _SMTPUserName = string.Empty;
            _SMTPPassword = string.Empty;

            _EmailOnFailure = false;
            _EmailOnSuccess = false;
        }

        public Notifier(string EmailFrom, string EmailTo, string SMTPServer, string SMTPPort, string User, string Password, bool EmailOnSuccess, bool EmailOnFailure)
        {
            _EmailFrom = EmailFrom;
            _EmailTo = EmailTo;
            _SMTPServer = SMTPServer;
            _SMTPPort = SMTPPort;
            _SMTPUserName = User;
            _SMTPPassword = Password;

            _EmailOnFailure = EmailOnFailure;
            _EmailOnSuccess = EmailOnSuccess;
        }

        public Notifier(ApplicationTypes.iApplicationSettings Settings)
        {
            if (Settings != null)
            {

                _EmailFrom = Settings.EmailFrom;
                _EmailTo = Settings.EmailTo;
                _SMTPServer = Settings.SMTPServer;
                _SMTPPort = Settings.SMTPPort;
                _SMTPUserName = Settings.SMTPUserName;
                _SMTPPassword = Settings.SMTPPassword;

                _EmailOnFailure = Settings.EmailOnFailure;
                _EmailOnSuccess = Settings.EmailOnSuccess;
            }
            else
            {
                Trace.TraceError("Settings is null; default values will be assumed.");

                _EmailFrom = string.Empty;
                _EmailTo = string.Empty;
                _SMTPServer = string.Empty;
                _SMTPPort = string.Empty;
                _SMTPUserName = string.Empty;
                _SMTPPassword = string.Empty;

                _EmailOnFailure = false;
                _EmailOnSuccess = false;
            }
        }

        public bool Email(string Subject, string Body, ApplicationTypes.NotificationType NotificationType)
        {
            Trace.TraceInformation("Enter.");

            try
            {
                switch (NotificationType)
                {
                    case ApplicationTypes.NotificationType.FAILURE:

                        if (!_EmailOnFailure)
                        {
                            Trace.TraceWarning("Email on failure is disabled.");
                            return false;
                        }

                        break;

                    case ApplicationTypes.NotificationType.SUCCESS:

                        if (!_EmailOnSuccess)
                        {
                            Trace.TraceWarning("Email on success is disabled.");
                            return false;
                        }

                        break;
                }

                if (_EmailFrom == string.Empty)
                {
                    Trace.TraceWarning("EmailFrom field has not been provided.");
                    return false;
                }

                if (_EmailTo == string.Empty)
                {
                    Trace.TraceWarning("EmailTo field has not been provided.");
                    return false;
                }

                if (_SMTPServer == string.Empty)
                {
                    Trace.TraceWarning("SMTPServer field has not been provided.");
                    return false;
                }

                if (_SMTPPort == string.Empty)
                {
                    Trace.TraceWarning("SMTPPort field has not been provided.");
                    return false;
                }

                System.Net.Mail.MailMessage _email = new System.Net.Mail.MailMessage();

                _email.Subject = Subject;
                _email.Body = Body;

                _email.From = new System.Net.Mail.MailAddress(_EmailFrom);

                foreach (string _ToAddress in _EmailTo.Split(';'))
                {
                    _email.To.Add(new System.Net.Mail.MailAddress(_ToAddress));
                }

                System.Net.Mail.SmtpClient _SMTPClient = new System.Net.Mail.SmtpClient(_SMTPServer, int.Parse(_SMTPPort));

                if (_SMTPUserName.Length > 0)
                {
                    //Trace.TraceInformation("UserName appears to have been specified for the SMTP server.");
                    _SMTPClient.Credentials = new System.Net.NetworkCredential(_SMTPUserName, _SMTPPassword);
                }
                else
                {
                    //Trace.TraceInformation("No UserName was specified for the SMTP server.");
                }

                _SMTPClient.Timeout = 20000;

                _SMTPClient.Send(_email);

                _email.Dispose();
                _email = null;

                //_SMTPClient.Dispose();
                _SMTPClient = null;

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
                return false;
            }
        }

        public bool WriteToEventLog(string EventSource, string EventMessage, System.Diagnostics.EventLogEntryType EventType)
        {
            try
            {
                EventLog.WriteEntry(EventSource, EventMessage, EventType);

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                return false;
            }
        }
    }
}
