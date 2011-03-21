using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;

namespace Compiler.SymbolTable
{
    class ParameterSymbol : Symbol
    {
        public Parameter parameter
        {
            get;
            set;
        }
        public ParameterSymbol (string name,SymbolType symbolType, Parameter parameter) :base(name,symbolType)
        {
            this.parameter = parameter;
        }
    }
}
