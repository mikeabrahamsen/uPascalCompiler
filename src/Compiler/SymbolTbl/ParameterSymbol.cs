using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;

namespace Compiler.SymbolTbl
{
    class ParameterSymbol : Symbol
    {
        /// <summary>
        /// Gets and sets the parameter
        /// </summary>
        public Parameter parameter
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor for parameter symbol
        /// </summary>
        /// <param name="name"></param>
        /// <param name="symbolType"></param>
        /// <param name="parameter"></param>
        public ParameterSymbol (string name,SymbolType symbolType, Parameter parameter) 
            :base(name,symbolType)
        {
            this.parameter = parameter;
        }
    }
}
