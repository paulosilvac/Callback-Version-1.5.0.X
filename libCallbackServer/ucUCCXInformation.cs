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
    public partial class ucUCCXInformation : UserControl
    {
        public event EventHandler Changed;

        public ucUCCXInformation()
        {
            InitializeComponent();
        }

        private void ucUCCXInformation_Load(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtUCCXNode1IPAddress_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtUCCXNode2IPAddress_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtUCCXApplicationPort_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtUCCXApplicationPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtUCCXRealtimeDataPort_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtUCCXRealtimeDataPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtAuthorizationPrefix_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtUCCXAdminUser_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtUCCXAdminPassword_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtRealtimePrefix_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtCallbackPrefix_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtNumberOfIVRPorts_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtNumberOfIVRPorts_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtMaxIVRPortUsagePercent_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtMaxIVRPortUsagePercent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
