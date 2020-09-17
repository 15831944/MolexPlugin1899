namespace MolexPlugin
{
    partial class ElectrodeERForm
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
            this.textBoxXNumber = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.buttOK = new System.Windows.Forms.Button();
            this.buttCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_YNumber = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxXNumber
            // 
            this.textBoxXNumber.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxXNumber.Location = new System.Drawing.Point(131, 33);
            this.textBoxXNumber.Name = "textBoxXNumber";
            this.textBoxXNumber.Size = new System.Drawing.Size(71, 25);
            this.textBoxXNumber.TabIndex = 4;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label15.Location = new System.Drawing.Point(22, 39);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(43, 19);
            this.label15.TabIndex = 5;
            this.label15.Text = "X个数";
            // 
            // buttOK
            // 
            this.buttOK.BackColor = System.Drawing.Color.White;
            this.buttOK.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttOK.ForeColor = System.Drawing.SystemColors.MenuText;
            this.buttOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttOK.Location = new System.Drawing.Point(12, 163);
            this.buttOK.Name = "buttOK";
            this.buttOK.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttOK.Size = new System.Drawing.Size(71, 35);
            this.buttOK.TabIndex = 20;
            this.buttOK.Text = "确定";
            this.buttOK.UseVisualStyleBackColor = false;
            this.buttOK.Click += new System.EventHandler(this.buttOK_Click);
            // 
            // buttCancel
            // 
            this.buttCancel.BackColor = System.Drawing.Color.White;
            this.buttCancel.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttCancel.ForeColor = System.Drawing.SystemColors.MenuText;
            this.buttCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttCancel.Location = new System.Drawing.Point(131, 163);
            this.buttCancel.Name = "buttCancel";
            this.buttCancel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttCancel.Size = new System.Drawing.Size(71, 35);
            this.buttCancel.TabIndex = 20;
            this.buttCancel.Text = "取消";
            this.buttCancel.UseVisualStyleBackColor = false;
            this.buttCancel.Click += new System.EventHandler(this.buttCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(22, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 19);
            this.label1.TabIndex = 5;
            this.label1.Text = "Y个数";
            // 
            // textBox_YNumber
            // 
            this.textBox_YNumber.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_YNumber.Location = new System.Drawing.Point(131, 92);
            this.textBox_YNumber.Name = "textBox_YNumber";
            this.textBox_YNumber.Size = new System.Drawing.Size(71, 25);
            this.textBox_YNumber.TabIndex = 4;
            // 
            // ElectrodeERForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(225, 222);
            this.Controls.Add(this.buttCancel);
            this.Controls.Add(this.buttOK);
            this.Controls.Add(this.textBox_YNumber);
            this.Controls.Add(this.textBoxXNumber);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label15);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ElectrodeERForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ElectrodeERForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxXNumber;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button buttOK;
        private System.Windows.Forms.Button buttCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_YNumber;
    }
}