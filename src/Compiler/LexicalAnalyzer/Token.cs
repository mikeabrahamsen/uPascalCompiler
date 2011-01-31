﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.LexicalAnalyzer
{
    class Token
    {
        
        public Token(Tag t)
        {
            Tag = t;
        }

        public Tag Tag
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Tag.ToString();
        }
    }
}
