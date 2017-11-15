using System;
using System.Collections; // ArrayList

namespace Compiler
{
    /// <summary>
    /// Emitter is a singleton class that creates assembler files. It works with the
    /// FileManager to keep track of what assembler code to write to what file.
    /// A number of assembler files are used during compilations:
    ///    string.inc
    ///    proclist.inc
    ///    main.asm
    ///    etc.
    /// </summary>
    class Emitter
    {
        // We will often need to work with the FileManager.
        private FileManager fm = FileManager.Instance;

        // We maintain an ArrayList of strings that correspond to the main procedure (procedure 0) 
        //    and other procedures.
        static ArrayList procedureStrings;

        // This is the index to the ArrayList of strings. When 0 it refers to the main procedure.
        //    When > 0 it points to the current procedure being built (statements) by the emitter.
        static int currentProcedure;

        // how much memory is needed for the main procedure?
        static int mainMemUse;

        static string stringConstants; // stores all strings used in the program
        static int nextStringNum; // remembers the next available string number
        static int nextCondNum;
        static int nextLoopNum;
        static Stack curCondStack;
        static Stack loopStack;

        // The single object instance for this class.
        private static Emitter c_eInstance;

        // To prevent access by more than one thread. This is the specific lock 
        //    belonging to the Emitter Class object.
        private static Object c_eLock = typeof(Emitter);

        // Instead of a constructor, we offer a static method to return the only instance.
        private Emitter()
        { } // private constructor so no one else can create one.

        /// <summary>
        /// Management for static instance of this class
        /// </summary>
        public static Emitter Instance
        {
            get
            {
                lock (c_eLock)
                {
                    // if this is the first request, initialize the one instance
                    if (c_eInstance == null)
                    {
                        c_eInstance = new Emitter();
                        c_eInstance.Reset();
                    }

                    // in either case, return a reference to the only instance
                    return c_eInstance;
                }
            }
        } // Emitter Instance

        /// <summary>
        /// PRE:  The single emitter exists. This must be called BEFORE the main procedure
        ///    is entered. It sets the current procedure index to -1.
        /// POST: We are ready to start parsing and related assembly.
        /// </summary>
        public void Reset()
        {
            // create the array of strings and add the first (main) string
            procedureStrings = new ArrayList();
            currentProcedure = -1; // thus the main procedure will have index 0

            // initialize the string constant storage
            stringConstants = ";===== string constants for the program: ======\r\n";
            // next string constant number ("Str0  DB ...") /\/\ condition number condtrue_0	
            nextStringNum = nextCondNum = nextLoopNum = 0;
            curCondStack = new Stack();
            loopStack = new Stack();

        } // Reset

        // #########################################################################################
        // ASSEMBLER METHODS   ASSEMBLER METHODS   ASSEMBLER METHODS   ASSEMBLER METHODS     
        // #########################################################################################

        /// <summary>
        /// MASM code for the handling of the conditionals in an IF statement
        /// </summary>
        /// <param name="op"></param>
        public void PrepIf(string op)
        {
            ToProc(new ArrayList { "", "", "", "", "", "", "; begin condition" });
            ToProc(new ArrayList { "pop", "ECX", "", "", "; RHS" });
            ToProc(new ArrayList { "pop", "EAX", "", "", "; LHS" });
            ToProc(new ArrayList { "mov", "EBX,", "1", "", "; assume TRUE" });
            ToProc(new ArrayList { "cmp", "EAX,", "ECX", "; EAX - ECX" });
            switch (op)
            {
                case "<":
                    ToProc(new ArrayList { "jl", "condtrue_" + nextCondNum });
                    break;
                case ">":
                    ToProc(new ArrayList { "jg", "condtrue_" + nextCondNum });
                    break;
                case "<=":
                    ToProc(new ArrayList { "jle", "condtrue_" + nextCondNum });
                    break;
                case ">=":
                    ToProc(new ArrayList { "jge", "condtrue_" + nextCondNum });
                    break;
                case "=":
                    ToProc(new ArrayList { "je", "condtrue_" + nextCondNum });
                    break;
                case "<>":
                    ToProc(new ArrayList { "jne", "condtrue_" + nextCondNum });
                    break;
            }
            ToProc(new ArrayList { "mov", "EBX,", "0", " ", "; oops it was FALSE" });
            ToProc(new ArrayList { "condtrue_" + nextCondNum++ + ":" });

        }   // PrepIf

