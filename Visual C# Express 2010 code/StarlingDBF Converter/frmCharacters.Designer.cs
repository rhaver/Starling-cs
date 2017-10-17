namespace StarlingDBFConverter
{
    partial class frmCharacters
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
            this.dgvCharFrequencies = new System.Windows.Forms.DataGridView();
            this.contextMenuDgvCharFrequencies = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCharFrequencies)).BeginInit();
            this.contextMenuDgvCharFrequencies.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvCharFrequencies
            // 
            this.dgvCharFrequencies.AllowUserToAddRows = false;
            this.dgvCharFrequencies.AllowUserToDeleteRows = false;
            this.dgvCharFrequencies.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCharFrequencies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCharFrequencies.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column5,
            this.Column4});
            this.dgvCharFrequencies.ContextMenuStrip = this.contextMenuDgvCharFrequencies;
            this.dgvCharFrequencies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCharFrequencies.Location = new System.Drawing.Point(0, 0);
            this.dgvCharFrequencies.Name = "dgvCharFrequencies";
            this.dgvCharFrequencies.ReadOnly = true;
            this.dgvCharFrequencies.RowHeadersVisible = false;
            this.dgvCharFrequencies.Size = new System.Drawing.Size(516, 273);
            this.dgvCharFrequencies.TabIndex = 1;
            // 
            // contextMenuDgvCharFrequencies
            // 
            this.contextMenuDgvCharFrequencies.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.contextMenuDgvCharFrequencies.Name = "contextMenuDgvCharFrequencies";
            this.contextMenuDgvCharFrequencies.Size = new System.Drawing.Size(233, 26);
            this.contextMenuDgvCharFrequencies.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuDgvCharFrequencies_Opening);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.copyToolStripMenuItem.Text = "Copy selected cell(s) to clipboard";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // Column1
            // 
            this.Column1.FillWeight = 80F;
            this.Column1.HeaderText = "code point";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.ToolTipText = "Unicode code point for the character.";
            // 
            // Column2
            // 
            this.Column2.FillWeight = 88.83249F;
            this.Column2.HeaderText = "character";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.ToolTipText = "Unicode character.";
            // 
            // Column3
            // 
            this.Column3.FillWeight = 52.54047F;
            this.Column3.HeaderText = "name";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.ToolTipText = "Unicode name of the character.";
            // 
            // Column5
            // 
            this.Column5.FillWeight = 150.0775F;
            this.Column5.HeaderText = "block";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.ToolTipText = "Unicode block to which the character belongs.";
            // 
            // Column4
            // 
            this.Column4.FillWeight = 131.8272F;
            this.Column4.HeaderText = "count";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.ToolTipText = "The number of times this character occurs in the text fields of the opened Starli" +
    "ng database.";
            // 
            // frmCharacters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 273);
            this.Controls.Add(this.dgvCharFrequencies);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "frmCharacters";
            this.Text = "Character frequencies";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCharFrequencies)).EndInit();
            this.contextMenuDgvCharFrequencies.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvCharFrequencies;
        private System.Windows.Forms.ContextMenuStrip contextMenuDgvCharFrequencies;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
    }
}