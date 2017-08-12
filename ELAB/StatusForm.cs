using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using MySql.Data;
using MySql.Data.MySqlClient;
using namespaceIniFile;

namespace ELAB
{
    public partial class StatusForm : Form
    {
        public StatusForm()
        {
            InitializeComponent();
        }

        //数据获取与初始化
        string CourseName = "Name";
        string BuildingNumber = "1";
        string RoomNumber = "225";
        string StuName = "春雷";
        int StuNumber = 15051236;
        string ComputerNumber = "A01";


        string UsingTimeString;

        public static string Path = @"../../../config.ini";

        namespaceIniFile.IniFile IniFile1 = new IniFile(Path);
        

        void InitInfo()
        {
            BuildingNumber = IniFile1.IniReadValue("location", "BuildingNumber");
            RoomNumber = IniFile1.IniReadValue("location", "RoomNumber");
            ComputerNumber = IniFile1.IniReadValue("location", "ComputerNumber");

            StuName = SInfoClass.StuInfoClass.StuName;
            StuNumber = SInfoClass.StuInfoClass.StuNumber;

        }

        //设置学号与姓名
        void SetStuInfo()
        {
            label2.Text = StuNumber.ToString();
            label4.Text = StuName;
        }

        //时钟部分

        Timer time = new Timer();
        Stopwatch sw; //秒表对象
        TimeSpan ts;

        void UsingTime()
        {
            sw = new Stopwatch();
            time.Tick += new EventHandler(time_Tick);  //时钟触发信号
            time.Interval = 1;
            sw.Start();
            time.Start();
        }

        void time_Tick(object sender, EventArgs e)
        {
            ts = sw.Elapsed;
            UsingTimeString = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
            label8.Text = UsingTimeString;
        }

        //上机科目部分
        private MySqlConnection conn;
        string connStr = string.Format("server={0};user id={1}; password={2}; database=mysql; pooling=false",
                MysqlConfig.MysqlConfig.Srv, MysqlConfig.MysqlConfig.User, MysqlConfig.MysqlConfig.Pwd);

        //获取上机课目信息
        void GetCourseInfo()
        {
            try
            {
                conn = new MySqlConnection(connStr);
                if (conn.State == ConnectionState.Closed) conn.Open();

                // SQL数据读取器
                MySqlDataReader dataReader = null;

                // SQL命令执行器
                MySqlCommand sqlCmd = new MySqlCommand();

                // 设置SQL命令执行器的连接
                sqlCmd.Connection = conn;

                //查询语句
                string cmd = string.Format("USE lab;SELECT CourseName FROM Course Where RoomNumber = \"{0}\" and BuildingNumber = \"{1}\" and BeginTime < now() and EndTime > now();",RoomNumber,BuildingNumber);

                try
                {
                    // 执行SQL命令
                    sqlCmd.CommandText = cmd;
                    sqlCmd.ExecuteNonQuery();

                    dataReader = sqlCmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        CourseName = dataReader.GetString(0);
                    }
                    label6.Text = CourseName;

                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("读取数据失败: " + ex.Message);
                }
                finally
                {
                    if (dataReader != null)
                        dataReader.Close();
                }

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("数据库连接失败(获取科目失败): " + ex.Message);
            }
        }

        //窗口位置默认右下角
        void WindowDefaultLocation()
        {
            //默认右下角
            int x = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size.Width - this.Width;
            int y = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size.Height - this.Height;
            Point p = new Point(x, y);
            this.PointToScreen(p);
            this.Location = p;
        }

        //上机签到信息
        void SendInRecord()
        {
            //传输上机信息数据到服务器
            try
            {
                conn = new MySqlConnection(connStr);
                if (conn.State == ConnectionState.Closed) conn.Open();

                // SQL命令执行器
                MySqlCommand sqlCmd = new MySqlCommand();

                // 设置SQL命令执行器的连接
                sqlCmd.Connection = conn;

                //插入语句
                string cmd = string.Format("USE lab;INSERT INTO `InRecord` (`StuId`,`InTime`,`BuildingNumber`,`RoomNumber`,`ComputerNumber`) VALUES ({0},now(),'{1}','{2}','{3}');", StuNumber, BuildingNumber, RoomNumber, ComputerNumber);

                try
                {
                    // 执行SQL命令
                    sqlCmd.CommandText = cmd;
                    sqlCmd.ExecuteNonQuery();

                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("传输上机数据失败: " + ex.Message);
                }
                finally
                {
                    
                }

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("数据库连接失败: " + ex.Message);
            }
        }


        private void StatusForm_Load(object sender, EventArgs e)
        {
            WindowDefaultLocation();
            UsingTime();
            InitInfo();
            SetStuInfo();
            GetCourseInfo();
            SendInRecord();
        }

        //防止窗口移动
        private void StatusForm_Move(object sender, EventArgs e)
        {
            int x = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size.Width - this.Width;
            int y = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size.Height - this.Height;
            Point p = new Point(x, y);
            this.PointToScreen(p);
            this.Location = p;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认要下机吗？\n请确认已保存好所有需要的文件！","提醒",MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                //传输下机信息数据到服务器
                try
                {
                    conn = new MySqlConnection(connStr);
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    // SQL命令执行器
                    MySqlCommand sqlCmd = new MySqlCommand();

                    // 设置SQL命令执行器的连接
                    sqlCmd.Connection = conn;

                    //插入语句
                    string cmd = string.Format("USE lab;INSERT INTO `OutRecord` (`StuId`,`OutTime`,`TotalTime`,`BuildingNumber`,`RoomNumber`,`ComputerNumber`) VALUES ({0},now(),'{1}','{2}','{3}','{4}');", StuNumber, UsingTimeString, BuildingNumber, RoomNumber, ComputerNumber);

                    try
                    {
                        // 执行SQL命令
                        sqlCmd.CommandText = cmd;
                        sqlCmd.ExecuteNonQuery();

                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("传输下机数据失败: " + ex.Message);
                    }
                    finally
                    {
                        //执行关闭操作
                        if (conn.State == ConnectionState.Open) conn.Close();
                        Application.Exit();
                        
                        //System.Environment.Exit(0);
                    }

                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("数据库连接失败: " + ex.Message);
                }
            }
            
        }
    }

}
