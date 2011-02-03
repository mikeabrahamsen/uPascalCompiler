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
        MP_COLON, MP_EOF, MP_INTEGER_LIT,
        MP_FIXED_LIT, MP_FLOAT_LIT, MP_STRING_LIT,
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
        MP_TIMES = 42

        //TODO: add other tags
        // Set non composite tags to numbers
    }
    /// <summary>
    /// Handles functions for the Lexical Analyzer
    /// </summary>
    class LexicalAnalyzer
    {        
        public static int line = 1;
        public static int column = 1;
        char currentChar = ' ';
        private StreamReader file = new StreamReader("program1.mp");
        private List<Word> words = new List<Word>();

        /// <summary>
        /// Sets up the Lexical Analyzer
        /// </summary>
        public LexicalAnalyzer()
        {
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
                column = 0;
                Token token = GetNextToken();
               
                    output = string.Format("{0,-20} {1,-5} {2,-5} {3}",
                      token.Tag, line, column, token.Lexeme);
                
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
                
                words.Add( new Word(lexeme, (int)(Tags)Enum.Parse(typeof(Tags),name,false)));
            }
        }
        /// <summary>
        /// Reads one char at a time from the file
        /// </summary>
        private void ReadChar()
        {
            column++;
            currentChar = Convert.ToChar( file.Read() );
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
        /// Scan to the next character
        /// </summary>
        private void SkipWhiteSpace()
        {
            for (; ; ReadChar())
            {
                if (currentChar == ' ' || currentChar == '\t')
                {
                    continue;
                }
                else if (currentChar == '\n')
                {
                    line++;
                }
                else
                {
                    break;
                }
            }
        }
        /// <summary>
        /// Return the token with corresponding line, column, and lexeme information
        /// </summary>
        /// <returns></returns>
        public Token GetNextToken()
        {
            SkipWhiteSpace();
                /*
                if (char.IsDigit(peek))
                {

                }
                 * */
            switch(currentChar)
            {
                case '(':
                    ReadChar();
                    return new Token('(');
                case ')':
                    ReadChar();
                    return new Token(')');
                case ';':
                    ReadChar();
                    return new Token(';');
                case ':':
                    if(ReadChar('='))
                    {
                        return new Word(":=", (int)Tags.MP_ASSIGN);
                    }
                    else
                    {
                        return new Token('=');
                    }
                default:
                    break;
            }
            if (char.IsLetter( currentChar ))
            {
                StringBuilder sb = new StringBuilder();
                do
                {
                    sb.Append( currentChar );
                    ReadChar();
                } while (char.IsLetterOrDigit( currentChar ));
                string s = sb.ToString();

                foreach (Word w in words)
                {
                    if (w.Lexeme.Equals( s ))
                    {
                        return w;
                    }
                }

                Word tempWord = new Word( s, (int)Tags.MP_IDENTIFIER );
                words.Add( tempWord );
                return tempWord;
            }
           Word word = new Word( "Not yet implemented",(int)Tags.MP_IDENTIFIER );
            currentChar = ' ';
            return word;                
        }
    }

    
}
