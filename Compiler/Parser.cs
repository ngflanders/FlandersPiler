using System;
using System.Collections;

namespace Compiler
{
    /// <summary>
    /// Parser is a singleton class that receives a list of tokens
    /// and parses them according to Modula-2 grammar.
    /// </summary>
    internal class Parser
    {
        public struct UserDecType
        {
            public string typeName;
            public string baseType;
            public int startRange;
            public int endRange;
        }

        // singleton instances
        private FileManager fm = FileManager.Instance;
        private Lexer lexer = Lexer.Instance;
        private SymbolTable symTbl = SymbolTable.Instance;
        private Emitter emitter = Emitter.Instance;

        // Store the current token from the tokenizer.
        private static Token curTok;
        private static bool not;

        // The single object instance for this class.
        private static Parser pInstance;

        // To prevent access by more than one thread. This is the specific lock
        //    belonging to the Parser Class object.
        private static Object pLock = typeof(Parser);

        // Instead of a constructor, we offer a static method to return the only
        //    instance.
        private Parser() { } // private constructor so no one else can create one.

        /// <summary>
        /// Management for static instance of this class
        /// </summary>
        public static Parser Instance
        {
            get
            {
                lock (pLock)
                {
                    // if this is the first request, initialize the one instance
                    if (pInstance == null)
                    {
                        pInstance = new Parser();
                        pInstance.Reset();
                    }

                    // in either case, return a reference to the only instance
                    return pInstance;
                }
            }
        } // Parser Instance

        /// <summary>
        /// Reset all the things that need to be reset in the parser and SymTable
        /// </summary>
        public void Reset()
        {
            lexer.Reset();
            symTbl.Reset();
            emitter.Reset();
            curTok = null;
            not = false;
        } // Reset

        /// <summary>
        /// Match is used to validate that the current token is expected.
        /// PRE:  The current token has been loaded.
        /// POST: The current token is verified and the next one loaded. If errors are encountered,
        ///    an exception is thrown.
        /// </summary>
        private void Match(Token.TOKENTYPE tokType)
        {
            // Have we loaded a token from the tokenizer?
            if (curTok == null)
                throw new Exception("Parser - Match: curTok is null.");

            // Is the current token the one we expected?
            if (curTok.tokType == tokType)
            {
                // Is this the normal end of the source code file?
                if (tokType == Token.TOKENTYPE.EOF)
                    return;

                // Otherwise load the next token from the tokenizer.
                curTok = lexer.GetNextToken(); // get the next token
            }
            else
            {
                // We have the wrong token; bail out gracefully
                string strMsg = string.Format("Expected {0}; found {1} ('{2}')at source line {3}",
                    tokType.ToString(), curTok.tokType.ToString(), curTok.lexName, curTok.lineNumber);
                throw new Exception("Parser - Match: " + strMsg);
            }
        } // Match

        /// <summary>
        /// Handles MOD2 code before the BEGIN keyword
        /// </summary>
        private void Declatations()
        {
            while (curTok.tokType != Token.TOKENTYPE.BEGIN)
            {
                switch (curTok.tokType)
                {
                    case Token.TOKENTYPE.TYPE: TYPE(); break;
                    case Token.TOKENTYPE.VAR: DECLVAR(); break;
                    case Token.TOKENTYPE.CONST: CONSTANT(); break;
                    case Token.TOKENTYPE.PROCEDURE: PROCEDURE(); break;
                }
            }
        } // DECLARATIONS

        /// <summary>
        /// Directs and facilitates the handling of mod2 tokens to different helper functions
        /// </summary>
        private void StatementSequence()
        {
            while (curTok.tokType != Token.TOKENTYPE.END &&
                curTok.tokType != Token.TOKENTYPE.ELSE) // TODO move exit to here
            {
                CommentSrcLine();
                switch (curTok.tokType)
                {
                    case Token.TOKENTYPE.WRLN: WRLN(); break;
                    case Token.TOKENTYPE.WRSTR: WRSTR(); break;
                    case Token.TOKENTYPE.WRINT: WRINT(); break;
                    case Token.TOKENTYPE.WRREAL: WRREAL(); break;
                    case Token.TOKENTYPE.ID: IDEN(); break;
                    case Token.TOKENTYPE.RDINT: RDINT(); break;
                    case Token.TOKENTYPE.IF: IFF(); break;
                    case Token.TOKENTYPE.LOOP: LOOP(); break;
                    case Token.TOKENTYPE.EXIT: EXIT(); break;
                    case Token.TOKENTYPE.CLS: CLS(); break;
                }
            }
        }

