using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    public class CallbackAlgorithmFilter
    {
        String _Name = String.Empty;
        bool _Enabled = false;
        String _Operation = String.Empty;
        int _Value = 0;

        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; }
        }

        public String Operation
        {
            get { return _Operation; }
            set { _Operation = value; }
        }

        public int Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public CallbackAlgorithmFilter()
        {
            _Name = String.Empty;
            _Enabled = false;
            _Operation = String.Empty;
            _Value = 0;
        }
    }
}
