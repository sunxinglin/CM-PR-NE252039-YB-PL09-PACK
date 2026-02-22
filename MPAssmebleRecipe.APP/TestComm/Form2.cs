using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestComm
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public Action<string> OnDataChange;
        private void button1_Click(object sender, EventArgs e)
        {
            string str = this.textBox1.Text.Trim();
            OnDataChange?.Invoke(str);
            this.Close();
        }
    }
}
