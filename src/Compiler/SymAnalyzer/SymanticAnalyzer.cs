using System;
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

        /// <summary>
        /// Gets and sets the label count
        /// </summary>
        public int labelCount
        {
            get;
            set;
        }
        public SymanticAnalyzer ()
        {
            symbolTableStack = new Stack<SymbolTable>();
            labelCount = 0;
        }
        /// <summary>
        /// Return the next label
        /// </summary>
        public string NextLabel 
        {
            get { return "L" + labelCount++; }
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
        public void ProcessId (IdentifierRecord idRecord)
        {
            FindSymbol(idRecord);
        }
        public IdentifierRecord FindSymbol (IdentifierRecord idRecord)
        {
            Symbol symbol;
            foreach(SymbolTable st in symbolTableStack)
            {
                symbol = st.Find(idRecord.lexeme);
                if(symbol != null)
                {
                    idRecord.symbolTable = st;
                    idRecord.symbol = symbol;
                    return idRecord;
                }
            }
            return idRecord;
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
        
        public void GenerateReadStatement (IdentifierRecord idRecord)
        {
            Console.WriteLine("call       string [mscorlib]System.Console::ReadLine()");
            Console.WriteLine("call       int32 [mscorlib]System.Int32::Parse(string)");
            Console.WriteLine("stloc. " + idRecord.symbol.offset);
        }

        internal void GenerateWriteStatement ()
        {
            Console.WriteLine("call void [mscorlib]System.Console::WriteLine(int32)");
        }

        internal void GeneratePush (LiteralRecord litRecord, ref VariableType factorRecord)
        {
            Console.WriteLine("ldc.i4.s " + litRecord.lexeme);
            factorRecord = litRecord.type;
        }

        internal void GenerateIdPush (IdentifierRecord idRecord, ref VariableType factorRecord)
        {
            Console.WriteLine("ldloc." + idRecord.symbol.offset);
            //factorRecord = idRecord.symbol.type;
        }

        internal void GenerateArithmetic (VariableType termTailRecord, string addOpRecord, VariableType termRecord, ref VariableType resultRecord)
        {
            switch(termTailRecord)
            {
                case VariableType.Integer:
                    if (addOpRecord.Equals("+"))
                    {
                        Console.WriteLine("add");
                    }
                    else if(addOpRecord.Equals("-"))
                    {
                        Console.WriteLine("sub");
                    }
                    else if(addOpRecord.Equals("*"))
                    {
                        Console.WriteLine("mul");
                    }
                    resultRecord = VariableType.Integer;
                break;
                default:
                Console.WriteLine("Not Assigned");
                    break;

            }
        }

        internal void GenerateAssign (IdentifierRecord idRecord, VariableType expressionRecord)
        {
            Console.WriteLine("stloc." + idRecord.symbol.offset);
        }

        internal void GenerateReturn ()
        {
            Console.WriteLine("ret");
        }
    }
}
