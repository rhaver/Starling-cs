using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using StarlingDBFreader.Properties;
using TagFixer;

// TODO: limit the number of rows in dgvTagSettings to TagNester.getMaximumTagCount() ?

namespace StarlingDBFConverter
{
    public partial class frmTags : Form
    {
        /// <summary>
        /// Form constructor.
        /// </summary>
        public frmTags()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handler for the button to restore default tag fixing values.
        /// </summary>
        private void btnRestoreDefaults_Click(object sender, EventArgs e)
        {
            dgvTagSettings.Rows.Clear();

            dgvTagSettings.Rows.Add(new String[] { @"\B", @"\b", @"\B", @"\b" });
            dgvTagSettings.Rows.Add(new String[] { @"\I", @"\i", @"\I", @"\i" });
            dgvTagSettings.Rows.Add(new String[] { @"\U", @"\u", @"\U", @"\u" });
            dgvTagSettings.Rows.Add(new String[] { @"\H", @"\h", @"\H", @"\h" });
            dgvTagSettings.Rows.Add(new String[] { @"\L", @"\l", @"\L", @"\l" });
            dgvTagSettings.Rows.Add(new String[] { @"\C", @"\c", @"\C", @"\c" });
            dgvTagSettings.Rows.Add(new String[] { @"\X", @"\x", @"\X", @"\x" });

            chkLogTagFixing.Checked = false;
        }

        /// <summary>
        /// Handler for the button to load tag fixing values that feature HTML style replacements.
        /// </summary>
        private void btnHtmlTags_Click(object sender, EventArgs e)
        {
            dgvTagSettings.Rows.Clear();

            dgvTagSettings.Rows.Add(new String[] { @"\B", @"\b", @"<b>", @"</b>" });
            dgvTagSettings.Rows.Add(new String[] { @"\I", @"\i", @"<i>", @"</i>" });
            dgvTagSettings.Rows.Add(new String[] { @"\U", @"\u", @"<u>", @"</u>" });
            dgvTagSettings.Rows.Add(new String[] { @"\H", @"\h", @"<sup>", @"</sup>" });
            dgvTagSettings.Rows.Add(new String[] { @"\L", @"\l", @"<sub>", @"</sub>" });
            dgvTagSettings.Rows.Add(new String[] { @"\C", @"\c", @"<span style=""letter-spacing: -0.2em;"">", @"</span>" });
            dgvTagSettings.Rows.Add(new String[] { @"\X", @"\x", @"<a href=""#"">", @"</a>" });
        }

        /// <summary>
        /// Handler for the button to load tag fixing values that feature square bracketed tag replacements (à la BBCode).
        /// </summary>
        private void btnSquareTags_Click(object sender, EventArgs e)
        {
            dgvTagSettings.Rows.Clear();

            dgvTagSettings.Rows.Add(new String[] { @"\B", @"\b", @"[b]", @"[/b]" });
            dgvTagSettings.Rows.Add(new String[] { @"\I", @"\i", @"[i]", @"[/i]" });
            dgvTagSettings.Rows.Add(new String[] { @"\U", @"\u", @"[u]", @"[/u]" });
            dgvTagSettings.Rows.Add(new String[] { @"\H", @"\h", @"[h]", @"[/h]" });
            dgvTagSettings.Rows.Add(new String[] { @"\L", @"\l", @"[l]", @"[/l]" });
            dgvTagSettings.Rows.Add(new String[] { @"\C", @"\c", @"[c]", @"[/c]" });
            dgvTagSettings.Rows.Add(new String[] { @"\X", @"\x", @"[x]", @"[/x]" });
        }

        /// <summary>
        /// Helper function to check if a DataGridView cell is empty.
        /// </summary>
        private bool isEmpty(Object o)
        {
            return (o == null || o == String.Empty);
        }

        /// <summary>
        /// Handler for the OK button, storing the tag fixing values.
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            StringCollection sc = new StringCollection();
            // walk through all rows
            for (int row = 0; row < dgvTagSettings.RowCount; row++)
                if (!dgvTagSettings.Rows[row].IsNewRow)
                {
                    if (isEmpty(dgvTagSettings[0, row].Value) || isEmpty(dgvTagSettings[1, row].Value))
                        MessageBox.Show(String.Format("Empty original tags are not allowed\nRow {0} is skipped.", row + 1), frmMain.appName);
                    else
                    {
                        // this row is not the new row, nor are the first two fields empty, so it contains valid tag
                        sc.Add(dgvTagSettings[0, row].Value as String);
                        sc.Add(dgvTagSettings[1, row].Value as String);
                        sc.Add(dgvTagSettings[2, row].Value as String);
                        sc.Add(dgvTagSettings[3, row].Value as String);
                    }
                }
            // store in settings
            Settings.Default.tagCollection = sc;
            Settings.Default.logTagFixing = chkLogTagFixing.Checked;
            Settings.Default.Save();
        }

        /// <summary>
        /// Handler for the form loading, initializing the DataGridView with the tags as they stored in the Settings.
        /// </summary>
        private void frmTags_Load(object sender, EventArgs e)
        {
            // read the StringList from Settings, chop it into blocks of four and add these to the datagridview
            dgvTagSettings.Rows.Clear();

            StringCollection sc = Settings.Default.tagCollection;
            for (int i = 0; (i + 3 < sc.Count) && (i / 4 < TagNester.getMaximumTagCount()); i = i + 4)
            {
                dgvTagSettings.Rows.Add(new String[] { sc[i], sc[i + 1], sc[i + 2], sc[i + 3] });
            }

            chkLogTagFixing.Checked = Settings.Default.logTagFixing;
        }

        /// <summary>
        /// Handler for copying cells from <c>dgvTagSettings</c>.
        /// </summary>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMain.copySelectedCellsToClipboard(dgvTagSettings);
        }

        /// <summary>
        /// Handler for opening the context menu for <c>dgvTagSettings</c>.
        /// </summary>
        private void contextMenuDgvTagSettings_Opening(object sender, CancelEventArgs e)
        {
            contextMenuDgvTagSettings.Enabled = dgvTagSettings.GetCellCount(DataGridViewElementStates.Selected) > 0;
        }

        /// <summary>
        /// Method to give all the rows in <c>dgvTagSettings</c> its row number as header.
        /// </summary>
        private void updateRowHeaders()
        {
            for (int row = 0; row < dgvTagSettings.RowCount; row++)
                dgvTagSettings.Rows[row].HeaderCell.Value = (row + 1).ToString();
        }

        /// <summary>
        /// Handler for row addition.
        /// </summary>
        private void dgvTagSettings_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            updateRowHeaders();
        }

        /// <summary>
        /// Handler for row removal.
        /// </summary>
        private void dgvTagSettings_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            updateRowHeaders();
        }

    }
}