        /// <summary>
        /// Directs and facilitates the handling of prep for parsing and writing to files
        /// to various helper functions 
        /// </summary>
        public void Parse()
        {
            Reset();
            curTok = lexer.GetNextToken();
            symTbl.enterMain(fm.MAIN_PROC);

            Module();
            emitter.MainProcPostamble(symTbl.CloseScope());
            emitter.WriteAllFiles();
        } // Parse

        /// <summary>
        /// Handles the expected structure of a MOD2 program, and calls helper functions to
        /// handle the different tokens which could be encountered.
        /// </summary>
        private void Module()
        {
            // Enter Program
            Match(Token.TOKENTYPE.MODULE);
            string moduleName = curTok.lexName;
            emitter.MainProcPreamble();
            Match(Token.TOKENTYPE.ID);
            Match(Token.TOKENTYPE.SEMI_COLON);
            Declatations();
            Match(Token.TOKENTYPE.BEGIN);

            StatementSequence();
            Match(Token.TOKENTYPE.END);

            // End Program
            if (curTok.lexName != moduleName)
                throw new Exception("Module name not repeated at close of module.");
            Match(Token.TOKENTYPE.ID);
            Match(Token.TOKENTYPE.DOT);
            Match(Token.TOKENTYPE.EOF);
        } // Module

        /// <summary>
        /// Handles declaring new data types TYPE ListType = ARRAY [11 .. 30] OF INTEGER ;
        /// </summary>
        private void TYPE()
        {
            Match(Token.TOKENTYPE.TYPE);

            while (curTok.tokType == Token.TOKENTYPE.ID)
            {
                UserDecType newType = new UserDecType();
                newType.typeName = curTok.lexName;
                Match(Token.TOKENTYPE.ID);

                Match(Token.TOKENTYPE.EQUAL);

                if (curTok.tokType == Token.TOKENTYPE.ARRAY)
                {
                    Match(Token.TOKENTYPE.ARRAY);
                    Match(Token.TOKENTYPE.LEFT_BRACK);
                    newType.startRange = Int32.Parse(curTok.lexName);
                    Match(Token.TOKENTYPE.INT_NUM);
                    Match(Token.TOKENTYPE.DOT_DOT);

                    newType.endRange = Int32.Parse(curTok.lexName);
                    Match(Token.TOKENTYPE.INT_NUM);
                    Match(Token.TOKENTYPE.RIGHT_BRACK);
                    Match(Token.TOKENTYPE.OF);
                }

                newType.baseType = curTok.lexName;
                curTok = lexer.GetNextToken(); // TODO adjust logic to closer fit EBNF

                if (!symTbl.AddType(newType))
                    ErrorHandler.Error(ERROR_CODE.VAR_REDECLARATION, "Symbol Table - addSymbol()",
                        "Symbol TYPE Already Exists in Scope", curTok.lineNumber);

                Match(Token.TOKENTYPE.SEMI_COLON);
            }
        } // TYPE

        /// <summary>
        /// Handles declaring variables VAR i : INTEGER
        /// </summary>
        private void DECLVAR()
        {
            Match(Token.TOKENTYPE.VAR);

            while (curTok.tokType == Token.TOKENTYPE.ID)
            {
                ArrayList al = new ArrayList();

                al.Add(curTok);
                Match(Token.TOKENTYPE.ID);

                while (curTok.tokType != Token.TOKENTYPE.COLON)
                {
                    Match(Token.TOKENTYPE.COMMA);
                    al.Add(curTok);
                    Match(Token.TOKENTYPE.ID);
                }

                Match(Token.TOKENTYPE.COLON);

                // expects the next token to be the declared variable's type
                Token dataType = curTok;
                foreach (Token id in al)
                {
                    if (!symTbl.AddVar(id.lexName, dataType))
                        ErrorHandler.Error(ERROR_CODE.VAR_REDECLARATION, "Symbol Table - addSymbol()",
                            "Symbol Already Exists in Scope", id.lineNumber);
                }
                curTok = lexer.GetNextToken();

                Match(Token.TOKENTYPE.SEMI_COLON);
            }
        } // DECLVAR

