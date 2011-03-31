using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.SymbolTbl;
using Compiler.Library;
namespace Compiler.SymAnalyzer
{
    class TypeRecord
    {
        public TypeRecord (SymbolType symbolType, VariableType variableType)
        {
            this.symbolType = symbolType;
            this.variableType = variableType;
        }
        public SymbolType symbolType
        {
            get;
            set;
        }   
        public VariableType variableType
        {
            get;
            set;
        }
    }
}
