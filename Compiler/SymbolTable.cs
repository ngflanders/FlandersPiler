using System;
using System.Collections;
using System.Collections.Generic; // ArrayList, HashTable, Stack

namespace Compiler
{
    /******************* Stack Description *******************************************
        Next are the parameters (the first parameter is pushed first,
        Last are the local variables, pushed onto the stack as they are encountered.

        Here is a hypothetical stack with two local variables and three parameters (e.g. Towers of Hanoi).
        All are assumed to be four byte integers 

            Note that the Intel stack decrements and that the "new" value of BP
                (the value used during statements within this procedure) is equal to the
                lowest address.

            param 1     ebp + 24
            param 2     ebp + 20
            param 3     ebp + 16
            local 1     ebp + 12
            local 2     ebp + 8
            rtn adr     ebp + 4
            EBP         ebp

    Before we start executing PROC statments, we push everything onto the stack, then call
    the PROC (which pushes return address onto the stack), then we "push BP" and finally "mov BP,SP". 
    Now everything is just where we want it during PROC execution.
    *********************************************************************************/

    /// <summary>
    /// Symbol constitutes each entry in the symbol table. It stores information needed by
    ///    the parser and emitter for each identifier (ID). ID's may be variables, constants,
    ///    or procedures.
    /// </summary>
    public class Symbol
    {
        /// <summary>
        /// This enumerated type defines the type of identifier/variable. 
        /// Note that TYPE_SIMPLE is a simple type like INTEGER or CARDINAL
        /// TYPE_TYPE_SIMPLE is the name of a derived type based on a simple type
        /// TYPE_ARRAY is the type of an array. The items in the array are identified by 
        ///       STORE_TYPE that follows
        /// TYPE_TYPE_ARRAY is the name of a derived array type (e.g. "listType")
        /// </summary>
        public enum SYMBOL_TYPE
        {
            TYPE_NONE,
            TYPE_CONST,
            TYPE_TYPE_SIMPLE,
            TYPE_SIMPLE,
            TYPE_TYPE_ARRAY,
            TYPE_ARRAY,
            TYPE_TYPE_STRING,
            TYPE_STRING,
            TYPE_PROC,
            TYPE_FUNCTION,
            TYPE_ERROR = -1
        };

        // What kind of storage is needed for this symbol?
        public enum STORE_TYPE
        {
            STORE_NONE,
            TYPE_INT,
            TYPE_RL,
            TYPE_CD
        };

        // VAL_PARM and REF_PARM are parameters (within the scope of a procedure)
        //     LOCAL_VAR are local variables within a procedure or in main (stored on the stack)
        public enum PARM_TYPE
        {
            VAL_PARM,
            REF_PARM,
            LOCAL_VAR
        };

        // track the symbol type of this symbol
        public SYMBOL_TYPE symbolType;

        // track the store type of this symbol
        public STORE_TYPE storeType;

        // track the param type of this symbol
        public PARM_TYPE paramType;

        public string symName;

        public int scopeNumber, // scope of this symbol
            memOffset,  // offset in bytes within current stack frame
            lowerBound, // array lower limit
            upperBound; // array upper limit

        // value for a constant integer
        public double constDoubleValue;

        // for procedures only, used to retain information about variables used
        public ProcVarList paramVarList;
        public ProcVarList localVarList;

        // Create an empty Symbol with scope retrieved from SymbolTable.
        private Symbol()
        {
            scopeNumber = SymbolTable.CUR_SCOPE_NUM;
        }

        // Simple symbol constructor
        public Symbol(string name, int offset)
        {
            symName = name;
            memOffset = offset;
            scopeNumber = SymbolTable.CUR_SCOPE_NUM;
        }

        // copy constructor
        public Symbol(Symbol sym)
        {
            symbolType = sym.symbolType;
            storeType = sym.storeType;
            paramType = sym.paramType;
            scopeNumber = sym.scopeNumber;
            constDoubleValue = sym.constDoubleValue;
            memOffset = sym.memOffset;
            lowerBound = sym.lowerBound;
            upperBound = sym.upperBound;
            paramVarList = sym.paramVarList; //  Warning...shallow copy
        }

        // Overriden ToString function for printing out the symbol's member data
        public override string ToString()
        {
            return String.Format("{0,-16}{1,-8}{2,-16}{3,-16}{4,-16}{5,0}\r\n",
                symName, scopeNumber, symbolType, storeType, paramType, memOffset);
        }
    } // Symbol class

