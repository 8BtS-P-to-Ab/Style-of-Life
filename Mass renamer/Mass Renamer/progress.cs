using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mass_Renamer
{
    public partial class progress : Form
    {
        public progress()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void progress_Load(object sender, EventArgs e)
        {
            progressBar1.Value = 0;

        }
    }
}
