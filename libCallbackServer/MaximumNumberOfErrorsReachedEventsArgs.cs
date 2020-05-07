using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    public class MaximumNumberOfErrorsReachedEventsArgs:EventArgs
    {
        private String _Description = String.Empty;

        public String Description
        {
            get { return _Description; }
            set { _Description = String.Empty; }
        }

        public MaximumNumberOfErrorsReachedEventsArgs()
        {
            _Description = String.Empty;
        }

        public MaximumNumberOfErrorsReachedEventsArgs(String Description)
        {
            _Description = Description;
        }
    }
}
