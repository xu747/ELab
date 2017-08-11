using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ELAB
{
    public partial class JExamAdmin : Form
    {
        public JExamAdmin()
        {
            InitializeComponent();
            WindowDefaultLocation();
            textBox1.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "jsjjcsys") System.Environment.Exit(0);
        }



        //窗口位置默认居中
        void WindowDefaultLocation()
        {
            //默认右下角
            int x = (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size.Width - this.Width)/2;
            int y = (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size.Height - this.Height)/2;
            Point p = new Point(x, y);
            this.PointToScreen(p);
            this.Location = p;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.button1.Focus();
                button1_Click(sender, e);
            }
        }
    }
}
