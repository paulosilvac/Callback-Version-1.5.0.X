using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class ContactQueuedInformation : IComparable<ContactQueuedInformation>
    {
        String sID = String.Empty;
        String sType = String.Empty;
        String sImplID = String.Empty;
        String sStartTime = String.Empty;
        String sDuration = String.Empty;
        String sApplication = String.Empty;
        String sTask = String.Empty;
        String sSession = String.Empty;
        String sQueuedFor = String.Empty;

        String[] aQueuedFor = null;

        public String ID
        {
            get { return sID; }
            set { sID = value; }
        }

        public String Type
        {
            get { return sType; }
            set { sType = value; }
        }

        public String ImplID
        {
            get { return sImplID; }
            set { sImplID = value; }
        }

        public String StartTime
        {
            get { return sStartTime; }
            set { sStartTime = value; }
        }

        public String Duration
        {
            get { return sDuration; }
            set { sDuration = value; }
        }

        public String Application
        {
            get { return sApplication; }
            set { sApplication = value; }
        }

        public String Task
        {
            get { return sTask; }
            set { sTask = value; }
        }

        public String Session
        {
            get { return sSession; }
            set { sSession = value; }
        }

        public String QueuedFor
        {
            get { return sQueuedFor; }
            set 
            { 
                sQueuedFor = value;

                try
                {
                    if (sQueuedFor == null || sQueuedFor == String.Empty)
                    {
                        aQueuedFor = new String[] { };
                    }

                    String sQueuedForCleansed = sQueuedFor.Replace("[", "").Replace("]", "");

                    aQueuedFor = sQueuedForCleansed.Split(',');
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("Exception while parsing sQueuedFor");
                    aQueuedFor = new String[] { };
                }
            }
        }

        public ContactQueuedInformation()
        {
            sID = String.Empty;
            sType = String.Empty;
            sImplID = String.Empty;
            sStartTime = String.Empty;
            sDuration = String.Empty;
            sApplication = String.Empty;
            sTask = String.Empty;
            sSession = String.Empty;
            sQueuedFor = String.Empty;
            aQueuedFor = new String[] { };
        }

        public ContactQueuedInformation(String ID, String Type, String ImplID, String StartTime, String Duration, String Application, String Task, String Session, String QueuedFor)
        {
            sID = ID;
            sType = Type;
            sImplID = ImplID;
            sStartTime = StartTime;
            sDuration = Duration;
            sApplication = Application;
            sTask = Task;
            sSession = Session;
            sQueuedFor = QueuedFor;

            try
            {
                if (sQueuedFor == null || sQueuedFor == String.Empty)
                {
                    aQueuedFor = new String[] { };
                }

                String sQueuedForCleansed = sQueuedFor.Replace("[", "").Replace("]", "");

                aQueuedFor =  sQueuedForCleansed.Split(',');
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception while parsing sQueuedFor");
                aQueuedFor =  new String[] { };
            }
        }

        public bool IsContactQueuedFor(String CSQ)
        {
            try
            {
                if (CSQ == null)
                {
                    Trace.TraceWarning("CSQ is null.");
                    return false;
                }

                if (CSQ == String.Empty)
                {
                    Trace.TraceWarning("CSQ is empty.");
                    return false;
                }

                if (aQueuedFor == null)
                {
                    Trace.TraceWarning("aQueuedFor is null.");
                    return false;
                }

                if (aQueuedFor.Length == 0)
                {
                    Trace.TraceWarning("aQueuedFor is empty.");
                    return false;
                }

                foreach (String sQueue in aQueuedFor)
                {
                    if (sQueue == CSQ)
                    {
                        //Trace.TraceInformation("Contact is queued for " + CSQ);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
                return false;
            }
        }

        public int CompareTo(ContactQueuedInformation comparePart)
        {
            if (comparePart == null)
            {
                return -1;
            }
            else
            {
                return this.ID.CompareTo(comparePart.ID);
            }
        }
    }
}
