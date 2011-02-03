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
        MP_WRITE,MP_WRITELN, MP_IDENTIFIER,
        MP_GEQUAL,MP_LEQUAL,
        MP_NEQUAL,MP_ASSIGN,
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
        
        char currentChar = ' ';
        
        private StreamReader file = new StreamReader("Program1.mp");
        private List<Word> ReservedWords = new List<Word>();

        /// <summary>
        /// Sets up the Lexical Analyzer
        /// </summary>
        public LexicalAnalyzer()
        {
            Column = 1;
            Line = 1;
            //TODO: place in try catch
            LoadTokens( "mpTokens.txt" );
            Scan();
           
        }

        /// <summary>
        /// Scans the file gathering token information
        /// </summary>
        private void Scan()
        {
            string output;
            while (!file.EndOfStream)
            {
                Column = 0;
                Token token = GetNextToken();
               
                    output = string.Format("{0,-20} {1,-5} {2,-5} {3}",
                      token.Tag, Line, Column, token.Lexeme);
                
                Console.WriteLine(output);
            }
        }
        /// <summary>
        /// Loads necessary tokens from a file
        /// </summary>
        /// <param name="filename"></param>
        private void LoadTokens(string filename)
        {
            StreamReader tokens = new StreamReader( filename );
            string s,name,lexeme;
            
            while (!tokens.EndOfStream)
            {
                s = tokens.ReadLine();
                               
                MatchCollection matchTokenName = Regex.Matches( s, "(?<name>[a-zA-Z_]+).+\"(?<lexeme>.+)\"");
                
                Match token = matchTokenName[0];
                name = token.Groups["name"].ToString ();
                lexeme = token.Groups["lexeme"].ToString ();
                
                // Add all of the tokens to the word list
                
                ReservedWords.Add( new Word(lexeme, (int)(Tags)Enum.Parse(typeof(Tags),name,false)));
            }
        }
       
        /// <summary>
        /// Reads one char at a time from the file
        /// </summary>
        private void ReadChar()
        {
            Column++;
            currentChar = Convert.ToChar(file.Read());
        }

        /// <summary>
        /// Peeks at the next character without consuming
        /// </summary>
        private bool PeekChar (char c)
        {
            ReadChar();
   
            return c.Equals(currentChar);
        }

        private char Peek()
        {
            return Convert.ToChar( file.Peek() );
        }
        
        /// <summary>
        /// Return the token with corresponding line, column, and lexeme information
        /// </summary>
        /// <returns></returns>
        private Token GetNextToken()
        {
            SkipWhiteSpace();

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
                    return ScanLessThan();                    
                case '>':
                    return ScanGreaterThan();
                case ',':
                    return ScanComma();
                case '\'':
                    return ScanStringLiteral();
            }
            if (char.IsLetter( currentChar ))
            {
                return ScanIdentifier();                
            }
            
            //This does not work, it never gets to the first '9' within the Program1.mp hello world line we added
            // the problem is that currentChar never hits the 9? I think it may be a problem with reading the
            // char value as an ascii character? 9 is tab... so that would explain why it never shows up before '!'

            //The 9 in the program should show up in the string literal.
            if (char.IsDigit(currentChar))
            {
                ReadChar();
                return ScanNumericLiteral();
            }
            
           Word word = new Word( "Not yet implemented",(int)Tags.MP_IDENTIFIER );
            currentChar = ' ';
            return word;                
        }

        private Token ScanComma()
        {
            ReadChar();
            return new Token(',');
        }
        private Token ScanStringLiteral()
        {
            bool finishState = false;
            States state;
            char next;
            state = States.S0;
            StringBuilder sb = new StringBuilder();
            while(!finishState)
            {
                switch (state)
                {
                    case States.S0:
                        //start state, there is now a " ' " in the charbuffer
                        state = States.S1;
                        break;
                    case States.S1:
                        //read until we get a " ' "
                        ReadChar();
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
                        next = Peek();
                        if(next.Equals('\''))
                        {
                            sb.Append(currentChar);
                            ReadChar();
                            sb.Append(currentChar);
                            state = States.S1;
                            break;
                        }
                        else
                        {
                            ReadChar();
                            finishState = true;
                            break;
                        }                        
                }
            }
            string s = sb.ToString();
                       

            Word tempWord = new Word(s, (int)Tags.MP_STRING_LIT);
            ReservedWords.Add(tempWord);
            return tempWord;
        }

        private Token ScanEndOfFile()
        {
            return new Token((int)Tags.MP_EOF);
        }
        private enum States
        {
            S0 = 0,
            S1,S2,S3,S4,S5

        }
        private Token ScanNumericLiteral()
        {
            
            char next = Peek();
            StringBuilder sb = new StringBuilder();
            sb.Append(currentChar);
            
            if (next.Equals('.'))
            {
                ReadChar();
                sb.Append(currentChar);
                next = Peek();

                do{
                    ReadChar();
                    sb.Append(currentChar); 
                    next = Peek();
                } while (next >= '0' && next <= '9');

                string s = sb.ToString();

               if(!(next.Equals('e') || next.Equals('E')))
               {
                   return new Word(s, (int)Tags.MP_FIXED_LIT);
               }

            }

            if (next >= '0' && next <= '9')
            {
                do
                {
                    
                    ReadChar();
                    sb.Append(currentChar);
                    next = Peek();
                } while (next >= '0' && next <= '9');
                
                string s = sb.ToString();

                if (!(next.Equals('e') || next.Equals('E') 
                    ))
                {
                    return new Word(s, (int)Tags.MP_INTEGER_LIT);
                }               
            }

            if (next.Equals('e') || next.Equals('E'))
            {
                ReadChar();
                next = Peek();
                if (next >= '0' && next <= '9')
                {
                    ReadChar();
                    sb.Append(currentChar);
                    string s = sb.ToString();
                    return new Word(s, (int)Tags.MP_FLOAT_LIT);
                   
                }

                if(next.Equals('+') || next.Equals('-'))
                {
                    //plusminus
                    ReadChar();
                    sb.Append(currentChar);
                    next = Peek();

                    //digits
                    do
                    {
                        ReadChar();
                        sb.Append(currentChar);
                        next = Peek();
                    } while (next <= '0' && next >= '9');
                }
                string returnMe = sb.ToString();
                return new Word(returnMe, (int)Tags.MP_FLOAT_LIT);
            }



            return new Token((int)Tags.MP_AND);
        }

        private Token ScanIdentifier()
        {
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Append(currentChar);
                ReadChar();
            } while (char.IsLetterOrDigit(currentChar) || currentChar == (char)95);
            string s = sb.ToString();

            foreach (Word w in ReservedWords)
            {
                if (w.Lexeme.Equals(s))
                {
                    return w;
                }
            }

            Word tempWord = new Word(s, (int)Tags.MP_IDENTIFIER);
            ReservedWords.Add(tempWord);
            return tempWord;
        }

        private Token ScanGreaterThan()
        {
            if (PeekChar('='))
            {
                return new Word(">=", (int)Tags.MP_GEQUAL);
            }
            else
            {
                return new Token('>');
            }  
        }

        private Token ScanLessThan()
        {
            if (PeekChar('='))
            {
                return new Word("<=", (int)Tags.MP_LEQUAL);
            }
            else
            {
                return new Token('<');
            }
        }

        private Token ScanColonOrAssignOp()
        {
            if (PeekChar('='))
            {
                return new Word(":=", (int)Tags.MP_ASSIGN);
            }
            else
            {
                return new Token(':');
            }
        }

        private Token ScanSemicolon()
        {
            ReadChar();
            return new Token(';');
        }

        private Token ScanRightParen()
        {
            ReadChar();
            return new Token(')');
        }

        private Token ScanLeftParen()
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
                if(currentChar == 32 || currentChar == '\t')
                {
                    continue;
                }
                else if(currentChar == 10 || currentChar == 13)
                {
                    Line++;
                }
                else
                {
                    break;
                }
            }
        }
    }

    
}