    /// <summary>
    /// This handles one procedure completely. It stores the list
    ///    of variables (ArrayList of string), the local memory needed, 
    ///    and the parameter count for this one procedure.
    /// </summary>
    public class ProcVarList
    {
        ArrayList vars;
        int memAmount;

        // only public constructor
        public ProcVarList(string name)
        {
            PROC_NAME = name;
            vars = new ArrayList();
            memAmount = 0;
        }

        public void Add(Symbol s, int memSize)
        {
            vars.Add(s);
            memAmount += memSize;
        }

        public int getMem()
        {
            return memAmount;
        }

        public ArrayList getVars()
        {
            return vars;
        }

        public string PROC_NAME { get; }
    } // ProcVarList class

    /// <summary>
    /// The Symbol Table is a Singleton class that manages identifiers. 
    ///    The table is maintained as a stack of Scope objects (defined below).
    /// </summary>
    class SymbolTable
    {
        // reference to the file manager
        static FileManager fm = FileManager.Instance;

        // keep track of the scope number
        static int currentScope;

        /// <summary>
        /// class to define and keep track of scopes for the scope stack
        /// </summary>
        class Scope
        {
            string name;
            int nextMemOffset; // next available memory offset
            Dictionary<string, Symbol> symbols; // to store identifiers
            Symbol procSymRef;
            List<Symbol> defTypes;
            List<Symbol> procs;

            // Scope constructor sets the scope number and creates a new hashtable to keep symbols for this scope
            public Scope(string name)
            {
                this.name = name;
                SCOPE_NUMBER = currentScope++;
                nextMemOffset = 8;
                symbols = new Dictionary<string, Symbol>();
                defTypes = new List<Symbol>();
                procs = new List<Symbol>();
            }

            // Scope constructor sets the scope number and creates a new hashtable to keep symbols for this scope
            public Scope(string name, Symbol procRef)
            {
                procSymRef = procRef;
                this.name = name;
                SCOPE_NUMBER = currentScope++;
                nextMemOffset = 8;
                symbols = new Dictionary<string, Symbol>();
                defTypes = new List<Symbol>();
                procs = new List<Symbol>();
            }


            /// <summary>
            /// Takes in a token and adds that symbol to dictionary
            /// </summary>
            /// <param name="t"></param>
            public void addConstSym(string id, string value)
            {
                Symbol s = new Symbol(id, 0);
                s.symbolType = Symbol.SYMBOL_TYPE.TYPE_CONST;
                s.storeType = Symbol.STORE_TYPE.TYPE_INT;
                s.paramType = Symbol.PARM_TYPE.VAL_PARM;
                s.constDoubleValue = Double.Parse(value);

                symbols.Add(id, s);
                fm.SYM_LIST += s;
            }

            /// <summary>
            /// Takes in an id and creates an int variable Symbol and adds that symbol to dictionary
            /// </summary>
            public void addIntVarSym(string id)
            {
                Symbol s = new Symbol(id, nextMemOffset);
                s.symbolType = Symbol.SYMBOL_TYPE.TYPE_SIMPLE;
                s.storeType = Symbol.STORE_TYPE.TYPE_INT;
                s.paramType = Symbol.PARM_TYPE.LOCAL_VAR;

                if (procSymRef != null)
                    procSymRef.localVarList.Add(s, 4);

                nextMemOffset += 4;
                symbols.Add(id, s);
                fm.SYM_LIST += s;
            } // addIntVarSym

            /// <summary>
            /// 
            /// </summary>
            public void addCustomVarSym(string id, Symbol typeSym)
            {
                Symbol s = new Symbol(typeSym);
                s.symName = id;
                s.memOffset = nextMemOffset;
                s.symbolType = Symbol.SYMBOL_TYPE.TYPE_ARRAY;

                nextMemOffset += 4 * (s.upperBound - s.lowerBound + 1);
                symbols.Add(id, s);
                fm.SYM_LIST += s;
            } // addCustomVarSym

            /// <summary>
            /// Takes in an id and creates an int variable Symbol and adds that symbol to dictionary
            /// </summary>
            public void addIntParam(string id, bool isRef)
            {
                Symbol s = new Symbol(id, nextMemOffset);
                s.symbolType = Symbol.SYMBOL_TYPE.TYPE_SIMPLE;
                s.storeType = Symbol.STORE_TYPE.TYPE_INT;

                if (isRef == true)
                    s.paramType = Symbol.PARM_TYPE.REF_PARM;
                else
                    s.paramType = Symbol.PARM_TYPE.VAL_PARM;

                procSymRef.paramVarList.Add(s, 4);

                nextMemOffset += 4;
                symbols.Add(id, s);
                fm.SYM_LIST += s;
            } // addIntParam


