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
            Parser.GetFolderUserFolder(folderBrowserDialog1.SelectedPath);

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
    }
}



class Parser
{
    public const string SVN = "svn";
    public const string BINOUTPUT = "BINOUTPUT";
    public const string Objects = "Objects";
    public const string Listings = "Listings";
    public const string RTE = "RTE";


    public static void GetFolderUserFolder(string Path)
    {
        string ReportPath = @"D:\Report2.txt";

        List<string> folders = new List<string>(Directory.EnumerateDirectories(Path));
        //UserFolderList<string> = Directory.EnumerateDirectories(folderBrowserDialog1.SelectedPath);
        // add remove special folder
        FileStream file1 = new FileStream(ReportPath, FileMode.Create);
        StreamWriter writer = new StreamWriter(file1);

        foreach (string line in folders)
        {
            if ((line.Contains(SVN)) ||
                (line.Contains(BINOUTPUT)) ||
                (line.Contains(Objects)) ||
                (line.Contains(Listings)) ||
                (line.Contains(RTE)))
            {

            }
            else
            {
                writer.Write(line);
                writer.Write("\r\n");
                string[] dirs = Directory.GetFiles(line, "*.c");
                writer.Write("\r\n");
                foreach (string dir in dirs)
                {
                    writer.Write(dir);
                    writer.Write("\r\n");
                    //Console.WriteLine();
                }
            }
        }
        writer.Close();
    }
   /* public static void GetFiliList(string Upath, string type)
    {
        string[] dirs = Directory.GetFiles(Upath, type);
        foreach (string dir in dirs)
        {
            writer.Write(dir);
            //Console.WriteLine();
        }
    }*/

}