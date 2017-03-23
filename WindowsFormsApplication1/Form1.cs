using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public string ReportPath;
        

        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //// add find map. file 
            Parser.GetFolderFileName(folderBrowserDialog1.SelectedPath, ReportPath);    
            label3.Text = "Result: OK";
            /*       var allFiles = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.map", SearchOption.AllDirectories);
                   MapFilepath = allFiles[0];
                   //// parser
                   lines = System.IO.File.ReadAllLines(MapFilepath);
                   FileStream file1 = new FileStream(ReportPath, FileMode.Create);
                   StreamWriter writer = new StreamWriter(file1);
                   //File.Create(ReportPath);
                   ///new commit
                   ///
                   foreach (string line in lines)
                   {
                       if ((line.Contains("Thumb Code")) && (line.Contains("(i.")))
                       {
                           writer.Write(line);
                           writer.Write("\r\n");
                       }
                   }
                   writer.Close();*/
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            saveFileDialog1.Title = "Save an Text File";
            saveFileDialog1.ShowDialog();
           
            textBox2.Text = ReportPath = saveFileDialog1.FileName; ;
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
    public string[] FileInProject;


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
                    string Tmp;
                    Tmp = dir.Replace(line, "");
                    Tmp = Tmp.Replace(@"\","");
                    writer.Write("\t");
                    writer.Write(Tmp);
                    writer.Write("\r\n");
                }
                writer.Write("\r\n");
            }
        }
        writer.Close();
    }
}