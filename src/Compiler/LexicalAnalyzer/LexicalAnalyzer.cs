using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Compiler.LexicalAnalyzer.Library;

namespace Compiler.LexicalAnalyzer
{

    /// <summary>
    /// Handles functions for the Lexical Analyzer
    /// </summary>
    class LexicalAnalyzer
    {
        /// <summary>
        /// Get and set the line number
        /// </summary>
        public int Line
        {
            get;
            private set;
        }
        /// <summary>
        /// Get and set the column number
        /// </summary>
        public int Column
        {
            get;
            private set;
        }
        public int TokenStartColumn
        {
            get;
            private set;
        }
        /// <summary>
        /// Get and set the CurrentCharacter
        /// </summary>
        public char CurrentChar
        {
            get;
            private set;
        }
        /// <summary>
        /// Get and set the NextCharacter
        /// </summary>
        public char NextChar
        {
            get;
            private set;
        }
        private StreamReader file;
        private List<Word> ReservedWords = new List<Word>();
        

        public bool Finished
        {
            get;
            private set;
        }
        public LexicalAnalyzer ()
        {
            Finished = false;
            Column = 1;
            Line = 1;
            LoadTokens("mpTokens.txt");
        }
       
        public void OpenFile(string mpFile)
        {
            file = new StreamReader(mpFile);
            ReadChar();
            //Scan();
        }
       
        /// <summary>
        /// Loads necessary tokens from a file
        /// </summary>
        /// <param name="filename"></param>
        private void LoadTokens (string filename)
        {
            StreamReader tokens = new StreamReader(filename);
            string s, name, lexeme;

            while(!tokens.EndOfStream)
            {
                s = tokens.ReadLine();

                MatchCollection matchTokenName = Regex.Matches(s, "(?<name>[a-zA-Z_]+).+\"(?<lexeme>.+)\"");

                Match token = matchTokenName[0];
                name = token.Groups["name"].ToString();
                lexeme = token.Groups["lexeme"].ToString();

                // Add all of the tokens to the word list
                ReservedWords.Add(new Word(lexeme, (int)(Tags)Enum.Parse(typeof(Tags), name, false)));
            }
        }

        /// <summary>
        /// Reads one char at a time from the file
        /// </summary>
        private void ReadChar ()
        {
            Column++;
            if(!file.EndOfStream)
            {
                CurrentChar = Convert.ToChar(file.Read());
                if(!file.EndOfStream)
                {
                    NextChar = Convert.ToChar(file.Peek());
                }
                else
                {
                    NextChar = (char)3;
                }
                
            }
            else
            {
                CurrentChar = (char)3;
            }
            
        }

        /// <summary>
        /// Peeks at the next character without consuming
        /// </summary>
        private bool ReadChar (char c)
        {
            ReadChar();
            return c.Equals(CurrentChar);
        }

