using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;
using Compiler.SymbolTbl;

namespace Compiler.SemAnalyzer
{
    /// <summary>
    /// Class for method information
    /// </summary>
    class MethodRecord
    {
        /// <summary>
        /// Gets and sets the symbol type
        /// </summary>
        public SymbolType symbolType
        {
            get;
            set;
        }

        /// <summary>
        /// gets and sets the methodName
        /// </summary>
        public string name
        {
            get;
            set;
        }

        /// <summary>
        /// gets and sets the parameter list
        /// </summary>
        public List<Parameter> parameterList
        {
            get;
            set;
        }

        /// <summary>
        /// gets and sets the return type
        /// </summary>
        public VariableType returnType
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MethodRecord()
        {
            this.returnType = VariableType.Null;
        }

        /// <summary>
        /// Constructor with symbol type
        /// </summary>
        /// <param name="symbolType"></param>
        public MethodRecord(SymbolType symbolType)
        {
            this.returnType = VariableType.Null;
            this.symbolType = symbolType;

        }
    }
}