        public void StoreCondRes(bool not = false)
        {
            if (not == true) // TODO improve NOT
            {
                ToProc(new ArrayList { "sub", "EBX,", "1" });
                ToProc(new ArrayList { "neg", "EBX" });
            }
            ToProc(new ArrayList { "push", "EBX", "", "", "; store result of comparison" });
            ToProc();
        }

        public void TestAnd()
        {
            ToProc(new ArrayList { "mov", "EBX,", "0", "", "; assume AND is false" });
            ToProc(new ArrayList { "pop", "EAX", "", "", "; get result of righthand conditions" });
            ToProc(new ArrayList { "cmp", "EAX,", "0" });
            ToProc(new ArrayList { "jz", "andFalse_" + nextCondNum });
            ToProc(new ArrayList { "pop", "EAX", "", "", "; get result of lefthand conditions" });
            ToProc(new ArrayList { "cmp", "EAX,", "0" });
            ToProc(new ArrayList { "jz", "andFalse_" + nextCondNum });
            ToProc(new ArrayList { "mov", "EBX,", "1", "", "; if we havent jumped to the andFalse yet, it must be true" });
            ToProc(new ArrayList { "andFalse_" + nextCondNum + ":" });
            ToProc(new ArrayList { "push", "EBX", "", "", "; store result of compound conditional AND" });
            ToProc();
        }

        public void TestOr()
        {
            ToProc(new ArrayList { "mov", "EBX,", "1", "", "; assume OR is true" });
            ToProc(new ArrayList { "pop", "EAX", "", "", "; get result of righthand conditions" });
            ToProc(new ArrayList { "cmp", "EAX,", "1" });
            ToProc(new ArrayList { "jz", "orTrue_" + nextCondNum });
            ToProc(new ArrayList { "pop", "EAX", "", "", "; get result of lefthand conditions" });
            ToProc(new ArrayList { "cmp", "EAX,", "1" });
            ToProc(new ArrayList { "jz", "orTrue_" + nextCondNum });
            ToProc(new ArrayList { "mov", "EBX,", "0", "", "; if we havent jumped to the orTrue yet, it must be false" });
            ToProc(new ArrayList { "orTrue_" + nextCondNum + ":" });
            ToProc(new ArrayList { "push", "EBX", "", "", "; store result of compound conditional AND" });
            ToProc();
        }

        /// <summary>
        /// 
        /// </summary>
        public void IfConditional()
        {
            ToProc(new ArrayList { "pop", "EBX" });
            ToProc(new ArrayList { "cmp", "EBX,", "0", "", "; test result of overall conditional" });
            ToProc(new ArrayList { "jz", "else_" + nextCondNum });
            curCondStack.Push(nextCondNum);
        }

        /// <summary>
        /// MASM code for the end of an IF statement's body, jumping past the ELSE body
        /// </summary>
        public void CloseIf()
        {
            ToProc(new ArrayList { "jmp", "end_if_" + curCondStack.Peek() });
            ToProc(new ArrayList { "else_" + curCondStack.Peek() + ":" });
        }

        /// <summary>
        /// MASM code for the label marking the end of the ELSE body
        /// </summary>
        public void CloseElse()
        {
            ToProc(new ArrayList { "end_if_" + curCondStack.Pop() + ":" });
        }

        /// <summary>
        /// MASM code for the label marking the beginning of the loop
        /// where the loop jumps back up to when it hits the bottom
        /// </summary>
        public void PrepLoop()
        {
            ToProc(new ArrayList { "loopstart_" + nextLoopNum + ":" });
            loopStack.Push(nextLoopNum);
            nextLoopNum++;
        }

        /// <summary>
        /// MASM code handling the EXITing of a loop body
        /// </summary>
        public void ExitLoop()
        {
            ToProc(new ArrayList { "jmp", "loopend_" + loopStack.Peek() });
        }

        /// <summary>
        /// MASM code for the bottom END of the loop which jumps us back up to the 
        /// beginning of the loop
        /// </summary>
        public void EndLoop()
        {
            ToProc(new ArrayList { "jmp", "loopstart_" + loopStack.Peek() });
            ToProc(new ArrayList { "loopend_" + loopStack.Pop() + ":" });
        }

