using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;

namespace Compiler.SymbolTable
{
    abstract class Symbol
    {
        public string name
        {
            get;
            set;
        }
        public SymbolType symbolType
        {
            get;
            private set;
        }
        
        public Symbol ()
        { }
        public Symbol (string name,SymbolType symbolType)
        {
            this.name = name;
            this.symbolType = symbolType;
        }
        
    }

}
