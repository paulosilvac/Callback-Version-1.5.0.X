using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace com.workflowconcepts.applications.uccx
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            try
            {
                System.Diagnostics.Process.Start(Path() + "bin\\InstallTestWrapper-NT.bat");
            }
            catch (Exception ex)
            {
                try
                {
                    System.Diagnostics.Process.Start(Path() + "bin\\UninstallTestWrapper-NT.bat");

                    System.Diagnostics.Process.Start(Path() + "bin\\InstallTestWrapper-NT.bat");
                }
                catch (Exception innerEx)
                {
                    System.Windows.Forms.MessageBox.Show("Exception: " + innerEx.Message, "Contact Data Collections Installer", System.Windows.Forms.MessageBoxButtons.OK);
                }
            }
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);

            try
            {
                System.Diagnostics.Process.Start(Path() + "bin\\UninstallTestWrapper-NT.bat");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Exception: " + ex.Message, "Contact Data Collections Installer", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

        private String Path()
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();

            return asm.Location.Substring(0, asm.Location.LastIndexOf("\\") + 1);
        }
    }
}