        /// <summary>
        /// Adds a string constant to the string include file
        /// </summary>
        public void AddString(string content)
        {
            stringConstants += "str" + nextStringNum++ + "\tdb\t\"" + content + "\",0\r\n";
        }

        /// <summary>
        /// Adds a comment to the source proc file
        /// </summary>
        public void AddComment(string content)
        {
            ToProc(new ArrayList { "\r\n;", content });
        }

        /// <summary>
        /// Function call to the print macro, and uses the previously input string
        /// </summary>
        public void WRSTR()
        {
            ToProc(new ArrayList { "print", "offset", "str" + (nextStringNum - 1) });
        }

        /// <summary>
        /// Function call to the newline function from helper.inc
        /// </summary>
        public void WRLN()
        {
            ToProc(new ArrayList { "call", "nwln" });
        }

        /// <summary>
        /// get user input as an integer
        /// </summary>
        public void RDINT()
        {
            ToProc(new ArrayList { "mov", "EAX, ", "sval(input())" });
            ToProc(new ArrayList { "push", "EAX" });
        }

        /// <summary>
        /// Negate the value in EAX
        /// </summary>
        public void NEG()
        {
            ToProc(new ArrayList { "pop", "EAX" });
            ToProc(new ArrayList { "neg", "EAX" });
            ToProc(new ArrayList { "push", "EAX" });
        }

        /// <summary>
        /// Handles the MASM code for many different simple operations
        /// </summary>
        /// <param name="op"></param>
        public void TwoPopOpPush(char op)
        {
            switch (op)
            {
                case '+':
                    ToProc(new ArrayList { "pop", "EAX" });
                    ToProc(new ArrayList { "pop", "EDX" });
                    ToProc(new ArrayList { "add", "EAX,", "EDX" });
                    ToProc(new ArrayList { "push", "EAX" });
                    break;
                case '-':
                    ToProc(new ArrayList { "pop", "EDX" });
                    ToProc(new ArrayList { "pop", "EAX" });
                    ToProc(new ArrayList { "sub", "EAX,", "EDX" });
                    ToProc(new ArrayList { "push", "EAX" });
                    break;
                case '*':
                    ToProc(new ArrayList { "pop", "EDX" });
                    ToProc(new ArrayList { "pop", "EAX" });
                    ToProc(new ArrayList { "mul", "EDX" });
                    ToProc(new ArrayList { "push", "EAX" });
                    break;
                case '/':
                    ToProc(new ArrayList { "mov", "EDX,", "0" }); // zero EDX
                    ToProc(new ArrayList { "pop", "EBX" }); // divisor: how much to divide by 
                    ToProc(new ArrayList { "pop", "EAX" }); // dividend: thing to be divided
                    ToProc(new ArrayList { "div", "EBX" });
                    ToProc(new ArrayList { "push", "EAX" });// div result ends up in EAX
                    break;
                case '%':
                    ToProc(new ArrayList { "mov", "EDX,", "0" }); // zero EDX
                    ToProc(new ArrayList { "pop", "EBX" }); // divisor: how much to divide by 
                    ToProc(new ArrayList { "pop", "EAX" }); // dividend: thing to be divided
                    ToProc(new ArrayList { "div", "EBX" });
                    ToProc(new ArrayList { "mov", "EAX,", "EDX" }); // zero EDX
                    ToProc(new ArrayList { "push", "EDX" });// div remainder ends up in EDX
                    break;
            }
        }   // TwoPopOpPush

        /// <summary>
        /// function to mov data into register and push onto stack
        ///  can handle numbers, strings, or variables ( [EBP + 8] )
        /// </summary>
        /// <typeparam name="T">Any Datatype</typeparam>
        /// <param name="data">data which you want to push onto the stack</param>
        public void pushOntoStackToProc<T>(T data)
        {
            ToProc(new ArrayList { "mov", "EAX,", data.ToString() });
            ToProc(new ArrayList { "push", "EAX" });
        }

        // pop value off the stack into EAX
        public void popToEAX()
        {
            ToProc(new ArrayList { "pop", "EAX" });
        }

        public void EAXToStack()
        {
            ToProc(new ArrayList { "push", "EAX" });
        }

        public void print()
        {
            ToProc(new ArrayList { "pop", "EAX" });
            ToProc(new ArrayList { "print", "str$(EAX)" });
        }

        // Code emitted to clear the screen
        public void CLS()
        {
            ToProc(new ArrayList { "cls" });
        }

