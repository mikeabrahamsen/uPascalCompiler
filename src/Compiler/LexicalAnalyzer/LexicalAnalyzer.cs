using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Compiler.LexicalAnalyzer
{
    /// <summary>
    /// Enum values for all of the tokens
    /// </summary>
    public enum Tags
    {
        MP_AND = 256,
        MP_BEGIN, MP_DIV, MP_DO, MP_DOWNTO,
        MP_ELSE, MP_END, MP_FIXED, MP_FLOAT, MP_FOR,
        MP_FUNCTION, MP_IF, MP_INTEGER, MP_MOD, MP_NOT,
        MP_OR, MP_PROCEDURE, MP_PROGRAM, MP_READ, MP_REPEAT,
        MP_THEN, MP_TO, MP_UNTIL, MP_VAR, MP_WHILE,
        MP_WRITE, MP_WRITELN, MP_IDENTIFIER,
        MP_GEQUAL, MP_LEQUAL,
        MP_NEQUAL, MP_ASSIGN,
        MP_INTEGER_LIT,
        MP_FIXED_LIT, MP_FLOAT_LIT, MP_STRING_LIT,
        MP_COLON = 58,
        MP_PERIOD = 46,
        MP_COMMA = 44,
        MP_SCOLON = 59,
        MP_LPAREN = 40,
        MP_RPAREN = 41,
        MP_EQUAL = 61,
        MP_GTHAN = 62,
        MP_LTHAN = 60,
        MP_PLUS = 43,
        MP_MINUS = 45,
        MP_TIMES = 42,
        MP_EOF = 3

        //TODO: add other tags
        // Set non composite tags to numbers
    }
    /// <summary>
    /// Handles functions for the Lexical Analyzer
    /// </summary>
    class LexicalAnalyzer
    {
        public int Line
        {
            get;
            private set;
        }
        public int Column
        {
            get;
            private set;
        }
        public char CurrentChar
        {
            get;
            set;
        }
        public char NextChar
        {
            get;
            set;
        }
        private StreamReader file = new StreamReader("Program1.mp");
        private List<Word> ReservedWords = new List<Word>();
        private bool finished = false;

        /// <summary>
        /// Sets up the Lexical Analyzer
        /// </summary>
        public LexicalAnalyzer ()
        {
            Column = 1;
            Line = 1;
            //TODO: place in try catch
            LoadTokens("mpTokens.txt");
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

            if(!file.EndOfStream)
            {
                CurrentChar = Convert.ToChar(file.Read());
                if(!file.EndOfStream)
                {
                    NextChar = Convert.ToChar(file.Peek());
                }
                else
                {
                    NextChar = '\0';
                }
                Column++;
            }
            else
            {
                finished = true;
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
                    return ScanLessThan();
                case '>':
                    return ScanGreaterThan();
                case ',':
                    return ScanComma();
                case '\'':
                    return ScanStringLiteral();
                case '.':
                    return ScanPeriod();
                case '\0':
                    return ScanEndOfFile();
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

        private Token ScanPeriod ()
        {
            ReadChar();
            return new Token((int)Tags.MP_PERIOD);
        }

        private void ScanComment ()
        {
            while(!CurrentChar.Equals('}'))
            {
                ReadChar();
            }
            ReadChar();
        }

        private Token ScanComma ()
        {
            ReadChar();
            return new Token(',');
        }
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
                }
            }
            string s = sb.ToString();
            ReadChar();
            return new Word(s, (int)Tags.MP_STRING_LIT);            
        }

        private Token ScanEndOfFile ()
        {
            ReadChar();
            return new Token((int)Tags.MP_EOF);
        }
        private enum States
        {
            S0 = 0,
            S1, S2, S3, S4, S5, S6, S7

        }
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
                if(w.Lexeme.Equals(s))
                {
                    return w;
                }
            }
            
            return new Word(s, (int)Tags.MP_IDENTIFIER);                           
        }

        private Token ScanGreaterThan ()
        {
            if(ReadChar('='))
            {
                ReadChar();
                return new Word(">=", (int)Tags.MP_GEQUAL);
            }
            else
            {
                ReadChar();
                return new Token('>');
            }
        }

        private Token ScanLessThan ()
        {
            if(ReadChar('='))
            {
                ReadChar();
                return new Word("<=", (int)Tags.MP_LEQUAL);
            }
            else
            {
                ReadChar();
                return new Token('<');
            }
        }

        private Token ScanColonOrAssignOp ()
        {
            if(ReadChar('='))
            {
                ReadChar();
                return new Word(":=", (int)Tags.MP_ASSIGN);
            }
            else
            {
                ReadChar();
                return new Token(':');
            }
        }

        private Token ScanSemicolon ()
        {
            ReadChar();
            return new Token(';');
        }

        private Token ScanRightParen ()
        {
            ReadChar();
            return new Token(')');
        }

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
