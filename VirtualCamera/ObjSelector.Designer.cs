namespace VirtualCamera
{
    partial class ObjSelector
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
            this.filePathsBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // filePathsBox
            // 
            this.filePathsBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filePathsBox.FormattingEnabled = true;
            this.filePathsBox.Location = new System.Drawing.Point(0, 0);
            this.filePathsBox.Name = "filePathsBox";
            this.filePathsBox.Size = new System.Drawing.Size(134, 161);
            this.filePathsBox.TabIndex = 0;
            this.filePathsBox.SelectedIndexChanged += new System.EventHandler(this.FilePathsBox_SelectedIndexChanged);
            // 
            // ObjSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(150, 450);
            this.Controls.Add(this.filePathsBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Location = new System.Drawing.Point(100, 100);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Name = "ObjSelector";
            this.ShowIcon = false;
            this.Text = "Select Object";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox filePathsBox;
    }
}