        /// <summary>
        /// strings in the arraylist will be seperated and then
        /// it will have a newline at the end
        /// </summary>
        /// <param name="strings"></param>
        private static void ToProc(ArrayList strings = null)
        {
            if (strings != null)
            {
                foreach (string str in strings)
                {
                    procedureStrings[currentProcedure] += str + "\t";
                }
            }
            procedureStrings[currentProcedure] += "\r\n";
        }

        /// <summary>
        /// move EAX into [EBP+offset]
        /// </summary>
        /// <param name="offset"></param>
        public void StoreVar(int offset)
        {
            popToEAX();
            ToProc(new ArrayList { "mov", "[EBP+" + offset + "],", "EAX" });
        }

        /// <summary>
        /// move EAX into [EBP+offset]
        /// </summary>
        /// <param name="offset"></param>
        public void StoreRefVar(int offset)
        {
            popToEAX();
            ToProc(new ArrayList { "mov", "EBX,", "[EBP+" + offset + "]" });
            ToProc(new ArrayList { "mov", "[EBX],", "EAX" });
        }

        /// <summary>
        /// move [EBP+offset] into EAX
        /// </summary>
        /// <param name="offset"></param>
        public void GetVar(int offset)
        {
            ToProc(new ArrayList { "mov", "EAX,", "[EBP+" + offset + "]" });
        }

        /// <summary>
        /// move [EBP+offset] into EAX
        /// </summary>
        /// <param name="offset"></param>
        public void GetRefVar(int offset)
        {
            ToProc(new ArrayList { "mov", "EAX,", "[EBP+" + offset + "]" });
        }

        public void MoveRefVar()
        {
            ToProc(new ArrayList { "mov", "EBX,", "[EAX]" });
            ToProc(new ArrayList { "push", "EBX" });
        }


        public void GetValFromArray(int offset, int lowBound)
        {
            ToProc(new ArrayList { "pop", "EAX" });
            ToProc(new ArrayList { "sub", "EAX,", lowBound.ToString() });
            ToProc(new ArrayList { "mov", "EDX,", "4" });
            ToProc(new ArrayList { "mul", "EDX" });
            ToProc(new ArrayList { "add", "EAX,", offset.ToString() });

            ToProc(new ArrayList { "mov", "EAX,", "[EBP+EAX]" });
            ToProc(new ArrayList { "push", "EAX" });
        }

        public void PutValInArray(int offset, int lowBound)
        {
            ToProc(new ArrayList { "pop", "EBX", "", "; value to be stored" });
            ToProc(new ArrayList { "pop", "EAX", "", "; array index" });

            ToProc(new ArrayList { "sub", "EAX,", lowBound.ToString() });
            ToProc(new ArrayList { "mov", "EDX,", "4" });
            ToProc(new ArrayList { "mul", "EDX" });
            ToProc(new ArrayList { "add", "EAX,", offset.ToString() });

            ToProc(new ArrayList { "mov", "[EBP+EAX],", "EBX" });
        }

        public void GetValFromRefArray(int offset, int lowBound)
        {
            ToProc(new ArrayList { "pop", "EAX" });
            ToProc(new ArrayList { "sub", "EAX,", lowBound.ToString() });
            ToProc(new ArrayList { "mov", "EDX,", "4" });
            ToProc(new ArrayList { "mul", "EDX", "", "; result stored in EAX" });

            ToProc(new ArrayList { "mov", "EDX,", "[EBP+" + offset.ToString() + "]" });
            ToProc(new ArrayList { "add", "EAX,", "EDX" });

            ToProc(new ArrayList { "mov", "EBX,", "[EAX]" });
            ToProc(new ArrayList { "push", "EBX" });
        }

        public void PutValInRefArray(int offset, int lowBound)
        {
            ToProc(new ArrayList { "pop", "EBX", "", "; value to be stored" });
            ToProc(new ArrayList { "pop", "EAX", "", "; array index" });

            // Get location within array
            ToProc(new ArrayList { "sub", "EAX,", lowBound.ToString() });
            ToProc(new ArrayList { "mov", "EDX,", "4" });
            ToProc(new ArrayList { "mul", "EDX", "", "", "; result stored in EAX" });

            // Get location of array by reference
            ToProc(new ArrayList { "mov", "EDX,", "[EBP+" + offset.ToString() + "]" });

            // Add (location within array) to (location of array) 
            ToProc(new ArrayList { "add", "EAX,", "EDX" });

            ToProc(new ArrayList { "mov", "[EAX],", "EBX" });
        }

