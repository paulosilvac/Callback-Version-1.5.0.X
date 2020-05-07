using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace com.smi.ivr.proxyservices.manager
{
    public partial class ucWebServerSettings : UserControl
    {
        public event EventHandler Changed;

        public ucWebServerSettings()
        {
            InitializeComponent();
        }

        private void ucWebServerSettings_Load(object sender, EventArgs e)
        {

        }

        private void txtWebServerIPAddress_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtWebServerPrefix_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtWebServerPort_TextChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Enter.");

            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtWebServerPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtContactDataCollectionPort_TextChanged(object sender, EventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void txtContactDataCollectionPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
