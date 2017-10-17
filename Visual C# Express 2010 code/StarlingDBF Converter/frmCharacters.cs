using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StarlingDBFConverter
{
    public partial class frmCharacters : Form
    {
        /// <summary>
        /// Form constructor.
        /// </summary>
        public frmCharacters()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Add a row to the DataGridView containing the character tallying information.
        /// </summary>
        /// <param name="c">The character to which the information pertains.</param>
        /// <param name="charName">The Unicode name of this character.</param>
        /// <param name="charBlock">The Unicode block to which this character belongs.</param>
        /// <param name="charFrequency">The attested frequency of this character.</param>
        public void appendListView(char c, String charName, String charBlock, uint charFrequency)
        {
            dgvCharFrequencies.Rows.Add(("U+" + ((int)c).ToString("X4")), c, charName, charBlock, charFrequency);
        }

        /// <summary>
        /// Handler for copying cells from <c>dgvCharFrequencies</c>.
        /// </summary>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMain.copySelectedCellsToClipboard(dgvCharFrequencies);
        }

        /// <summary>
        /// Handler for opening the context menu for <c>dgvCharFrequencies</c>.
        /// </summary>
        private void contextMenuDgvCharFrequencies_Opening(object sender, CancelEventArgs e)
        {
            contextMenuDgvCharFrequencies.Enabled = dgvCharFrequencies.GetCellCount(DataGridViewElementStates.Selected) > 0;
        }
    }
}
