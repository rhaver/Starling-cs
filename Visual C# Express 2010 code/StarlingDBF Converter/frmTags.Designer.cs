namespace StarlingDBFConverter
{
    partial class frmTags
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvTagSettings = new System.Windows.Forms.DataGridView();
            this.colTagStartOld = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTagEndOld = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTagStartNew = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTagEndNew = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuDgvTagSettings = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnRestoreDefaults = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkLogTagFixing = new System.Windows.Forms.CheckBox();
            this.btnHtmlTags = new System.Windows.Forms.Button();
            this.btnSquareTags = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTagSettings)).BeginInit();
            this.contextMenuDgvTagSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvTagSettings
            // 
            this.dgvTagSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTagSettings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTagSettings.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTagSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTagSettings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTagStartOld,
            this.colTagEndOld,
            this.colTagStartNew,
            this.colTagEndNew});
            this.dgvTagSettings.ContextMenuStrip = this.contextMenuDgvTagSettings;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTagSettings.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvTagSettings.Location = new System.Drawing.Point(20, 17);
            this.dgvTagSettings.Name = "dgvTagSettings";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.NullValue = null;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTagSettings.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvTagSettings.Size = new System.Drawing.Size(514, 197);
            this.dgvTagSettings.TabIndex = 0;
            this.dgvTagSettings.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvTagSettings_RowsAdded);
            this.dgvTagSettings.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgvTagSettings_RowsRemoved);
            // 
            // colTagStartOld
            // 
            this.colTagStartOld.HeaderText = "Original opening tag";
            this.colTagStartOld.Name = "colTagStartOld";
            // 
            // colTagEndOld
            // 
            this.colTagEndOld.HeaderText = "Original closing tag";
            this.colTagEndOld.Name = "colTagEndOld";
            // 
            // colTagStartNew
            // 
            this.colTagStartNew.HeaderText = "Replacement opening tag";
            this.colTagStartNew.Name = "colTagStartNew";
            // 
            // colTagEndNew
            // 
            this.colTagEndNew.HeaderText = "Replacement closing tag";
            this.colTagEndNew.Name = "colTagEndNew";
            // 
            // contextMenuDgvTagSettings
            // 
            this.contextMenuDgvTagSettings.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.contextMenuDgvTagSettings.Name = "contextMenuDgvTagSettings";
            this.contextMenuDgvTagSettings.Size = new System.Drawing.Size(233, 26);
            this.contextMenuDgvTagSettings.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuDgvTagSettings_Opening);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.copyToolStripMenuItem.Text = "Copy selected cell(s) to clipboard";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(378, 242);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnRestoreDefaults
            // 
            this.btnRestoreDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRestoreDefaults.Location = new System.Drawing.Point(20, 242);
            this.btnRestoreDefaults.Name = "btnRestoreDefaults";
            this.btnRestoreDefaults.Size = new System.Drawing.Size(143, 23);
            this.btnRestoreDefaults.TabIndex = 2;
            this.btnRestoreDefaults.Text = "load default tag settings";
            this.btnRestoreDefaults.UseVisualStyleBackColor = true;
            this.btnRestoreDefaults.Click += new System.EventHandler(this.btnRestoreDefaults_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(459, 242);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkLogTagFixing
            // 
            this.chkLogTagFixing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkLogTagFixing.AutoSize = true;
            this.chkLogTagFixing.Location = new System.Drawing.Point(20, 220);
            this.chkLogTagFixing.Name = "chkLogTagFixing";
            this.chkLogTagFixing.Size = new System.Drawing.Size(225, 17);
            this.chkLogTagFixing.TabIndex = 4;
            this.chkLogTagFixing.Text = "append cells impacted by correction to log";
            this.chkLogTagFixing.UseVisualStyleBackColor = true;
            // 
            // btnHtmlTags
            // 
            this.btnHtmlTags.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnHtmlTags.Location = new System.Drawing.Point(169, 242);
            this.btnHtmlTags.Name = "btnHtmlTags";
            this.btnHtmlTags.Size = new System.Drawing.Size(75, 23);
            this.btnHtmlTags.TabIndex = 5;
            this.btnHtmlTags.Text = "HTML tags";
            this.btnHtmlTags.UseVisualStyleBackColor = true;
            this.btnHtmlTags.Click += new System.EventHandler(this.btnHtmlTags_Click);
            // 
            // btnSquareTags
            // 
            this.btnSquareTags.Location = new System.Drawing.Point(250, 242);
            this.btnSquareTags.Name = "btnSquareTags";
            this.btnSquareTags.Size = new System.Drawing.Size(75, 23);
            this.btnSquareTags.TabIndex = 6;
            this.btnSquareTags.Text = "[ ]-tags";
            this.btnSquareTags.UseVisualStyleBackColor = true;
            this.btnSquareTags.Click += new System.EventHandler(this.btnSquareTags_Click);
            // 
            // frmTags
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 273);
            this.Controls.Add(this.btnSquareTags);
            this.Controls.Add(this.btnHtmlTags);
            this.Controls.Add(this.chkLogTagFixing);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRestoreDefaults);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dgvTagSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(440, 240);
            this.Name = "frmTags";
            this.Text = "Layout tag settings";
            this.Load += new System.EventHandler(this.frmTags_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTagSettings)).EndInit();
            this.contextMenuDgvTagSettings.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvTagSettings;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnRestoreDefaults;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ContextMenuStrip contextMenuDgvTagSettings;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTagStartOld;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTagEndOld;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTagStartNew;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTagEndNew;
        private System.Windows.Forms.CheckBox chkLogTagFixing;
        private System.Windows.Forms.Button btnHtmlTags;
        private System.Windows.Forms.Button btnSquareTags;
    }
}