        public void ValParam(int oldOffset)
        {
            // ToProc(new ArrayList { "push", "[EBP+" + oldOffset + "]" });
            //  ToProc(new ArrayList { "push", "EAX" });

        }

        public void RefParam(int offset)
        {
            ToProc(new ArrayList { "lea", "EAX,", "[EBP+" + offset.ToString() + "]" });
            ToProc(new ArrayList { "push", "EAX" });
        }

        public void RefElemArrayParam(int offset, int lowBound)
        {
            ToProc(new ArrayList { "pop", "EAX" });


            ToProc(new ArrayList { "sub", "EAX,", lowBound.ToString() });
            ToProc(new ArrayList { "mov", "EDX,", "4" });
            ToProc(new ArrayList { "mul", "EDX", "", "", "; result stored in EAX" });



            ToProc(new ArrayList { "mov", "ECX,", "[EBP+" + offset.ToString()+"]" });
            ToProc(new ArrayList { "add", "EAX,", "ECX" });
            ToProc(new ArrayList { "push", "EAX" });
        }

        public void MakeRoomForLocs(int size)
        {
            ToProc(new ArrayList { "sub", "ESP,", size.ToString(), "", "; make room for local variables" });
        }

        public void CallProc(string name)
        {

            ToProc(new ArrayList { "call", name });
        }

        // #########################################################################################
        // FILE HANDLER METHODS   FILE HANDLER METHODS   FILE HANDLER METHODS   FILE HANDLER METHODS   
        // #########################################################################################

        /// <summary>
        /// PRE:  The parse is complete.
        ///    c_iMainMemoryUse stores the amount of memory needed by the main procedure.
        /// POST: A string is created and the assembler "shell" is written to it.
        /// </summary>
        public string MainAFile()
        {
            // create a time stamp
            DateTime dt = DateTime.Now;

            string stringTemp = "; " + fm.COMPILER + " output for: " + fm.SOURCE_FILE + "\r\n"
                                + "; Created: " + dt.ToString("F") + "\r\n\r\n"
                                + "; ¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤\r\n"
                                + "\tinclude " + fm.MASM_DIR + "include\\masm32rt.inc\r\n"
                                + "; ¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤\r\n\r\n"
                                + ".stack 1000H\r\n\r\n" // plenty of stack space: 4096 bytes
                                + ".data\r\n" // begin DATA section
                                + "\tinclude " + fm.SOURCE_NAME + "_strings.inc\t; all string literals\r\n"
                                + "\r\n"
                                + ".code\r\n" // begin CODE section
                                + "\tinclude " + fm.SOURCE_NAME + "_procs.inc\t; all program procedures\r\n"
                                +
                                "\tinclude helper.inc\t; includes some helper functions for printing and debugging\r\n"
                                + "\r\n"
                                + "start:\r\n\r\n"
                                + "\tcls\r\n"
                                + "\tsub\tESP," + string.Format("{0}", mainMemUse) +
                                "\t; Room for main proc local vars\r\n"
                                + "\tcall " + fm.MAIN_PROC + "\r\n"
                                + "\tinkey\r\n"
                                + "\texit\r\n\r\n"
                                + "end start";

            return stringTemp;
        } // MainAFile

        /// <summary>
        /// PRE:  The name of the procedure is passed. We have already called 
        ///    EnterNewProcScope which tracks the current scope number. Note that this
        ///    array of procedure strings must remain parallel to the array of procedures
        ///    maintained by SymbolTable.
        /// POST: The preamble is emitted. This includes creating the assembly string
        ///    and increasing the procedure index.
        ///    
        /// Note the special version for the main procedure
        /// </summary>
        public void ProcPreamble(string strProcName)
        {
            // create the initial three lines of every procedure
            string stringTemp = "\r\n;============== BEGIN PROCEDURE ============\r\n"
                                + strProcName + " PROC\t; Procedure definition\r\n"
                                + "\tpush\tEBP\t; save EBP since we use it \r\n"
                                + "\tmov\tEBP,ESP\r\n"
                                + ";~~~~~~~~~~~~~ PREAMBLE END ~~~~~~~~~~~~~~~~\r\n";

            // add this string to the array of procedure strings
            procedureStrings.Add(stringTemp);

            // point to this newly-added fasm (file) string
            currentProcedure = SymbolTable.CUR_SCOPE_NUM;
        } // ProcPreamble

