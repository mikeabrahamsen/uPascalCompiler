using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
        MP_WRITE, MP_IDENTIFIER
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
            //TODO: Load These From File
            words.Add( new Word("Program", Tag.MP_PROGRAM) );
            words.Add( new Word("begin",Tag.MP_BEGIN) );
            words.Add( new Word("end", Tag.MP_END) );
            words.Add( new Word("and", Tag.MP_AND) );            
            words.Add( new Word("div", Tag.MP_DIV) );
            words.Add(new Word("do",Tag.MP_DO));
            words.Add(new Word("downto",Tag.MP_DOWNTO));
            words.Add(new Word("else",Tag.MP_ELSE));
            words.Add(new Word("fixed",Tag.MP_FIXED));
            words.Add(new Word("float",Tag.MP_FLOAT));
            words.Add(new Word("for",Tag.MP_FOR));
            words.Add(new Word("function",Tag.MP_FUNCTION));
            words.Add(new Word("if",Tag.MP_IF));
            words.Add(new Word("integer",Tag.MP_INTEGER));
            words.Add(new Word("mod",Tag.MP_MOD));
            words.Add(new Word("not",Tag.MP_NOT));
            words.Add(new Word("or",Tag.MP_OR));
            words.Add(new Word("procedure",Tag.MP_PROCEDURE));

            while (!file.EndOfStream)
            {
                column = 0;
               Word word =  Scan();
               Console.WriteLine( word.Tag + "\t" + line + "\t" + column + "\t" + word.lexeme );
            }
        }

        private void readchar()
        {
            column++;
            peek = Convert.ToChar( file.Read() );
        }
        
        public Word Scan()
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
                Word word = new Word( "Nothing",Tag.MP_IDENTIFIER );
                peek = ' ';
                return word;                
        }
    }

    
}
