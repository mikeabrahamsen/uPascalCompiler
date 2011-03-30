using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.SymbolTable;
namespace Compiler.SymanticAnalyzer
{
    class SymanticAnalyzer
    {
        public Stack<SymbolTable.SymbolTable> symbolTableStack
        {
            get;
            set;
        } 
        public SymanticAnalyzer ()
        {
            symbolTableStack = new Stack<SymbolTable.SymbolTable>();
        }
        
        public void CreateSymbolTable(string recordName)
        {
            SymbolTable.SymbolTable symbolTable = new SymbolTable.SymbolTable(recordName, symbolTableStack.Count);
            symbolTableStack.Push(symbolTable);
        }
    }
}
