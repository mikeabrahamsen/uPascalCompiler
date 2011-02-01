using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Compiler.LexicalAnalyzer
{
    public enum Tag
    {
        MP_AND = 256,
        MP_BEGIN, MP_DIV, MP_DO, MP_DOWNTO,
        MP_ELSE, MP_END, MP_FIXED, MP_FLOAT, MP_FOR,
        MP_FUNCTION, MP_IF, MP_INTEGER, MP_MOD, MP_NOT,
        MP_OR, MP_PROCEDURE, MP_PROGRAM, MP_READ, MP_REPEAT,
        MP_THEN, MP_TO, MP_UNTIL, MP_VAR, MP_WHILE,
        MP_WRITE, MP_IDENTIFIER,MP_PERIOD,MP_COMMA,
MP_SCOLON,MP_LPAREN,MP_RPAREN,
MP_EQUAL,
MP_GTHAN,
MP_GEQUAL,
MP_LTHAN,
MP_LEQUAL,
MP_NEQUAL,
MP_ASSIGN,
MP_PLUS,MP_MINUS,
MP_TIMES ,
MP_COLON 
        //TODO: add other tags
    }
    class LexicalAnalyzer
    {        
        public static int line = 1;
        public static int column = 1;
        char peek = ' ';
        private StreamReader file = new StreamReader("program1.mp");
        private List<Word> words = new List<Word>();

        public LexicalAnalyzer()
        {
            //TODO: place in try catch
            LoadTokens( "mpTokens.txt" );
   
            while (!file.EndOfStream)
            {
                column = 0;
               Word word =  GetNextToken();
               Console.WriteLine( word.Tag + "\t" + line + "\t" + column + "\t" + word.lexeme );
            }
        }
        public void LoadTokens(string filename)
        {
            StreamReader tokens = new StreamReader( filename );
            string s,name,lexeme;
            
            while (!tokens.EndOfStream)
            {
                s = tokens.ReadLine();
                // grab the name and the lexeme from the file
                // TODO: fix regex to group what is between the quotes but not including
                MatchCollection matchTokenName = Regex.Matches( s, "(?<x>[a-zA-Z_]+)");
                MatchCollection matchTokenLexeme = Regex.Matches( s, "(?<x>\".+\")" );
                
                name = matchTokenName[0].Value;
                lexeme = matchTokenLexeme[0].Value;
                //grab everything between the quotes quotes
                lexeme = lexeme.Substring( 1, lexeme.Length -2 );

                words.Add( new Word(lexeme, (Tag)Enum.Parse(typeof(Tag),name,false)));
            }
        }
        private void readchar()
        {
            column++;
            peek = Convert.ToChar( file.Read() );
        }
        private void SkipWhiteSpace()
        {
            for (; ; readchar())
            {
                if (peek == ' ' || peek == '\t')
                {
                    continue;
                }
                else if (peek == '\n')
                {
                    line++;
                }
                else
                {
                    break;
                }
            }
        }
        public Word GetNextToken()
        {
            SkipWhiteSpace();
                /*
                if (char.IsDigit(peek))
                {

                }
                 * */
                if (char.IsLetter( peek ))
                {
                    StringBuilder sb = new StringBuilder();
                    do
                    {
                        sb.Append( peek );
                        readchar();
                    } while (char.IsLetterOrDigit( peek ));
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
                peek = ' ';
                return word;                
        }
    }

    
}
