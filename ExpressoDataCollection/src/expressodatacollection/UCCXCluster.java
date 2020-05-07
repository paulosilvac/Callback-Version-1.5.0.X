package expressodatacollection;

import org.tanukisoftware.wrapper.WrapperManager;

public class UCCXCluster 
{
    private String _UCCXNode1IPAddress = "";
    private String _UCCXNode1RTRPort = "";
    
    private String _UCCXNode2IPAddress = "";
    private String _UCCXNode2RTRPort = "";
    
    private String _CurrentMasterIPAddress = "";
    private String _CurrentMasterRTRPort = "";
    
    public UCCXCluster(ApplicationSettings Settings)
    {
        if(Settings != null)
        {
            this._UCCXNode1IPAddress = Settings.getUCCXNode1IPAddress();
            this._UCCXNode1RTRPort = Settings.getUCCXNode1RTRPort();
            
            this._UCCXNode2IPAddress = Settings.getUCCXNode2IPAddress();
            this._UCCXNode2RTRPort = Settings.getUCCXNode2RTRPort();
        }        
    }
    
    public String getUCCXNode1IPAddress()
    {
        return this._UCCXNode1IPAddress;
    }
    
    public String getUCCXNode1RTRPort()
    {
        return this._UCCXNode1RTRPort;
    }
    
    public String getUCCXNode2IPAddress()
    {
        return this._UCCXNode2IPAddress;
    }
    
    public String getUCCXNode2RTRPort()
    {
        return this._UCCXNode2RTRPort;
    }
    
    public synchronized String getCurrentMasterIPAddress()
    {
        return this._CurrentMasterIPAddress;
    }
    
    public synchronized String getCurrentMasterRTRPort()
    {
        return this._CurrentMasterRTRPort;
    }
    
    public synchronized void setCurrentMaster(String NodeIPAddress, String NodeRTRPort)
    {
        this._CurrentMasterIPAddress = NodeIPAddress;
        this._CurrentMasterRTRPort = NodeRTRPort;
        
        WrapperManager.log(WrapperManager.WRAPPER_LOG_LEVEL_INFO,this.getClass().getName() + ".setCurrentMaster(): Node Master IP Addres: " + NodeIPAddress + " Node Master Port: " + NodeRTRPort);
        //System.out.println("NodeMonitor.run(): " + Thread.currentThread().getName() + " has started.");        
    }
}
