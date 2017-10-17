using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Runtime.InteropServices;

using StarlingDBFreader.Properties;
using StarlingDB;
using TagFixer;
using StringUtils;
using CustomDialogs;
using DataTableUtils;

/*
 * Low priority requirements:
 * - search/find
 *  - not just String fields, but all fields (ToString)
 *  - regular expression search
 *  - replace
 * - custom export (String.Format fields)
 */

namespace StarlingDBFConverter
{

    public partial class frmMain : Form
    {
        #region setting TextBox tab width
        // http://stackoverflow.com/questions/1298406/how-to-set-the-tab-width-in-a-windows-forms-textbox-control
        private const int EM_SETTABSTOPS = 0x00CB;

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr h, int msg, int wParam, int[] lParam);

        private static void setTabWidth(TextBox textBox, int tabWidth)
        {
            SendMessage(textBox.Handle, EM_SETTABSTOPS, 1, new int[] { tabWidth * 4 });
        }
        #endregion

        /// <summary>
        /// Object to contain a Starling database, once it's loaded.
        /// Invariant: this object must never be null.
        /// </summary>
        private LoadedDatabase theLoadedDatabase = new LoadedDatabase();

        private UnicodeInfo uniInfo = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public frmMain()
        {
            InitializeComponent();

            // set the tab width for both textboxes
            setTabWidth(txtInf, 4);
            setTabWidth(txtLog, 4);

            // initialize tagnester with the tags stored in the settings
            setTagNesterFromSettings();

            // initialize unicode info object
            String[] chardata = null;
            using (MemoryStream ms = new MemoryStream(Resources.UnicodeCharData_gzip))
            {
                using (GZipStream gz = new GZipStream(ms, CompressionMode.Decompress))
                {
                    using (StreamReader sr = new StreamReader(gz))
                    {
                        String s = sr.ReadToEnd();
                        chardata = Newlines.splitLines(s);
                    }
                }
            }
            uniInfo = new UnicodeInfo(chardata, Newlines.splitLines(Resources.UnicodeBlocks));

            // activate Double Buffering for drawing text in the progressbar
            this.DoubleBuffered = true;

            // attach tags to the toolbar menu items for normalization
            composedCharactersNFCToolStripMenuItem.Tag = NormalizationForm.FormC;
            decomposedCharactersNFDToolStripMenuItem.Tag = NormalizationForm.FormD;

            // set the form title
            setFormTitle();
        }

        #region GUI helpers

        /// <summary>
        /// The application name, to be displayed in the Form title
        /// </summary>
        public const String appName = "StarlingDBF reader 1.0";

        /// <summary>
        /// (In)active form for showing character frequencies.
        /// </summary>
        private frmCharacters characterForm = null;

        /// <summary>
        /// Method to set the Form title (with an optional filename to display).
        /// </summary>
        /// <param name="filename">Optional filename</param>
        private void setFormTitle(String filename = null)
        {
            if (filename == null || filename == "")
                this.Text = appName;
            else
                this.Text = String.Format("{0} – \"{1}\"", appName, filename);
        }

        /// <summary>
        /// Method for setting the percentage and drawing a message in a ProgressBar
        /// </summary>
        /// <param name="pb">The ProgressBar.</param>
        /// <param name="percentage">The percentage.</param>
        /// <param name="message">The message.</param>
        private void showProgress(ProgressBar pb, int percentage, String message)
        {
            if (pb != null)
                using (Graphics gr = pb.CreateGraphics())
                {
                    
                    gr.SmoothingMode = SmoothingMode.AntiAlias;
                    gr.DrawString(message, SystemFonts.DefaultFont, Brushes.Red, new PointF(0, 0));
                    //pb.Invalidate();
                    pb.Value = percentage;
                }
        }

