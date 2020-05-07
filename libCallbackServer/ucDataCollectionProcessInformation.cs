using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public partial class ucDataCollectionProcessInformation : UserControl
    {
        ApplicationSettings _ApplicationSettings = null;

        public ucDataCollectionProcessInformation()
        {
            InitializeComponent();

            _ApplicationSettings = null;
        }

        public void SetApplicationSettings(ApplicationSettings Settings)
        {
            _ApplicationSettings = Settings;
        }

        public void Reset()
        {
            lblJVM.Text = String.Empty;
            lblStartupTime.Text = String.Empty;
            lblUCCXMasterNode.Text = String.Empty;
            lblContactsSummary.Text = String.Empty;
            lblCSQSummary.Text = String.Empty;
        }

        private void ucDataCollectionProcessInformation_Load(object sender, EventArgs e)
        {
            this.btnRefreshProcessInformation.Image = (Image)(new Bitmap(com.workflowconcepts.applications.uccx.Properties.Resources.gtk_refresh, new Size(24, 24)));

            lblJVM.Text = String.Empty;
            lblStartupTime.Text = String.Empty;
            lblUCCXMasterNode.Text = String.Empty;
            lblContactsSummary.Text = String.Empty;
            lblCSQSummary.Text = String.Empty;
        }

        private void btnRefreshProcessInformation_Click(object sender, EventArgs e)
        {
            if (_ApplicationSettings == null)
            {
                System.Windows.Forms.MessageBox.Show("Invalid ApplicationSettings. Make sure the Callback Server service is running." + Environment.NewLine
                                                        + "If the problem persists, please contact your system's administrator."
                                                        , System.Windows.Forms.Application.ProductName
                                                        , System.Windows.Forms.MessageBoxButtons.OK
                                                        , System.Windows.Forms.MessageBoxIcon.Warning);

                return;
            }

            DataCollectionProcessInformationClient c = new DataCollectionProcessInformationClient(_ApplicationSettings);

            if (!c.GetInformation())
            {
                System.Windows.Forms.MessageBox.Show("Error while contacting Data Collection service." + Environment.NewLine
                                                        + "Please contact your system's administrator."
                                                        , System.Windows.Forms.Application.ProductName
                                                        , System.Windows.Forms.MessageBoxButtons.OK
                                                        , System.Windows.Forms.MessageBoxIcon.Warning);
            }

            lblJVM.Text = c.JVM();
            lblStartupTime.Text = c.StartupTime();
            lblUCCXMasterNode.Text = c.UCCXMasterNode();
            lblContactsSummary.Text = c.ContactsSummary();
            lblCSQSummary.Text = c.CSQsSummary();

            c = null;
        }
    }
}
