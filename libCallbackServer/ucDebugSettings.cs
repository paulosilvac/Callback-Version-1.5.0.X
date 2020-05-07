using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public partial class ucDebugSettings : UserControl
    {
        public event EventHandler Changed;

        public ucDebugSettings()
        {
            InitializeComponent();
        }

        private void ckbDebugEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            cbDebugLevel.Enabled = ckbDebugEnabled.Checked;
            cbRetainUnit.Enabled = ckbDebugEnabled.Checked;
            txtRetainValue.Enabled = ckbDebugEnabled.Checked;

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtRetainValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void cbDebugLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void cbRetainUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtRetainValue_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtRetainValue_Leave(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (txtRetainValue.Text.Length == 0)
            {
                txtRetainValue.Text = "100";
            }
        }

        private void cbDebugLevel_Leave(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (this.cbDebugLevel.Text.Length == 0)
            {
                this.cbDebugLevel.Text = "Verbose";
            }
        }
    }
}
