namespace VirtualCamera
{
    partial class MainWindow
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
            this.Canvas = new System.Windows.Forms.Panel();
            this.labelRZ = new System.Windows.Forms.Label();
            this.labelRY = new System.Windows.Forms.Label();
            this.labelRX = new System.Windows.Forms.Label();
            this.labelZ = new System.Windows.Forms.Label();
            this.labelY = new System.Windows.Forms.Label();
            this.labelX = new System.Windows.Forms.Label();
            this.labelFoV = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Canvas
            // 
            this.Canvas.BackColor = System.Drawing.Color.Black;
            this.Canvas.Controls.Add(this.labelRZ);
            this.Canvas.Controls.Add(this.labelRY);
            this.Canvas.Controls.Add(this.labelRX);
            this.Canvas.Controls.Add(this.labelZ);
            this.Canvas.Controls.Add(this.labelY);
            this.Canvas.Controls.Add(this.labelX);
            this.Canvas.Controls.Add(this.labelFoV);
            this.Canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Canvas.Location = new System.Drawing.Point(0, 0);
            this.Canvas.Name = "Canvas";
            this.Canvas.Size = new System.Drawing.Size(800, 450);
            this.Canvas.TabIndex = 0;
            this.Canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.Canvas_Paint);
            // 
            // labelRZ
            // 
            this.labelRZ.AutoSize = true;
            this.labelRZ.BackColor = System.Drawing.Color.Black;
            this.labelRZ.ForeColor = System.Drawing.Color.White;
            this.labelRZ.Location = new System.Drawing.Point(3, 78);
            this.labelRZ.Name = "labelRZ";
            this.labelRZ.Size = new System.Drawing.Size(35, 13);
            this.labelRZ.TabIndex = 5;
            this.labelRZ.Text = "";
            // 
            // labelRY
            // 
            this.labelRY.AutoSize = true;
            this.labelRY.BackColor = System.Drawing.Color.Black;
            this.labelRY.ForeColor = System.Drawing.Color.White;
            this.labelRY.Location = new System.Drawing.Point(3, 65);
            this.labelRY.Name = "labelRY";
            this.labelRY.Size = new System.Drawing.Size(35, 13);
            this.labelRY.TabIndex = 5;
            this.labelRY.Text = "";
            // 
            // labelRX
            // 
            this.labelRX.AutoSize = true;
            this.labelRX.BackColor = System.Drawing.Color.Black;
            this.labelRX.ForeColor = System.Drawing.Color.White;
            this.labelRX.Location = new System.Drawing.Point(3, 52);
            this.labelRX.Name = "labelRX";
            this.labelRX.Size = new System.Drawing.Size(35, 13);
            this.labelRX.TabIndex = 4;
            this.labelRX.Text = "";
            // 
            // labelZ
            // 
            this.labelZ.AutoSize = true;
            this.labelZ.BackColor = System.Drawing.Color.Black;
            this.labelZ.ForeColor = System.Drawing.Color.White;
            this.labelZ.Location = new System.Drawing.Point(3, 39);
            this.labelZ.Name = "labelZ";
            this.labelZ.Size = new System.Drawing.Size(35, 13);
            this.labelZ.TabIndex = 3;
            this.labelZ.Text = "";
            // 
            // labelY
            // 
            this.labelY.AutoSize = true;
            this.labelY.BackColor = System.Drawing.Color.Black;
            this.labelY.ForeColor = System.Drawing.Color.White;
            this.labelY.Location = new System.Drawing.Point(3, 26);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(35, 13);
            this.labelY.TabIndex = 2;
            this.labelY.Text = "";
            // 
            // labelX
            // 
            this.labelX.AutoSize = true;
            this.labelX.BackColor = System.Drawing.Color.Black;
            this.labelX.ForeColor = System.Drawing.Color.White;
            this.labelX.Location = new System.Drawing.Point(3, 13);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(35, 13);
            this.labelX.TabIndex = 1;
            this.labelX.Text = "";
            // 
            // labelFoV
            // 
            this.labelFoV.AutoSize = true;
            this.labelFoV.BackColor = System.Drawing.Color.Black;
            this.labelFoV.ForeColor = System.Drawing.Color.White;
            this.labelFoV.Location = new System.Drawing.Point(3, 0);
            this.labelFoV.Name = "labelFoV";
            this.labelFoV.Size = new System.Drawing.Size(35, 13);
            this.labelFoV.TabIndex = 0;
            this.labelFoV.Text = "FoV: 90";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Canvas);
            this.DoubleBuffered = true;
            this.Name = "MainWindow";
            this.ShowIcon = false;
            this.Text = "Virtual Camera";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Canvas;
        private System.Windows.Forms.Label labelRZ;
        private System.Windows.Forms.Label labelRY;
        private System.Windows.Forms.Label labelRX;
        private System.Windows.Forms.Label labelZ;
        private System.Windows.Forms.Label labelY;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.Label labelFoV;
    }
}