        /// <summary>
        /// Handles declaring variables CONST lowest = 3; hi = 7; middle = 7;
        /// </summary>
        private void CONSTANT()
        {
            Match(Token.TOKENTYPE.CONST);

            while (curTok.tokType == Token.TOKENTYPE.ID)
            {
                string id, value;
                Token token = curTok;
                id = token.lexName;

                Match(Token.TOKENTYPE.ID);
                Match(Token.TOKENTYPE.EQUAL);

                value = curTok.lexName;

                Match(Token.TOKENTYPE.INT_NUM);
                Match(Token.TOKENTYPE.SEMI_COLON);
                if (!symTbl.AddConstant(id, value))
                    ErrorHandler.Error(ERROR_CODE.VAR_REDECLARATION, "Symbol Table - addSymbol()",
                        "Symbol Already Exists in Scope", token.lineNumber);
            }
        } // CONSTANT

        /// <summary>
        /// Handles declaring procedures PROCEDURE little ( ) ; BEGIN  WRSTR("Hello Procedure!"); WRLN; END little;
        /// </summary>
        private void PROCEDURE()
        {
            int memSize;

            Match(Token.TOKENTYPE.PROCEDURE);
            string name = curTok.lexName;
            Match(Token.TOKENTYPE.ID);

            symTbl.enterScope(name);

            Match(Token.TOKENTYPE.LEFT_PAREN);

            while (curTok.tokType == Token.TOKENTYPE.ID || curTok.tokType == Token.TOKENTYPE.VAR)
            {
                bool var = (curTok.tokType == Token.TOKENTYPE.VAR);

                if (var)
                    Match(Token.TOKENTYPE.VAR);

                ArrayList al = new ArrayList();

                al.Add(curTok);
                Match(Token.TOKENTYPE.ID);

                while (curTok.tokType != Token.TOKENTYPE.COLON)
                {
                    Match(Token.TOKENTYPE.COMMA);
                    al.Add(curTok);
                    Match(Token.TOKENTYPE.ID);
                }

                Match(Token.TOKENTYPE.COLON);

                // expects the next token to be the declared variable's type
                Token dataType = curTok;
                al.Reverse();

                foreach (Token id in al)
                {
                    if (!symTbl.AddParam(id.lexName, dataType, var))
                        ErrorHandler.Error(ERROR_CODE.VAR_REDECLARATION, "Symbol Table - addSymbol()",
                            "Symbol Already Exists in Scope", curTok.lineNumber);
                }

                curTok = lexer.GetNextToken();
            }

            Match(Token.TOKENTYPE.RIGHT_PAREN);
            Match(Token.TOKENTYPE.SEMI_COLON);
            Declatations();
            Match(Token.TOKENTYPE.BEGIN);
            emitter.ProcPreamble(name);
            StatementSequence();
            Match(Token.TOKENTYPE.END);
            memSize = symTbl.CloseScope();
            emitter.ProcPostamble(name, memSize);
            if (curTok.lexName != name)
                ErrorHandler.Error(ERROR_CODE.UKNOWN_ERROR, "PROCEDURE()", "END name did not match PROCEDURE name");

            curTok = lexer.GetNextToken();
            Match(Token.TOKENTYPE.SEMI_COLON);

        } // PROCEDURE

        // Handles collecting the expected Tokens for a WRLN command and calls the emitter to write MASM code for WRLN
        private void WRLN()
        {
            Match(Token.TOKENTYPE.WRLN);
            Match(Token.TOKENTYPE.SEMI_COLON);
            emitter.WRLN();
        } // WRLN

        /// <summary>
        /// Handles collecting the expected Tokens for a WRSTR command
        /// and calls the emitter to write MASM code for WRSTR
        /// </summary>
        private void WRSTR()
        {
            Match(Token.TOKENTYPE.WRSTR);
            Match(Token.TOKENTYPE.LEFT_PAREN);
            Token token = curTok;
            Match(Token.TOKENTYPE.STRING);
            emitter.AddString(token.lexName);
            Match(Token.TOKENTYPE.RIGHT_PAREN);
            Match(Token.TOKENTYPE.SEMI_COLON);
            emitter.WRSTR();
        } // WRSTR

