using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Lexer;
using Compiler.Library;

namespace Compiler.SymbolTbl
{
    class VariableSymbol : Symbol
    {
        /// <summary>
        /// gets and sets the size
        /// </summary>
        public int size
        {
            get;
            set;
        }
        /// <summary>
        /// gets and sets the offset
        /// </summary>
        public int offset
        {
            get;
            set;
        }

        /// <summary>
        /// gets and sets the variable type
        /// </summary>
        public VariableType variableType
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
            this.variableType = var;
        }

        /// <summary>
        /// overrides the ToString method to a formatted output
        /// </summary>
        /// <returns></returns>
        public override string ToString ()
        {
            return String.Format("{0,-10}{1,-20}{2,-10}{3,-10}{4,-10}",name,symbolType,variableType,size,offset);
        }
    }
}
