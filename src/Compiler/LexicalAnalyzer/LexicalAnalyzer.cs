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
    public enum Tag
    {
        MP_AND = 256,
        MP_BEGIN, MP_DIV, MP_DO, MP_DOWNTO,
        MP_ELSE, MP_END, MP_FIXED, MP_FLOAT, MP_FOR,
        MP_FUNCTION, MP_IF, MP_INTEGER, MP_MOD, MP_NOT,
        MP_OR, MP_PROCEDURE, MP_PROGRAM, MP_READ, MP_REPEAT,
        MP_THEN, MP_TO, MP_UNTIL, MP_VAR, MP_WHILE,
        MP_WRITE, MP_IDENTIFIER,MP_PERIOD,MP_COMMA,
        MP_SCOLON,MP_LPAREN,MP_RPAREN,MP_EQUAL,
        MP_GTHAN,MP_GEQUAL,MP_LTHAN,MP_LEQUAL,
        MP_NEQUAL,MP_ASSIGN,MP_PLUS,MP_MINUS,
        MP_TIMES, MP_COLON, MP_EOF, MP_INTEGER_LIT,
        MP_FIXED_LIT, MP_FLOAT_LIT, MP_STRING_LIT

        //TODO: add other tags
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
            while (!file.EndOfStream)
            {
                column = 0;
                Word word = GetNextToken();
                Console.WriteLine( word.Tag + "\t" + line + "\t" + column + "\t" + word.lexeme );
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
                words.Add( new Word(lexeme, (Tag)Enum.Parse(typeof(Tag),name,false)));
            }
        }
        /// <summary>
        /// Reads one char at a time from the file
        /// </summary>
        private void readchar()
        {
            column++;
            currentChar = Convert.ToChar( file.Read() );
        }

        /// <summary>
        /// Peeks at the next character without consuming
        /// </summary>
        private char readchar (char c)
        {
            return Convert.ToChar(file.Peek ());
        }
        /// <summary>
        /// Scan to the next character
        /// </summary>
        private void SkipWhiteSpace()
        {
            for (; ; readchar())
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
        public Word GetNextToken()
        {
            SkipWhiteSpace();
                /*
                if (char.IsDigit(peek))
                {

                }
                 * */
                if (char.IsLetter( currentChar ))
                {
                    StringBuilder sb = new StringBuilder();
                    do
                    {
                        sb.Append( currentChar );
                        readchar();
                    } while (char.IsLetterOrDigit( currentChar ));
                    string s = sb.ToString();

                    foreach (Word w in words)
                    {
                        if (w.lexeme.Equals( s ))
                        {
                            return w;
                        }
                    }

                    Word tempWord = new Word( s, Tag.MP_IDENTIFIER );
                    words.Add( tempWord );
                    return tempWord;
                }
                Word word = new Word( "Not yet implemented",Tag.MP_IDENTIFIER );
                currentChar = ' ';
                return word;                
        }
    }

    
}
