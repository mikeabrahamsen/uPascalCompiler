using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.SymbolTbl;
using Compiler.Library;
namespace Compiler.SemAnalyzer
{
    class TypeRecord
    {
        /// <summary>
        /// Constructor for TypeRecord
        /// </summary>
        /// <param name="symbolType"></param>
        /// <param name="variableType"></param>
        public TypeRecord (SymbolType symbolType, VariableType variableType)
        {
            this.symbolType = symbolType;
            this.variableType = variableType;
        }
        /// <summary>
        /// Gets and sets the symbol type
        /// </summary>
        public SymbolType symbolType
        {
            get;
            set;
        }   
        /// <summary>
        /// Gets and sets the variable type
        /// </summary>
        public VariableType variableType
        {
            get;
            set;
        }
    }
}
