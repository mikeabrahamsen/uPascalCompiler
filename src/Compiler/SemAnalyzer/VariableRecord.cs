using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;

namespace Compiler.SemAnalyzer
{
    class VariableRecord
    {
        /// <summary>
        /// Gets and sets the IoMode
        /// </summary>
        public IOMode ioMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the Variable Type
        /// </summary>
        public VariableType variableType
        {
            get;
            set;
        }

    }
}
