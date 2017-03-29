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
            //// add find map. file 
            //      Parser.GetFolderFileName(folderBrowserDialog1.SelectedPath, ReportPath);
            //      Parser.GetFunctionList(folderBrowserDialog1.SelectedPath, ReportPath, Parser.FileInProject);
            projectObj.ParseProject();
            projectObj.GenerateReport(ReportPath);
            
            AnalysisResultLbl.Text = "Result: OK";

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



class Parser
{
    public const string SVN = "svn";
    public const string BINOUTPUT = "BINOUTPUT";
    public const string Objects = "Objects";
    public const string Listings = "Listings";
    public const string RTE = "RTE";
    public static List<string> FileInProject = new List<string>();


    public static void GetFolderFileName(string Project, string Report)
    {
        List<string> folders = new List<string>(Directory.EnumerateDirectories(Project));
        FileStream file1 = new FileStream(Report, FileMode.Create);
        StreamWriter writer = new StreamWriter(file1);

        foreach (string line in folders)
        {
            if ((line.Contains(SVN)) ||
                (line.Contains(BINOUTPUT)) ||
                (line.Contains(Objects))   ||
                (line.Contains(Listings))  ||
                (line.Contains(RTE)))
            {

            }
            else
            {
                string Tmp1;

                Tmp1 = line.Replace(Project, "");
                Tmp1 = Tmp1.Replace(@"\", "");
                writer.Write(Tmp1);
                writer.Write("\r\n");
                string[] dirs = Directory.GetFiles(line, "*.c");
                foreach (string dir in dirs)
                {
                    string Tmp, lower;
                    
                    Tmp = dir.Replace(line, "");
                    Tmp = Tmp.Replace(@"\","");
                    writer.Write("\t");
                    writer.Write(Tmp);//
                    lower = Tmp.Remove(Tmp.Length - 2);
                    FileInProject.Add(lower.ToLower());
                    writer.Write("\r\n");
                }
                writer.Write("\r\n");
            }
        }
        writer.Write("***************************Function List*********************************");
        writer.Close();
    }
    public static void GetFunctionList(string Project, string ReportPath, List<string> FilesList)
    {
        string MapFilePath;
        string[] lines;
        FileStream textReport = new FileStream(ReportPath, FileMode.Append);
        StreamWriter report = new StreamWriter(textReport);

       var allFiles = Directory.GetFiles(Project, "*.map", SearchOption.AllDirectories);
       MapFilePath = allFiles[0];

       lines = System.IO.File.ReadAllLines(MapFilePath);
       report.Write("\r\n");
       foreach (string line in lines)
       {
           if (line.Contains("Thumb Code")) //&& (line.Contains("(i.")))
           {
                string temp;
               
                foreach(string filesname in FilesList)
                {
                   // ads1118.o(i.
                    temp = "\\b" + filesname + ".o"+"\\b";
                    // temp = "(i." + filesname;
                    if (Regex.IsMatch(line, temp, RegexOptions.IgnoreCase))
                    {
                        string firstWord = line.Trim();
                       // firstWord.Substring(0, firstWord.IndexOf(" "));
                        report.Write(firstWord.Substring(0, firstWord.IndexOf(" ")));
                        report.Write("\r\n");
                    }
                }       
           }
       }
       report.Close();

    }
}