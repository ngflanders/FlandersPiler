using System;
using System.Collections.Specialized; // OrderedDictionary, StringDictionary

namespace Compiler
{
    class Lexer
    {
        // Track defaults and file locations.
        FileManager fm = FileManager.Instance;

        // reference to the instance of SourceReader
        private SourceReader sr = FileManager.Instance.SOURCE_READER;

        // A dictionary of Modula-2 keywords, ordered dictionary
        // holds order of insertion
        OrderedDictionary odKeywords = new OrderedDictionary();

        // keep track of total tokens
        private int tokenCount;

        // The single object instance for this class.
        private static Lexer lexInstance;

        // To prevent access by more than one thread. This is the specific lock 
        //    belonging to the Lexer Class object.
        private static Object lexLock = typeof(Lexer);

        /// <summary>
        /// Instead of a constructor, we offer a static method to return the onlyinstance.
        /// </summary>
        private Lexer()
        {} // private constructor so no one else can create one.

        /// <summary>
        /// returns the single instance of the lexer object
        /// </summary>
        public static Lexer Instance
        {
            get
            {
                lock (lexLock)
                {
                    if (lexInstance == null)
                    {
                        lexInstance = new Lexer();
                        lexInstance.AddKeywords();
                    }
                    return lexInstance;
                }
            }
        } // Lexer Instance

        
        /// <summary>
        /// Reset the lexing process.
        /// </summary>
        public void Reset()
        {
            // set the file position back to the starting point
            fm.SOURCE_READER.Reset();

            // reset the tokenList to just the header
            fm.ResetTokenList();

            // we don't have a token yet
            tokenCount = 0;
        } // Reset

        /// <summary>
        /// This is the principal function of this class. The Token object is created,
        ///     loaded with correct information, and returned.
        /// </summary>
        /// <returns>Token loaded with the correct data.
        /// If lexing has failed, the TOKENTYPE is ERROR</returns>
        public Token GetNextToken()
        {
            char c = fm.SOURCE_READER.GetNextOneChar();

            // loop thru cleauring things we dont want to make tokens of (NL, space, comments)
            while (Char.IsWhiteSpace(c) || (c == '(' && sr.PeekNextChar() == '*'))
            {
                if (c == '(' && sr.PeekNextChar() == '*')
                {
                    sr.GetNextOneChar();
                    ReadComment();
                }
                c = sr.GetNextOneChar();
            }

            if (c == SourceReader.EOF_SENTINEL)
                return PrintToken(new Token(Token.TOKENTYPE.EOF, "End Of File", sr.LINE_NUMBER));

            // read a identifier or keyword
            if (Char.IsLetter(c) || c == '_')
            {
                String temp = ReadId(c);
                return PrintToken(CheckKeywords(temp));
            }

            // Reads ints and floats
            if (Char.IsDigit(c))
                return PrintToken(ReadNum(c));

            // reads symbols and punctuation
            if (Char.IsPunctuation(c) || Char.IsSymbol(c))
                return PrintToken(ReadPunc(c));

            
            // if we find a token...create a new token and return it
            return PrintToken(new Token(Token.TOKENTYPE.ERROR, c.ToString(), sr.LINE_NUMBER));

        } // GetNextToken
        
        /// <summary>
        /// Token List string in FileManager is updated with all tokens from the source file
        ///    attractively formatted.
        /// </summary>
        public void ListTokens()
        {
            // reset prior to lexing a file
            Reset();
            while (GetNextToken().tokType != Token.TOKENTYPE.EOF) ;
        } // ListTokens
        
