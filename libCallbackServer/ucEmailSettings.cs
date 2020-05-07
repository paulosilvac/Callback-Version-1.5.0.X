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
    public partial class ucEmailSettings : UserControl
    {
        public event EventHandler Changed;

        public ucEmailSettings()
        {
            InitializeComponent();
        }

        private void ucEmailSettings_Load(object sender, EventArgs e)
        {

        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            Cursor.Current = Cursors.WaitCursor;

            if (this.txtEmailFrom.Text == string.Empty)
            {
                MessageBox.Show("Invalid value for Email From.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmailFrom.Focus();
                return;
            }

            if (this.txtEmailTo.Text == string.Empty)
            {
                MessageBox.Show("Invalid value for Email To.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmailTo.Focus();
                return;
            }

            if (this.txtSMTPServer.Text == string.Empty)
            {
                MessageBox.Show("Invalid value for SMTP Server.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSMTPServer.Focus();
                return;
            }

            if (this.txtSMTPPort.Text == string.Empty)
            {
                MessageBox.Show("Invalid value for SMTP Port.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSMTPPort.Focus();
                return;
            }

            try
            {
                Notifier notifier = new Notifier(txtEmailFrom.Text, txtEmailTo.Text, txtSMTPServer.Text, txtSMTPPort.Text, txtSMTPUser.Text, txtSMTPPassword.Text,true,true);

                if (notifier.Email(Application.ProductName + " - Test Email","",ApplicationTypes.NotificationType.SUCCESS))
                {
                    MessageBox.Show("Test email was sent.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Test email was not sent.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                notifier = null;

            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                MessageBox.Show("Error occurred while sending the test email." + Environment.NewLine + "Please contact your System's Administrator.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Cursor.Current = Cursors.Default;
        }        

        private void txtEmailFrom_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtEmailTo_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtSMTPServer_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtSMTPPort_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtSMTPUser_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtSMTPPassword_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void ckbEmailFailureNotifications_CheckedChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            txtEmailFrom.Enabled = ckbEmailFailureNotifications.Checked || ckbEmailSuccessNotifications.Checked;
            txtEmailTo.Enabled = ckbEmailFailureNotifications.Checked || ckbEmailSuccessNotifications.Checked;
            txtSMTPServer.Enabled = ckbEmailFailureNotifications.Checked || ckbEmailSuccessNotifications.Checked;
            txtSMTPUser.Enabled = ckbEmailFailureNotifications.Checked || ckbEmailSuccessNotifications.Checked;
            txtSMTPPassword.Enabled = ckbEmailFailureNotifications.Checked || ckbEmailSuccessNotifications.Checked;

            btnTest.Enabled = ckbEmailFailureNotifications.Checked || ckbEmailSuccessNotifications.Checked;

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void ckbEmailSuccessNotifications_CheckedChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            txtEmailFrom.Enabled = ckbEmailFailureNotifications.Checked || ckbEmailSuccessNotifications.Checked;
            txtEmailTo.Enabled = ckbEmailFailureNotifications.Checked || ckbEmailSuccessNotifications.Checked;
            txtSMTPServer.Enabled = ckbEmailFailureNotifications.Checked || ckbEmailSuccessNotifications.Checked;
            txtSMTPUser.Enabled = ckbEmailFailureNotifications.Checked || ckbEmailSuccessNotifications.Checked;
            txtSMTPPassword.Enabled = ckbEmailFailureNotifications.Checked || ckbEmailSuccessNotifications.Checked;

            btnTest.Enabled = ckbEmailFailureNotifications.Checked || ckbEmailSuccessNotifications.Checked;

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtSMTPPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
