using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.SymbolTbl;
namespace Compiler.SemAnalyzer
{
    class IdentifierRecord
    {
        public string lexeme
        {
            get;
            set;            
        }
        public SymbolTable symbolTable
        {
            get;
            set;
        }
        public Symbol symbol
        {
            get;
            set;
        }
    }
}
