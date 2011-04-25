using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;

namespace Compiler.SymbolTbl
{
    class Parameter
    {
        /// <summary>
        /// Gets and sets the IO mode
        /// </summary>
        public IOMode mode
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
        public string name
        {
            get;
            set;
        }
        /// <summary>
        /// Constructor for a parameter
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="variableType"></param>
        public Parameter (string name,IOMode mode, VariableType variableType)
        {
            this.mode = mode;
            this.variableType = variableType;
            this.name = name;

        }
    }
}
