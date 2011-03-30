using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.SymbolTable
{
    class SymbolTable
    {
        public string name
        {
            get;
            set;
        }
        public int nestingLevel
        {
            get;
            set;
        }
        public int activationRecordSize
        {
            get;
            set;
        }

        public List<Symbol> symbolTable
        {
            get;
            set;
        }
        /// <summary>
        /// Symbol table constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nestingLevel"></param>
        /// <param name="activationRecordSize"></param>
        public SymbolTable (string name,int nestingLevel)
        {
            this.name = name;
            this.nestingLevel = nestingLevel;
            symbolTable = new List<Symbol>();
        }

        /// <summary>
        /// Inserts a symbol into the symbol table
        /// </summary>
        /// <param name="symbol"></param>
        public void Insert (Symbol symbol)
        {
            symbolTable.Add(symbol);
        }

        /// <summary>
        /// Find a symbol based on the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Symbol Find (string name)
        {
            foreach(Symbol symbol in symbolTable)
            {
                if (name.Equals(symbol.name))
                {
                    return symbol;
                }
            }
            return null;
        }
        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("{0,-10}{1,-20}{2,-10}{3,-10}{4,-10}{5,-10}","Name","Kind","Type","Size","Offset","Label"));
            sb.AppendLine("------------------------------------------------------------------");
            foreach(Symbol s in symbolTable)
            {
                sb.AppendLine(s.ToString());
                
            }
            return sb.ToString();
        }
    }
}
