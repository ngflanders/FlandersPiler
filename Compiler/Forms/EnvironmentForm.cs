using System;
using System.Windows.Forms;

namespace Compiler.Forms
{
    public partial class EnvironmentForm : Form
    {
        // Track defaults and file locations.
        private FileManager fm = FileManager.Instance;

        /// <summary>
        /// Default Form Constuctor
        /// </summary>
        public EnvironmentForm()
        {
            InitializeComponent();
            Text = fm.COMPILER + " - Environment Form";
            MASMDirTextBox.Text = fm.MASM_DIR;
            SourceDirTextBox.Text = fm.SOURCE_DIR;
        }

        /// <summary>
        /// Event handler for the MASM selector button
        /// Launches the folder browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MASMDirSelector_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Browse for MASM Directory";
            fbd.SelectedPath = fm.MASM_DIR;
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                fm.MASM_DIR = fbd.SelectedPath;
                MASMDirTextBox.Text = fm.MASM_DIR;
            }

        }

        /// <summary>
        /// Event handler for the Source dir selector button
        /// Launches the folder browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceDirSelector_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Browse for Source Directory";
            fbd.SelectedPath = fm.SOURCE_DIR;
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                fm.SOURCE_DIR = fbd.SelectedPath + @"\";
                SourceDirTextBox.Text = fm.SOURCE_DIR;
            }
        }

        /// <summary>
        /// Event handler for the OK button
        /// Closes the Environment form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseEnvironmentFormButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
