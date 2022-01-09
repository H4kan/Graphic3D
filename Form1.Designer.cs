
namespace MysteryProject
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
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.constBtn = new System.Windows.Forms.Button();
            this.gouraudBtn = new System.Windows.Forms.Button();
            this.phongBtn = new System.Windows.Forms.Button();
            this.cam1Btn = new System.Windows.Forms.Button();
            this.cam2Btn = new System.Windows.Forms.Button();
            this.cam3Btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.White;
            this.pictureBox.Location = new System.Drawing.Point(24, 9);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(653, 406);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // constBtn
            // 
            this.constBtn.Location = new System.Drawing.Point(1024, 38);
            this.constBtn.Name = "constBtn";
            this.constBtn.Size = new System.Drawing.Size(132, 40);
            this.constBtn.TabIndex = 1;
            this.constBtn.Text = "Constant shading";
            this.constBtn.UseVisualStyleBackColor = true;
            this.constBtn.Click += new System.EventHandler(this.constBtn_Click);
            // 
            // gouraudBtn
            // 
            this.gouraudBtn.Location = new System.Drawing.Point(1024, 93);
            this.gouraudBtn.Name = "gouraudBtn";
            this.gouraudBtn.Size = new System.Drawing.Size(132, 40);
            this.gouraudBtn.TabIndex = 2;
            this.gouraudBtn.Text = "Gouraud shading";
            this.gouraudBtn.UseVisualStyleBackColor = true;
            this.gouraudBtn.Click += new System.EventHandler(this.gouraudBtn_Click);
            // 
            // phongBtn
            // 
            this.phongBtn.Location = new System.Drawing.Point(1024, 148);
            this.phongBtn.Name = "phongBtn";
            this.phongBtn.Size = new System.Drawing.Size(132, 40);
            this.phongBtn.TabIndex = 3;
            this.phongBtn.Text = "Phong shading";
            this.phongBtn.UseVisualStyleBackColor = true;
            this.phongBtn.Click += new System.EventHandler(this.phongBtn_Click);
            // 
            // cam1Btn
            // 
            this.cam1Btn.Location = new System.Drawing.Point(1024, 249);
            this.cam1Btn.Name = "cam1Btn";
            this.cam1Btn.Size = new System.Drawing.Size(132, 40);
            this.cam1Btn.TabIndex = 4;
            this.cam1Btn.Text = "Camera 1";
            this.cam1Btn.UseVisualStyleBackColor = true;
            this.cam1Btn.Click += new System.EventHandler(this.cam1Btn_Click);
            // 
            // cam2Btn
            // 
            this.cam2Btn.Location = new System.Drawing.Point(1024, 304);
            this.cam2Btn.Name = "cam2Btn";
            this.cam2Btn.Size = new System.Drawing.Size(132, 40);
            this.cam2Btn.TabIndex = 5;
            this.cam2Btn.Text = "Camera 2";
            this.cam2Btn.UseVisualStyleBackColor = true;
            this.cam2Btn.Click += new System.EventHandler(this.cam2Btn_Click);
            // 
            // cam3Btn
            // 
            this.cam3Btn.Location = new System.Drawing.Point(1024, 361);
            this.cam3Btn.Name = "cam3Btn";
            this.cam3Btn.Size = new System.Drawing.Size(132, 40);
            this.cam3Btn.TabIndex = 6;
            this.cam3Btn.Text = "Camera 3";
            this.cam3Btn.UseVisualStyleBackColor = true;
            this.cam3Btn.Click += new System.EventHandler(this.cam3Btn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 660);
            this.Controls.Add(this.cam3Btn);
            this.Controls.Add(this.cam2Btn);
            this.Controls.Add(this.cam1Btn);
            this.Controls.Add(this.phongBtn);
            this.Controls.Add(this.gouraudBtn);
            this.Controls.Add(this.constBtn);
            this.Controls.Add(this.pictureBox);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button constBtn;
        private System.Windows.Forms.Button gouraudBtn;
        private System.Windows.Forms.Button phongBtn;
        private System.Windows.Forms.Button cam1Btn;
        private System.Windows.Forms.Button cam2Btn;
        private System.Windows.Forms.Button cam3Btn;
    }
}

