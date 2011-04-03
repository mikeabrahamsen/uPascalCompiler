using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;
namespace Compiler.SymAnalyzer
{
    class LiteralRecord
    {
        public string lexeme
        {
            get;
            set;
        }
        public VariableType type
        {
            get;
            set;
        }

    }
}
