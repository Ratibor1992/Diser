using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using SharpSvn;

namespace WindowsFormsApplication1
{
    public partial class MainWindowForm : Form
    {
        public string ReportPath;
        public string []filesEnum;
        private Project projectObj;

        public MainWindowForm()
        {
            InitializeComponent();
            ReportFilePathTb.Text = ReportPath = @"D:\";
            projectObj = new Project(ReportPath);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                projectObj.Path = ProjectPathTb.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(ProjectPathTb.Text))
            {
                AnalysisResultLbl.Text = "Project not selected";
            }
            
            else if (!ReportFilePathTb.Text.Contains("*.txt"))
            {
                AnalysisResultLbl.Text = "Report Path not selected!";
            }
            else
            {
                projectObj.ParseProject();
                projectObj.GenerateReport(ReportPath);
                AnalysisResultLbl.Text = "Result: OK";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            saveFileDialog1.Title = "Save an Text File";
            saveFileDialog1.ShowDialog();
           
            ReportFilePathTb.Text = ReportPath = saveFileDialog1.FileName;
        }
    }
}


