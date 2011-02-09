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
        private bool finished = false;

        /// <summary>
        /// Sets up the Lexical Analyzer
        /// </summary>
        public LexicalAnalyzer (String mpFile)
        {
            Column = 1;
            Line = 1;
            //TODO: place in try catch - wont need once we do command line args. this will be moved
            LoadTokens("mpTokens.txt");
            file = new StreamReader(mpFile);
            ReadChar();
            Scan();
        }

        /// <summary>
        /// Scans the file gathering token information
        /// </summary>
        private void Scan ()
        {
            string output;
            Token token;           
            
            while(!finished)
            {
                token = GetNextToken();
                output = string.Format("{0,-20} {1,-5} {2,-5} {3}",
                    token.Tag, Line, (Column - token.Lexeme.Length) - 1, token.Lexeme);
                Console.WriteLine(output); 
            }            
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
        private Token GetNextToken ()
        {
            SkipWhiteSpace();
            if(CurrentChar.Equals('{'))
            {
                ScanComment();
                SkipWhiteSpace();
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
            if(CurrentChar.Equals((char)95) && char.IsLetterOrDigit(NextChar))
            {
                return ScanIdentifier();
            }
            if(char.IsLetter(CurrentChar))
            {
                return ScanIdentifier();
            }
            if(char.IsDigit(CurrentChar))
            {
                return ScanNumericLiteral();
            }

            Word word = new Word("Not yet implemented", (int)Tags.MP_IDENTIFIER);
            CurrentChar = ' ';
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
        private void ScanComment ()
        {

            while(!(NextChar == (char)3)) // or EOF6
            {
                ReadChar();

                if (CurrentChar.Equals('}'))
                {
                    ReadChar();
                    return;
                }
            }

            ReadChar();

            if (CurrentChar == (char)3)
                Console.WriteLine("MP_RUN_COMMENT..... LINE.... COL.... of start of comment.");               
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
            while(!finishState)
            {
                switch(state)
                {
                    case States.S0:
                        state = States.S1;
                        break;
                    case States.S1:                        
                        ReadChar();
                        if(CurrentChar.Equals('\n'))
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
                        Console.WriteLine("MP_RUN_STRING error...");
                        finishState = true;
                        return new Token((int)Tags.MP_RUN_STRING);                        
                }
            }
            string s = sb.ToString();
            ReadChar();
            return new Word(s, (int)Tags.MP_STRING_LIT);            
        }
        /// <summary>
        /// Create token for EOF
        /// </summary>
        /// <returns>Token</returns>
        private Token ScanEndOfFile ()
        {
            finished = true;
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
            StringBuilder sb = new StringBuilder();
            while(char.IsLetterOrDigit(NextChar) || NextChar == (char)95) 
            {                  
                sb.Append(CurrentChar);
                ReadChar();              
            }
            sb.Append(CurrentChar);
            string s = sb.ToString();
            ReadChar();
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