        /// <summary>
        /// 
        /// </summary>
        public void MainProcPreamble()
        {
            ProcPreamble(fm.MAIN_PROC);
        } // MainProcPreamble

        /// <summary>
        /// PRE:  The name of the procedure and 
        ///    the amount of memory needed in the stack is passed.
        ///    SymbolTable.ExitProcScope() has been called to re-establish 
        ///    the correct new scope.
        /// POST: The postamble is emitted to the current string.
        ///    The procedure index is returned to the correct value
        ///    (by querying SymbolTable).
        ///    
        /// Note the special version for the main procedure.
        /// </summary>
        public void ProcPostamble(string strProcName, int iMemUse)
        {
            procedureStrings[currentProcedure] += ";~~~~~~~~~~~~~ POSTAMBLE BEGIN ~~~~~~~~~~~~~\r\n"
                                                  + "\tmov\tESP,EBP\r\n"
                                                  + "\tpop\tEBP\r\n"
                                                  + "\tret\t" + string.Format("{0}", iMemUse) + "\r\n"
                                                  + strProcName + " endp\r\n"
                                                  + ";=============== END PROCEDURE =============";

            // restore the correct scope
            currentProcedure = SymbolTable.CUR_SCOPE_NUM;
        } // ProcPostamble

        /// <summary>
        /// PRE:  The amount of memory needed in the stack is passed.
        ///    SymbolTable.ExitProcScope() has been called to re-establish 
        ///    the correct new scope.
        /// POST: The postamble is emitted to the current string.
        ///    The procedure index is returned to the correct value
        ///    (by querying SymbolTable). 
        ///    
        ///    Since this is the end of main (parsing is complete),
        ///    we now know the amount of memory needed by the main procedure.
        ///    We retain this for use while writing the outermost assembler file.
        /// </summary>
        public void MainProcPostamble(int iMemUse)
        {
            ProcPostamble(fm.MAIN_PROC, (mainMemUse = iMemUse));
        } // MainProcPostamble

        /// <summary>
        /// PRE:  The assembler files have all been "written" to strings.
        /// POST: The files are written to the disk.
        /// </summary>
        public void WriteAllFiles()
        {
            // write the outermost "shell" assembler file
            fm.FileAFile(MainAFile(), fm.SOURCE_NAME + ".asm");

            // Create proclist.inc and write it. All the procedures will be written to a single file
            //    named "proclist.inc".
            string strProcInc = "; These are all the procedures for " + fm.SOURCE_FILE + ". Main is first.\r\n";

            foreach (string strProc in procedureStrings)
                strProcInc += strProc + "\r\n";

            // write the string of all procedures to the proper file
            // fm.FileAFile(fm.PROC_LIST = strProcInc, fm.SOURCE_NAME + "_procs.inc");
            fm.FileAFile(strProcInc, fm.SOURCE_NAME + "_procs.inc");

            // write the string of strings to the proper file
            // fm.FileAFile(fm.STRING_CONSTANTS = stringConstants, fm.SOURCE_NAME + "_strings.inc");
            fm.FileAFile(stringConstants, fm.SOURCE_NAME + "_strings.inc");

            // Create the Helper include file
            BuildHelperInclude();

            // Create the command file and invoke it to complete the assembly
            BuildBatFile();

            // Create the run file that will call the created exe file
            BuildRunFile();
        } // WriteAFiles