            /// <summary>
            /// 
            /// </summary>
            public void addCustomVarParam(string id, Symbol typeSym, bool isRef)
            {
                int size = 0;
                Symbol s = new Symbol(typeSym);
                s.symName = id;
                s.memOffset = nextMemOffset;
                s.symbolType = Symbol.SYMBOL_TYPE.TYPE_ARRAY;
                if (isRef == true)
                {
                    s.paramType = Symbol.PARM_TYPE.REF_PARM;
                    size = 4;
                }
                else
                {
                    s.paramType = Symbol.PARM_TYPE.VAL_PARM;
                    size = 4 * (s.upperBound - s.lowerBound + 1);
                }


                procSymRef.paramVarList.Add(s, size);

                nextMemOffset += size;
                symbols.Add(id, s);
                fm.SYM_LIST += s;
            } // addCustomVarSym


            /// <summary>
            /// Takes in a token and adds that symbol to dictionary
            /// </summary>
            /// <param name="t"></param>
            public void addArrayTypeSym(string id, int low, int high)
            {
                Symbol s = new Symbol(id, 0);

                s.symbolType = Symbol.SYMBOL_TYPE.TYPE_TYPE_ARRAY;
                s.storeType = Symbol.STORE_TYPE.TYPE_INT;
                s.paramType = Symbol.PARM_TYPE.LOCAL_VAR;
                s.lowerBound = low;
                s.upperBound = high;

                symbols.Add(id, s);

                defTypes.Add(s);

                fm.SYM_LIST += s;
            } // addSymbolArray

            public int getNextMemOffset()
            {
                return nextMemOffset;
            }

            public string getName()
            {
                return name;
            }

            public Symbol getProcRef()
            {
                return procSymRef;
            }

            public List<Symbol> getDefTypes()
            {
                return defTypes;
            }

            public List<Symbol> getProcs()
            {
                return procs;
            }

            /// <summary>
            /// Used for adding a procedure
            /// </summary>
            /// <param name="name"></param>
            public void addProcSymbol(string name)
            {
                Symbol s = new Symbol(name, 0);
                s.symbolType = Symbol.SYMBOL_TYPE.TYPE_PROC;
                s.storeType = Symbol.STORE_TYPE.STORE_NONE;
                s.paramType = Symbol.PARM_TYPE.REF_PARM;

                s.paramVarList = new ProcVarList(name);
                s.localVarList = new ProcVarList(name);

                procs.Add(s);
                symbols.Add(name, s);
                fm.SYM_LIST += s;
            }

            public void inheritProc(Symbol oldProc)
            {
                if (!procs.Contains(oldProc))
                    procs.Add(oldProc);
                if (!symbols.ContainsKey(oldProc.symName))
                    symbols.Add(oldProc.symName, oldProc);
                fm.SYM_LIST += oldProc;
            }

            // Checks if the provided symbol name already exists in the scope
            public bool symbolInScope(string name)
            {
                return symbols.ContainsKey(name);
            }

            // Returns the Symbol in this scope's dictionary of symbols
            public Symbol GetSymbolInScope(string name)
            {
                return symbols[name];
            }

            // Overriden Scope toString method for iterating over all symbols in a scope
            public override string ToString()
            {
                string result = "";
                foreach (var keyValuePair in symbols)
                {
                    Symbol t = keyValuePair.Value;
                    result += t + "\r\n";
                }
                return result;
            }

            // get scope number
            public int SCOPE_NUMBER { get; } // SCOPE_NUMBER
        } // Scope

        static Stack<Scope> scopeStack; // stack for storing scopes

        // The single object instance for this class.
        private static SymbolTable stInstance;

        // To prevent access by more than one thread. This is the specific lock belonging to the SymbolTable Class object.
        private static Object stLock = typeof(SymbolTable);

        // Instead of a constructor, we offer a static method to return the only instance.
        // private constructor so no one else can create one.
        private SymbolTable()
        { }

        /// <summary>
        /// Management for static instance of this class
        /// </summary>
        public static SymbolTable Instance
        {
            get
            {
                lock (stLock)
                {
                    // if this is the first request, initialize the one instance
                    if (stInstance == null)
                    {
                        stInstance = new SymbolTable();
                        stInstance.Reset(); // reset all variables
                    }

                    // in either case, return a reference to the only instance
                    return stInstance;
                }
            }
        } // SymbolTable Instance