        /// <summary>
        /// Method for a backgroundworker to report progress.
        /// </summary>
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState == null)
                showProgress(toolStripProgressBar1.ProgressBar, e.ProgressPercentage, "");
            else
                showProgress(toolStripProgressBar1.ProgressBar, e.ProgressPercentage, e.UserState.ToString());
        }

        #endregion

        #region Opening/closing Starling database

        /// <summary>
        /// Handler for opening a file
        /// </summary>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openDbfDialog.ShowDialog() == DialogResult.OK)
            {
                // get the file the user has selected and check if it exists
                String fileDbf = openDbfDialog.FileName;
                if (File.Exists(fileDbf))
                {
                    // check if there's a homonymous .var file (normally, there should be)
                    String fileVar = Path.ChangeExtension(fileDbf, "var");
                    if (File.Exists(fileVar) || MessageBox.Show(String.Format("\"{0}\" does not exist. Continue anyway?", fileVar), appName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        this.Enabled = false;
                        toolStripProgressBar1.Enabled = true;
                        // commence loading the file with a BackGroundWorker
                        backgroundDbfLoader.RunWorkerAsync(fileDbf);
                    }
                }
            }
        }

        /// <summary>
        /// Method for loading a Starling database, decoding it and returning it as a DataTable, while reporting progress
        /// (in the range [min..max]) to the backgroundworker.
        /// </summary>
        /// <param name="fname">Starling database filename.</param>
        /// <param name="logStatus">Status message to log and report to the backgroundworker.</param>
        /// <param name="bw">The backgroundworker.</param>
        /// <param name="min">Lower end of the percentile range to which to scale the progress.</param>
        /// <param name="max">Higher end of the percentile range to which to scale the progress.</param>
        /// <returns>A DataTable with contents representing the Starling database.</returns>
        private DataTable getDatabaseContents(String fname, String logStatus, BackgroundWorker bw, int min, int max)
        {
            DataTable dt = null;

            bw.ReportProgress(min, logStatus);
            if (File.Exists(fname))
            {
                Trace.WriteLine(String.Format("{0} \"{1}\".", logStatus, fname));
                Trace.Indent();
                StarlingDbfReader sdb = null;
                // open the database, with an eventhandler for showing progress
                StarlingDbfReader.ReportProgressEventHandler evtHandlerLoading = (double progressPercent, string message) => sdb_onProgressChanged(progressPercent, message, min, (max + min) / 2);
                try
                {
                    sdb = new StarlingDbfReader(fname, evtHandlerLoading);
                    Trace.WriteLine(String.Format("{0} (decoding contents)", logStatus));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    MessageBox.Show(ex.Message, appName);
                    if (sdb != null)
                        sdb.onProgressChanged -= evtHandlerLoading;
                }

                // decode the database contents, and attach an event handler to show progress
                Trace.Indent();
                StarlingDbfReader.ReportProgressEventHandler evtHandlerDecoding = (double progressPercent, string message) => sdb_onProgressChanged(progressPercent, message, (max + min) / 2, max);
                if (sdb != null)
                {
                    sdb.onProgressChanged += evtHandlerDecoding;
                    dt = sdb.getDataTableDecoded();
                    sdb.onProgressChanged -= evtHandlerDecoding;
                }
                sdb = null;
                Trace.Unindent();
                Trace.Unindent();
            }
            bw.ReportProgress(max, String.Empty);

            return dt;
        }

        /// <summary>
        /// Wrapper to make <c>backgroundDbfLoader</c> report a (scaled) progress percentage.
        /// </summary>
        /// <param name="progressPercent">Progress percentage, in the range [0..100].</param>
        /// <param name="message">Status message.</param>
        /// <param name="min">Optional, lower end of the percentile range to which to scale the progress.</param>
        /// <param name="max">Optional, higher end of the percentile range to which to scale the progress.</param>
        void sdb_onProgressChanged(double progressPercent, string message, int min = 0, int max = 100)
        {
            backgroundDbfLoader.ReportProgress(Convert.ToInt32(min + ((max - min) / 100d) * progressPercent), message);
        }

        /// <summary>
        /// Method that loads the Starling database file(s) in the background, while logging and reporting progress.
        /// </summary>
        private void backgroundDbfLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            // initialize logging infrastructure
            MemoryStream ms = new MemoryStream();
            TraceListener l = new DelimitedListTraceListener(ms);
            Trace.Listeners.Add(l);

            // get input filename
            String fileDbf = e.Argument as String;
            Trace.WriteLine(String.Format("Loading \"{0}\".", fileDbf));
            Trace.WriteLine(String.Format("Starling decoder fallback character: {0} (U+{1})", StarlingDecoder.fallbackCharacter, StarlingDecoder.stringToStringOfUnicodeValues(StarlingDecoder.fallbackCharacter)));

            // declare vars that will contain each resulting part
            DataTable dbf = null;
            DataTable prt = null;
            String inf = null;
            String log = null;

            // load .dbf
            backgroundDbfLoader.ReportProgress(0, "Opening file");
            dbf = getDatabaseContents(fileDbf, "Opening main database", backgroundDbfLoader, 0, 40);

            // load .inf (if it exists)
            backgroundDbfLoader.ReportProgress(80, "Loading metadata");
            String fileInf = Path.ChangeExtension(fileDbf, "inf");
            if (File.Exists(fileInf))
            {
                Trace.WriteLine(String.Format("Opening info file \"{0}\".", fileInf));
                Trace.Indent();
                try
                {
                    using (FileStream fsInf = File.OpenRead(fileInf))
                    {
                        byte[] ba = new byte[fsInf.Length];
                        for (long i = 0; i < ba.LongLength; i++)
                            ba[i] = (byte)fsInf.ReadByte();
                        inf = StarlingDecoder.decode(ba);
                    }
                }
                catch (Exception ex)
                {
                    Trace.Indent();
                    Trace.Write(ex.Message);
                    Trace.Unindent();
                    MessageBox.Show(ex.Message, appName);
                }
                Trace.Unindent();
            }

            // load .prt (if it exists)
            backgroundDbfLoader.ReportProgress(85, "Loading print settings");
            String filePrt = Path.ChangeExtension(fileDbf, "prt");
            prt = getDatabaseContents(filePrt, "Opening print settings", backgroundDbfLoader, 85, 95);

            // get log
            Trace.Flush();
            log = Newlines.adjustNewlines(Encoding.UTF8.GetString(ms.ToArray())).Replace(new String(' ', l.IndentSize), "\t");
            Trace.Listeners.Remove(l);
            ms.Close();

            // load it all up
            LoadedDatabase collated = new LoadedDatabase();
            collated.Fill(fileDbf, dbf, prt, inf, log, StarlingDecoder.normalizationForm);

            backgroundDbfLoader.ReportProgress(95, "Preparing overview");

            e.Result = collated;
        }

        /// <summary>
        /// Method to finish up loading Starling file(s), when the actual work has been completed in the background.
        /// </summary>
        private void backgroundDbfLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // get result
            theLoadedDatabase = e.Result as LoadedDatabase;

            // update results in GUI
            theLoadedDatabase.updateGUI(dgvDbf, dgvPrt, txtInf, txtLog);
            // force event so that right row/column index is displayed
            if (0 < dgvDbf.RowCount && 0 < dgvDbf.ColumnCount)
            {
                dgvDbf.ClearSelection();
                dgvDbf.CurrentCell = dgvDbf[0, 0];
                dataGridView_CellEnter(dgvDbf, new DataGridViewCellEventArgs(0, 0));
            }

            // flip controls in GUI
            setFormTitle(Path.GetFileName(theLoadedDatabase.Filename));
            toolStripProgressBar1.Enabled = false;
            fileToolStripMenuItem_DropDownOpening(this, null);
            this.Enabled = true;

            GC.Collect();
        }

        /// <summary>
        /// Closing the loaded Starling database.
        /// </summary>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // close a potentially present character frequency form
            if (characterForm != null)
            {
                characterForm.Close();
                characterForm = null;
            }

            // show empty database in GUI
            theLoadedDatabase = new LoadedDatabase();
            theLoadedDatabase.updateGUI(dgvDbf, dgvPrt, txtInf, txtLog);

            // flip controls in GUI
            setFormTitle(null);
            showProgress(toolStripProgressBar1.ProgressBar, 0, "");
            fileToolStripMenuItem_DropDownOpening(this, null);
            clearMarkingToolStripMenuItem.Enabled = false;
            goTonextMarkedCellToolStripMenuItem.Enabled = false;

            GC.Collect();
        }

        #endregion
                
        #region GUI row/col numbers

        /// <summary>
        /// Method to update the row/column index in the toolstrip when a cell is selected in a DataGridView.
        /// </summary>
        private void dataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (sender.GetType() == typeof(DataGridView))
            {
                DataGridView d = (DataGridView)sender;
                toolStripLabelColumn.Text = String.Format("{0}/{1}", e.ColumnIndex + 1, d.ColumnCount);
                toolStripLabelRow.Text = String.Format("{0}/{1}", e.RowIndex + 1, d.RowCount);
            }
        }

        /// <summary>
        /// Method to reset the row/column index in the toolstrip when the data is removed from a DataGridView.
        /// </summary>
        private void dgv_DataSourceChanged(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(DataGridView))
                if (((DataGridView)sender).DataSource == null)
                {
                    toolStripLabelColumn.Text = @"–";
                    toolStripLabelRow.Text = @"–";
                }
        }

        #endregion

        #region Miscellaneous GUI handlers

        /// <summary>
        /// Handler to close the application.
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Method to reset the progress bar when it is being enabled or disabled.
        /// </summary>
        private void toolStripProgressBar1_EnabledChanged(object sender, EventArgs e)
        {
            showProgress(((ToolStripProgressBar)sender).ProgressBar, 0, "");
        }

        /// <summary>
        /// Handler for opening the File menu, where dropdown items are en/disabled.
        /// </summary>
        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            openToolStripMenuItem.Enabled = !theLoadedDatabase.Loaded;
            exportToolStripMenuItem.Enabled = theLoadedDatabase.Loaded;
            closeToolStripMenuItem.Enabled = theLoadedDatabase.Loaded;
        }

        /// <summary>
        /// Handler for opening the Edit menu, where dropdown items are en/disabled.
        /// </summary>
        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            findToolStripMenuItem.Enabled = theLoadedDatabase.Loaded;
            findNextToolStripMenuItem.Enabled = (searchString != null) && (searchString != String.Empty);
        }

        /// <summary>
        /// Handler for opening the Tools menu, where dropdown items are en/disabled.
        /// </summary>
        private void toolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            tagsToolStripMenuItem.Enabled = theLoadedDatabase.Loaded;
            unicodeNormalizationToolStripMenuItem.Enabled = theLoadedDatabase.Loaded;
            oCRErrorLocationToolStripMenuItem.Enabled = theLoadedDatabase.Loaded;
        }

        /// <summary>
        /// Procedure to fetch XeLaTeX code for the Starling decoder character tables and copy it to the clipboard.
        /// </summary>
        private void generateCharacterTablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String s = StarlingDecoder.generateXeLaTeX();
            Clipboard.SetText(s);
            MessageBox.Show("XeLaTeX code copied to clipboard.", appName);
        }

        #endregion

        #region Copying to clipboard

        /// <summary>
        /// Method to copy the currently selected cells in a DataGridView to the clipboard.
        /// </summary>
        /// <param name="dgv">The DataGridView.</param>
        public static void copySelectedCellsToClipboard(DataGridView dgv)
        {
            try
            {
                Clipboard.SetDataObject(dgv.GetClipboardContent());
            }
            catch (Exception e)
            {
                MessageBox.Show("Copying data failed." + Environment.NewLine + e.Message, appName);
            }
        }

        /// <summary>
        /// Handler for copying cells from <c>dgvDbf</c>.
        /// </summary>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copySelectedCellsToClipboard(dgvDbf);
        }

        /// <summary>
        /// Handler for copying cells from <c>dgvPrt</c>.
        /// </summary>
        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            copySelectedCellsToClipboard(dgvPrt);
        }

        /// <summary>
        /// Handler for opening the context menu for <c>dgvDbf</c>.
        /// </summary>
        private void contextMenuDgvDbf_Opening(object sender, CancelEventArgs e)
        {
            // only enable the items in the menu if at least one cell is selected
            contextMenuDgvDbf.Enabled = dgvDbf.GetCellCount(DataGridViewElementStates.Selected) > 0;
        }

        /// <summary>
        /// Handler for opening the context menu for <c>dgvPrt</c>.
        /// </summary>
        private void contextMenuDgvPrt_Opening(object sender, CancelEventArgs e)
        {
            // only enable the items in the menu if at least one cell is selected
            contextMenuDgvDbf.Enabled = dgvPrt.GetCellCount(DataGridViewElementStates.Selected) > 0;
        }

        #endregion

        #region Find (search functionality)

        /// <summary>
        /// Last searched for text.
        /// </summary>
        private String searchString = null;

        /// <summary>
        /// Setting for case sensitivity.
        /// </summary>
        private StringComparison searchSetting = StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// Handler for the Find menu item.
        /// </summary>
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // prepare parameters for Find dialog
            String searchText;
            if (searchString == null)
                searchText = String.Empty;
            else
                searchText = searchString;
            bool searchCaseSensitive;
            searchCaseSensitive = searchSetting == StringComparison.Ordinal;

            // show Find dialog
            if (PromptDialogs.InputCheckBox("Find", "Text to search for", "case sensitive", ref searchText, ref searchCaseSensitive) == DialogResult.OK)
            {
                // set search case sensitivity
                if (searchCaseSensitive)
                    searchSetting = StringComparison.Ordinal;
                else
                    searchSetting = StringComparison.OrdinalIgnoreCase;
                // make sure search string is in the same normal form as the data
                searchString = searchText.Normalize(theLoadedDatabase.NormalizationForm);
                // call actual searching functionality (and enable 'search next')
                findNextToolStripMenuItem.Enabled = true;
                findNextToolStripMenuItem_Click(sender, e);
            }
        }

        /// <summary>
        /// Handler for the Find Next menu item, which is also called by the regular Find menu item.
        /// </summary>
        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (theLoadedDatabase.Loaded)
            {
                // make sure we are searching the right source
                DataTable dt = theLoadedDatabase.dbfDataTable;
                if (dt != null && dt == dgvDbf.DataSource)
                {
                    DataGridViewCell c = dgvDbf.CurrentCell;
                    // search next cell that matches query
                    Tuple<int, int> foundCell = theLoadedDatabase.searchContents(searchString, searchSetting, LoadedDatabase.SearchPart.beyondCurrentCell, c.RowIndex, c.ColumnIndex);
                    if (foundCell != null)
                    {
                        // found one, so select it
                        dgvDbf.ClearSelection();
                        dgvDbf.CurrentCell = dgvDbf.Rows[foundCell.Item1].Cells[foundCell.Item2];
                    }
                    else
                    {
                        // none found, if the starting position was the first cell, this means this never occurs
                        if (c.RowIndex == 0 & c.ColumnIndex == 0)
                            MessageBox.Show("Text not found.", appName);
                        // otherwise, consider searching from the beginning
                        else if (MessageBox.Show("Text not found. Search from beginning?", appName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            // search from the beginning up to the selected cell
                            foundCell = theLoadedDatabase.searchContents(searchString, searchSetting, LoadedDatabase.SearchPart.beforeCurrentCell, c.RowIndex, c.ColumnIndex);
                            if (foundCell != null)
                            {
                                dgvDbf.ClearSelection();
                                dgvDbf.CurrentCell = dgvDbf.Rows[foundCell.Item1].Cells[foundCell.Item2];
                            }
                            else
                                // not found
                                MessageBox.Show("Text not found.", appName);
                        }
                    }
                }
            }
        }

        #endregion

        #region Normalization

        /// <summary>
        /// Handler for the opening of the drop down menu, that shows the possible Unicode normalization forms and dis/enables them based on the loaded databases current normalization form.
        /// </summary>
        private void unicodeNormalizationToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
                foreach (ToolStripMenuItem t in tsmi.DropDownItems)
                {
                    bool toolstripitem_represents_current_normalform;
                    if (theLoadedDatabase.Loaded)
                        toolstripitem_represents_current_normalform = ((NormalizationForm)t.Tag == theLoadedDatabase.NormalizationForm);
                    else
                        toolstripitem_represents_current_normalform = false;
                    t.Enabled = !toolstripitem_represents_current_normalform;
                    t.Checked = toolstripitem_represents_current_normalform;
                }
            }
        }

        /// <summary>
        /// Handler for clicking a ToolStripItem for changing the loaded database's Unicode normalization form.
        /// </summary>
        private void unicodeNormalizationFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                // get the desired normalization form from the ToolStripItem's tag
                ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
                NormalizationForm nf = (NormalizationForm)tsmi.Tag;

                // verify that a database is loaded, the desired normalization form differs from the current one, and that the user wants to execute this costly operation
                if (theLoadedDatabase.Loaded)
                    if (theLoadedDatabase.NormalizationForm != nf)
                        if (MessageBox.Show("Warning: normalizing large datasets can take a long time." + Environment.NewLine + "Are you sure you want to continue?", appName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            // flip GUI controls
                            this.Enabled = false;
                            dgvDbf.Visible = false;
                            dgvPrt.Visible = false;
                            toolStripProgressBar1.Enabled = true;
                            // commence normalizing process through a BackGroundWorker 
                            backgroundNormalizer.RunWorkerAsync(new Tuple<LoadedDatabase, NormalizationForm>(theLoadedDatabase, nf));
                        }
            }
        }
        
        /// <summary>
        /// Method that normalizes the Unicode strings in the loaded database, while logging and reporting progress.
        /// </summary>
        private void backgroundNormalizer_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is Tuple<LoadedDatabase, NormalizationForm>)
            {
                Tuple<LoadedDatabase, NormalizationForm> arg = e.Argument as Tuple<LoadedDatabase, NormalizationForm>;
                if (arg.Item1 != null)
                {
                    // attach an event handler to the LoadedDatabase to handle its progress reports
                    LoadedDatabase.ReportProgressEventHandler evt = (double progressPercent, String message) => backgroundNormalizer.ReportProgress(Convert.ToInt32(progressPercent), message);
                    arg.Item1.onProgressChanged += evt;
                    // execute the normalization process
                    arg.Item1.changeNormalization(arg.Item2);
                    // detach the event handler
                    arg.Item1.onProgressChanged -= evt;
                }
            }
        }

        /// <summary>
        /// Wrap up the normalization process when the database's contents have been updated
        /// </summary>
        private void backgroundNormalizer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // flip and update GUI controls
            dgvDbf.Visible = true;
            dgvPrt.Visible = true;
            this.Enabled = true;
            theLoadedDatabase.updateGUI(dgvDbf, dgvPrt, txtInf, txtLog);
            toolStripProgressBar1.Enabled = false;
        }

        #endregion

        #region Fixing tags

        /// <summary>
        /// Object for tag nesting correction functionality.
        /// </summary>
        private TagNester theTagNester = new TagNester();

        /// <summary>
        /// Set the properties of the tag nesting object to match those of the application settings.
        /// </summary>
        private void setTagNesterFromSettings()
        {
            // freshen up the tag nester
            if (theTagNester == null)
                theTagNester = new TagNester();
            theTagNester.clearTags();

            // add tags based on the application settings
            StringCollection sc = Settings.Default.tagCollection;
            for (int i = 0; (i + 3 < sc.Count) && (i / 4 < TagNester.getMaximumTagCount()); i = i + 4)
                theTagNester.addTag(sc[i], sc[i + 1], sc[i + 2], sc[i + 3]);
        }

        /// <summary>
        /// Handler for the menu item for editing tag settings.
        /// </summary>
        private void editTagSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // make a new form with the tag settings
            frmTags newFrmTags = new frmTags();
            // show it and update the tag settings if it was closed with the OK button
            if (newFrmTags.ShowDialog() == DialogResult.OK)
                setTagNesterFromSettings();
        }

        /// <summary>
        /// Handler for the menu item for performing tag correction.
        /// </summary>
        private void performTagCorrectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // make sure a database is loaded and the user wants to proceed executing this (costly) operation
            if (theLoadedDatabase.Loaded)
                if (MessageBox.Show("Warning: correcting tags can take a long time." + Environment.NewLine + "Are you sure you want to continue?", appName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    // flip GUI controls
                    this.Enabled = false;
                    dgvDbf.Visible = false;
                    dgvPrt.Visible = false;
                    toolStripProgressBar1.Enabled = true;
                    // commence the tag fixing process through a BackGroundWorker, to whom we pass on the loaded database, the tag nesting object, and a bool indication if differences should be logged
                    backgroundTagFixer.RunWorkerAsync(new Tuple<LoadedDatabase, TagNester, bool>(theLoadedDatabase, theTagNester, Settings.Default.logTagFixing));
                }
        }

        /// <summary>
        /// Method that renests tags in the loaded database, while logging and reporting progress.
        /// </summary>
        private void backgroundTagFixer_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is Tuple<LoadedDatabase, TagNester, bool>)
            {
                // retrieve the components involved in fixing the tags
                Tuple<LoadedDatabase, TagNester, bool> arg = e.Argument as Tuple<LoadedDatabase, TagNester, bool>;
                // start an empty log
                String log = null;
                if (arg.Item1 != null && arg.Item2 != null)
                {
                    // attach an event handler to the loaded database to process progress reports
                    LoadedDatabase.ReportProgressEventHandler evt = (double progressPercent, String message) => backgroundTagFixer.ReportProgress(Convert.ToInt32(progressPercent), message);
                    arg.Item1.onProgressChanged += evt;
                    // execute the tag nesting and receive any logged information
                    log = arg.Item1.correctTags(arg.Item2, arg.Item3);
                    // detach the event handler
                    arg.Item1.onProgressChanged -= evt;
                }
                // pass on the logged information
                e.Result = log;
            }
        }

        /// <summary>
        /// Method for finalizing the tag nesting process after the loaded database has been worked.
        /// </summary>
        private void backgroundTagFixer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // flip GUI controls
            dgvDbf.Visible = true;
            dgvPrt.Visible = true;
            this.Enabled = true;
            // update the kept log
            theLoadedDatabase.appendToLog(e.Result as String);
            // update GUI elements
            theLoadedDatabase.updateGUI(dgvDbf, dgvPrt, txtInf, txtLog);
            // disable progress bar
            toolStripProgressBar1.Enabled = false;
        }

        #endregion

        #region Export functionality

        /// <summary>
        /// Generic export function, which lets the user select a folder and then calls the passed along export function on the loaded database.
        /// </summary>
        /// <param name="exportFunction">Export function to be called on the loaded database and the export path.</param>
        private void exportToFolder(Action<LoadedDatabase, String> exportFunction)
        {
            if (theLoadedDatabase.Loaded)
            {
                // select folder
                String originPath = Path.GetDirectoryName(openDbfDialog.FileName);
                folderBrowserDialog.SelectedPath = originPath;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    if (folderBrowserDialog.SelectedPath == originPath)
                        MessageBox.Show("Please pick a different folder than the one containing the original files.", appName);
                    else
                    {
                        // call the export function on the loaded database and the selected path
                        exportFunction(theLoadedDatabase, folderBrowserDialog.SelectedPath);
                        MessageBox.Show("Done exporting.");
                    }
                }
            }
            else
                MessageBox.Show("No file is loaded.", appName);
        }

        /// <summary>
        /// Handler for the menu item for exporting the loaded database to a CSV.
        /// </summary>
        private void cSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // call the generic exporting function with a dummy function that calls the method on the loaded database to export it as CSV to a path
            exportToFolder((LoadedDatabase db, String folder) => db.exportToFolderCSV(folder));
        }

        /// <summary>
        /// Handler for the menu item for exporting the loaded database to a MySql database creation query.
        /// </summary>
        private void mySqlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // call the generic exporting function with a dummy function that calls the method on the loaded database to export it as MySql code to a path
            exportToFolder((LoadedDatabase db, String folder) => db.exportToFolderMySql(folder));
        }

        #endregion

        #region Marking isolated cyrillic characters

        /// <summary>
        /// Handler for the menu item to mark isolated Cyrillic characters in the loaded database's DataGridView.
        /// </summary>
        private void markIsolatedCyrillicCharactersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (theLoadedDatabase.Loaded)
            {
                // flip GUI controls
                this.Enabled = false;
                dgvDbf.Visible = false;
                // commence the marking procedure through a BackGroundWorker
                backgroundCyrillicMarker.RunWorkerAsync(dgvDbf);
            }
        }

        /// <summary>
        /// Method to mark isolated Cyrillic characters in the loaded database's DataGridView.
        /// </summary>
        private void backgroundCyrillicMarker_DoWork(object sender, DoWorkEventArgs e)
        {
            // define Cyrillic character range
            const uint cyrRangeStart = 0x0400, cyrRangeEnd = 0x052f;
            char[] cyrYers = new char[] { 'ъ', 'ь' };

            if (e.Argument != null && e.Argument is DataGridView)
            {
                DataGridView dgv = e.Argument as DataGridView;

                // initialize progress reporting info
                int cellcounter = 0;
                int stringcolumns = 0;
                for (int col = 0; col < dgv.ColumnCount; col++)
                    if (dgv.Columns[col].ValueType == typeof(String))
                        stringcolumns++;
                int totalcells = dgv.RowCount * stringcolumns;

                // walk through the cells to mark those with isolated Cyrillic characters
                for (int col = 0; col < dgv.ColumnCount; col++)
                    if (dgv.Columns[col].ValueType == typeof(String))
                        for (int row = 0; row < dgv.RowCount; row++)
                        {
                            // treat each word separately (split on spaces)
                            foreach (String s in dgv[col, row].Value.ToString().Split(null))
                            {
                                int cyrchars = 0;
                                int yerchars = 0;
                                // count Cyrillic characters in the word (and also yers)
                                foreach (char c in s)
                                    if (cyrRangeStart <= c && c <= cyrRangeEnd)
                                    {
                                        cyrchars++;
                                        if (cyrYers.Contains(c))
                                            yerchars++;
                                    }
                                // mark cell based on Cyrillic character count (these conditions are somewhat arbitrary)
                                cyrchars = cyrchars - yerchars;
                                if ((cyrchars == 1 && 1 < s.Length) || (0 < cyrchars && ((100d * cyrchars) / s.Length < 20)))
                                {
                                    String msg = String.Format("\"{0}\" contains {1} cyrillic chars{2}", s, cyrchars + yerchars, Environment.NewLine);
                                    //Trace.Write(String.Format("[row/col: {0}/{1}]: {2}", row + 1, col + 1, msg));
                                    dgv[col, row].Style.BackColor = Color.Yellow;
                                    dgv[col, row].ToolTipText += msg;
                                }
                            }
                            // report progress
                            backgroundCyrillicMarker.ReportProgress(Convert.ToInt32((100d * ++cellcounter) / totalcells));
                        }
            }
        }

        /// <summary>
        /// Restore GUI controls after the marking of cells in the loaded database's DataGridView with isolated Cyrillic characters is done.
        /// </summary>
        private void backgroundCyrillicMarker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // flip GUI controls
            dgvDbf.Visible = true;
            clearMarkingToolStripMenuItem.Enabled = true;
            goTonextMarkedCellToolStripMenuItem.Enabled = true;
            this.Enabled = true;
        }

        /// <summary>
        /// Handler for the menu item for cleaning the marking of cells in the loaded database's DataGridView with isolated Cyrillic characters.
        /// </summary>
        private void clearMarkingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // clear markings
            for (int row = 0; row < dgvDbf.RowCount; row++)
                for (int col = 0; col < dgvDbf.ColumnCount; col++)
                {
                    dgvDbf[col, row].ToolTipText = "";
                    dgvDbf[col, row].Style.BackColor = dgvDbf.DefaultCellStyle.BackColor;
                }
            // disable menu items
            clearMarkingToolStripMenuItem.Enabled = false;
            goTonextMarkedCellToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Handler for the menu item for going to the next marked cell in the loaded database's DataGridView, where a cell is marked if it has non-empty tooltip text.
        /// </summary>
        private void goTonextMarkedCellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool foundNext = false;
            // walk through the rows, starting with the currently selected row
            for (int row = dgvDbf.CurrentCell.RowIndex; row < dgvDbf.RowCount && !foundNext; row++)
                // walk through the columns, starting with the leftmost (unless we're in the starting row, when we start in the next column over from the selected cell
                for (int col = (row == dgvDbf.CurrentCell.RowIndex ? dgvDbf.CurrentCell.ColumnIndex + 1 : 0); col < dgvDbf.ColumnCount && !foundNext; col++)
                    if (dgvDbf[col, row].ToolTipText != String.Empty)
                    {
                        // marked cell found, focus attention on it
                        dgvDbf.ClearSelection();
                        dgvDbf.CurrentCell = dgvDbf[col, row];
                        // invalidate loop conditions
                        foundNext = true;
                    }
            // no marked cell found, alert user
            if (!foundNext)
                MessageBox.Show("None found.", appName);
        }

        #endregion

        #region Tallying character frequencies

        /// <summary>
        /// Handler for the menu item for tallying character frequencies.
        /// </summary>
        private void tallyCharacterFrequenciesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // close any previous form
            if (characterForm != null)
            {
                characterForm.Close();
                characterForm = null;
            }

            if (theLoadedDatabase.Loaded)
            {
                DataTable dt = theLoadedDatabase.dbfDataTable;
                if (dt != null)
                {
                    // tally characters
                    SortedDictionary<char, uint> charTally = new SortedDictionary<char, uint>();
                    for (int col = 0; col < dt.Columns.Count; col++)
                    {
                        if (dt.Columns[col].DataType == typeof(String))
                            for (int row = 0; row < dt.Rows.Count; row++)
                                tallyChars(dt.Rows[row][col] as String, ref charTally);
                    }

                    // initialize form
                    characterForm = new frmCharacters();
                    characterForm.FormClosed += (object snd, FormClosedEventArgs ex) => characterForm = null;

                    // populate form with results and show it
                    foreach (KeyValuePair<char, uint> kv in charTally)
                        characterForm.appendListView(kv.Key, uniInfo.getCharName((char)kv.Key), uniInfo.getCharBlock((char)kv.Key), kv.Value);
                    characterForm.Show();
                }
            }
        }

        /// <summary>
        /// Helper function to add a tally of characters in a String to a kept dictionary of characters and their frequencies.
        /// </summary>
        /// <param name="s">The String whose characters need to be tallied.</param>
        /// <param name="stick">The dictionary into which to feed the tallied information.</param>
        private void tallyChars(String s, ref SortedDictionary<char, uint> stick)
        {
            foreach (char c in s)
            {
                if (stick.ContainsKey(c))
                    stick[c]++;
                else
                    stick.Add(c, 1);
            }
        }
        
        #endregion

    }

    /// <summary>
    /// Class to handle interaction with a loaded Starling database.
    /// </summary>
    public class LoadedDatabase
    {
        #region Data containers and interfaces

        /// <summary>
        /// Boolean value indicating whether the object is loaded with data, i.e. whether it has been filled.
        /// </summary>
        private bool is_loaded = false;

        /// <summary>
        /// Get the boolean value indicating whether the object has been filled with data.
        /// </summary>
        public bool Loaded
        {
            get { return is_loaded; }
        }

        /// <summary>
        /// The filename (including path) of the currently loaded Starling database (.dbf).
        /// </summary>
        private String filename;

        /// <summary>
        /// Get the filename (including path) of the currently loaded Starling database.
        /// </summary>
        public String Filename
        {
            get { return filename; }
        }

        /// <summary>
        /// The Starling database's main data, in the guise of a DataTable.
        /// </summary>
        private DataTable contents;

        /// <summary>
        /// Gets the Starling database's main data container, as a DataTable.
        /// </summary>
        public DataTable dbfDataTable
        {
            get { return contents; }
        }

        /// <summary>
        /// A Starling database's print settings, in the guise of a DataTable.
        /// </summary>
        private DataTable print_settings;

        /// <summary>
        /// A Starling database's metadata, from an association .inf file, as a String.
        /// </summary>
        private String metadata;

        /// <summary>
        /// Logged information pertaining to the opening/decoding of a Starling database, as a String.
        /// </summary>
        private String log;

        /// <summary>
        /// Method for filling the object with Starling database data.
        /// </summary>
        /// <param name="filename">The full path and filename of the Starling database (.dbf).</param>
        /// <param name="dbf">The Starling database's main data (.dbf).</param>
        /// <param name="prt">The Starling database's print settings (.prt).</param>
        /// <param name="inf">The Starling database's metadata (.inf).</param>
        /// <param name="log">Logged information pertaining to the opening/decoding of the Starling database.</param>
        /// <param name="nf">Unicode normalization form of the textual data.</param>
        public void Fill(String filename, DataTable dbf, DataTable prt, String inf, String log, NormalizationForm nf)
        {
            this.filename = filename;
            contents = dbf;
            print_settings = prt;
            metadata = inf;
            this.log = log;
            normalization = nf;
            is_loaded = true;
        }

        /// <summary>
        /// Method for clearing the loaded data.
        /// </summary>
        public void Clear()
        {
            is_loaded = false;
            contents = null;
            print_settings = null;
            metadata = null;
            log = null;
            filename = null;
            GC.Collect();
        }
        
        /// <summary>
        /// Method for appending a String to the log.
        /// </summary>
        /// <param name="s">The String to be appended.</param>
        public void appendToLog(String s)
        {
            if (s != null && s != String.Empty)
                log = String.Concat(log, Environment.NewLine, s);
        }

        #endregion

        #region GUI

        /// <summary>
        /// Method for displaying the loaded database information in GUI components.
        /// </summary>
        /// <param name="dgvDbf">DataGridView in which to display the Starling database's main data (.dbf).</param>
        /// <param name="dgvPrt">DataGridView in which to display the Starling database's print settings (.prt).</param>
        /// <param name="txtInf">TextBox in which to display the Starling database's metadata (.inf).</param>
        /// <param name="txtLog">TextBox in which to display the logged information.</param>
        public void updateGUI(DataGridView dgvDbf, DataGridView dgvPrt, TextBox txtInf, TextBox txtLog)
        {
            // clear GUI elements first
            dgvDbf.DataSource = null;
            dgvPrt.DataSource = null;
            txtInf.Text = String.Empty;
            txtLog.Text = String.Empty;
            // load in new data, or disable the GUI elements
            if (is_loaded)
            {
                // load main data
                dgvDbf.Enabled = (contents != null);
                dgvDbf.DataSource = contents;
                // load print info
                dgvPrt.Enabled = (print_settings != null);
                dgvPrt.DataSource = print_settings;
                // load metadata
                if (metadata != null)
                {
                    txtInf.Enabled = true;
                    txtInf.Text = Newlines.adjustNewlines(metadata);
                }
                // load log
                if (log != null)
                    txtLog.Text = log;
                // focus on first cell
                if (0 < dgvDbf.ColumnCount && 0 < dgvDbf.RowCount)
                {
                    dgvDbf.CurrentCell = dgvDbf.Rows[0].Cells[0];
                    dgvDbf.Focus();
                }
            }
            else
            {
                // disable stuff
                dgvDbf.Enabled = false;
                dgvPrt.Enabled = false;
                txtInf.Enabled = false;
            }
        }

        #endregion

        #region Progress reporting

        /// <summary>
        /// Delegate for an event handler that gets a progress update.
        /// </summary>
        /// <param name="progressPercent">The progress in percent.</param>
        /// <param name="message">A status message.</param>
        public delegate void ReportProgressEventHandler(double progressPercent, String message);

        /// <summary>
        /// Event that reports progress.
        /// </summary>
        public event ReportProgressEventHandler onProgressChanged;

        /// <summary>
        /// Wrapper for firing the progress reporting event.
        /// </summary>
        /// <param name="percent">The progress in percent.</param>
        /// <param name="status">A status message.</param>
        private void reportProgress(double percent, String status)
        {
            if (onProgressChanged != null)
                onProgressChanged(percent, status);
        }

        #endregion

        #region Search functionality

        /// <summary>
        /// Enumerator specifying whether to look through the part up to the current cell, or from the current cell onwards.
        /// </summary>
        public enum SearchPart
        { 
            /// <summary>
            /// Search the part from the beginning up to the current cell.
            /// </summary>
            beforeCurrentCell, 
            /// <summary>
            /// Search from the current cell to the end.
            /// </summary>
            beyondCurrentCell
        };

        /// <summary>
        /// Helper function that searches through a DataTable between specified coordinates and returns the first cell coordinate where the
        /// contents satisfy the search query (outlined by the <value>searchString</value> and <value>searchSetting</value>.
        /// </summary>
        /// <param name="dt">The DataTable to be searched through.</param>
        /// <param name="searchString">The text to be searched for.</param>
        /// <param name="searchSetting">Case sensitivity settings for the search.</param>
        /// <param name="startRow">Row coordinate of the first cell to inspect.</param>
        /// <param name="startCol">Column coordinate of the first cell to inspect.</param>
        /// <param name="stopRow">Row coordinate of the last cell to inspect.</param>
        /// <param name="stopCol">Column coordinate of the last cell to inspect.</param>
        /// <returns>Cell coordinates of the first encountered cell that satisfies the search query, or null if none exists.</returns>
        private Tuple<int, int> searchThroughDataTable(DataTable dt, String searchString, StringComparison searchSetting, int startRow, int startCol, int stopRow, int stopCol)
        {
            Tuple<int, int> result = null;
            bool found = false;
            // make sure we don't go out of the DataTable's range by checking the upper limits
            stopRow = Math.Min(stopRow, dt.Rows.Count - 1);
            stopCol = Math.Min(stopCol, dt.Columns.Count - 1);
            for (int row = startRow; row <= stopRow && !found; row++)
                // in case we are in the start- or stop-row, the start- or stop-column may need to be adjusted
                for (int col = (row == startRow ? startCol : 0); col <= (row == stopRow ? stopCol : dt.Columns.Count - 1) && !found; col++)
                    if (dt.Columns[col].DataType == typeof(String))
                    {
                        // subject the cell's contents to the search query
                        String cellContent = dt.Rows[row][col] as String;
                        if (0 <= cellContent.IndexOf(searchString, searchSetting))
                        {
                            found = true;
                            result = new Tuple<int, int>(row, col);
                        }
                    }
            return result;
        }

        /// <summary>
        /// Method for searching through the Starling database's main data, from/until a particular cell.
        /// </summary>
        /// <param name="searchString">String for which to search.</param>
        /// <param name="searchSetting">Search case sensitivity settings.</param>
        /// <param name="searchPart">Part of the database to search.</param>
        /// <param name="currentCellRow">Row coordinate of the cell from/until which to search.</param>
        /// <param name="currentCellCol">Column coordinate of the cell from/until which to search.</param>
        /// <returns>Coordinates of the first encountered cell satisfying the search query, or null if none is found.</returns>
        public Tuple<int, int> searchContents(String searchString, StringComparison searchSetting, SearchPart searchPart = SearchPart.beyondCurrentCell, int currentCellRow = 0, int currentCellCol = 0)
        {
            if (contents != null)
                switch (searchPart)
                {
                    case SearchPart.beforeCurrentCell:
                        return searchThroughDataTable(contents, searchString, searchSetting, 0, 0, currentCellRow, currentCellCol - 1);
                    case SearchPart.beyondCurrentCell:
                    default:
                        return searchThroughDataTable(contents, searchString, searchSetting, currentCellRow, currentCellCol + 1, contents.Rows.Count - 1, contents.Columns.Count - 1);
                }
            else
                return null;
        }

        #endregion

        #region Database generic string manipulation

        /// <summary>
        /// Method to perform a String manipulation on all the currently loaded data, while reporting progress and logging alterations.
        /// </summary>
        /// <param name="manipulator">String manipulation function to apply to all the data.</param>
        /// <param name="logDifferences">Indicator whether or not to log instances where the manipulating function actually alters the original text.</param>
        /// <param name="status">General status message to pass on while reporting progress.</param>
        /// <returns>Log of Strings that were altered by the manipulation.</returns>
        private String performStringManipulation(Func<String, String> manipulator, bool logDifferences, String status)
        {
            StringBuilder sb = new StringBuilder();
            reportProgress(0, status + " (.inf)");

            // process the .inf data
            if (metadata != null)
            {
                String r = manipulator(metadata);
                if (logDifferences)
                {
                    sb.AppendLine(status + " (.inf)");
                    if (String.CompareOrdinal(r, metadata) != 0)
                        sb.AppendLine("\tcontents have changed");
                }
                metadata = r;
            }
            // process the .prt data
            sb.Append(performStringManipulationOnDataTable(print_settings, manipulator, logDifferences, status + " (.prt)"));
            // process the .dbf data
            sb.Append(performStringManipulationOnDataTable(contents, manipulator, logDifferences, status + " (.dbf)"));

            reportProgress(100, "");
            return (logDifferences ? sb.ToString() : null);
        }

        /// <summary>
        /// Method to perform a String manipulation on all the (String) cells in a DataTable, while reporting progress and logging alterations.
        /// </summary>
        /// <param name="dt">The DataTable on which to apply the String manipulation.</param>
        /// <param name="manipulator">The String manipulation function.</param>
        /// <param name="logDifferences">Indicator whether or not to log instances where the manipulating function actually alters the original text.</param>
        /// <param name="status">Status message to pass on while reporting progress.</param>
        /// <returns>Log of Strings that were altered by the manipulation.</returns>
        private String performStringManipulationOnDataTable(DataTable dt, Func<String, String> manipulator, bool logDifferences, String status)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(status);

            if (dt != null)
            {
                // initialize progress reporting variables (calculating how many cells contain data of type String)
                int cellcounter = 0;
                int stringcolumns = 0;
                for (int col = 0; col < dt.Columns.Count; col++)
                    if (dt.Columns[col].DataType == typeof(String))
                        stringcolumns++;
                int totalcells = dt.Rows.Count * stringcolumns;

                // walk through all cells that contain data of type String
                for (int col = 0; col < dt.Columns.Count; col++)
                    if (dt.Columns[col].DataType == typeof(String))
                    {
                        for (int row = 0; row < dt.Rows.Count; row++)
                        {
                            // apply the String manipulation function to the cell's content
                            String cellContent_old = dt.Rows[row][col] as String;
                            String cellContent_new = manipulator(cellContent_old);
                            dt.Rows[row][col] = cellContent_new;
                            // if applicable, check if the contents have changed and log them in that case
                            if (logDifferences)
                                if (String.CompareOrdinal(cellContent_new, cellContent_old) != 0)
                                    sb.AppendFormat("\t[{0},{1}]: \"{2}\" => \"{3}\"{4}", row + 1, col + 1, Newlines.escapeNewlines(cellContent_old), Newlines.escapeNewlines(cellContent_new), Environment.NewLine);
                            // report progress
                            reportProgress((100d * ++cellcounter) / totalcells, status);
                        }
                    }
            }
            return sb.ToString();
        }

        #endregion

        #region Normalization

        /// <summary>
        /// The normalization form of the Unicode text in the loaded database.
        /// </summary>
        private NormalizationForm normalization;

        /// <summary>
        /// Gets the Unicode normalization form of the textual data in the loaded database.
        /// </summary>
        public NormalizationForm NormalizationForm
        {
            get { return normalization; }
        }
        
        /// <summary>
        /// Method to change the Unicode normalization form of the textual data in the loaded database, while reporting progress and logging alterations
        /// </summary>
        /// <param name="nf">The desired Unicode normalization form.</param>
        public void changeNormalization(NormalizationForm nf)
        {
            if (nf != normalization)
            {
                // change normalization form of the data
                Func<String, String> normalizationWrapper = (String s) => s.Normalize(nf);
                performStringManipulation(normalizationWrapper, false, "Normalizing");
                // update the variable containing the current normalization form
                normalization = nf;
            }
        }

        #endregion

        #region Tag fixing

        /// <summary>
        /// Method to correct tags in the loaded database, by means of a <c>TagNester</c>, while reporting progress and logging alterations.
        /// </summary>
        /// <param name="tn">The TagNester.</param>
        /// <param name="logDifferences">Bool indicating whether before and after differences should be logged.</param>
        /// <returns>A log containing the alterations induced by the tag correction process that were found, if requested.</returns>
        public String correctTags(TagNester tn, bool logDifferences)
        {
            Func<String, String> nestfixWrapper = (String s) => tn.fixNesting(s);
            return performStringManipulation(nestfixWrapper, logDifferences, "Correcting tags");
        }

        #endregion

        #region Exporting data

        /// <summary>
        /// Wrapper for writing a String to a file, checking against accidental overwriting.
        /// </summary>
        /// <param name="outPath"></param>
        /// <param name="outFilename"></param>
        /// <param name="outString"></param>
        private void writeTextFile(String outPath, String outFilename, String outString)
        {
            if (outString != null)
            {
                // build target file path
                String targetFile = outPath + @"\" + outFilename;
                // check if file exists and if so if it can be overwritten
                bool targetFileExists = File.Exists(targetFile);
                if (!targetFileExists || (targetFileExists && MessageBox.Show(String.Format("File \"{0}\" exists.{1}Overwrite?", outFilename, Environment.NewLine), frmMain.appName, MessageBoxButtons.YesNo) == DialogResult.Yes))
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(targetFile))
                        {
                            sw.Write(outString);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(String.Format("An error occurred while saving file \"{0}\":{1}{2}", outFilename, Environment.NewLine, e.Message), frmMain.appName);
                    }
            }
        }

        /// <summary>
        /// Method to export all database files to delimiter-separated value (DSV) text files (CSV by default) and the metadata files to flat text files.
        /// </summary>
        /// <param name="folderName">Folder in which to put the outputted files.</param>
        /// <param name="separator">Delimiter to be used in the DSV file(s).</param>
        /// <param name="quotingchar">Text qualifier to be used in the DSV file(s).</param>
        /// <param name="linebreakreplacement">Replacement character for newlines (a null value leaves newlines intact).</param>
        public void exportToFolderCSV(String folderName, char separator = ',', char? quotingchar = '"', char? linebreakreplacement = null)
        {
            if (is_loaded)
            {
                String bareFilenameExt = Path.GetFileName(filename);
                String bareFilename = Path.GetFileNameWithoutExtension(bareFilenameExt);
                String dsv;

                // write metadata (.inf)
                if (metadata != null)
                    writeTextFile(folderName, String.Format("{0}_inf.txt", bareFilename), metadata);
                // write log (.log)
                if (log != null)
                    writeTextFile(folderName, String.Format("{0}_log.txt", bareFilename), log);
                // write database's main contents (.dbf)
                if (contents != null)
                {
                    // get DSV representation of database's main contents
                    dsv = DataTableConverter.dataTableToDSV(contents, separator, quotingchar, linebreakreplacement);
                    // output to file
                    writeTextFile(folderName, String.Format("{0}_dbf.csv", bareFilename), dsv);
                }
                // write print settings (.prt)
                if (print_settings != null)
                {
                    // get DSV representation of print settings
                    dsv = DataTableConverter.dataTableToDSV(print_settings, separator, quotingchar, linebreakreplacement);
                    // output to file
                    writeTextFile(folderName, String.Format("{0}_prt.csv", bareFilename), dsv);
                }
            }
        }

        /// <summary>
        /// Method to export all database files to MySql database creation queries and the metadata files to flat text files.
        /// </summary>
        /// <param name="folderName">Folder in which to put the outputted files.</param>
        public void exportToFolderMySql(String folderName)
        {
            if (is_loaded)
            {
                String bareFilenameExt = Path.GetFileName(filename);
                String bareFilename = Path.GetFileNameWithoutExtension(bareFilenameExt);
                String mysql ="";

                // write metadata (.inf)
                if (metadata != null)
                    writeTextFile(folderName, String.Format("{0}_inf.txt", bareFilename), metadata);
                // write log (.log)
                if (log != null)
                    writeTextFile(folderName, String.Format("{0}_log.txt", bareFilename), log);
                // write database's main contents (.dbf)
                if (contents != null)
                {
                    // get MySql representation of database's main contents
                    mysql = DataTableConverter.dataTableToMySql(contents, bareFilename);
                    // output to file
                    writeTextFile(folderName, String.Format("{0}_dbf.sql", bareFilename), mysql);
                }
                // write print settings (.prt)
                if (print_settings != null)
                {
                    // get MySql representation of print settings
                    mysql = DataTableConverter.dataTableToMySql(print_settings, bareFilename);
                    // output to file
                    writeTextFile(folderName, String.Format("{0}_prt.sql", bareFilename), mysql);
                }
            }
        }

        #endregion

    }

}
