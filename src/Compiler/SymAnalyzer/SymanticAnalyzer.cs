﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.SymbolTbl;
using Compiler.Library;
namespace Compiler.SymAnalyzer
{
    class SymanticAnalyzer
    {
        public Stack<SymbolTable> symbolTableStack
        {
            get;
            set;
        } 
        public SymanticAnalyzer ()
        {
            symbolTableStack = new Stack<SymbolTable>();
        }
        
        public void CreateSymbolTable(string recordName)
        {
            SymbolTable symbolTable = new SymbolTable(recordName, symbolTableStack.Count);
            symbolTableStack.Push(symbolTable);
        }
        public void ProcessId(string idRecord,List<string>idRecordList)
        {
            idRecordList.Add(idRecord);
        }
        public void SymbolTableInsert (List<string> idRecordList, TypeRecord typeRecord)
        {
            Symbol symbol = null;
            int size = 0;
            foreach(string lexeme in idRecordList)
            {
                switch(typeRecord.symbolType)
                {
                    case SymbolType.VariableSymbol:
                        size = 1; //choose size based on variabletype
                        symbol = new VariableSymbol(lexeme, typeRecord.symbolType, typeRecord.variableType, 
                                    size, symbolTableStack.Peek().activationRecordSize);
                         
                        break;
                    default:
                        //throw exception
                        break;
                }
                symbolTableStack.Peek().activationRecordSize += size; //increment activation record size
                symbolTableStack.Peek().Insert(symbol);
            }
           
        }
    }
}
