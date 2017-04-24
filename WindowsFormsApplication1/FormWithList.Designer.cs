namespace WindowsFormsApplication1
{
    partial class FormWithList
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
            this.FunctionListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.functionText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // FunctionListBox
            // 
            this.FunctionListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.FunctionListBox.FormattingEnabled = true;
            this.FunctionListBox.Location = new System.Drawing.Point(12, 25);
            this.FunctionListBox.Name = "FunctionListBox";
            this.FunctionListBox.Size = new System.Drawing.Size(260, 290);
            this.FunctionListBox.TabIndex = 1;
            this.FunctionListBox.SelectedIndexChanged += new System.EventHandler(this.FunctionListBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Function List";
            // 
            // functionText
            // 
            this.functionText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.functionText.Location = new System.Drawing.Point(276, 25);
            this.functionText.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.functionText.MinimumSize = new System.Drawing.Size(4, 290);
            this.functionText.Multiline = true;
            this.functionText.Name = "functionText";
            this.functionText.Size = new System.Drawing.Size(755, 290);
            this.functionText.TabIndex = 4;
            // 
            // FormWithList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1041, 329);
            this.Controls.Add(this.functionText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FunctionListBox);
            this.Name = "FormWithList";
            this.Text = "FormWithList";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox FunctionListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox functionText;
    }
}