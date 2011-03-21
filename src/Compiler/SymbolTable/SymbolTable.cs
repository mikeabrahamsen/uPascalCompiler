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

        public SymbolTable (string name,int nestingLevel,int activationRecordSize)
        {
            this.name = name;
            this.nestingLevel = nestingLevel;
            this.activationRecordSize = activationRecordSize;
            symbolTable = new List<Symbol>();
        }

        public void Insert (Symbol symbol)
        {
            symbolTable.Add(symbol);
        }

        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder();
            foreach(Symbol s in symbolTable)
            {
                sb.AppendLine(s.ToString());
                
            }
            return sb.ToString();
        }
    }
}
