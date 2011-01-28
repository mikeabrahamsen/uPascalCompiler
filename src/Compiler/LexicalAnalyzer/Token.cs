using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.LexicalAnalyzer
{
    class Token
    {
        
        public Token(int t)
        {
            Tag = t;
        }

        public int Tag
        {
            get;
            set;
        }

        public string toString()
        {
            return Tag.ToString();
        }
    }
}
