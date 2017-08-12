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
using System.Runtime.InteropServices;



using MySql.Data;
using MySql.Data.MySqlClient;
using MysqlConfig;

namespace ELAB
{

    public partial class JExamForm : Form
    {
        private MySqlConnection conn;
        private DataTable table;
        private MySqlDataAdapter dataAdapter;
        private MySqlCommandBuilder sqlCmdBuilder;

        public JExamForm()
        {
            InitializeComponent();
        }

        //数据获取与初始化
        int StuNumber = 15051236;

        void InitInfo()
        {
            StuNumber = SInfoClass.StuInfoClass.StuNumber;
        }


        string[] problem = new string[3];
        int[] examID = new int[3];
        string[] answer= new string[3];

        string connStr = string.Format("server={0};user id={1}; password={2}; database=mysql; pooling=false",
                MysqlConfig.MysqlConfig.Srv, MysqlConfig.MysqlConfig.User, MysqlConfig.MysqlConfig.Pwd);

        //生成随机数
        List<int> GenerateRandom(int iMax, int iNum)
        {
            long lTick = DateTime.Now.Ticks;
            List<int> lstRet = new List<int>();
            for (int i = 0; i < iNum; i++)
            {
                Random ran = new Random((int)lTick * (i+2));
                int iTmp = ran.Next(iMax);
                lstRet.Add(iTmp);
                lTick += (new Random((int)lTick).Next(978));
            }
            return lstRet;
        }

        string GetProblem(int ID)
        {

            //radioButton初始化
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;


            string problem = "问题";

            if (conn.State == ConnectionState.Closed)  conn.Open();

            // SQL数据读取器
            MySqlDataReader dataReader = null;

            // SQL命令执行器
            MySqlCommand sqlCmd = new MySqlCommand();

            // 设置SQL命令执行器的连接
            sqlCmd.Connection = conn;

            //查询语句
            string cmd = string.Format("USE lab; SELECT problem FROM JExam WHERE ID = {0}",ID);

            try
            {
                // 执行SQL命令
                sqlCmd.CommandText = cmd;
                sqlCmd.ExecuteNonQuery();
             
                dataReader = sqlCmd.ExecuteReader();

                while (dataReader.Read())
                {
                    problem = dataReader.GetString(0);
                }
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
            return problem;
        }

        string GetAnswer(int ID)
        {
            string answer = "错误";

            if (conn.State == ConnectionState.Closed) conn.Open();

            // SQL数据读取器
            MySqlDataReader dataReader = null;

            // SQL命令执行器
            MySqlCommand sqlCmd = new MySqlCommand();

            // 设置SQL命令执行器的连接
            sqlCmd.Connection = conn;

            //查询语句
            string cmd = string.Format("USE lab; SELECT answer FROM JExam WHERE ID = {0}", ID);

            try
            {
                // 执行SQL命令
                sqlCmd.CommandText = cmd;
                sqlCmd.ExecuteNonQuery();

                dataReader = sqlCmd.ExecuteReader();

                while (dataReader.Read())
                {
                    answer = dataReader.GetString(0);
                }
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
            return answer;
        }


        void GetExamInfo()
        {
            List<int> RandomNumber = new List<int>();
            RandomNumber = GenerateRandom(9, 4);
            try
            {
                conn = new MySqlConnection(connStr);
                if (conn.State == ConnectionState.Closed) conn.Open();

                for (int i = 0; i < 3; i++)
                {
                    problem[i] = GetProblem(RandomNumber[i]);
                    answer[i] = GetAnswer(RandomNumber[i]);
                    examID[i] = RandomNumber[i];
                }

                label3.Text = problem[0];
                label5.Text = problem[1];
                label7.Text = problem[2];

                MessageBox.Show("数据获取成功！请开始答题！");

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("数据库连接失败: " + ex.Message);
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            GetExamInfo();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //获取选择的答案
            string[] StuChoice = new string[3];

            if (radioButton1.Checked == true) StuChoice[0] = "正确";
            else if (radioButton2.Checked == true) StuChoice[0] = "错误";

            if (radioButton3.Checked == true) StuChoice[1] = "正确";
            else if (radioButton4.Checked == true) StuChoice[1] = "错误";

            if (radioButton5.Checked == true) StuChoice[2] = "正确";
            else if (radioButton6.Checked == true) StuChoice[2] = "错误";


            //传输答题数据到服务器
            try
            {
                conn = new MySqlConnection(connStr);
                if (conn.State == ConnectionState.Closed) conn.Open();

                // SQL命令执行器
                MySqlCommand sqlCmd = new MySqlCommand();

                // 设置SQL命令执行器的连接
                sqlCmd.Connection = conn;

                for (int i = 0; i < 3; i++)
                {
                    //插入语句
                    string cmd = string.Format("USE lab;INSERT INTO `JExamRecord` (`StuId`,`ExamTime`,`ExamId`,`StuAnswer`) VALUES ({0},now(),{1},'{2}');", StuNumber, examID[i], StuChoice[i]);

                    try
                    {
                        // 执行SQL命令
                        sqlCmd.CommandText = cmd;
                        sqlCmd.ExecuteNonQuery();

                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("传输答题数据失败: " + ex.Message);
                    }
                    finally
                    {

                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("数据库连接失败: " + ex.Message);
            }


            //flag == 1 为答题正确
            int flag1 = 0, flag2 = 0, flag3 = 0;

            //判断第一题正确性
            if (answer[0] == "正确")
            {
                if (radioButton1.Checked == true) flag1 = 1;
                else flag1 = 0;
            }
            else
            {
                if (radioButton2.Checked == true) flag1 = 1;
                else flag1 = 0;
            }

            //判断第二题正确性
            if (answer[1] == "正确")
            {
                if (radioButton3.Checked == true) flag2 = 1;
                else flag2 = 0;
            }
            else
            {
                if (radioButton4.Checked == true) flag2 = 1;
                else flag2 = 0;
            }

            //判断第三题正确性
            if (answer[2] == "正确")
            {
                if (radioButton5.Checked == true) flag3 = 1;
                else flag3 = 0;
            }
            else
            {
                if (radioButton6.Checked == true) flag3 = 1;
                else flag3 = 0;
            }

            //判断总正确情况
            if (flag1 == 1 & flag2 == 1 & flag3 == 1)
            {
                //MessageBox.Show("答题正确，请开始上机！");
                conn.Close();
                Form CExamForm = new CExamForm();
                this.Hide();
                CExamForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("答题错误,请继续答题！");
                GetExamInfo();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form JExamAdmin = new JExamAdmin();
            JExamAdmin.ShowDialog();
        }
    }
}
