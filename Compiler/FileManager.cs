using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;    // FileStream, Directory, etc.

namespace Compiler
{
    class FileManager
    {
        /// <summary>
        /// Static instance of this class
        /// </summary>
        private static FileManager fmInstance;

        /// <summary>
        /// To prevent access by more than one thread. This is the specific lock 
        /// belonging to the FileManager Class object.
        /// </summary>
        private static Object fmLock = typeof(FileManager);

        /// <summary>
        /// Instead of a constructor, we offer a static method to return the only instance.
        /// - Private constructor so no one else can create one.
        /// </summary>
        private FileManager() { }

        /// <summary>
        /// Management for static instance of this class
        /// </summary>
        public static FileManager Instance
        {
            get
            {
                lock (fmLock)
                {
                    if (fmInstance == null) // if no instnace exists, we need to create one
                    {
                        fmInstance = new FileManager();

                        // initalize FileManager variables
                        fmInstance.Initialize();
                    }
                    return fmInstance; // return the only instance of this calss
                }
            }

        } // Instance

        /// <summary>
        /// called during initial creation of the FileManager instance
        /// </summary>
        private void Initialize()
        {
            // Set the name of the comiler, e.g. "Brewpiler or Tompiler".
            //  Compiler name is used on all forms and error messages.
            COMPILER = "FlandersPiler";

            // set the default path to the masm folder...usually located in the c: directory
            MASM_DIR = @"C:\masm32\";

            // set the default souce directory based on which computer we are working from
            if (Directory.Exists(@"C:\Users\Nicholas\Projects\compilers\FlandersPiler\MODS"))
                SOURCE_DIR = @"C:\Users\Nicholas\Projects\compilers\FlandersPiler\MODS\";
            else if (Directory.Exists(@"D:\StudentCompilers\FlandersPiler"))    // working from John's Computer
                SOURCE_DIR = @"D:\StudentCompilers\FlandersPiler\MODS\";
            else if (Directory.Exists(@"C:\Users\john.broere\Desktop\FlandersPiler"))   // working from lab computer
                SOURCE_DIR = @"C:\Users\john.broere\Desktop\FlandersPiler\MODS\";

            // set the default source file...change this to help speed up debugging
            SOURCE_FILE = "00_NickTest.mod";
            SOURCE_FILE_TEXT = new Dictionary<int,string>();

            // get a copy of a SourceReader - will only be one and will be accessed through FileManager
            SOURCE_READER = new SourceReader();

            // get a copy of a Facade - will only be one and will be accessed through FileManager
            FACADE = new Facade();

            // reset all error variables
            ErrorReset();

        } // Initialize

        /// <summary>
        /// Reset token list
        /// </summary>
        public void ResetTokenList()
        {
            SOURCE_FILE_TEXT = new Dictionary<int, string>();
            TOK_LIST = String.Format("Token List:\n  TK  Ln Type {0,30}Lexeme\n", " ");
        }
        
        /// <summary>
        /// Reset Symbol list
        /// </summary>
        public void ResetSymbolList()
        {
            SYM_LIST = String.Format("{0,30}{1,17}{2,18}{3,13}\r\n{4,0}{5,17}{6,7}{7,16}{8,16}{9,18}\r\n" +
                "=============== ======= =============== =============== =============== ========\r\n",
                "Symbol", "Storage", "Parameter", "Memory", "Name", "Scope", "type", "type", 
                "type", "offset");
        }
        
        /// <summary>
        /// Reset the error variables
        /// </summary>
        public void ErrorReset()
        {
            ERROR_LOG = "";
            ERROR_COUNT = 0;
            CURRENT_ERROR = ERROR_CODE.NONE;
            SHOW_ERROR_WINDOW = true;

        } // ErrorReset

        /**********************************************************************************************************************
            GET AND SET FUNCTIONS
        **********************************************************************************************************************/

        /// <summary>
        /// set or get the compiler name e.g. "BrewPiler | StudentPiler"
        /// </summary>
        public string COMPILER { get; set; } // COMPILER

        /// <summary>
        /// set or get the compiler name e.g. "BrewPiler | StudentPiler"
        /// </summary>
        public string MAIN_PROC
        {
            get { return "__main__"; }
        } // COMPILER

