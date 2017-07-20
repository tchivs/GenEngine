using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenEngine
{
    public partial class Form1 : Form
    {
        //要操作的文件路径
        public string[] FileNames { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   如果有选择文件就开工. </summary>
        ///
        /// <value> True if this object is can work, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool IsCanWork => FileNames != null&& !String.IsNullOrEmpty(textBox1.Text) && !String.IsNullOrEmpty(textBox2.Text);

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LogText.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".gen",
                Filter = "GenFile(*.gen)|*.gen",
                Multiselect = true,
                Title="Please Choose Your Gen File",
                AutoUpgradeEnabled= true,
                RestoreDirectory=true
            };
            if (openFileDialog.ShowDialog()== DialogResult.OK)
            {
                this.FileNames = openFileDialog.FileNames;
                foreach (var VARIABLE in FileNames)
                {
                    string str = Path.GetFileName(VARIABLE);
                    LogText.AppendText(str+"\n");
                }
                textBox1.Text = Engine.GetOldPci(FileNames[0]);
              
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (IsCanWork)
            {
                ///实例化类，并引入参数
                Engine eg = new Engine(FileNames, textBox1.Text, textBox2.Text);
                ///开启一个新的线程执行主方法
                Thread th = new Thread(eg.ReadFile);
                th.IsBackground = true;
                //启动线程
                th.Start();
                
                //循环导出文件名并输入到日志窗口
                foreach (var VARIABLE in eg._NewNames)
                {
                    LogText.AppendText("Succesfully!\n");
                    LogText.AppendText("Save to\t" + VARIABLE + "\n");
                }
                //提示完成！
                LogText.AppendText("Finish!\n");

            }
            else
            {
                LogText.AppendText("请选择文件并输入PCI！！！\n");
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (IsCanWork)
            {
                //打开目录

                OpenFolderAndSelectFile( FileNames[0]);
            }
            

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Opens folder and select file. </summary>
        ///
        /// <remarks>   Topiv, 2017-06-26. </remarks>
        ///
        /// <param name="fileFullName"> Name of the file full. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void OpenFolderAndSelectFile(String fileFullName)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe")
            {
                Arguments = "/e,/select," + fileFullName
            };
            System.Diagnostics.Process.Start(psi);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //this.Close();
            this.LogText.Clear();
            LogText.AppendText("1.1-修复BUG:当路径中含有源PCI或者目的PCI时，由于新的文件名的目录是直接替换源文件的目录的，导致目录中PCI的值发生改变无法找到该目录。\t\n");
            LogText.AppendText("1.2-修复BUG:修改后有一定机率没有轨迹的出现\t\n");

        }
    }
}
