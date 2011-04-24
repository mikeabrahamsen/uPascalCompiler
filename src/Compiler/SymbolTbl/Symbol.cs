using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;

namespace Compiler.SymbolTbl
{
    abstract class Symbol
    {
        /// <summary>
        /// Gets and sets the name of the symbol
        /// </summary>
        public string name
        {
            get;
            set;
        }
        /// <summary>
        /// gets the symbol type, this is only set when the symbol is created
        /// </summary>
        public SymbolType symbolType
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a symbol
        /// </summary>
        /// <param name="name"></param>
        /// <param name="symbolType"></param>
        public Symbol (string name,SymbolType symbolType)
        {
            this.name = name;
            this.symbolType = symbolType;
        }
        
    }

}
