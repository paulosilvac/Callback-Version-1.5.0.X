using System;
using System.ServiceProcess;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.TraceInformation("Enter.");

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new com.workflowconcepts.applications.uccx.Service() };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
