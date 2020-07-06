using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public partial class ucBasicInsertionThrottling : UserControl
    {
        public event EventHandler Changed;
        public ucBasicInsertionThrottling()
        {
            InitializeComponent();
        }

        private void ucBasicInsertionThrottling_Load(object sender, EventArgs e)
        {

        }

        private void ckbEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            txtMaximumRecordsAtATime.Enabled = !ckbEnabled.Checked;
            txtMaximumRecordsAtATime.Enabled = ckbEnabled.Checked;

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtMaximumRecordsAtATime_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtMaximumRecordsAtATime_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtMaximumRecordsAtATime_Leave(object sender, EventArgs e)
        {
            if (txtMaximumRecordsAtATime.Text.Length == 0)
            {
                txtMaximumRecordsAtATime.Text = Constants.BASIC_INSERTION_THROTTLING_MAXIMUM_RECORDS_AT_A_TIME.ToString();
            }
            else
            {
                try
                {
                    int i = int.Parse(txtMaximumRecordsAtATime.Text);

                    if (i <= 0)
                    {
                        txtMaximumRecordsAtATime.Text = Constants.BASIC_INSERTION_THROTTLING_MAXIMUM_RECORDS_AT_A_TIME.ToString();
                    }
                }
                catch
                {
                    txtMaximumRecordsAtATime.Text = Constants.BASIC_INSERTION_THROTTLING_MAXIMUM_RECORDS_AT_A_TIME.ToString();
                }
            }
        }
    }
}
