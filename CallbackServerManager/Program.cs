using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            com.vanticore.utilities.debug.DebugManager dm = new com.vanticore.utilities.debug.DebugManager(Environment.GetCommandLineArgs());

            dm.Initialize(Application.ProductName, Constants.LogFilesPath, com.vanticore.utilities.debug.DebugManager.FileSizes.OneMegaByte);

            Trace.TraceInformation("Debug was turned on via the command line.");

            bool SingleInstance = false;

            System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out SingleInstance);

            if (!SingleInstance)
            {
                MessageBox.Show("Application is already running." + Environment.NewLine + "Please close other instances.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Trace.TraceError("Another instance of this application is already running." + Environment.NewLine + "This instance will terminate.");

                Application.Exit();
            }
            else
            {
                Trace.TraceWarning("First instance of this application.");
            }

            Trace.TraceInformation("Runtime: " + Environment.Version.ToString());
            Trace.TraceInformation("Product Version: " + Application.ProductVersion);

            try
            {
                System.Security.Principal.WindowsPrincipal wp = new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent());

                if (!wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
                {
                    MessageBox.Show("Current user is not part of BuiltIn\\Administrators." + Environment.NewLine + "Application will not work properly." + Environment.NewLine + "Please contact your System's Administrator.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Trace.TraceWarning("User is not part of BuiltIn\\Administrators");
                }
                else
                {
                    Trace.TraceInformation("User is part of BuiltIn\\Administrators");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception determining user group:" + ex.Message + Environment.NewLine + "Stack Trace:" + ex.StackTrace);
                MessageBox.Show("Exception determining user group:" + Environment.NewLine + ex.Message + Environment.NewLine + "Application will not work properly." + Environment.NewLine + "Please contact your System's Administrator.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Context());

            GC.KeepAlive(mutex);
        }
    }
}