        // Used for adding the first scope to the stack and adding a simple proc symbol
        public void enterMain(string name)
        {
            scopeStack.Push(new Scope(name));
            TOP_SCOPE.addProcSymbol(fm.MAIN_PROC);
        }

        // Adds the name of the proc to the curr scope, then pushing new scope onto the stack
        public void enterScope(string name)
        {
            List<Symbol> types;
            List<Symbol> procs;
            if (!TOP_SCOPE.symbolInScope(name))
            {
                types = TOP_SCOPE.getDefTypes();
                procs = TOP_SCOPE.getProcs();

                TOP_SCOPE.addProcSymbol(name);
                Symbol s = TOP_SCOPE.GetSymbolInScope(name);
                scopeStack.Push(new Scope(name, s));

                foreach (Symbol type in types)
                {
                    TOP_SCOPE.addArrayTypeSym(type.symName, type.lowerBound, type.upperBound);
                }

                foreach (Symbol proc in procs)
                {
                    TOP_SCOPE.inheritProc(proc);
                }
            }
            else
                ErrorHandler.Error(ERROR_CODE.PROC_REDECLARATION, "Symbol Table - enterScope()",
                    "Proc Already Exists in Scope");
        }

        // Pops the top scope off of the stack
        public int CloseScope()
        {
            Scope scp = scopeStack.Pop();
            return scp.getNextMemOffset() - 8;
        }

        public bool AddVar(string name, Token dataToken)
        {
            if (!TOP_SCOPE.symbolInScope(name))
            {
                if (dataToken.tokType == Token.TOKENTYPE.INTEGER)
                    TOP_SCOPE.addIntVarSym(name);
                else if (dataToken.tokType == Token.TOKENTYPE.ID && TOP_SCOPE.symbolInScope(dataToken.lexName))
                    TOP_SCOPE.addCustomVarSym(name, TOP_SCOPE.GetSymbolInScope(dataToken.lexName));
                else
                    return false;
            }
            else
                return false;
            return true;
        }

        // Parameter
        public bool AddParam(string name, Token dataToken, bool isRef)
        {
            if (!TOP_SCOPE.symbolInScope(name))
            {
                if (dataToken.tokType == Token.TOKENTYPE.INTEGER)
                    TOP_SCOPE.addIntParam(name, isRef);
                else if (dataToken.tokType == Token.TOKENTYPE.ID && TOP_SCOPE.symbolInScope(dataToken.lexName))
                    TOP_SCOPE.addCustomVarParam(name, TOP_SCOPE.GetSymbolInScope(dataToken.lexName), isRef);
                else
                    return false;
            }
            else
                return false;
            return true;
        }

        public bool AddType(Parser.UserDecType userType)
        {
            if (!TOP_SCOPE.symbolInScope(userType.typeName))
            {
                TOP_SCOPE.addArrayTypeSym(userType.typeName, userType.startRange, userType.endRange);

                TOP_SCOPE.GetSymbolInScope(userType.typeName);
            }
            else
                return false;
            return true;
        }

        public bool AddConstant(string id, string value)
        {
            if (!TOP_SCOPE.symbolInScope(id))
                TOP_SCOPE.addConstSym(id, value);
            else
                return false;
            return true;
        }

        public Symbol GetSymbol(string name)
        {
            if (TOP_SCOPE.symbolInScope(name))
                return TOP_SCOPE.GetSymbolInScope(name);
            return null;
        }

        public Symbol GetProcSymRef()
        {
            return TOP_SCOPE.getProcRef();
        }

        // Reset the symbol table
        public void Reset()
        {
            fm.ResetSymbolList();
            currentScope = 0;                   // first scope will be scope 0
            scopeStack = new Stack<Scope>();    // no scopes on the stack yet
        }

        // get the top scope on the stack and return it
        private static Scope TOP_SCOPE
        {
            get
            {
                if (scopeStack == null || scopeStack.Count <= 0) return null;
                else return scopeStack.Peek();
            }
        }

        // get the scope number of the top scope on the scope stack
        public static int CUR_SCOPE_NUM
        {
            get
            {
                if (TOP_SCOPE == null) return -1;
                else return TOP_SCOPE.SCOPE_NUMBER;
            }
        }
    } // SymbolTable class
} // Compiler namespace