        /// <summary>
        /// set or get the source directory name e.g. "C:\Compilers\TEST_MOD\"
        /// </summary>
        public string SOURCE_DIR { get; set; } // SOURCE_DIR

        /// <summary>
        /// set or get the MASM directory name e.g. "C:\Compilers\MASM32\"
        /// </summary>
        public string MASM_DIR { get; set; } // MASM_DIR

        /// <summary>
        /// set or get the ASM directory name 
        /// </summary>
        public string ASM_DIR
        { get { return SOURCE_DIR + SOURCE_NAME + @"_" + COMPILER + @"\"; } } // ASM_DIR
        
        /// <summary>
        /// get the token file path for the current mod file 
        /// </summary>
        public string TOKEN_FILE_PATH
        { get { return ASM_DIR + SOURCE_NAME + @"_tokens.txt"; } } // TOKEN_FILE_PATH
        
        /// <summary>
        /// get the Symbols file path 
        /// </summary>
        public string SYMBOLS_PATH
        { get { return ASM_DIR + SOURCE_NAME + @"_symbols.txt"; } } // TEST_SYMBOLS_PATH
        
        /// <summary>
        /// set or get the source file name e.g. "01_Test.mod"
        /// </summary>
        public string SOURCE_FILE { get; set; } // SOURCE_FILE

        /// <summary>
        /// set or get the whole source file text contents
        /// </summary>
        public Dictionary<int, string> SOURCE_FILE_TEXT { get; set; } // SOURCE_FILE

        /// <summary>
        /// return the source name without file extension ".mod e.g. 01_Test"
        /// </summary>
        public string SOURCE_NAME
        { get { return SOURCE_FILE.Substring(0, SOURCE_FILE.Length - 4); } } // SOURCE_NAME

        /// <summary>
        /// set and get the error log
        /// </summary>
        public string ERROR_LOG { get; set; } // ERROR_LOG
        
        /// <summary>
        /// set and get the token list string
        /// </summary>
        public string TOK_LIST { get; set; } // TOK_LIST

        /// <summary>
        /// set and get the token list string
        /// </summary>
        public string SYM_LIST { get; set; } // SYM_LIST

        /// <summary>
        /// number of errors encountered
        /// set and get the error count
        /// </summary>
        public int ERROR_COUNT { get; set; } // ERROR_COUNT

        /// <summary>
        /// current error encountered
        /// set and get the current error
        /// </summary>
        public ERROR_CODE CURRENT_ERROR { get; set; } // CURRENT_ERROR

        /// <summary>
        /// allow error window to be displayed
        /// get and set error window status
        /// </summary>
        public bool SHOW_ERROR_WINDOW { get; set; } // SHOW_ERROR_WINDOW

        /// <summary>
        /// reference to the SourceReader and current open source file (as an object)
        /// set or get the current source reader
        /// </summary>
        public SourceReader SOURCE_READER { get; set; } // SOURCE_READER

        /// <summary>
        /// set or get the current source reader
        /// </summary>
        public Facade FACADE { get; set; } // SOURCE_READER


        /**********************************************************************************************************************
            Folder and File FUNCTIONS
        **********************************************************************************************************************/
        
        /// <summary>
        /// Create a clean assembly directory
        /// </summary>
        public void ResetASMDIR()
        { Filer.CreateCleanDir(ASM_DIR); }


        //fm.FileAFile(strMakeFile, "!" + fm.SOURCE_NAME + ".bat");
        public void FileAFile(string content, string fileName)
        {
            Filer.WriteStringToFile(content, ASM_DIR + fileName);
        }

        /// <summary>
        /// Save token list to assembly directory
        /// </summary>
        public void FileTokenList()
        { Filer.WriteStringToFile(TOK_LIST, TOKEN_FILE_PATH); }

        /// <summary>
        /// Save token list to assembly directory
        /// </summary>
        public void FileSymbolTable()
        {
            Filer.WriteStringToFile(SYM_LIST, SYMBOLS_PATH);
        }

        /// <summary>
        /// used by ErrorHandler to file ErrorLog
        /// </summary>
        public void FileErrorLog()
        {
            Filer.WriteStringToFile(ERROR_LOG, SOURCE_DIR + @"error_log.txt");
        } // FileErrorLog

    } // FileManager class

} // Compiler namespace