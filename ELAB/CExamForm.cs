using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ELAB
{
    public partial class CExamForm : Form
    {
        public CExamForm()
        {
            InitializeComponent();
        }

        private MySqlConnection conn;
        private DataTable table;
        private MySqlDataAdapter dataAdapter;
        private MySqlCommandBuilder sqlCmdBuilder;

        //数据获取与初始化
        int StuNumber = 15051236;

        void InitInfo()
        {

        }


        string[] problem = new string[2];
        string[,] choice = new string[2,4];
        int[] examID = new int[2];
        string[] answer = new string[2];

        string connStr = string.Format("server={0};user id={1}; password={2}; database=mysql; pooling=false",
                MysqlConfig.MysqlConfig.Srv, MysqlConfig.MysqlConfig.User, MysqlConfig.MysqlConfig.Pwd);

        //生成随机数
        List<int> GenerateRandom(int iMax, int iNum)
        {
            long lTick = DateTime.Now.Ticks;
            List<int> lstRet = new List<int>();
            for (int i = 0; i < iNum; i++)
            {
                Random ran = new Random((int)lTick * (i + 2));
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
            radioButton8.Checked = false;
            radioButton7.Checked = false;


            string problem = "问题";

            if (conn.State == ConnectionState.Closed) conn.Open();

            // SQL数据读取器
            MySqlDataReader dataReader = null;

            // SQL命令执行器
            MySqlCommand sqlCmd = new MySqlCommand();

            // 设置SQL命令执行器的连接
            sqlCmd.Connection = conn;

            //查询语句
            string cmd = string.Format("USE lab; SELECT problem FROM CExam WHERE ID = {0}", ID);

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
            string answer = "A";

            if (conn.State == ConnectionState.Closed) conn.Open();

            // SQL数据读取器
            MySqlDataReader dataReader = null;

            // SQL命令执行器
            MySqlCommand sqlCmd = new MySqlCommand();

            // 设置SQL命令执行器的连接
            sqlCmd.Connection = conn;

            //查询语句
            string cmd = string.Format("USE lab; SELECT answer FROM CExam WHERE ID = {0}", ID);

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

        string GetChoice(int ID,int ChoiceNum)
        {
            string choice = "选项";

            string choice_ABCD = "A";

            switch (ChoiceNum)
            {
                case 0: choice_ABCD = "A"; break;
                case 1: choice_ABCD = "B"; break;
                case 2: choice_ABCD = "C"; break;
                case 3: choice_ABCD = "D"; break;
            }

            if (conn.State == ConnectionState.Closed) conn.Open();

            // SQL数据读取器
            MySqlDataReader dataReader = null;

            // SQL命令执行器
            MySqlCommand sqlCmd = new MySqlCommand();

            // 设置SQL命令执行器的连接
            sqlCmd.Connection = conn;

            //查询语句
            string cmd = string.Format("USE lab; SELECT choice_{0} FROM CExam WHERE ID = {1}", choice_ABCD, ID);
            //string cmd = string.Format("USE lab; SELECT choice_A FROM CExam WHERE ID = {0}", ID);

            try
            {
                // 执行SQL命令
                sqlCmd.CommandText = cmd;
                sqlCmd.ExecuteNonQuery();

                dataReader = sqlCmd.ExecuteReader();

                while (dataReader.Read())
                {
                    choice = dataReader.GetString(0);
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
            return choice;
        }


        void GetExamInfo()
        {
            List<int> RandomNumber = new List<int>();
            RandomNumber = GenerateRandom(9, 4);


            //D选项恢复显示
            if (label7.Visible == false) label7.Show();
            if (radioButton4.Visible == false) radioButton4.Show();
            if (label10.Visible == false) label10.Show();
            if (radioButton8.Visible == false) radioButton8.Show();

            //选项初始化
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            radioButton7.Checked = false;
            radioButton8.Checked = false;

            try
            {
                conn = new MySqlConnection(connStr);
                if (conn.State == ConnectionState.Closed) conn.Open();

                for (int i = 0; i < 2; i++)
                {
                    problem[i] = GetProblem(RandomNumber[i]);
                    answer[i] = GetAnswer(RandomNumber[i]);
                    examID[i] = RandomNumber[i];

                    for (int j = 0; j < 4; j++)
                    {
                        choice[i,j] = GetChoice(RandomNumber[i], j);
                    }
                }

                label3.Text = problem[0];
                radioButton1.Text = choice[0,0];
                radioButton2.Text = choice[0,1];
                radioButton3.Text = choice[0,2];

                //判断D选项是否存在
                if (choice[0, 3] == "空")
                {
                    label7.Hide();
                    radioButton4.Hide();
                }
                else
                {
                    radioButton4.Text = choice[0, 3];
                }
                

                label8.Text = problem[1];
                radioButton5.Text = choice[1,0];
                radioButton6.Text = choice[1,1];
                radioButton7.Text = choice[1,2];

                //判断D选项是否存在
                if (choice[1, 3] == "空")
                {
                    label10.Hide();
                    radioButton8.Hide();
                }
                else
                {
                    radioButton8.Text = choice[1, 3];
                }

                MessageBox.Show("数据获取成功！请开始答题！");

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("数据库连接失败: " + ex.Message);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            radioButton7.Checked = false;
            radioButton8.Checked = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //获取选择的答案
            string [] StuChoice = new string[2];

            if (radioButton1.Checked == true) StuChoice[0] = "A";
            else if (radioButton2.Checked == true) StuChoice[0] = "B";
            else if (radioButton3.Checked == true) StuChoice[0] = "C";
            else if (radioButton4.Checked == true) StuChoice[0] = "D";

            if (radioButton5.Checked == true) StuChoice[1] = "A";
            else if (radioButton6.Checked == true) StuChoice[1] = "B";
            else if (radioButton7.Checked == true) StuChoice[1] = "C";
            else if (radioButton8.Checked == true) StuChoice[1] = "D";


            //传输答题数据到服务器
            try
            {
                conn = new MySqlConnection(connStr);
                if (conn.State == ConnectionState.Closed) conn.Open();

                // SQL命令执行器
                MySqlCommand sqlCmd = new MySqlCommand();

                // 设置SQL命令执行器的连接
                sqlCmd.Connection = conn;

                for (int i = 0; i < 2; i++)
                {
                    //插入语句
                    string cmd = string.Format("USE lab;INSERT INTO `CExamRecord` (`StuId`,`ExamTime`,`ExamId`,`StuAnswer`) VALUES ({0},now(),{1},'{2}');", StuNumber, examID[i], StuChoice[i]);

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
            int flag1 = 0, flag2 = 0;

            //判断第一题正确性
            if (answer[0] == "A")
            {
                if (radioButton1.Checked == true) flag1 = 1;
                else flag1 = 0;
            }
            else if (answer[0] == "B")
            {
                if (radioButton2.Checked == true) flag1 = 1;
                else flag1 = 0;
            }
            else if (answer[0] == "C")
            {
                if (radioButton3.Checked == true) flag1 = 1;
                else flag1 = 0;
            }
            else if (answer[0] == "D")
            {
                if (radioButton4.Checked == true) flag1 = 1;
                else flag1 = 0;
            }

            //判断第二题正确性
            if (answer[1] == "A")
            {
                if (radioButton5.Checked == true) flag2 = 1;
                else flag2 = 0;
            }
            else if (answer[1] == "B")
            {
                if (radioButton6.Checked == true) flag2 = 1;
                else flag2 = 0;
            }
            else if (answer[1] == "C")
            {
                if (radioButton7.Checked == true) flag2 = 1;
                else flag2 = 0;
            }
            else if (answer[1] == "D")
            {
                if (radioButton8.Checked == true) flag2 = 1;
                else flag2 = 0;
            }

            //判断总正确情况
            if (flag1 == 1 & flag2 == 1)
            {
                MessageBox.Show("答题正确，请开始上机！");
                conn.Close();
                Form StatusForm1 = new StatusForm();
                this.Hide();
                StatusForm1.ShowDialog();
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

        private void CExamForm_Load(object sender, EventArgs e)
        {
            GetExamInfo();
        }
    }
}
