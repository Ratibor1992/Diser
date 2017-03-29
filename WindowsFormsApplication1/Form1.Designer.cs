namespace WindowsFormsApplication1
{
    partial class MainWindowForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SelectProjFoldBtn = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.ProjectPathTb = new System.Windows.Forms.TextBox();
            this.AnalysisBtn = new System.Windows.Forms.Button();
            this.ReportFilePathTb = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SelectReportFileBtn = new System.Windows.Forms.Button();
            this.AnalysisResultLbl = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // SelectProjFoldBtn
            // 
            this.SelectProjFoldBtn.Location = new System.Drawing.Point(73, 84);
            this.SelectProjFoldBtn.Name = "SelectProjFoldBtn";
            this.SelectProjFoldBtn.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.SelectProjFoldBtn.Size = new System.Drawing.Size(131, 23);
            this.SelectProjFoldBtn.TabIndex = 0;
            this.SelectProjFoldBtn.Text = "Select Project Folder";
            this.SelectProjFoldBtn.UseVisualStyleBackColor = true;
            this.SelectProjFoldBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // ProjectPathTb
            // 
            this.ProjectPathTb.Location = new System.Drawing.Point(12, 59);
            this.ProjectPathTb.Name = "ProjectPathTb";
            this.ProjectPathTb.Size = new System.Drawing.Size(260, 20);
            this.ProjectPathTb.TabIndex = 1;
            // 
            // AnalysisBtn
            // 
            this.AnalysisBtn.Location = new System.Drawing.Point(73, 243);
            this.AnalysisBtn.Name = "AnalysisBtn";
            this.AnalysisBtn.Size = new System.Drawing.Size(131, 23);
            this.AnalysisBtn.TabIndex = 2;
            this.AnalysisBtn.Text = "Start Analisys";
            this.AnalysisBtn.UseVisualStyleBackColor = true;
            this.AnalysisBtn.Click += new System.EventHandler(this.button2_Click);
            // 
            // ReportFilePathTb
            // 
            this.ReportFilePathTb.Location = new System.Drawing.Point(12, 138);
            this.ReportFilePathTb.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ReportFilePathTb.Name = "ReportFilePathTb";
            this.ReportFilePathTb.Size = new System.Drawing.Size(260, 20);
            this.ReportFilePathTb.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 42);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Project Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 122);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Report Path";
            // 
            // SelectReportFileBtn
            // 
            this.SelectReportFileBtn.Location = new System.Drawing.Point(73, 162);
            this.SelectReportFileBtn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.SelectReportFileBtn.Name = "SelectReportFileBtn";
            this.SelectReportFileBtn.Size = new System.Drawing.Size(131, 24);
            this.SelectReportFileBtn.TabIndex = 7;
            this.SelectReportFileBtn.Text = "Select Report Path";
            this.SelectReportFileBtn.UseVisualStyleBackColor = true;
            this.SelectReportFileBtn.Click += new System.EventHandler(this.button3_Click);
            // 
            // AnalysisResultLbl
            // 
            this.AnalysisResultLbl.AutoSize = true;
            this.AnalysisResultLbl.Location = new System.Drawing.Point(109, 288);
            this.AnalysisResultLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.AnalysisResultLbl.Name = "AnalysisResultLbl";
            this.AnalysisResultLbl.Size = new System.Drawing.Size(40, 13);
            this.AnalysisResultLbl.TabIndex = 8;
            this.AnalysisResultLbl.Text = "Result:";
            // 
            // MainWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 309);
            this.Controls.Add(this.AnalysisResultLbl);
            this.Controls.Add(this.SelectReportFileBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ReportFilePathTb);
            this.Controls.Add(this.AnalysisBtn);
            this.Controls.Add(this.ProjectPathTb);
            this.Controls.Add(this.SelectProjFoldBtn);
            this.Name = "MainWindowForm";
            this.Text = "MainWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SelectProjFoldBtn;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox ProjectPathTb;
        private System.Windows.Forms.Button AnalysisBtn;
        private System.Windows.Forms.TextBox ReportFilePathTb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SelectReportFileBtn;
        private System.Windows.Forms.Label AnalysisResultLbl;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

