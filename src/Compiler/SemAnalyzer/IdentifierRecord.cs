using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.SymbolTbl;
namespace Compiler.SemAnalyzer
{
    class IdentifierRecord
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
        /// Gets and sets the symbol table
        /// </summary>
        public SymbolTable symbolTable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the Varible Symbol
        /// </summary>
        public VariableSymbol symbol
        {
            get;
            set;
        }
    }
}
