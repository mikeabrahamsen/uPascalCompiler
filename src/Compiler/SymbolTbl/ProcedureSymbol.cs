using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;

namespace Compiler.SymbolTbl
{
    class ProcedureSymbol : Symbol
    {
        /// <summary>
        /// Gets and sets the parameterList
        /// </summary>
        public List<Parameter> paramList
        {
            get;
            set;
        }
        /// <summary>
        /// Gets and sets the label
        /// </summary>
        public string label
        {
            get;
            set;
        }
        /// <summary>
        /// Constructor for Procedure Symbol
        /// </summary>
        /// <param name="name"></param>
        /// <param name="symbolType"></param>
        /// <param name="label"></param>
        /// <param name="paramList"></param>
        public ProcedureSymbol (string name, SymbolType symbolType,string label,List<Parameter> paramList)
            : base(name, symbolType)
        {
            this.label = label;
            this.paramList = paramList;
        }
        /// <summary>
        /// Overrides the ToString method to a formatted output
        /// </summary>
        /// <returns></returns>
        public override string ToString ()
        {
            return String.Format("{0,-10}{1,-20}{2,-10}{3,-10}{4,-10}{5,-10}", name, symbolType, "---", "---", "---",label);
        }
    }
}
