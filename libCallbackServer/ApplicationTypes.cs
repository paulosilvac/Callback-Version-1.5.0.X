using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    namespace ApplicationTypes
    {
        public enum ApplicationSettingsReturn { FILE_NOT_FOUND, ERROR, SUCCESS, NONE, INVALID_VALUE };
        public enum NotificationType { FAILURE, SUCCESS };

        public interface iRequestHandler
        {
            bool Handle(System.Net.HttpListenerContext Context, Guid ReqID);
        }

        public interface iInformationServer
        {
            bool GetInformation(out String Information);
        }

        public interface iApplicationSettings
        {
            bool EmailOnSuccess
            {
                get;
                set;
            }

            bool EmailOnFailure
            {
                get;
                set;
            }

            String EmailFrom
            {
                get;
                set;
            }

            String EmailTo
            {
                get;
                set;
            }

            String SMTPServer
            {
                get;
                set;
            }

            String SMTPPort
            {
                get;
                set;
            }

            String SMTPUserName
            {
                get;
                set;
            }

            String SMTPPassword
            {
                get;
                set;
            }

            String UCCXNode1IPAddress
            {
                get;
                set;
            }

            String UCCXApplicationPort
            {
                get;
                set;
            }

            String UCCXAuthorizationPrefix
            {
                get;
                set;
            }

            String UCCXAdminUser
            {
                get;
                set;
            }

            String UCCXAdminPassword
            {
                get;
                set;
            }

            String EncryptedUCCXAdminUser
            {
                get;
            }

            String EncryptedUCCXAdminPassword
            {
                get;
            }

            ApplicationSettingsReturn Load();

            ApplicationSettingsReturn Save();

            bool ParseArgs(string[] args);
        }

        public delegate void SettingsCallback(iApplicationSettings Settings);
    }
}