        /// <summary>
        /// Handles collecting the expected Tokens for a WRINT command
        /// </summary>
        private void WRINT()
        {
            Match(Token.TOKENTYPE.WRINT);
            Match(Token.TOKENTYPE.LEFT_PAREN);
            GetExpression();
            Match(Token.TOKENTYPE.RIGHT_PAREN);
            Match(Token.TOKENTYPE.SEMI_COLON);
            emitter.print();
        }

        /// <summary>
        /// Handles collecting the expected Tokens for a WRINT command
        /// </summary>
        private void WRREAL()
        {
            Match(Token.TOKENTYPE.WRREAL);
            Match(Token.TOKENTYPE.LEFT_PAREN);
            GetExpression();
            Match(Token.TOKENTYPE.RIGHT_PAREN);
            Match(Token.TOKENTYPE.SEMI_COLON);
            emitter.print();
        }

        private void IDEN()
        {
            string id = curTok.lexName;
            Match(Token.TOKENTYPE.ID);
            if (curTok.tokType != Token.TOKENTYPE.LEFT_PAREN)
            {
                VARASGN(id);
            }
            else
            {   // Procedure Call
                Symbol proc = symTbl.GetSymbol(id);
                if (proc == null)
                {
                    // check procSymRef for recursive call
                    proc = symTbl.GetProcSymRef();

                    if (proc == null)
                        ErrorHandler.Error(ERROR_CODE.SYMBOL_UNDEFINED, "IDEN",
                            "Proc wasn't declared & not a recursive call");
                }

                emitter.MakeRoomForLocs(proc.localVarList.getMem());

                Match(Token.TOKENTYPE.LEFT_PAREN);

                ArrayList parameters = proc.paramVarList.getVars();
                parameters.Reverse();
                int paramNum = 0;
                while (curTok.tokType != Token.TOKENTYPE.RIGHT_PAREN && paramNum < parameters.Count)
                {
                    Symbol parm = (Symbol)parameters[paramNum];
                    if (parm.paramType == Symbol.PARM_TYPE.VAL_PARM)
                    {
                        GetExpression(); // GetExpression leaves the result on the stack which is where we want it

                    }
                    else if (parm.paramType == Symbol.PARM_TYPE.REF_PARM)
                    {
                        string name = curTok.lexName;
                        Match(Token.TOKENTYPE.ID);
                        if (curTok.tokType != Token.TOKENTYPE.LEFT_BRACK)
                        {
                            emitter.RefParam(symTbl.GetSymbol(name).memOffset);
                        } else
                        {
                            Match(Token.TOKENTYPE.LEFT_BRACK);
                            GetExpression();
                            Match(Token.TOKENTYPE.RIGHT_BRACK);
                            emitter.RefElemArrayParam(symTbl.GetSymbol(name).memOffset, symTbl.GetSymbol(name).lowerBound);
                        }
                    }

                    if (curTok.tokType != Token.TOKENTYPE.COMMA)
                        break;
                    Match(Token.TOKENTYPE.COMMA);

                    paramNum++;
                }
                Match(Token.TOKENTYPE.RIGHT_PAREN);

                emitter.CallProc(id);
                Match(Token.TOKENTYPE.SEMI_COLON);
            }
        }

        /// <summary>
        /// Handles collecting the expected Tokens for a "i := 32" command
        /// </summary>
        private void VARASGN(string id)
        {
            Symbol sym;

            if (curTok.tokType != Token.TOKENTYPE.LEFT_BRACK)
            {
                Match(Token.TOKENTYPE.ASSIGN);
                GetExpression();
                sym = symTbl.GetSymbol(id);
                if (sym.paramType != Symbol.PARM_TYPE.REF_PARM)
                {
                    emitter.StoreVar(sym.memOffset);
                }
                else
                {
                    emitter.StoreRefVar(sym.memOffset);
                }
                Match(Token.TOKENTYPE.SEMI_COLON);
            }
            else
            {
                sym = symTbl.GetSymbol(id);
                Match(Token.TOKENTYPE.LEFT_BRACK);
                GetExpression();
                Match(Token.TOKENTYPE.RIGHT_BRACK);
                Match(Token.TOKENTYPE.ASSIGN);
                GetExpression();
                if (sym.paramType == Symbol.PARM_TYPE.REF_PARM)
                {
                    emitter.PutValInRefArray(sym.memOffset, sym.lowerBound);
                }
                else
                {
                    emitter.PutValInArray(sym.memOffset, sym.lowerBound);
                }
                Match(Token.TOKENTYPE.SEMI_COLON);
            }
        }

