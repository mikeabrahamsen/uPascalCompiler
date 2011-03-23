using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;

namespace Compiler.SymbolTable
{
    class ProcedureSymbol : Symbol
    {

        public List<Parameter> paramList
        {
            get;
            set;
        }
        public string label
        {
            get;
            set;
        }
        public ProcedureSymbol (string name, SymbolType symbolType,string label,List<Parameter> paramList)
            : base(name, symbolType)
        {
            this.label = label;
            this.paramList = paramList;
        }
    }
}
