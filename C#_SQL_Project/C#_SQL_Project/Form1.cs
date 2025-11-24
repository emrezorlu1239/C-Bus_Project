using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C__SQL_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            ControlClass.control = true;
            
            Register_Form register_Form = new Register_Form();
            register_Form.Show();
            this.Hide();

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            ControlClass.control = false;
            
            Register_Form register_Form = new Register_Form();
            register_Form.Show();
            this.Hide();

            
        }
    }
}
