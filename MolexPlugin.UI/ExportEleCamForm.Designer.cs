namespace MolexPlugin.UI
{
    partial class ExportEleCamForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportEleCamForm));
            this.listViewEleInfo = new System.Windows.Forms.ListView();
            this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label3 = new System.Windows.Forms.Label();
            this.butSave = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.but_Close = new System.Windows.Forms.Button();
            this.butOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewEleInfo
            // 
            this.listViewEleInfo.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewEleInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewEleInfo.CheckBoxes = true;
            this.listViewEleInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader,
            this.columnHeader1});
            this.listViewEleInfo.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listViewEleInfo.FullRowSelect = true;
            this.listViewEleInfo.GridLines = true;
            this.listViewEleInfo.Location = new System.Drawing.Point(13, 19);
            this.listViewEleInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listViewEleInfo.Name = "listViewEleInfo";
            this.listViewEleInfo.Size = new System.Drawing.Size(302, 308);
            this.listViewEleInfo.TabIndex = 27;
            this.listViewEleInfo.UseCompatibleStateImageBehavior = false;
            this.listViewEleInfo.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader
            // 
            this.columnHeader.Text = "选择";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "电极名";
            this.columnHeader1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader1.Width = 240;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(6, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 19);
            this.label3.TabIndex = 28;
            this.label3.Text = "保存";
            // 
            // butSave
            // 
            this.butSave.BackColor = System.Drawing.Color.White;
            this.butSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butSave.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.butSave.Image = ((System.Drawing.Image)(resources.GetObject("butSave.Image")));
            this.butSave.Location = new System.Drawing.Point(264, 21);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(51, 44);
            this.butSave.TabIndex = 29;
            this.butSave.UseVisualStyleBackColor = false;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.butSave);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(0, 351);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(324, 71);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "设置";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listViewEleInfo);
            this.groupBox2.Location = new System.Drawing.Point(0, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(324, 340);
            this.groupBox2.TabIndex = 30;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "电极信息";
            // 
            // but_Close
            // 
            this.but_Close.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.but_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.but_Close.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.but_Close.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.but_Close.Location = new System.Drawing.Point(179, 434);
            this.but_Close.Name = "but_Close";
            this.but_Close.Size = new System.Drawing.Size(81, 38);
            this.but_Close.TabIndex = 31;
            this.but_Close.Text = "取消";
            this.but_Close.UseVisualStyleBackColor = false;
            this.but_Close.Click += new System.EventHandler(this.but_Close_Click);
            // 
            // butOK
            // 
            this.butOK.BackColor = System.Drawing.Color.PaleTurquoise;
            this.butOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.butOK.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.butOK.Location = new System.Drawing.Point(47, 434);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(81, 38);
            this.butOK.TabIndex = 32;
            this.butOK.Text = "确定";
            this.butOK.UseVisualStyleBackColor = false;
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // ExportEleCamForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(333, 484);
            this.Controls.Add(this.but_Close);
            this.Controls.Add(this.butOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportEleCamForm";
            this.ShowInTaskbar = false;
            this.Text = "导出电极程序";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewEleInfo;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button butSave;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button but_Close;
        private System.Windows.Forms.Button butOK;
    }
}