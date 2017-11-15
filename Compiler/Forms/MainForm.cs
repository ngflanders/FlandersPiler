using System;
using System.IO;
using System.Windows.Forms;

namespace Compiler.Forms
{
    public partial class MainForm : Form
    {
        // Track defaults and file locations.
        private FileManager fm = FileManager.Instance;
        
        /// <summary>
        /// class constructor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            Text = fm.COMPILER;
            SourceFileTextBox.Text = fm.SOURCE_FILE;
            UpdateUI();
        } // MainForm

        /// <summary>
        /// Test the error handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestError(object sender, EventArgs e)
        {
            ErrorHandler.Error(ERROR_CODE.NONE, "Main Form", "Test Error");
            

        } // TestError

        /// <summary>
        /// Display information about the compiler program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void About(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        } // About

        /// <summary>
        /// Exit the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit(object sender, EventArgs e)
        {
            // clean up this form and dispose of it
            Close();
            Dispose();

        } // Exit

        /// <summary>
        /// File Selector dialog which opens to SOURCE_DIR and
        /// allows user to select a .mod file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceFileSelector(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = fm.SOURCE_DIR;
            openFileDialog1.Filter = "mod files (*.mod)|*.mod";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fm.SOURCE_FILE = openFileDialog1.SafeFileName;
                SourceFileTextBox.Text = fm.SOURCE_FILE;
            }
        }

        /// <summary>
        /// Opens the Environment Form and creates a FormClosing event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnvironmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnvironmentForm ef = new EnvironmentForm();
            ef.FormClosing += new FormClosingEventHandler(EnvironmentFormClosing);
            ef.ShowDialog();
        }

        /// <summary>
        /// Event handler for the Environment FormClosing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnvironmentFormClosing(object sender, EventArgs e)
        {
            UpdateUI();
            if (!File.Exists(fm.SOURCE_DIR + @"/" + fm.SOURCE_FILE))
            {
                SourceFileSelector(null, null);
            }
        }

        /// <summary>
        /// Updates the status bar and buttons based on FM changes
        /// </summary>
        public void UpdateUI()
        {
            MASMDirLabel.Text = $"MASM Dir: {fm.MASM_DIR}";
            SourceDirLabel.Text = $"Source Dir: {fm.SOURCE_DIR}";
            GetChars.Enabled = File.Exists(fm.SOURCE_DIR + @"\" + fm.SOURCE_FILE);
        }

        /// <summary>
        /// Opens the GetChars Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetChars_Click(object sender, EventArgs e)
        {
            new GetCharsForm().ShowDialog();
        }

        private void listTokensButton_Click(object sender, EventArgs e)
        {
            fm.FACADE.ListTokens();
        }

        private void testSymbolsButton_Click(object sender, EventArgs e)
        {
            fm.FACADE.TestSymTable();
        }

        private void parseAssembeButton_Click(object sender, EventArgs e)
        {
            fm.FACADE.ParseAssemble();
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            fm.FACADE.RunProgram();
        }
    } // MainForm class

} // Compiler.Forms namespace