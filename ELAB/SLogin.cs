using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.Xml;
using System.Xml.Linq;
using System.Timers;

namespace ELAB
{
    public partial class SLogin : Form
    {

        delegate void MyDelegate();


        public SLogin()
        {
            InitializeComponent();

        }

        public static int SuccessFlag = 0;
        public static int AlreadyShowFlag = 0;

        public ChromiumWebBrowser chromeBrowser;

        public void InitializeChromium()
        {
            CefSettings settings = new CefSettings();
            settings.Locale = "zh-CN";

            // Initialize cef with the provided settings
            CefSharp.Cef.Initialize(settings);

            // Create a browser component
            chromeBrowser = new ChromiumWebBrowser("http://cas.hdu.edu.cn/cas/login?service=http://127.0.0.1");

            // Add it to the form and fill it to the form window.
            this.Controls.Add(chromeBrowser);
            chromeBrowser.Dock = DockStyle.Fill;

            chromeBrowser.FrameLoadStart += chromeBrowser_FrameLoadStart;

        }

        //登录并获取ticket

        public string ticket;

        void chromeBrowser_FrameLoadStart(object sender, CefSharp.FrameLoadStartEventArgs e)
        {

            if (e.Url.IndexOf("http://127.0.0.1") == 0)
            {
                ticket = e.Url.Substring(25);
                MessageBox.Show(ticket);
                GetInfo();
            }


        }

        void GetInfo()
        {
            string url = string.Format("http://cas.hdu.edu.cn/cas/serviceValidate?ticket="+ticket+"&service=http://127.0.0.1");
            XmlDocument stuInfo = new XmlDocument();
            OXml.OperateXml OperateXml1 = new OXml.OperateXml();
            stuInfo = OperateXml1.GetXMLFromUrl(url);
            OperateXml1.GetStuInfoFromXml(stuInfo);
            SuccessFlag = 1;
        }

        private void SLogin_Load(object sender, EventArgs e)
        {

            /*
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
            */

            //MyDelegate del = new MyDelegate(Judge);
            //this.BeginInvoke(del);

            InitializeChromium();
        }

        private void SLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (AlreadyShowFlag == 0)
            {
                if (SuccessFlag == 1)
                {
                    Form JExamForm = new JExamForm();
                    this.Hide();
                    AlreadyShowFlag = 1;                 
                    JExamForm.ShowDialog(); 
                }
            }
        }
    }
}
