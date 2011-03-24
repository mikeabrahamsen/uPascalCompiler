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
        public override string ToString ()
        {
            return String.Format("{0,-10}{1,-20}{2,-10}{3,-10}{4,-10}{5,-10}", name, symbolType, "---", "---", "---",label);
        }
    }
}
