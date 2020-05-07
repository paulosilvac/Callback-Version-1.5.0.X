using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class Context : System.Windows.Forms.ApplicationContext
    {
        frmCallbackServerManager _frmMainForm = null;

        public Context()
        {
            System.Windows.Forms.Application.ApplicationExit += new EventHandler(Application_ApplicationExit); ;
            System.Windows.Forms.Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            _frmMainForm = new frmCallbackServerManager();

            _frmMainForm.FormClosing += new System.Windows.Forms.FormClosingEventHandler(_frmMainForm_FormClosing);

            _frmMainForm.Show();
        }

        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Trace.TraceInformation("Enter.");

            System.Windows.Forms.Application.ApplicationExit -= new EventHandler(Application_ApplicationExit); ;
            System.Windows.Forms.Application.ThreadException -= new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            try
            {
                Trace.TraceError("ThreadException:" + e.Exception.Message + Environment.NewLine + "StackTrace:" + e.Exception.StackTrace);

                if (e.Exception.InnerException != null)
                {
                    Trace.TraceError("InnerException:" + e.Exception.InnerException.Message + Environment.NewLine + "StackTrace:" + e.Exception.InnerException.StackTrace);
                }

            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);
            }
            finally
            {
                System.Windows.Forms.MessageBox.Show("Unexpected exception in application." + Environment.NewLine
                                                        + "ThreadException:" + e.Exception.Message + Environment.NewLine
                                                        + "StackTrace:" + e.Exception.StackTrace + Environment.NewLine
                                                        + "Please contact your system's administrator.", System.Windows.Forms.Application.ProductName, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                System.Windows.Forms.Application.Exit();
            }
        }

        void Application_ApplicationExit(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            System.Windows.Forms.Application.ApplicationExit -= Application_ApplicationExit;
            System.Windows.Forms.Application.ThreadException -= Application_ThreadException;
        }

        void _frmMainForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            Trace.TraceInformation("Enter.");

            TerminateApplication();
        }

        void TerminateApplication()
        {
            Trace.TraceInformation("Enter.");

            if (_frmMainForm != null)
            {
                _frmMainForm.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(_frmMainForm_FormClosing);
            }


            Environment.Exit(0);
        }
    }
}
