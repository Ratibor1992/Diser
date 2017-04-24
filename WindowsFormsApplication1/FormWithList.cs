using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication1;


namespace WindowsFormsApplication1
{
    public partial class FormWithList : Form
    {
        private Project _project;
        DrawGraph G;
        List<Vertex> V;
        List<Edge> E;
        string[] lines;

        public FormWithList(Project project)
        {
            _project = project;
            InitializeComponent();

            functionText.Multiline = true;
            functionText.ScrollBars = ScrollBars.Vertical;
           // GraphPicture.Au
            foreach (var func in _project.Functions)
            {
                FunctionListBox.Items.Add(func.Name);
            }
        }

        private void FunctionListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            functionText.Clear();
            lines = System.IO.File.ReadAllLines(_project.Functions[FunctionListBox.SelectedIndex].SeparetedFunctionPath);
            foreach (var line in lines)
            {
                functionText.AppendText(line + "\r\n");
            }
        }
    }
}