        /// <summary>
        /// Return the token with corresponding line, column, and lexeme information
        /// </summary>
        /// <returns></returns>
        public Token GetNextToken ()
        {
            SkipWhiteSpace();
            TokenStartColumn = Column + 1;
            if(CurrentChar.Equals('{'))
            {
                return ScanComment();                
            }
            switch(CurrentChar)
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
            if(CurrentChar.Equals((char)95) && char.IsLetterOrDigit(NextChar))
            {
                return ScanIdentifier();
            }
            if(Regex.IsMatch(CurrentChar.ToString(), @"^[a-zA-Z]+$"))
            {
                return ScanIdentifier();
            }
            if(char.IsDigit(CurrentChar))
            {
                return ScanNumericLiteral();
            }

            ErrorFound = true;
            ErrorMessage = string.Format("invalid character found on line: {0} starting position: {1}", Line, Column -1);
            Word word = new Word(CurrentChar.ToString(), (int)Tags.MP_ERROR);
            ReadChar();
            return word;
        }
        /// <summary>
        /// Create token for '*'
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanMultiply ()
        {
            ReadChar();
            return new Token((int)Tags.MP_TIMES);
        }
        /// <summary>
        /// Create token for '='
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanEqual ()
        {
            ReadChar();
            return new Token((int)Tags.MP_EQUAL);
        }
        /// <summary>
        /// Create token for '+'
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanPlusOperator ()
        {
            ReadChar();
            return new Token((int)Tags.MP_PLUS);
        }
        /// <summary>
        /// Create token for '-'
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanMinusOperator ()
        {
            ReadChar();
            return new Token((int)Tags.MP_MINUS);
        }
        /// <summary>
        /// Create token for '.'
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanPeriod ()
        {
            ReadChar();
            return new Token((int)Tags.MP_PERIOD);
        }
        /// <summary>
        /// Skips over comments. 
        /// TODO: Prints an error if EOF was found before closing comment
        /// </summary>
        private Token ScanComment ()
        {
            StringBuilder sb = new StringBuilder();
            string s;
            while(!(NextChar == (char)3)) // or EOF6
            {
                ReadChar();
                sb.Append(CurrentChar);
                if (CurrentChar.Equals('}'))
                {
                    ReadChar();
                    return new Token(null);
                }
            }

            ReadChar();

            if(CurrentChar == (char)3)
            {
                ErrorFound = true;
                ErrorMessage = string.Format("run comment found on line: {0} starting position: {1}", Line, TokenStartColumn );

                s = sb.ToString();
                return new Word(s,(int)Tags.MP_RUN_COMMENT);
                
            }

            return new Token(null);
                              
        }
        /// <summary>
        /// Create token for ','
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanComma ()
        {
            ReadChar();
            return new Token(',');
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
                        if(CurrentChar == 13)
                        {
                            state = States.S3;
                            break;
                        }
                        if(CurrentChar.Equals('\''))
                        {
                            state = States.S2;
                            break;
                        }
                        else
                        {
                            sb.Append(CurrentChar);
                            state = States.S1;
                            break;
                        }
                    case States.S2:
                        
                        if(NextChar.Equals('\''))
                        {
                            sb.Append(CurrentChar);
                            ReadChar();
                            sb.Append(CurrentChar);
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
                        ErrorFound = true;
                        ErrorMessage = string.Format("run string found on line: {0} starting position: {1}", Line, TokenStartColumn);
                        finishState = true;
                        s = sb.ToString();
                        return new Word(s,(int)Tags.MP_RUN_STRING);                        
                }
            }
            s = sb.ToString();
            ReadChar();
            return new Word(s, (int)Tags.MP_STRING_LIT);            
        }
        /// <summary>
        /// Create token for EOF
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanEndOfFile ()
        {
            Finished = true;
            return new Token((int)Tags.MP_EOF);
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
                        sb.Append(CurrentChar);
                        state = States.S1;
                        break;
                    case States.S1:
                        while(char.IsDigit(NextChar))
                        {
                            finishState = true;
                            ReadChar();
                            sb.Append(CurrentChar);
                        }
                        if(NextChar.Equals('e') || NextChar.Equals('E'))
                        {
                            finishState = false;
                            state = States.S3;
                            break;
                        }
                        if(NextChar.Equals('.'))
                        {
                            finishState = false;
                            state = States.S2;
                            break;
                        }
                        s = sb.ToString();
                        ReadChar();

                        if(s.Contains("."))
                        {
                            return new Word(s, (int)Tags.MP_FIXED_LIT);
                        }
                        return new Word(s, (int)Tags.MP_INTEGER_LIT);
                    case States.S2:
                        ReadChar();
                        sb.Append(CurrentChar);

                        finishState = false;
                        if(char.IsDigit(NextChar))
                        {
                            state = States.S1;
                            break;
                        }
                        break;
                    case States.S3:
                        finishState = false;
                        ReadChar();
                        sb.Append(CurrentChar);
                        if(char.IsDigit(NextChar))
                        {
                            state = States.S5;
                            break;
                        }
                        if(NextChar.Equals('+') || NextChar.Equals('-'))
                        {
                            state = States.S4;
                            break;
                        }
                        break;
                    case States.S4:
                        ReadChar();
                        sb.Append(CurrentChar);
                        finishState = false;
                        //TODO: this is messy - what if it's not a digit?
                        if(char.IsDigit(NextChar))
                        {
                            state = States.S5;
                            break;
                        }
                        break;
                    case States.S5:                        
                        while(char.IsDigit(NextChar))
                        {
                            ReadChar();
                            finishState = true;
                            sb.Append(CurrentChar);
                        }
                        s = sb.ToString();
                        ReadChar();
                        return new Word(s, (int)Tags.MP_FLOAT_LIT);                   
                }
            }
            ReadChar();
            return new Token(CurrentChar);
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
                        while(char.IsLetterOrDigit(CurrentChar))
                        {
                            sb.Append(CurrentChar);
                            ReadChar();
                        }
                        if(CurrentChar == '_')
                        {
                            finishState = false;
                            state = States.S1;
                        }
                            break;
                    case States.S1:
                            finishState = false;
                            if(char.IsLetterOrDigit(NextChar))
                            {
                                sb.Append(CurrentChar);
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
                if(w.Lexeme.Equals(s,StringComparison.OrdinalIgnoreCase))
                {
                    return w;
                }
            }
            
            return new Word(s, (int)Tags.MP_IDENTIFIER);                           
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
                return new Word(">=", (int)Tags.MP_GEQUAL);
            }
            else
            {
                return new Token('>');
            }
        }
        /// <summary>
        /// Create token for '<' or '<=' or '<>'
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanLessThanOrNotEqual ()
        {
            if(NextChar.Equals('='))
            {
                //Read In Next Character
                ReadChar();
                //Put file pointer to next character
                ReadChar();
                return new Word("<=", (int)Tags.MP_LEQUAL);
            }
            if(NextChar.Equals('>'))
            {
                //Read Next Character
                ReadChar();
                //Put file pointer to next character
                ReadChar();
                return new Word("<>", (int)Tags.MP_NEQUAL);
            }
            else
            {
                ReadChar();
                return new Token('<');
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
                return new Word(":=", (int)Tags.MP_ASSIGN);
            }
            else
            {
                return new Token(':');
            }
        }
        /// <summary>
        /// Create token for ';'
        /// </summary>
        /// <returns></returns>
        private Token ScanSemicolon ()
        {
            ReadChar();
            return new Token(';');
        }
        /// <summary>
        /// Create token for ')'
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanRightParen ()
        {
            ReadChar();
            return new Token(')');
        }
        /// <summary>
        /// Create token for '('
        /// </summary>
        /// <returns></returns>
        private Token ScanLeftParen ()
        {
            ReadChar();
            return new Token('(');
        }
        public bool ErrorFound
        {
            get;
            set;
        }
        public string ErrorMessage
        {
            get;
            set;
        }
        /// <summary>
        /// Scan to the next character
        /// </summary>
        private void SkipWhiteSpace ()
        {
            for(; ; ReadChar())
            {
                if(CurrentChar == 32 || CurrentChar == '\t' || CurrentChar == 10)
                {
                    continue;
                }
                else if(CurrentChar == 13)
                {
                    Line++;
                    Column = 0;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