        /// <summary>
        /// get user input as an integer
        /// </summary>
        private void RDINT()
        {
            Match(Token.TOKENTYPE.RDINT);
            emitter.RDINT();
            Match(Token.TOKENTYPE.LEFT_PAREN);
            Match(Token.TOKENTYPE.RIGHT_PAREN);
        }

        // Clear Screen
        private void CLS()
        {
            Match(Token.TOKENTYPE.CLS);
            emitter.CLS();
            Match(Token.TOKENTYPE.SEMI_COLON);
        }

        /// <summary>
        /// Handles code related to the syntax of an IF statement in MOD2
        /// </summary>
        private void IFF()
        {
            Match(Token.TOKENTYPE.IF);
            GetExpression();

            emitter.IfConditional();
            Match(Token.TOKENTYPE.THEN);
            StatementSequence();
            CommentSrcLine();
            emitter.CloseIf();
            if (curTok.tokType == Token.TOKENTYPE.ELSE)
            {
                Match(Token.TOKENTYPE.ELSE);
                StatementSequence();
            }
            CommentSrcLine();
            emitter.CloseElse();
            Match(Token.TOKENTYPE.END);
            Match(Token.TOKENTYPE.SEMI_COLON);
        }

        /// <summary>
        /// Handles code related to the syntax of an LOOP statement in MOD2
        /// </summary>
        private void LOOP()
        {
            Match(Token.TOKENTYPE.LOOP);
            emitter.PrepLoop();
            StatementSequence();
            Match(Token.TOKENTYPE.END);
            Match(Token.TOKENTYPE.SEMI_COLON);
            emitter.EndLoop();
        }

        // Handles code related to the syntax of an EXIT statement in MOD2
        private void EXIT()
        {
            Match(Token.TOKENTYPE.EXIT);
            Match(Token.TOKENTYPE.SEMI_COLON);
            emitter.ExitLoop();
        }

        private int Designator()
        {
            string id = curTok.lexName;
            Match(Token.TOKENTYPE.ID);
            if (curTok.tokType == Token.TOKENTYPE.LEFT_BRACK)
            {
                Match(Token.TOKENTYPE.LEFT_BRACK);
                GetExpression();
                Match(Token.TOKENTYPE.RIGHT_BRACK);
            }
            return symTbl.GetSymbol(id).memOffset;
        }

        /// <summary>
        /// expression -->
        ///      simpleExpression[relOperator simpleExression]
        /// </summary>
        private void GetExpression()
        {
            GetSimpleExpr();
            if (curTok.tokType == Token.TOKENTYPE.LESS_THAN ||
                curTok.tokType == Token.TOKENTYPE.GRTR_THAN ||
                curTok.tokType == Token.TOKENTYPE.GRTR_THAN_EQ ||
                curTok.tokType == Token.TOKENTYPE.LESS_THAN_EQ ||
                curTok.tokType == Token.TOKENTYPE.EQUAL ||
                curTok.tokType == Token.TOKENTYPE.NOT_EQ)
            {
                string lexeme = curTok.lexName; // get relational operator

                curTok = lexer.GetNextToken();  // prep to get right-hand side of relationship

                GetSimpleExpr();
                emitter.PrepIf(lexeme);
                emitter.StoreCondRes(not); // TODO find cleaner way to do NOT
                not = false;
            }

            if (curTok.tokType == Token.TOKENTYPE.RDINT)
                RDINT();
        }

        /// <summary>
        /// simpleExpression -->
        ///      [ + | - ] term { addOperator term }
        /// </summary>
        private void GetSimpleExpr()
        {
            // unary operator
            if (curTok.tokType == Token.TOKENTYPE.PLUS || curTok.tokType == Token.TOKENTYPE.MINUS)
            {
                curTok = lexer.GetNextToken();
                GetTerm();
                emitter.NEG();
            }
            else
                GetTerm();

            while (curTok.tokType == Token.TOKENTYPE.PLUS || curTok.tokType == Token.TOKENTYPE.MINUS ||
                   curTok.tokType == Token.TOKENTYPE.OR)
            {
                if (curTok.tokType == Token.TOKENTYPE.PLUS)
                {
                    curTok = lexer.GetNextToken();
                    GetTerm();
                    emitter.TwoPopOpPush('+');
                }
                else if (curTok.tokType == Token.TOKENTYPE.MINUS)
                {
                    curTok = lexer.GetNextToken();
                    GetTerm();
                    emitter.TwoPopOpPush('-');
                }
                else if (curTok.tokType == Token.TOKENTYPE.OR)
                {
                    Match(Token.TOKENTYPE.OR);
                    GetExpression();
                    emitter.TestOr();
                }
            }
        }

