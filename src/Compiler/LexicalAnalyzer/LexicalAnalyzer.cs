using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Compiler.Library;
using System.Reflection;
using Compiler.Parse;


namespace Compiler.Lexer
{

    /// <summary>
    /// Handles functions for the Lexical Analyzer
    /// </summary>
    class LexicalAnalyzer
    {
        /// <summary>
        /// Get and set the line number
        /// </summary>
        public int line
        {
            get;
            private set;
        }
        /// <summary>
        /// Get and set the column number
        /// </summary>
        public int column
        {
            get;
            private set;
        }
        /// <summary>
        /// gets and sets the start column of a token
        /// </summary>
        public int tokenStartColumn
        {
            get;
            private set;
        }
        /// <summary>
        /// Get and set the CurrentCharacter
        /// </summary>
        public char currentChar
        {
            get;
            private set;
        }
        /// <summary>
        /// Get and set the NextCharacter
        /// </summary>
        public char nextChar
        {
            get;
            private set;
        }
        private StreamReader file;
        private List<Word> ReservedWords = new List<Word>();
        
        /// <summary>
        /// Constructor for LexicalAnalyzer
        /// Sets line and column and Loads tokens from file
        /// </summary>
        public LexicalAnalyzer ()
        {
            column = 1;
            line = 1;
            LoadTokens("mpTokens.txt");
        }
       
        public void OpenFile(string mpFile)
        {
            file = new StreamReader(mpFile);
            ReadChar();
        }
       
        /// <summary>
        /// Loads necessary tokens from a file
        /// </summary>
        /// <param name="filename"></param>
        private void LoadTokens (string filename)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            try
            {
              StreamReader tokens = new StreamReader(assembly.GetManifestResourceStream("Compiler.LexicalAnalyzer.mpTokens.txt"));

              //StreamReader tokens = new StreamReader(stream);
              string s, name, lexeme;

              while(!tokens.EndOfStream)
              {
                  s = tokens.ReadLine();

                  MatchCollection matchTokenName = Regex.Matches(s, "(?<name>[a-zA-Z_]+).+\"(?<lexeme>.+)\"");

                  Match token = matchTokenName[0];
                  name = token.Groups["name"].ToString();
                  lexeme = token.Groups["lexeme"].ToString();

                  // Add all of the tokens to the word list
                  ReservedWords.Add(new Word(lexeme, (int)(Tags)Enum.Parse(typeof(Tags), name, false), line, column));
              }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            
        }

        /// <summary>
        /// 
        /// Reads one char at a time from the file
        /// </summary>
        private void ReadChar ()
        {
            column++;
            if(!file.EndOfStream)
            {
                currentChar = Convert.ToChar(file.Read());
                if(!file.EndOfStream)
                {                    
                    nextChar = Convert.ToChar(file.Peek());
                }
                else
                {
                    nextChar = (char)3;
                }                
            }
            else
            {
                currentChar = (char)3;
            }            
        }

        /// <summary>
        /// Peeks at the next character without consuming
        /// </summary>
        private bool ReadChar (char c)
        {
            ReadChar();
            return c.Equals(currentChar);
        }

