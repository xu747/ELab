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
    }
}
