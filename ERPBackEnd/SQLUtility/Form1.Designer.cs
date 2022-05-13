
namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblChoseTheFolder = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // lblChoseTheFolder
            // 
            this.lblChoseTheFolder.AutoSize = true;
            this.lblChoseTheFolder.BackColor = System.Drawing.Color.Aqua;
            this.lblChoseTheFolder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblChoseTheFolder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblChoseTheFolder.Font = new System.Drawing.Font("Times New Roman", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblChoseTheFolder.Location = new System.Drawing.Point(32, 138);
            this.lblChoseTheFolder.Name = "lblChoseTheFolder";
            this.lblChoseTheFolder.Padding = new System.Windows.Forms.Padding(1);
            this.lblChoseTheFolder.Size = new System.Drawing.Size(555, 77);
            this.lblChoseTheFolder.TabIndex = 0;
            this.lblChoseTheFolder.Text = "Choose The Folder";
            this.lblChoseTheFolder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblChoseTheFolder.UseMnemonic = false;
            this.lblChoseTheFolder.Click += new System.EventHandler(this.lblChoseTheFolder_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 379);
            this.Controls.Add(this.lblChoseTheFolder);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblChoseTheFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}

