namespace StarlingDBFConverter
{
    partial class frmMain
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvDbf = new System.Windows.Forms.DataGridView();
            this.contextMenuDgvDbf = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mySqlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unicodeNormalizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.composedCharactersNFCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decomposedCharactersNFDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.performTagCorrectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editTagSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oCRErrorLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tallyCharacterFrequenciesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.markIsolatedCyrillicCharactersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goTonextMarkedCellToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearMarkingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateCharacterTablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripLabelRow = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripLabelColumn = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControlDatabase = new System.Windows.Forms.TabControl();
            this.tabDbf = new System.Windows.Forms.TabPage();
            this.tabInf = new System.Windows.Forms.TabPage();
            this.txtInf = new System.Windows.Forms.TextBox();
            this.tabPrt = new System.Windows.Forms.TabPage();
            this.dgvPrt = new System.Windows.Forms.DataGridView();
            this.contextMenuDgvPrt = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.openDbfDialog = new System.Windows.Forms.OpenFileDialog();
            this.backgroundDbfLoader = new System.ComponentModel.BackgroundWorker();
            this.backgroundNormalizer = new System.ComponentModel.BackgroundWorker();
            this.backgroundTagFixer = new System.ComponentModel.BackgroundWorker();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.backgroundCyrillicMarker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDbf)).BeginInit();
            this.contextMenuDgvDbf.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControlDatabase.SuspendLayout();
            this.tabDbf.SuspendLayout();
            this.tabInf.SuspendLayout();
            this.tabPrt.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrt)).BeginInit();
            this.contextMenuDgvPrt.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvDbf
            // 
            this.dgvDbf.AllowUserToAddRows = false;
            this.dgvDbf.AllowUserToDeleteRows = false;
            this.dgvDbf.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDbf.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDbf.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDbf.ContextMenuStrip = this.contextMenuDgvDbf;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDbf.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvDbf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDbf.Location = new System.Drawing.Point(3, 3);
            this.dgvDbf.Name = "dgvDbf";
            this.dgvDbf.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDbf.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvDbf.Size = new System.Drawing.Size(492, 288);
            this.dgvDbf.TabIndex = 0;
            this.dgvDbf.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEnter);
            // 
            // contextMenuDgvDbf
            // 
            this.contextMenuDgvDbf.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.contextMenuDgvDbf.Name = "contextMenuDgvDbf";
            this.contextMenuDgvDbf.Size = new System.Drawing.Size(233, 26);
            this.contextMenuDgvDbf.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuDgvDbf_Opening);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.copyToolStripMenuItem.Text = "Copy selected cell(s) to clipboard";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(506, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cSVToolStripMenuItem,
            this.mySqlToolStripMenuItem});
            this.exportToolStripMenuItem.Enabled = false;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportToolStripMenuItem.Text = "&Export all";
            // 
            // cSVToolStripMenuItem
            // 
            this.cSVToolStripMenuItem.Name = "cSVToolStripMenuItem";
            this.cSVToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.cSVToolStripMenuItem.Text = "Text && &CSV";
            this.cSVToolStripMenuItem.Click += new System.EventHandler(this.cSVToolStripMenuItem_Click);
            // 
            // mySqlToolStripMenuItem
            // 
            this.mySqlToolStripMenuItem.Name = "mySqlToolStripMenuItem";
            this.mySqlToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.mySqlToolStripMenuItem.Text = "Text && &MySql";
            this.mySqlToolStripMenuItem.Click += new System.EventHandler(this.mySqlToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Enabled = false;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(149, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem,
            this.findNextToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            this.editToolStripMenuItem.DropDownOpening += new System.EventHandler(this.editToolStripMenuItem_DropDownOpening);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.findToolStripMenuItem.Text = "&Find in main database...";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // findNextToolStripMenuItem
            // 
            this.findNextToolStripMenuItem.Enabled = false;
            this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
            this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findNextToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.findNextToolStripMenuItem.Text = "Find &next";
            this.findNextToolStripMenuItem.Click += new System.EventHandler(this.findNextToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.unicodeNormalizationToolStripMenuItem,
            this.tagsToolStripMenuItem,
            this.oCRErrorLocationToolStripMenuItem,
            this.generateCharacterTablesToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            this.toolsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.toolsToolStripMenuItem_DropDownOpening);
            // 
            // unicodeNormalizationToolStripMenuItem
            // 
            this.unicodeNormalizationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.composedCharactersNFCToolStripMenuItem,
            this.decomposedCharactersNFDToolStripMenuItem});
            this.unicodeNormalizationToolStripMenuItem.Name = "unicodeNormalizationToolStripMenuItem";
            this.unicodeNormalizationToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.unicodeNormalizationToolStripMenuItem.Text = "&Unicode normalization";
            this.unicodeNormalizationToolStripMenuItem.DropDownOpening += new System.EventHandler(this.unicodeNormalizationToolStripMenuItem_DropDownOpening);
            // 
            // composedCharactersNFCToolStripMenuItem
            // 
            this.composedCharactersNFCToolStripMenuItem.Name = "composedCharactersNFCToolStripMenuItem";
            this.composedCharactersNFCToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.composedCharactersNFCToolStripMenuItem.Text = "&Composed characters (NFC)";
            this.composedCharactersNFCToolStripMenuItem.Click += new System.EventHandler(this.unicodeNormalizationFormToolStripMenuItem_Click);
            // 
            // decomposedCharactersNFDToolStripMenuItem
            // 
            this.decomposedCharactersNFDToolStripMenuItem.Name = "decomposedCharactersNFDToolStripMenuItem";
            this.decomposedCharactersNFDToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.decomposedCharactersNFDToolStripMenuItem.Text = "&Decomposed characters (NFD)";
            this.decomposedCharactersNFDToolStripMenuItem.Click += new System.EventHandler(this.unicodeNormalizationFormToolStripMenuItem_Click);
            // 
            // tagsToolStripMenuItem
            // 
            this.tagsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.performTagCorrectionToolStripMenuItem,
            this.editTagSettingsToolStripMenuItem});
            this.tagsToolStripMenuItem.Name = "tagsToolStripMenuItem";
            this.tagsToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.tagsToolStripMenuItem.Text = "&Layout tags";
            // 
            // performTagCorrectionToolStripMenuItem
            // 
            this.performTagCorrectionToolStripMenuItem.Name = "performTagCorrectionToolStripMenuItem";
            this.performTagCorrectionToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.performTagCorrectionToolStripMenuItem.Text = "Perform tag &correction";
            this.performTagCorrectionToolStripMenuItem.Click += new System.EventHandler(this.performTagCorrectionToolStripMenuItem_Click);
            // 
            // editTagSettingsToolStripMenuItem
            // 
            this.editTagSettingsToolStripMenuItem.Name = "editTagSettingsToolStripMenuItem";
            this.editTagSettingsToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.editTagSettingsToolStripMenuItem.Text = "&Edit tag settings...";
            this.editTagSettingsToolStripMenuItem.Click += new System.EventHandler(this.editTagSettingsToolStripMenuItem_Click);
            // 
            // oCRErrorLocationToolStripMenuItem
            // 
            this.oCRErrorLocationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tallyCharacterFrequenciesToolStripMenuItem,
            this.toolStripMenuItem3,
            this.markIsolatedCyrillicCharactersToolStripMenuItem,
            this.goTonextMarkedCellToolStripMenuItem,
            this.clearMarkingToolStripMenuItem});
            this.oCRErrorLocationToolStripMenuItem.Name = "oCRErrorLocationToolStripMenuItem";
            this.oCRErrorLocationToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.oCRErrorLocationToolStripMenuItem.Text = "&OCR error locating";
            // 
            // tallyCharacterFrequenciesToolStripMenuItem
            // 
            this.tallyCharacterFrequenciesToolStripMenuItem.Name = "tallyCharacterFrequenciesToolStripMenuItem";
            this.tallyCharacterFrequenciesToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.tallyCharacterFrequenciesToolStripMenuItem.Text = "&Tally character frequencies";
            this.tallyCharacterFrequenciesToolStripMenuItem.Click += new System.EventHandler(this.tallyCharacterFrequenciesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(221, 6);
            // 
            // markIsolatedCyrillicCharactersToolStripMenuItem
            // 
            this.markIsolatedCyrillicCharactersToolStripMenuItem.Name = "markIsolatedCyrillicCharactersToolStripMenuItem";
            this.markIsolatedCyrillicCharactersToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.markIsolatedCyrillicCharactersToolStripMenuItem.Text = "&Mark isolated Cyrillic characters";
            this.markIsolatedCyrillicCharactersToolStripMenuItem.Click += new System.EventHandler(this.markIsolatedCyrillicCharactersToolStripMenuItem_Click);
            // 
            // goTonextMarkedCellToolStripMenuItem
            // 
            this.goTonextMarkedCellToolStripMenuItem.Enabled = false;
            this.goTonextMarkedCellToolStripMenuItem.Name = "goTonextMarkedCellToolStripMenuItem";
            this.goTonextMarkedCellToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.goTonextMarkedCellToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.goTonextMarkedCellToolStripMenuItem.Text = "Go to &next marked cell";
            this.goTonextMarkedCellToolStripMenuItem.Click += new System.EventHandler(this.goTonextMarkedCellToolStripMenuItem_Click);
            // 
            // clearMarkingToolStripMenuItem
            // 
            this.clearMarkingToolStripMenuItem.Enabled = false;
            this.clearMarkingToolStripMenuItem.Name = "clearMarkingToolStripMenuItem";
            this.clearMarkingToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.clearMarkingToolStripMenuItem.Text = "&Clear marking";
            this.clearMarkingToolStripMenuItem.Click += new System.EventHandler(this.clearMarkingToolStripMenuItem_Click);
            // 
            // generateCharacterTablesToolStripMenuItem
            // 
            this.generateCharacterTablesToolStripMenuItem.Name = "generateCharacterTablesToolStripMenuItem";
            this.generateCharacterTablesToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.generateCharacterTablesToolStripMenuItem.Text = "Generate character tables";
            this.generateCharacterTablesToolStripMenuItem.Visible = false;
            this.generateCharacterTablesToolStripMenuItem.Click += new System.EventHandler(this.generateCharacterTablesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1,
            this.toolStripLabelRow,
            this.toolStripStatusLabel2,
            this.toolStripLabelColumn});
            this.statusStrip1.Location = new System.Drawing.Point(0, 344);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(506, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Enabled = false;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(150, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.toolStripProgressBar1.EnabledChanged += new System.EventHandler(this.toolStripProgressBar1_EnabledChanged);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(29, 17);
            this.toolStripStatusLabel1.Text = "row:";
            // 
            // toolStripLabelRow
            // 
            this.toolStripLabelRow.AutoSize = false;
            this.toolStripLabelRow.Name = "toolStripLabelRow";
            this.toolStripLabelRow.Size = new System.Drawing.Size(93, 17);
            this.toolStripLabelRow.Text = "–";
            this.toolStripLabelRow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(44, 17);
            this.toolStripStatusLabel2.Text = "column:";
            // 
            // toolStripLabelColumn
            // 
            this.toolStripLabelColumn.AutoSize = false;
            this.toolStripLabelColumn.Name = "toolStripLabelColumn";
            this.toolStripLabelColumn.Size = new System.Drawing.Size(107, 17);
            this.toolStripLabelColumn.Text = "–";
            this.toolStripLabelColumn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabControlDatabase
            // 
            this.tabControlDatabase.Controls.Add(this.tabDbf);
            this.tabControlDatabase.Controls.Add(this.tabInf);
            this.tabControlDatabase.Controls.Add(this.tabPrt);
            this.tabControlDatabase.Controls.Add(this.tabLog);
            this.tabControlDatabase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlDatabase.Location = new System.Drawing.Point(0, 24);
            this.tabControlDatabase.Name = "tabControlDatabase";
            this.tabControlDatabase.SelectedIndex = 0;
            this.tabControlDatabase.Size = new System.Drawing.Size(506, 320);
            this.tabControlDatabase.TabIndex = 1;
            // 
            // tabDbf
            // 
            this.tabDbf.Controls.Add(this.dgvDbf);
            this.tabDbf.Location = new System.Drawing.Point(4, 22);
            this.tabDbf.Name = "tabDbf";
            this.tabDbf.Padding = new System.Windows.Forms.Padding(3);
            this.tabDbf.Size = new System.Drawing.Size(498, 294);
            this.tabDbf.TabIndex = 0;
            this.tabDbf.Text = "Data";
            this.tabDbf.UseVisualStyleBackColor = true;
            // 
            // tabInf
            // 
            this.tabInf.Controls.Add(this.txtInf);
            this.tabInf.Location = new System.Drawing.Point(4, 22);
            this.tabInf.Name = "tabInf";
            this.tabInf.Padding = new System.Windows.Forms.Padding(3);
            this.tabInf.Size = new System.Drawing.Size(498, 294);
            this.tabInf.TabIndex = 1;
            this.tabInf.Text = "INF";
            this.tabInf.UseVisualStyleBackColor = true;
            // 
            // txtInf
            // 
            this.txtInf.AcceptsReturn = true;
            this.txtInf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInf.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInf.Location = new System.Drawing.Point(3, 3);
            this.txtInf.Multiline = true;
            this.txtInf.Name = "txtInf";
            this.txtInf.ReadOnly = true;
            this.txtInf.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInf.Size = new System.Drawing.Size(492, 288);
            this.txtInf.TabIndex = 0;
            // 
            // tabPrt
            // 
            this.tabPrt.Controls.Add(this.dgvPrt);
            this.tabPrt.Location = new System.Drawing.Point(4, 22);
            this.tabPrt.Name = "tabPrt";
            this.tabPrt.Size = new System.Drawing.Size(498, 294);
            this.tabPrt.TabIndex = 2;
            this.tabPrt.Text = "PRT";
            this.tabPrt.UseVisualStyleBackColor = true;
            // 
            // dgvPrt
            // 
            this.dgvPrt.AllowUserToAddRows = false;
            this.dgvPrt.AllowUserToDeleteRows = false;
            this.dgvPrt.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvPrt.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPrt.ContextMenuStrip = this.contextMenuDgvPrt;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPrt.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvPrt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPrt.Location = new System.Drawing.Point(0, 0);
            this.dgvPrt.Name = "dgvPrt";
            this.dgvPrt.ReadOnly = true;
            this.dgvPrt.Size = new System.Drawing.Size(498, 294);
            this.dgvPrt.TabIndex = 0;
            this.dgvPrt.DataSourceChanged += new System.EventHandler(this.dgv_DataSourceChanged);
            this.dgvPrt.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEnter);
            // 
            // contextMenuDgvPrt
            // 
            this.contextMenuDgvPrt.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem1});
            this.contextMenuDgvPrt.Name = "contextMenuDgvPrt";
            this.contextMenuDgvPrt.Size = new System.Drawing.Size(233, 26);
            this.contextMenuDgvPrt.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuDgvPrt_Opening);
            // 
            // copyToolStripMenuItem1
            // 
            this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
            this.copyToolStripMenuItem1.Size = new System.Drawing.Size(232, 22);
            this.copyToolStripMenuItem1.Text = "Copy selected cell(s) to clipboard";
            this.copyToolStripMenuItem1.Click += new System.EventHandler(this.copyToolStripMenuItem1_Click);
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.txtLog);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Size = new System.Drawing.Size(498, 294);
            this.tabLog.TabIndex = 3;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.Location = new System.Drawing.Point(0, 0);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(498, 294);
            this.txtLog.TabIndex = 0;
            // 
            // openDbfDialog
            // 
            this.openDbfDialog.Filter = "Starling database (*.dbf)|*.dbf";
            // 
            // backgroundDbfLoader
            // 
            this.backgroundDbfLoader.WorkerReportsProgress = true;
            this.backgroundDbfLoader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundDbfLoader_DoWork);
            this.backgroundDbfLoader.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundDbfLoader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundDbfLoader_RunWorkerCompleted);
            // 
            // backgroundNormalizer
            // 
            this.backgroundNormalizer.WorkerReportsProgress = true;
            this.backgroundNormalizer.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundNormalizer_DoWork);
            this.backgroundNormalizer.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundNormalizer.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundNormalizer_RunWorkerCompleted);
            // 
            // backgroundTagFixer
            // 
            this.backgroundTagFixer.WorkerReportsProgress = true;
            this.backgroundTagFixer.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundTagFixer_DoWork);
            this.backgroundTagFixer.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundTagFixer.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundTagFixer_RunWorkerCompleted);
            // 
            // backgroundCyrillicMarker
            // 
            this.backgroundCyrillicMarker.WorkerReportsProgress = true;
            this.backgroundCyrillicMarker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundCyrillicMarker_DoWork);
            this.backgroundCyrillicMarker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundCyrillicMarker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundCyrillicMarker_RunWorkerCompleted);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 366);
            this.Controls.Add(this.tabControlDatabase);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.Text = "frmMain";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDbf)).EndInit();
            this.contextMenuDgvDbf.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControlDatabase.ResumeLayout(false);
            this.tabDbf.ResumeLayout(false);
            this.tabInf.ResumeLayout(false);
            this.tabInf.PerformLayout();
            this.tabPrt.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrt)).EndInit();
            this.contextMenuDgvPrt.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            this.tabLog.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDbf;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mySqlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cSVToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openDbfDialog;
        private System.Windows.Forms.ToolStripStatusLabel toolStripLabelRow;
        private System.Windows.Forms.ToolStripStatusLabel toolStripLabelColumn;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.ComponentModel.BackgroundWorker backgroundDbfLoader;
        private System.Windows.Forms.TabControl tabControlDatabase;
        private System.Windows.Forms.TabPage tabDbf;
        private System.Windows.Forms.TabPage tabInf;
        private System.Windows.Forms.TabPage tabPrt;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.DataGridView dgvPrt;
        private System.Windows.Forms.TextBox txtInf;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unicodeNormalizationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem composedCharactersNFCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decomposedCharactersNFDToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundNormalizer;
        private System.Windows.Forms.ContextMenuStrip contextMenuDgvDbf;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuDgvPrt;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tagsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem performTagCorrectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editTagSettingsToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundTagFixer;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolStripMenuItem oCRErrorLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem markIsolatedCyrillicCharactersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearMarkingToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.ComponentModel.BackgroundWorker backgroundCyrillicMarker;
        private System.Windows.Forms.ToolStripMenuItem tallyCharacterFrequenciesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goTonextMarkedCellToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateCharacterTablesToolStripMenuItem;
    }
}

