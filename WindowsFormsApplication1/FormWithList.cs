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


        public FormWithList(Project project)
        {
            _project = project;
            InitializeComponent();

            foreach (var func in _project.Functions)
            {
                FunctionListBox.Items.Add(func.Name);
            }
        }
    }
}
