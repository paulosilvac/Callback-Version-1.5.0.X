package expressodatacollection;

import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;

import org.tanukisoftware.wrapper.WrapperManager;

public class Server
{    
    public Server()
    {
        
    }
   
    public static void main(String args[])
    {        
        System.out.println("Server.main(): Enter.");
        System.out.println("Server.main(): Version: 1.0.0.100");
        System.out.println("Server.main(): Runtme:" + System.getProperty("java.version"));
        
        ApplicationSettings _ApplicationSettings = new ApplicationSettings();
        
        UCCXCluster _Cluster = null;
        RealtimeReportsData _Data = null;
        
        ArrayList<java.net.Socket> Clients = new ArrayList<java.net.Socket>();
        
        try
        {
        
            if(_ApplicationSettings.Load())
            {
                System.out.println("Server.main(): _ApplicationSettings.Load() returned true.");
                System.out.println("Server.main(): _ApplicationSettings.Load() Data Collection IP: " + _ApplicationSettings.getDataCollectorIPAddress());
                System.out.println("Server.main(): _ApplicationSettings.Load() Data Collection Port: " + _ApplicationSettings.getDataCollectorPort());
                System.out.println("Server.main(): _ApplicationSettings.Load() Node1 IP: " + _ApplicationSettings.getUCCXNode1IPAddress());
                System.out.println("Server.main(): _ApplicationSettings.Load() Node1 Port: " + _ApplicationSettings.getUCCXNode1RTRPort());
                System.out.println("Server.main(): _ApplicationSettings.Load() Node2 IP: " + _ApplicationSettings.getUCCXNode2IPAddress());
                System.out.println("Server.main(): _ApplicationSettings.Load() Node2 Port: " + _ApplicationSettings.getUCCXNode2RTRPort());
                
            }
            else
            {
                System.out.println("Server.main(): _ApplicationSettings.Load() returned false.");
                
                return;
            }
        }
        catch(Exception ex)
        {
            System.out.println("Server.main(): Exception loading ApplicationSettings: " + ex.getMessage());
            return;
        }
        
        _Cluster = new UCCXCluster(_ApplicationSettings);
        
        NodeMonitor mnm = new NodeMonitor(_Cluster);
        
        _Data = new RealtimeReportsData();
        
        RMIClient rmic = new RMIClient(_Cluster,_Data);
        
        
        
        System.out.println("Server.main(): Server will terminate...");
     }
}