        /// <summary>
        /// PRE:  The parse is complete.
        /// POST: The command file is created for remaining steps of the assembly process
        ///    (compilation and linking to create an execcutable).
        ///    This command file is then run to complete the compilation.
        /// </summary>
        void BuildBatFile()
        {
            string strMakeFile =
                "@echo off\r\n\r\n"
                + "\tColor B\r\n\r\n" // hex color code idea from Mark Yanagihara-Brooks 2007
                                      //  0=white,        1=very dark blue,   2=dark green,   3=dark turquoise,
                                      //  4= dark red,    5=dark purple,      6=olive,        7=white, 
                                      //  8=grey,         9=dark blue,        A=green,        B=neat turquoise,
                                      //  C=red,          D=purple,           E=yellow.       F=white

                // set the assembly directory
                + "\tcd " + fm.ASM_DIR + "\r\n"
                + "\tif exist " + fm.SOURCE_NAME + ".obj del " + fm.SOURCE_NAME + ".obj\r\n"
                + "\tif exist " + fm.SOURCE_NAME + ".exe del " + fm.SOURCE_NAME + ".exe\r\n\r\n"

                // copy source mod file to assembly directory
                + "\tif not exist " + fm.SOURCE_FILE + " copy " + fm.SOURCE_DIR + fm.SOURCE_FILE + "\r\n\r\n"

                // change to root drive
                + "\tcd C:\\\r\n"
                + "\tC:\r\n\r\n"

                // make an assembly directory on the root drive and copy all needed files
                + "\tif exist C:\\CompilerOutput rmdir CompilerOutput /s /q\r\n"
                + "\tmkdir CompilerOutput\r\n\r\n"
                + "\tcd CompilerOutput\r\n\r\n"
                + "\tcopy " + fm.ASM_DIR + "\r\n\r\n"

                // assemble to create the object file
                + "\t" + fm.MASM_DIR + "bin\\ml /c /coff " + fm.SOURCE_NAME + ".asm\r\n"
                + "\tif errorlevel 1 goto errasm\r\n\r\n"

                // link the files to create the executable
                + "\t" + fm.MASM_DIR + "bin\\polink /SUBSYSTEM:CONSOLE " + fm.SOURCE_NAME + ".obj\r\n"
                + "\tif errorlevel 1 goto errlink\r\n\r\n"

                // copy all files back to ASM_DIR and remove temp root directory
                + "\tcd " + fm.ASM_DIR + "\r\n"
                + "\t" + fm.ASM_DIR[0] + ":\r\n"
                + "\tcopy C:\\CompilerOutput\\" + fm.SOURCE_NAME + ".exe\r\n"
                + "\trmdir C:\\CompilerOutput /s /q\r\n\r\n"

                // show folder contents and then go to the end
                + "\tdir\r\n"
                + "\tgoto TheEnd\r\n\r\n"

                // error on linking
                + ":errlink\r\n"
                + "\techo _\r\n"
                + "\techo Link Error\r\n"
                + "\tgoto TheEnd\r\n\r\n"

                // error on assembling
                + ":errasm\r\n"
                + "\techo _\r\n"
                + "\techo Assembly Error\r\n"
                + "\tgoto TheEnd\r\n\r\n"

                // successful program
                + ":TheEnd\r\n"
                + "\tpause\r\n"
                + "\tcls\r\n\r\n"
                + "@echo on";

            // Write the command string to the proper file.
            fm.FileAFile(strMakeFile, "!" + fm.SOURCE_NAME + ".bat");

            // Invoke the file just created. This uses the static method in our SystemCommand class.
            //    If an error occurs it will throw the appropriate exception. 
            //    TODO try and catch such an exception
            SystemCommand.SysCommand(fm.ASM_DIR + "!" + fm.SOURCE_NAME + ".bat");
        } // BuildBatFile

        /// <summary>
        /// 
        /// </summary>
        void BuildRunFile()
        {
            string strRunFile = "Color A\r\n\r\n"
                                // hex color code idea from Mark Yanagihara-Brooks 2007
                                // 0=white,     1=very dark blue,   2=dark green,   3=dark turquoise,
                                // 4= dark red, 5=dark purple,      6=olive,        7=white, 
                                // 8=grey,      9=dark blue,        A=green,        B=neat turquoise,
                                // C=red,       D=purple,           E=yellow,       F=white

                                // set the assembly directory
                                + "cd " + fm.ASM_DIR + "\r\n\r\n"
                                + fm.SOURCE_NAME + ".exe";

            // Write the run string to the proper file.
            fm.FileAFile(strRunFile, "!RUN.cmd");
        } // BuildRunFile

        /// <summary>
        /// PRE:    None
        /// POST:   The helper file is created.
        /// </summary>
        private void BuildHelperInclude()
        {
            string strHelper =
                "; This must be included in the code segment.\r\n\r\n"
                + "nwln proc\r\n"
                + "\tprint chr$(13,10)\r\n"
                + "\tret\r\n"
                + "nwln endp";

            fm.FileAFile(strHelper, "helper.inc");
        } // BuildHelperInclude
    } // Emitter Class
} // Compiler Namespace