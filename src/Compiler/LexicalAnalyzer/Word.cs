using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.Lexer
{
    /// <summary>
    /// A word is made up of a string and a tag
    /// The string will be the lexeme of the token found
    /// </summary>
    class Word : Token
    {
        
        public Word(string s, int tag,int line,int column) : base(tag,line,column)
        {
            lexeme = s;
        }
        
        public override string ToString()
        {
            return lexeme;
        }


    }
}
