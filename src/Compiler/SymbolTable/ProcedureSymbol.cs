using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;

namespace Compiler.SymbolTable
{
    class ProcedureSymbol : Symbol
    {
        public IOMode mode
        {
            get;
            set;
        }

        public ProcedureSymbol (string name, SymbolType symbolType, IOMode mode)
            : base(name, symbolType)
        {
            this.mode = mode;
        }
    }
}
