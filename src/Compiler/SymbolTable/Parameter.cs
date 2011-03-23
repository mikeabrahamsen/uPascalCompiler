using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;

namespace Compiler.SymbolTable
{
    class Parameter
    {
        public IOMode mode
        {
            get;
            set;
        }
        public VariableType variableType
        {
            get;
            set;
        }
        public Parameter (IOMode mode, VariableType variableType)
        {
            this.mode = mode;
            this.variableType = variableType;
        }
    }
}
