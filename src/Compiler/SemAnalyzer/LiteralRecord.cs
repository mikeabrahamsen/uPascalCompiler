using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;
namespace Compiler.SemAnalyzer
{
    class LiteralRecord
    {
        /// <summary>
        /// Gets and sets the lexeme
        /// </summary>
        public string lexeme
        {
            get;
            set;
        }
        /// <summary>
        /// Gets and sets the VariableType
        /// </summary>
        public VariableType type
        {
            get;
            set;
        }

    }
}
