using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    public class MaximumNumberOfCollisionsReachedEventsArgs:EventArgs
    {
        private String _Description = String.Empty;

        public String Description
        {
            get { return _Description; }
            set { _Description = String.Empty; }
        }

        public MaximumNumberOfCollisionsReachedEventsArgs()
        {
            _Description = String.Empty;
        }

        public MaximumNumberOfCollisionsReachedEventsArgs(String Description)
        {
            _Description = Description;
        }
    }
}
