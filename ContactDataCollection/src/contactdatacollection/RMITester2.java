package contactdatacollection;

public class RMITester2 
{
    public static void main(String[] args)
    {
        try    
        {
            com.cisco.wfframework.remote.engine.RemoteReport rReport = null;
            com.cisco.wfframework.remote.engine.RemoteWorkflowEngine rEngine = null;
            
            com.cisco.wf.subsystems.corereporting.CoreRemoteReport cReport = null;
            
            //"rmi://10.1.10.210:6999/com.cisco.wfframework.remote.engine.RemoteWorkflowEngine"
            String strServiceURL = "rmi://10.1.10.210:6999/com.cisco.wf.subsystems.corereporting.CoreRemoteReport";
            
            cReport = (com.cisco.wf.subsystems.corereporting.CoreRemoteReport)java.rmi.Naming.lookup(strServiceURL);
            
            if(cReport != null)
            {
                System.out.println("cReport is not null.");
                
                
            }
            else
            {
                System.out.println("cReport is null.");
            }
        }
        catch(Exception ex)
        {
            System.out.println("Exception:" + ex.getMessage());
        }
    }
}
