using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    public class ContactServiceQueueInformation
    {
        String sID = String.Empty;
        String sName = String.Empty;
        int iAgentsLoggedIn = 0;
        int iAgentsNotReady = 0;
        int iAgentsReady = 0;
        int iAgentsTalking = 0;
        int iAgentsWork = 0;
        int iContactsWaiting =0;
        int iLongestWaitingContact = 0;

        public String ID
        {
            get { return sID; }
            set { sID = value; }
        }

        public String Name
        {
            get { return sName; }
            set { sName = value; }
        }

        public int AgentsLoggedIn
        {
            get { return iAgentsLoggedIn; }
            set { iAgentsLoggedIn = value; }
        }

        public int AgentsTalking
        {
            get { return iAgentsTalking; }
            set { iAgentsTalking = value; }
        }

        public int AgentsReady
        {
            get { return iAgentsReady; }
            set { iAgentsReady = value; }
        }
        
        public int AgentsNotReady
        {
            get { return iAgentsNotReady; }
            set { iAgentsNotReady = value; }
        }

        public int AgentsWork
        {
            get { return iAgentsWork; }
            set { iAgentsWork = value; }
        }

        public int ContactsWaiting
        {
            get { return iContactsWaiting; }
            set { iContactsWaiting = value; }
        }

        public int LongestWaitingContact
        {
            get { return iLongestWaitingContact; }
            set { iLongestWaitingContact = value; }
        }

        public ContactServiceQueueInformation()
        {
            sID = String.Empty;
            sName = String.Empty;
            iAgentsLoggedIn = 0;
            iAgentsTalking = 0;
            iAgentsReady = 0;
            iAgentsNotReady = 0;
            iAgentsWork = 0;
            iContactsWaiting = 0;
            iLongestWaitingContact = 0;
        }

        public ContactServiceQueueInformation(String ID, String Name, int AgentsLoggedIn, int AgentsTalking, int AgentsReady, int AgentsNotReady, int AgentsWork, int ContactsWaiting, int LongestWaitingContact)
        {
            sID = ID;
            sName = Name;
            iAgentsLoggedIn = AgentsLoggedIn;
            iAgentsTalking = AgentsTalking;
            iAgentsReady = AgentsReady;
            iAgentsNotReady = AgentsNotReady;
            iAgentsWork = AgentsWork;
            iContactsWaiting = ContactsWaiting;
            iLongestWaitingContact = LongestWaitingContact;
        }
    }
}