        /// <summary>
        /// term -->
        ///      factor { mulOperator factor }
        /// </summary>
        private void GetTerm()
        {
            GetFactor();
            while (curTok.tokType == Token.TOKENTYPE.MULT || curTok.tokType == Token.TOKENTYPE.MOD ||
                   curTok.tokType == Token.TOKENTYPE.SLASH || curTok.tokType == Token.TOKENTYPE.DIV ||
                   curTok.tokType == Token.TOKENTYPE.AND)
            {
                if (curTok.tokType == Token.TOKENTYPE.MULT)
                {
                    curTok = lexer.GetNextToken();
                    GetFactor();
                    emitter.TwoPopOpPush('*');
                }
                else if (curTok.tokType == Token.TOKENTYPE.SLASH || curTok.tokType == Token.TOKENTYPE.DIV)
                {
                    curTok = lexer.GetNextToken();
                    GetFactor();
                    emitter.TwoPopOpPush('/');
                }
                else if (curTok.tokType == Token.TOKENTYPE.MOD)
                {
                    curTok = lexer.GetNextToken();
                    GetFactor();
                    emitter.TwoPopOpPush('%');
                }
                else if (curTok.tokType == Token.TOKENTYPE.AND)
                {
                    Match(Token.TOKENTYPE.AND);
                    GetFactor();
                    emitter.TestAnd();
                }
            }
        }

        /// <summary>
        /// factor -->
        ///      designator | ident( expList ) | number | ( expression ) | NOT factor 
        /// </summary>
        private void GetFactor()
        {
            if (curTok.tokType == Token.TOKENTYPE.ID)
            {
                Symbol s = symTbl.GetSymbol(curTok.lexName);
                if (s.symbolType == Symbol.SYMBOL_TYPE.TYPE_CONST)
                {
                    emitter.pushOntoStackToProc(s.constDoubleValue);
                    curTok = lexer.GetNextToken();
                }
                else if (s.symbolType == Symbol.SYMBOL_TYPE.TYPE_ARRAY)
                {
                    // Designator
                    string id = curTok.lexName;
                    Match(Token.TOKENTYPE.ID);
                    Match(Token.TOKENTYPE.LEFT_BRACK);
                    GetExpression();
                    Match(Token.TOKENTYPE.RIGHT_BRACK);
                    if (s.paramType == Symbol.PARM_TYPE.REF_PARM)
                    {
                        emitter.GetValFromRefArray(s.memOffset, s.lowerBound);
                    }
                    else
                    {
                        emitter.GetValFromArray(s.memOffset, s.lowerBound);
                    }
                }
                else
                {
                    emitter.GetVar(s.memOffset);

                    if (s.paramType != Symbol.PARM_TYPE.REF_PARM)
                        emitter.EAXToStack();
                    else
                        emitter.MoveRefVar();

                    curTok = lexer.GetNextToken();
                }

            }
            else if (curTok.tokType == Token.TOKENTYPE.INT_NUM)
            {
                emitter.pushOntoStackToProc(curTok.lexName);
                curTok = lexer.GetNextToken();
            }
            else if (curTok.tokType == Token.TOKENTYPE.REAL_NUM)
            {
                emitter.pushOntoStackToProc(curTok.lexName);
                curTok = lexer.GetNextToken();
            }
            else if (curTok.tokType == Token.TOKENTYPE.LEFT_PAREN)
            {
                Match(Token.TOKENTYPE.LEFT_PAREN);
                GetExpression();
                Match(Token.TOKENTYPE.RIGHT_PAREN);
            }
            else if (curTok.tokType == Token.TOKENTYPE.NOT)
            {
                Match(Token.TOKENTYPE.NOT);
                not = true;
                GetFactor();
            }
        }

        // Adds a comment with the contents of the source line for which the proceeding emitted MASM code is for 
        private void CommentSrcLine()
        {
            emitter.AddComment("Source Line:" + curTok.lineNumber + ": " + fm.SOURCE_FILE_TEXT[curTok.lineNumber]);
        }

    } // Parser
} // Compiler namespace