
namespace OmenHubLight.Forms
{
    partial class FormSetFourZone
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
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.checkBoxEnable = new System.Windows.Forms.CheckBox();
            this.colorBox1 = new System.Windows.Forms.PictureBox();
            this.colorBox2 = new System.Windows.Forms.PictureBox();
            this.colorBox4 = new System.Windows.Forms.PictureBox();
            this.colorBox3 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.colorBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // colorDialog
            // 
            this.colorDialog.Color = System.Drawing.Color.Red;
            this.colorDialog.FullOpen = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(362, 41);
            this.label3.TabIndex = 5;
            this.label3.Text = "Four Zone KeyBoard Light";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(32, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 28);
            this.label1.TabIndex = 6;
            this.label1.Text = "Enable Light";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(32, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 28);
            this.label2.TabIndex = 7;
            this.label2.Text = "Zone 1 (Right)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(32, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(151, 28);
            this.label4.TabIndex = 8;
            this.label4.Text = "Zone 2 (Middle)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(32, 190);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(121, 28);
            this.label5.TabIndex = 9;
            this.label5.Text = "Zone 3 (Left)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(32, 230);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(145, 28);
            this.label6.TabIndex = 10;
            this.label6.Text = "Zone 4 (WASD)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(32, 309);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(365, 20);
            this.label7.TabIndex = 11;
            this.label7.Text = "Pure RGB color may perform better on your keyboard.";
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.buttonCancel.Location = new System.Drawing.Point(662, 305);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(126, 32);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.buttonOk.Location = new System.Drawing.Point(530, 305);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(126, 32);
            this.buttonOk.TabIndex = 13;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = false;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // checkBoxEnable
            // 
            this.checkBoxEnable.AutoSize = true;
            this.checkBoxEnable.Location = new System.Drawing.Point(273, 78);
            this.checkBoxEnable.Name = "checkBoxEnable";
            this.checkBoxEnable.Size = new System.Drawing.Size(18, 17);
            this.checkBoxEnable.TabIndex = 14;
            this.checkBoxEnable.UseVisualStyleBackColor = true;
            // 
            // colorBox1
            // 
            this.colorBox1.BackColor = System.Drawing.Color.Red;
            this.colorBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorBox1.Location = new System.Drawing.Point(269, 115);
            this.colorBox1.Name = "colorBox1";
            this.colorBox1.Size = new System.Drawing.Size(25, 25);
            this.colorBox1.TabIndex = 15;
            this.colorBox1.TabStop = false;
            this.colorBox1.Click += new System.EventHandler(this.colorBox_Click);
            // 
            // colorBox2
            // 
            this.colorBox2.BackColor = System.Drawing.Color.Green;
            this.colorBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorBox2.Location = new System.Drawing.Point(269, 155);
            this.colorBox2.Name = "colorBox2";
            this.colorBox2.Size = new System.Drawing.Size(25, 25);
            this.colorBox2.TabIndex = 16;
            this.colorBox2.TabStop = false;
            this.colorBox2.Click += new System.EventHandler(this.colorBox_Click);
            // 
            // colorBox4
            // 
            this.colorBox4.BackColor = System.Drawing.Color.Yellow;
            this.colorBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorBox4.Location = new System.Drawing.Point(269, 235);
            this.colorBox4.Name = "colorBox4";
            this.colorBox4.Size = new System.Drawing.Size(25, 25);
            this.colorBox4.TabIndex = 17;
            this.colorBox4.TabStop = false;
            this.colorBox4.Click += new System.EventHandler(this.colorBox_Click);
            // 
            // colorBox3
            // 
            this.colorBox3.BackColor = System.Drawing.Color.Blue;
            this.colorBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorBox3.Location = new System.Drawing.Point(269, 195);
            this.colorBox3.Name = "colorBox3";
            this.colorBox3.Size = new System.Drawing.Size(25, 25);
            this.colorBox3.TabIndex = 18;
            this.colorBox3.TabStop = false;
            this.colorBox3.Click += new System.EventHandler(this.colorBox_Click);
            // 
            // FormSetFourZone
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(800, 349);
            this.Controls.Add(this.colorBox3);
            this.Controls.Add(this.colorBox4);
            this.Controls.Add(this.colorBox2);
            this.Controls.Add(this.colorBox1);
            this.Controls.Add(this.checkBoxEnable);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSetFourZone";
            this.Text = "Four Zone KeyBoard Light";
            this.Load += new System.EventHandler(this.FormSetFourZone_Load);
            ((System.ComponentModel.ISupportInitialize)(this.colorBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.CheckBox checkBoxEnable;
        private System.Windows.Forms.PictureBox colorBox1;
        private System.Windows.Forms.PictureBox colorBox2;
        private System.Windows.Forms.PictureBox colorBox4;
        private System.Windows.Forms.PictureBox colorBox3;
    }
}