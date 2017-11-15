using System;
using System.Windows.Forms;

namespace Compiler.Forms
{
    public partial class GetCharsForm : Form
    {
        // Track defaults and file locations.
        private FileManager fm = FileManager.Instance;
        

        /// <summary>
        /// Form Constructor
        /// </summary>
        public GetCharsForm()
        {
            InitializeComponent();
            Text = fm.COMPILER + " - GetChars Form";
            IntroStatementLabel.Text = $"The next character of {fm.SOURCE_NAME} is :";
            if (!fm.SOURCE_READER.Open())
                Close();
            UpdateButtons();
        }


        /// <summary>
        /// Handler of the Reset button, which brings Reader 
        /// back to the beginning of the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, EventArgs e)
        {
            fm.SOURCE_READER.Reset();
            DisplayChar(fm.SOURCE_READER.GetCurrChar());
            UpdateButtons();
        }


        /// <summary>
        /// Steps the Reader forward one char
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextCharButton_Click(object sender, EventArgs e)
        {
            DisplayChar(fm.SOURCE_READER.GetNextOneChar());
            UpdateButtons();
        }


        /// <summary>
        /// Steps the Reader back one char
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StepBackButton_Click(object sender, EventArgs e)
        {
            fm.SOURCE_READER.PushBackOneChar();
            DisplayChar(fm.SOURCE_READER.GetCurrChar());
            UpdateButtons();
        }


        /// <summary>
        /// Close the Reader and the Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoneButton_Click(object sender, EventArgs e)
        {
            fm.SOURCE_READER.Close();
            Close();
        }


        /// <summary>
        /// Handles the display of the current char
        /// </summary>
        /// <param name="message"></param>
        private void DisplayChar(char message)
        {
            string result;
            switch ((int)message)
            {
                case 10:
                    result = "RETURN";
                    break;
                case 32:
                    result = "SPACE";
                    break;
                case 255:
                    result = "EOF";
                    break;
                default:
                    result = Char.ToString(message);
                    break;
            }
            LineCharacterLabel.Text = string.Format("Line {0} - {1}",
                fm.SOURCE_READER.LINE_NUMBER, result);
        }


        /// <summary>
        /// Updates the status of the Form's buttons
        /// </summary>
        private void UpdateButtons()
        {
            StepBackButton.Enabled = fm.SOURCE_READER.CHAR_POS > 0;
            NextCharButton.Enabled = !fm.SOURCE_READER.END_OF_FILE;
        }

    }
}
