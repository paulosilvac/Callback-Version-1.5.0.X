using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.workflowconcepts.applications.uccx
{
    public partial class ucUserManagement : UserControl
    {
        public event EventHandler Changed;

        public ucUserManagement()
        {
            InitializeComponent();
        }

        private void ucUserManagement_Load(object sender, EventArgs e)
        {
            cmbRoles.SelectedIndex = 0;
        }

        private void btnAddUpdateUser_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {

        }
    }
}
