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
        public string ReportFolderPath;
        public string []filesEnum;
        private Project projectObj;

        public MainWindowForm()
        {
            InitializeComponent();
            projectObj = new Project(ReportPath);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                ReportFolderPath = projectObj.Path = ProjectPathTb.Text = folderBrowserDialog1.SelectedPath;

                ReportFolderPath = string.Concat(ReportFolderPath, Constants.REPORT_FOLDER);
                Directory.CreateDirectory(ReportFolderPath);
                ReportPath = string.Concat(ReportFolderPath, Constants.REPORT_NAME);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(ProjectPathTb.Text))
            {
                AnalysisResultLbl.Text = Constants.ANALISYS_RESULT_ERROR;
            }
            
            else
            {
                projectObj.ParseProject();
                projectObj.GenerateReport(ReportPath);
                projectObj.CopyFunctionsToSeparetedFiles(ReportFolderPath);
                
                foreach (var func in projectObj.Functions)
                {
                    func.NumberOfEdgesInGraph = projectObj.GetNumberOfEdgesFormFunction(func.SeparetedFunctionPath, func.Name);
                    projectObj.AddInforInFuctionFiles(func.SeparetedFunctionPath, func.NumberOfElemntsInGraph, func.NumberOfEdgesInGraph);
                    func.matrix = new int[func.NumberOfElemntsInGraph, func.NumberOfEdgesInGraph];
                    projectObj.AddInfoInsideMatrix(func.SeparetedFunctionPath, func.Name, func.matrix, func.NumberOfElemntsInGraph, func.NumberOfEdgesInGraph);
                    projectObj.AddMatrixInFuctionFiles(func.SeparetedFunctionPath, func.matrix, func.NumberOfElemntsInGraph, func.NumberOfEdgesInGraph);
                    //AddInfoInsideMatrix
                    //projectObj.AddMatrixInFuctionFiles(prj.SeparetedFunctionPath, prj.NumberOfElemntsInGraph, prj.NumberOfEdgesInGraph);
                }
                
                AnalysisResultLbl.Text = Constants.ANALISYS_RESULT_OK;
                OpenFunctionList.Visible = true;
            }
        }

        private void OpenFunctionList_Click(object sender, EventArgs e)
        {
            FormWithList frm = new FormWithList(projectObj);
            frm.Show();
        }
    }
}