        /// <summary>
        /// Checks the character, and possibly the next character and returns the appropriate token
        /// </summary>
        /// <param name="c"></param>
        /// <returns>The token associated with the punctuation</returns>
        private Token ReadPunc(char c)
        {
            switch (c)
            {
                case '"': return new Token(Token.TOKENTYPE.STRING, ReadQuotedString(), sr.LINE_NUMBER);
                case '[': return new Token(Token.TOKENTYPE.LEFT_BRACK, "[", sr.LINE_NUMBER);
                case ']': return new Token(Token.TOKENTYPE.RIGHT_BRACK, "]", sr.LINE_NUMBER);
                case '(': return new Token(Token.TOKENTYPE.LEFT_PAREN, "(", sr.LINE_NUMBER);
                case ')': return new Token(Token.TOKENTYPE.RIGHT_PAREN, ")", sr.LINE_NUMBER);
                case '+': return new Token(Token.TOKENTYPE.PLUS, "+", sr.LINE_NUMBER);
                case '-': return new Token(Token.TOKENTYPE.MINUS, "-", sr.LINE_NUMBER);
                case '*': return new Token(Token.TOKENTYPE.MULT, "*", sr.LINE_NUMBER);
                case '/': return new Token(Token.TOKENTYPE.SLASH, "/", sr.LINE_NUMBER);
                case ';': return new Token(Token.TOKENTYPE.SEMI_COLON, ";", sr.LINE_NUMBER);
                case '=': return new Token(Token.TOKENTYPE.EQUAL, "=", sr.LINE_NUMBER);
                case ',': return new Token(Token.TOKENTYPE.COMMA, ",", sr.LINE_NUMBER);
                case '.':
                    if (sr.PeekNextChar() == '.')
                    {
                        sr.GetNextOneChar();
                        return new Token(Token.TOKENTYPE.DOT_DOT, "..", sr.LINE_NUMBER);
                    }
                    else
                        return new Token(Token.TOKENTYPE.DOT, ".", sr.LINE_NUMBER);
                case ':':
                    if (sr.PeekNextChar() == '=')
                    {
                        sr.GetNextOneChar();
                        return new Token(Token.TOKENTYPE.ASSIGN, ":=", sr.LINE_NUMBER);
                    }
                    else
                        return new Token(Token.TOKENTYPE.COLON, ":", sr.LINE_NUMBER);
                case '<':
                    if (sr.PeekNextChar() == '=')
                    {
                        sr.GetNextOneChar();
                        return new Token(Token.TOKENTYPE.LESS_THAN_EQ, "<=", sr.LINE_NUMBER);
                    }
                    else if (sr.PeekNextChar() == '>')
                    {
                        sr.GetNextOneChar();
                        return new Token(Token.TOKENTYPE.NOT_EQ, "<>", sr.LINE_NUMBER);
                    }
                    else
                        return new Token(Token.TOKENTYPE.LESS_THAN, "<", sr.LINE_NUMBER);
                case '>':
                    if (sr.PeekNextChar() == '=')
                    {
                        sr.GetNextOneChar();
                        return new Token(Token.TOKENTYPE.GRTR_THAN_EQ, ">=", sr.LINE_NUMBER);
                    }
                    else
                        return new Token(Token.TOKENTYPE.GRTR_THAN, ">", sr.LINE_NUMBER);
                default:
                    return new Token(Token.TOKENTYPE.ERROR, c.ToString(), sr.LINE_NUMBER);
            }
            
        }
        
        /// <summary>
        /// Reads a Mod2 ident syntax grammer. 
        /// ident --> letter { letter | digit}
        /// letter --> A..Z | a..z | _
        /// </summary>
        /// <param name="c"></param>
        /// <returns>A string fitting the ident grammar</returns>
        private string ReadId(char c)
        {
            string temp = c.ToString();

            while (sr.PeekNextChar() == '_' || Char.IsLetterOrDigit(sr.PeekNextChar()))
                temp += sr.GetNextOneChar();

            return temp;
        }
        
        /// <summary>
        /// Reads a mod2 real or int
        /// </summary>
        /// <param name="c"></param>
        /// <returns>Real or Int Token</returns>
        private Token ReadNum(char c)
        {
            string temp = c.ToString();
            bool hasDec = false;    // assures only one period, and determines TokenType (real/int)

            while (Char.IsDigit(sr.PeekNextChar()) || sr.PeekNextChar() == '.')
            {
                c = sr.GetNextOneChar();
                temp += c;
                if (c == '.' && !hasDec)
                    hasDec = true;
                else if (c == '.' && hasDec)
                    return new Token(Token.TOKENTYPE.ERROR, temp, sr.LINE_NUMBER);
            }

            return new Token(hasDec ? Token.TOKENTYPE.REAL_NUM : Token.TOKENTYPE.INT_NUM,
                temp, sr.LINE_NUMBER);
        }
        
        /// <summary>
        /// Reads and ignores characters until the end of comment marker
        /// </summary>
        private void ReadComment()
        {
            while (sr.GetNextOneChar() != '*' || sr.PeekNextChar() != ')') ;

            sr.GetNextOneChar();
        }
        
        /// <summary>
        /// Reads a string within quotes to be treated as a whole token, and not lexed
        /// </summary>
        /// <returns>The string within the double quotes</returns>
        private string ReadQuotedString()
        {
            char c;
            string temp = "";

            while (sr.PeekNextChar() != SourceReader.EOF_SENTINEL && (c = sr.GetNextOneChar()) != '"'
                   && c != SourceReader.NL_SENTINEL)
                temp += c;

            return temp;
        }
        
        /// <summary>
        /// Identify reserved words from the keywords hashtable
        /// </summary>
        /// <param name="word"></param>
        /// <returns>The Token of the matching keyword, if no matches, ID token is returned</returns>
        private Token CheckKeywords(String word)
        {
            return new Token(odKeywords.Contains(word) ? (Token.TOKENTYPE)odKeywords[word] : Token.TOKENTYPE.ID,
                word, fm.SOURCE_READER.LINE_NUMBER);
        }
        
        /// <summary>
        /// Add reserved words to the keywords dictionary
        /// </summary>
        private void AddKeywords()
        {
            foreach (String s in Enum.GetNames(typeof(Token.TOKENTYPE)))
                odKeywords.Add(s, (Token.TOKENTYPE) Enum.Parse(typeof(Token.TOKENTYPE), s));
        }
        
        /// <summary>
        /// Adds the formatted token string to a file
        /// </summary>
        /// <param name="token"></param>
        /// <returns>The passed in token is returned to allow
        /// for simplified return statements in GetNextToken()</returns>
        private Token PrintToken(Token token)
        {
            fm.TOK_LIST += String.Format("\r\n{0,4}", tokenCount++) + token;
            return token;
        }
        
    } // Lexer

} // Compiler namespace