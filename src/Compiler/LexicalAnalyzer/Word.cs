using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.LexicalAnalyzer
{
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
