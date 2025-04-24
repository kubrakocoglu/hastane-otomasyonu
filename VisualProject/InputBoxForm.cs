using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisualProject
{
    public partial class InputBoxForm : Form
    {
        public string UserInput { get; private set; }

        public InputBoxForm(string prompt)
        {
            InitializeComponent();
            labelPrompt.Text = prompt;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            UserInput = textBoxInput.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        
    }
}
