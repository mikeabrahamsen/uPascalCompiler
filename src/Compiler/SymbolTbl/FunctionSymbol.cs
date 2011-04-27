using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;

namespace Compiler.SymbolTbl
{
    class FunctionSymbol : ProcedureSymbol
    {
        /// <summary>
        /// Gets and sets the returnVar
        /// </summary>
        public VariableType returnVar
        {
            get;
            set;
        }

        /// <summary>
        /// Constuctor for function symbol
        /// </summary>
        /// <param name="name"></param>
        /// <param name="symbolType"></param>
        /// <param name="label"></param>
        /// <param name="paramList"></param>
        /// <param name="returnVar"></param>
        public FunctionSymbol (string name,SymbolType symbolType,string label,
            List<Parameter> paramList,VariableType returnVar) 
            :base(name,symbolType,label,paramList)
        {
            this.returnVar = returnVar;
            this.variableType = returnVar;
        }
        
    }
}