        /// <summary>
        /// Return the token with corresponding line, column, and lexeme information
        /// </summary>
        /// <returns></returns>
        public Token GetNextToken ()
        {
            tokenStartColumn = column - 1;
            SkipWhiteSpace();
            if(currentChar.Equals('{'))
            {
                return ScanComment();                
            }
            switch(currentChar)
            {
                case '(':
                    return ScanLeftParen();
                case ')':
                    return ScanRightParen();
                case ';':
                    return ScanSemicolon();
                case ':':
                    return ScanColonOrAssignOp();
                case '<':
                    return ScanLessThanOrNotEqual();
                case '>':
                    return ScanGreaterThan();
                case ',':
                    return ScanComma();
                case '\'':
                    return ScanStringLiteral();
                case '.':
                    return ScanPeriod();
                case '+':
                    return ScanPlusOperator();
                case '-':
                    return ScanMinusOperator();
                case '=':
                    return ScanEqual();
                case '*':
                    return ScanMultiply();
                case (char)3:
                    return ScanEndOfFile();
            }
            //If currentchar is an underscore and next is a letter or digit
            if(currentChar.Equals((char)95) && char.IsLetterOrDigit(nextChar))
            {
                return ScanIdentifier();
            }
            if(Regex.IsMatch(currentChar.ToString(), @"^[a-zA-Z]+$"))
            {
                return ScanIdentifier();
            }
            if(char.IsDigit(currentChar))
            {
                return ScanNumericLiteral();
            }

            throw new SyntaxException("invalid character found on line: " + line + "starting position: " + (column - 1));
        }
        /// <summary>
        /// Create token for '*'
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanMultiply ()
        {
            ReadChar();
            return new Token((int)Tags.MP_TIMES,line,column);
        }
        /// <summary>
        /// Create token for '='
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanEqual ()
        {
            ReadChar();
            return new Token((int)Tags.MP_EQUAL, line, column);
        }
        /// <summary>
        /// Create token for '+'
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanPlusOperator ()
        {
            ReadChar();
            return new Token((int)Tags.MP_PLUS, line, column);
        }
        /// <summary>
        /// Create token for '-'
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanMinusOperator ()
        {
            ReadChar();
            return new Token((int)Tags.MP_MINUS, line, column);
        }
        /// <summary>
        /// Create token for '.'
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanPeriod ()
        {
            ReadChar();
            return new Token((int)Tags.MP_PERIOD, line, column);
        }
        /// <summary>
        /// Skips over comments. 
        /// </summary>
        private Token ScanComment ()
        {
            StringBuilder sb = new StringBuilder();
            int count = 1;
            while(!(nextChar == (char)3)) // or EOF6
            {
                
                ReadChar();
                sb.Append(currentChar);
                if (currentChar.Equals('}'))
                {
                    ReadChar();
                    return new Token();
                }
                count++;
            }
            ReadChar();

            if(currentChar == (char)3)
            {
                throw new SyntaxException("run comment found on line: " + line + " at position " + (column - count));                                
            }
            return new Token();
                              
        }
        /// <summary>
        /// Create token for ','
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanComma ()
        {
            ReadChar();
            return new Token(',', line, column);
        }
        /// <summary>
        /// Create token(word) for a string
        /// </summary>
        /// <returns>Word</returns>
        private Token ScanStringLiteral ()
        {
            bool finishState = false;
            States state;
            state = States.S0;
            StringBuilder sb = new StringBuilder();
            string s;
            while(!finishState)
            {
                switch(state)
                {
                    case States.S0:
                        state = States.S1;
                        break;
                    case States.S1:                        
                        ReadChar();
                        if(currentChar == 13)
                        {
                            state = States.S3;
                            break;
                        }
                        if(currentChar.Equals('\''))
                        {
                            state = States.S2;
                            break;
                        }
                        else
                        {
                            sb.Append(currentChar);
                            state = States.S1;
                            break;
                        }
                    case States.S2:
                        
                        if(nextChar.Equals('\''))
                        {
                            ReadChar();
                            sb.Append(currentChar);
                            state = States.S1;
                            break;
                        }
                        else
                        {
                            finishState = true;
                            break;
                        }
                    case States.S3:
                        //return error token
                        finishState = true;
                        throw new SyntaxException("run comment string on line: " + line + " starting position: " +tokenStartColumn);                                                                    
                }
            }
            s = sb.ToString();
            ReadChar();
            return new Word(s, (int)Tags.MP_STRING_LIT, line, column);            
        }
        /// <summary>
        /// Create token for EOF
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanEndOfFile ()
        {
            return new Token((int)Tags.MP_EOF, line, column);
        }
        /// <summary>
        /// Create token for Numeric Literal
        /// </summary>
        /// <returns>Word</returns>
        private Token ScanNumericLiteral ()
        {
            bool finishState = false;
            States state = States.S0;
            string s;
            StringBuilder sb = new StringBuilder();

            while(!finishState)
            {
                switch(state)
                {
                    case States.S0:
                        sb.Append(currentChar);
                        state = States.S1;
                        break;
                    case States.S1:
                        while(char.IsDigit(nextChar))
                        {
                            finishState = true;
                            ReadChar();
                            sb.Append(currentChar);
                        }
                        if(nextChar.Equals('e') || nextChar.Equals('E'))
                        {
                            finishState = false;
                            state = States.S3;
                            break;
                        }
                        if(nextChar.Equals('.'))
                        {
                            finishState = false;
                            state = States.S2;
                            break;
                        }
                        s = sb.ToString();
                        ReadChar();

                        if(s.Contains("."))
                        {
                            return new Word(s, (int)Tags.MP_FIXED_LIT, line, column);
                        }
                        return new Word(s, (int)Tags.MP_INTEGER_LIT, line, column);
                    case States.S2:
                        ReadChar();
                        sb.Append(currentChar);

                        finishState = false;
                        if(char.IsDigit(nextChar))
                        {
                            state = States.S1;
                            break;
                        }
                        break;
                    case States.S3:
                        finishState = false;
                        ReadChar();
                        sb.Append(currentChar);
                        if(char.IsDigit(nextChar))
                        {
                            state = States.S5;
                            break;
                        }
                        if(nextChar.Equals('+') || nextChar.Equals('-'))
                        {
                            state = States.S4;
                            break;
                        }
                        break;
                    case States.S4:
                        ReadChar();
                        sb.Append(currentChar);
                        finishState = false;
                        //TODO: this is messy - what if it's not a digit?
                        if(char.IsDigit(nextChar))
                        {
                            state = States.S5;
                            break;
                        }
                        break;
                    case States.S5:                        
                        while(char.IsDigit(nextChar))
                        {
                            ReadChar();
                            finishState = true;
                            sb.Append(currentChar);
                        }
                        s = sb.ToString();
                        ReadChar();
                        return new Word(s, (int)Tags.MP_FLOAT_LIT, line, column);                   
                }
            }
            ReadChar();
            return new Token(currentChar, line, column);
        }
        /// <summary>
        /// Create token for ID
        /// If the ID is a reseverd word, returns that reserved word token
        /// </summary>
        /// <returns>Word</returns>
        private Token ScanIdentifier ()
        {
            bool finishState = false;
            States state =States.S0;
            StringBuilder sb = new StringBuilder();
            while(!finishState)
            {
                switch(state)
                {
                    case States.S0:

                        finishState = true;
                        while(char.IsLetterOrDigit(currentChar))
                        {
                            sb.Append(currentChar);
                            ReadChar();
                        }
                        if(currentChar == '_')
                        {
                            finishState = false;
                            state = States.S1;
                        }
                            break;
                    case States.S1:
                            finishState = false;
                            if(char.IsLetterOrDigit(nextChar))
                            {
                                sb.Append(currentChar);
                                ReadChar();

                                state = States.S0;
                            }
                            else
                            {
                                finishState = true;
                            }
                            
                            break;
                }
            }
            
            string s = sb.ToString();
            foreach(Word w in ReservedWords)
            {  
                if(w.lexeme.Equals(s,StringComparison.OrdinalIgnoreCase))
                {
                    return w;
                }
            }

            return new Word(s, (int)Tags.MP_IDENTIFIER, line, column);                           
        }
        /// <summary>
        /// Create token for '>' or '>='
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanGreaterThan ()
        {
            if(ReadChar('='))
            {
                ReadChar();
                return new Word(">=", (int)Tags.MP_GEQUAL, line, column);
            }
            else
            {
                return new Token('>', line, column);
            }
        }
        /// <summary>
        /// Create token for '<' or '<=' or '<>'
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanLessThanOrNotEqual ()
        {
            if(nextChar.Equals('='))
            {
                //Read In Next Character
                ReadChar();
                //Put file pointer to next character
                ReadChar();
                return new Word("<=", (int)Tags.MP_LEQUAL, line, column);
            }
            if(nextChar.Equals('>'))
            {
                //Read Next Character
                ReadChar();
                //Put file pointer to next character
                ReadChar();
                return new Word("<>", (int)Tags.MP_NEQUAL, line, column);
            }
            else
            {
                ReadChar();
                return new Token('<', line, column);
            }
        }
        /// <summary>
        /// Create token for ':' or ':='
        /// </summary>
        /// <returns></returns>
        private Token ScanColonOrAssignOp ()
        {
            if(ReadChar('='))
            {
                ReadChar();
                return new Word(":=", (int)Tags.MP_ASSIGN, line, column);
            }
            else
            {
                return new Token(':', line, column);
            }
        }
        /// <summary>
        /// Create token for ';'
        /// </summary>
        /// <returns></returns>
        private Token ScanSemicolon ()
        {
            ReadChar();
            return new Token(';', line, column);
        }
        /// <summary>
        /// Create token for ')'
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanRightParen ()
        {
            ReadChar();
            return new Token(')', line, column);
        }
        /// <summary>
        /// Create token for '('
        /// </summary>
        /// <returns></returns>
        private Token ScanLeftParen ()
        {
            ReadChar();
            return new Token('(', line, column);
        }
        
        /// <summary>
        /// Scan to the next character
        /// </summary>
        private void SkipWhiteSpace ()
        {
            for(; ; ReadChar())
            {
                if(currentChar == 32 || currentChar == '\t' || currentChar == 10)
                {
                    continue;
                }
                else if(currentChar == 13)
                {
                    line++;
                    column = 0;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
