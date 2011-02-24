﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.Parser
{
    class SyntaxException : Exception
    {
        public SyntaxException(string errorMessage): base(errorMessage) 
        {
            
        }
        public string ErrorMessage
        {
            get
            {
                return base.Message.ToString();
            }
        }
    }
}
