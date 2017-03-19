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
            string MapFilepath;
            string ReportPath = @"D:\Report.txt";
            string[] lines;

            var allFiles = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.map", SearchOption.AllDirectories);
            MapFilepath = allFiles[0];
            //// parser
            lines = System.IO.File.ReadAllLines(MapFilepath);
            File.Create(ReportPath);
            ///new commit

        }
    }
}
