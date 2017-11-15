using System;
using System.IO;
using System.Text;

namespace Compiler
{
    class SourceReader
    {
        // file names, paths, and defaults
        FileManager fm = FileManager.Instance;
        
        StreamReader    streamReader;       // the Modula-2 source file

        private bool    endLineLastRead;    // did we reach an end of line on the last read?

        private string  fileName,           // path and file name
                        inputLine;          // the current input line

        private int     currentPos,         // current position in the input line
                        lineNumber,         // current line number in the source file
                        lineLength;         // length of the current inputLine

        /// <summary>
        /// Constructor
        /// </summary>
        public SourceReader()
        {
            fm.SOURCE_READER = this;    // register the current source reader object with the file mananger
        } // SourceReader

        public const char EOF_SENTINEL = (char)255; // End of File
        public const char NL_SENTINEL = (char)10;   // New Line
                

        /// <summary>
        /// Opens the streamReader which handles the reading from the file
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            try
            {
                fileName = fm.SOURCE_DIR + fm.SOURCE_FILE;
                FileStream fileStream = new FileStream(
                    fileName, FileMode.Open, FileAccess.Read);
                streamReader = new StreamReader(fileStream, Encoding.UTF8);
                lineNumber = 0;
                endLineLastRead = false;
                GetNextLine();
            }
            catch (Exception e)
            {
                ErrorHandler.Error(ERROR_CODE.FILE_OPEN_ERROR, 
                    "Source Reader", e.Message);
                return false;
            }
            return true;

        } // Open

        /// <summary>
        /// Closes the Stream Reader
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            streamReader.ReadToEnd();
            streamReader.Close();
            return true;
        } // Close

        /// <summary>
        /// Brings the streamReader back to the beginning of the file
        /// </summary>
        /// <returns></returns>
        public bool Reset()
        {
            // if EOF hit, must close and reopen
            if (endLineLastRead)
            {
                Close();
                Open();
            }
            else // seek back to the beginning of the file
            {
                if (streamReader != null)
                {
                    streamReader.DiscardBufferedData();
                    streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                    lineNumber = 0;
                    GetNextLine();
                }
            }
            return true;

        } // Reset

        /// <summary>
        /// Read the next line of the file
        /// </summary>
        /// <returns></returns>
        private bool GetNextLine()
        {
            lineNumber += 1;
            currentPos = 0;

            // while not EOF and the line read is empty, read another line
            while ((inputLine = streamReader.ReadLine()) != null && 
                (lineLength = (inputLine = inputLine.Trim()).Length) == 0)
                lineNumber += 1;
            
            if (inputLine == null)
                endLineLastRead = true;

            if (!fm.SOURCE_FILE_TEXT.ContainsKey(lineNumber))
                fm.SOURCE_FILE_TEXT.Add(lineNumber, inputLine);

            return inputLine != null;
        } // GetNextLine

        /// <summary>
        /// Read the next char of the line
        /// </summary>
        /// <returns></returns>
        public char GetNextOneChar()
        {
            char c;

            if (currentPos > lineLength)
                GetNextLine();

            if (currentPos == lineLength)
            {
                c = NL_SENTINEL;
                currentPos++;
            }
            else if (endLineLastRead)
                return EOF_SENTINEL;
            else
                c = inputLine[currentPos];

            currentPos++;
            return c;

        } // GetNextOneChar


        /// <summary>
        /// Read the char at the currentPos
        /// </summary>
        /// <returns></returns>
        public char GetCurrChar()
        {
            return inputLine[currentPos];
        } // GetCurrChar

        /// <summary>
        /// Read the char at the currentPos
        /// </summary>
        /// <returns></returns>
        public char PeekNextChar()
        {
            if (currentPos < lineLength)
                return inputLine[currentPos];
            else
                return NL_SENTINEL;
        } // GetCurrChar


        /// <summary>
        /// Step back one char
        /// </summary>
        public void PushBackOneChar()
        {
            if (currentPos > 0)
                currentPos -= 1;
        } // PushBackOneChar



        /// <summary>
        /// the current line number of the source file
        /// </summary>
        public int LINE_NUMBER
        { get { return lineNumber; } } // LINE_NUMBER


        /// <summary>
        /// the current index of the char in the line
        /// </summary>
        public int CHAR_POS
        { get { return currentPos; } } // CHAR_POS


        /// <summary>
        /// boolean of if EOF has been hit
        /// </summary>
        public bool END_OF_FILE
        { get { return endLineLastRead; } } // ENDOFFILE

    } // SourceReader class

} // Compiler namespace