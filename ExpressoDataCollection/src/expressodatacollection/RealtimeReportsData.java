package expressodatacollection;

public class RealtimeReportsData 
{
    private String _Data = "";
    
    public RealtimeReportsData()
    {
        _Data = "";
    }
    
    public synchronized String getData()
    {
        return this._Data;
    }
    
    public synchronized void setData(String Data)
    {
        this._Data = Data;
    }
}
