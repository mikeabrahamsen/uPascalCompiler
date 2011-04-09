using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;

namespace Compiler.SemAnalyzer
{
    class ControlRecord
    {
        /// <summary>
        /// Gets and sets the adding operator
        /// </summary>
        public string addingOperator
        {
            get;
            set;
        }

        /// <summary>
        /// gets and sets the branch type
        /// </summary>
        public BranchType branchType
        {
            get;
            set;
        }
    }
}
