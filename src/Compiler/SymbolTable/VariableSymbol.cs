using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.LexicalAnalyzer;
using Compiler.Library;

namespace Compiler.SymbolTable
{
    class VariableSymbol : Symbol
    {
        public int size
        {
            get;
            set;
        }
        public int offset
        {
            get;
            set;
        }

        public int variableType
        {
            get;
            set;
        }
        /// <summary>
        /// Constructor for Variable Symbol
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        /// <param name="offset"></param>
        public VariableSymbol ( string name, SymbolType symbolType, VariableType var,int size, int offset) :base(name,symbolType)
        {
            this.size = size;
            this.offset = offset;
        }

        public override string ToString ()
        {
            return name + " " + symbolType + " " +size + " " + offset + " " + variableType ;
        }
    }
}
