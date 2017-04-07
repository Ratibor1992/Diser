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
            this.BtnDetails = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FunctionListBox
            // 
            this.FunctionListBox.FormattingEnabled = true;
            this.FunctionListBox.Location = new System.Drawing.Point(12, 25);
            this.FunctionListBox.Name = "FunctionListBox";
            this.FunctionListBox.Size = new System.Drawing.Size(260, 290);
            this.FunctionListBox.TabIndex = 1;
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
            // BtnDetails
            // 
            this.BtnDetails.Location = new System.Drawing.Point(87, 324);
            this.BtnDetails.Name = "BtnDetails";
            this.BtnDetails.Size = new System.Drawing.Size(110, 23);
            this.BtnDetails.TabIndex = 3;
            this.BtnDetails.Text = "Show Details";
            this.BtnDetails.UseVisualStyleBackColor = true;
            // 
            // FormWithList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 359);
            this.Controls.Add(this.BtnDetails);
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
        private System.Windows.Forms.Button BtnDetails;
    }
}