using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.LexicalAnalyzer
{
    /// <summary>
    /// A word is made up of a string and a tag
    /// The string will be the lexeme of the token found
    /// </summary>
    class Word : Token
    {
        public String lexeme = string.Empty;
        
        public Word(string s, Tag tag) : base(tag)
        {
            lexeme = s;
        }

        public override string ToString()
        {
            return lexeme;
        }


    }
}
