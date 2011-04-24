using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.Parse
{
    class SyntaxException : Exception
    {
        /// <summary>
        /// Creates a Syntax Exception
        /// </summary>
        /// <param name="errorMessage"></param>
        public SyntaxException(string errorMessage): base(errorMessage) 
        {
            
        }

        /// <summary>
        /// Gets the ErrorMessage from an exception
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return base.Message.ToString();
            }
        }
    }
}
