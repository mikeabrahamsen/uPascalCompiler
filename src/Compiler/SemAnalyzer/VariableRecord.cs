using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;

namespace Compiler.SemAnalyzer
{
    class VariableRecord
    {
        public IOMode ioMode
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
