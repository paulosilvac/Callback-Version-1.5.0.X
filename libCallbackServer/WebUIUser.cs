using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    [Serializable]
    public class WebUIUser
    {
        private String _Name = String.Empty;
        private String _Password = String.Empty;
        private String _Role = String.Empty;

        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public String Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        public String Role
        {
            get { return _Role; }
            set { _Role = value; }
        }

        public WebUIUser()
        {
            _Name = String.Empty;
            _Password = String.Empty;
            _Role = String.Empty;
        }

        public WebUIUser(String Name, String Password, String Role)
        {
            _Name = Name;
            _Password = Password;
            _Role = Role;
        }
    }
}
