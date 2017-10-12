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
using WindowsFormsApplication1.ConnectionToKeil;


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
            //TODO: We should add automated calculation of vertex  count and edges from matrix
            V = new List<Vertex>();
            V.Add(new Vertex(0, 0));
            V.Add(new Vertex(0, 0));
            V.Add(new Vertex(0, 0));
            V.Add(new Vertex(0, 0));
            V.Add(new Vertex(0, 0));
            V.Add(new Vertex(0, 0));


            E = new List<Edge>();
            E.Add(new Edge(0, 1));
            E.Add(new Edge(1, 2));
            E.Add(new Edge(2, 3));
            E.Add(new Edge(3, 4));
            E.Add(new Edge(3, 5));
            E.Add(new Edge(1, 4));
            E.Add(new Edge(0, 4));


            G = new DrawGraph(graphPictureBox.Width, graphPictureBox.Height);
            G.VertexesReposition(graphPictureBox.Width, graphPictureBox.Height, V, E);
            G.drawALLGraph(V, E);
            graphPictureBox.Image = G.GetBitmap();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(FunctionListBox.SelectedIndex != -1)
            {
                TimeSpan functionTimeResult = ConnectionToKeilClass.GetFunctionRunTime(_project.Functions[FunctionListBox.SelectedIndex].StartAddress, _project.Functions[FunctionListBox.SelectedIndex].EndAddress);
                string oldText = functionText.Text;
                functionText.Text = "Total milliseconds: " + functionTimeResult.TotalMilliseconds.ToString() + System.Environment.NewLine;
                functionText.Text += "Total seconds: " + functionTimeResult.TotalSeconds.ToString() + System.Environment.NewLine;
                functionText.Text += oldText;
            }
        }
    }
}
