package contactdatacollection;

public class TestResult 
{
    public int NumberOfRecords = 0;
    public boolean MethodExecutedWithoutErros = true;
    public long OperationDuration = 0L;
    public String ErrorDescription = "";
    
    public TestResult()
    {
        NumberOfRecords = 0;
        OperationDuration = 0L;
        MethodExecutedWithoutErros = false;
        ErrorDescription = "";
    }
    
    public TestResult(int NumberOfRecords, long OperationDuration, boolean MethodExecutedWithoutErros, String ErrorDescription)
    {
        this.NumberOfRecords = NumberOfRecords;
        this.OperationDuration = OperationDuration;
        this.MethodExecutedWithoutErros = MethodExecutedWithoutErros;
        this.ErrorDescription = ErrorDescription;
    }
}
