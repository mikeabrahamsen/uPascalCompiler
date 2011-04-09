﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.SymbolTbl;
using Compiler.Library;
using System.ComponentModel;

namespace Compiler.SemAnalyzer
{
    class SemanticAnalyzer
    {
        /// <summary>
        /// Gets and sets the symbolTableStack
        /// </summary>
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

        /// <summary>
        /// Semantic Analyzer Constructor
        /// </summary>
        public SemanticAnalyzer ()
        {
            symbolTableStack = new Stack<SymbolTable>();
            labelCount = 0;
        }

        /// <summary>
        /// Return the next label
        /// </summary>
        public string nextLabel 
        {
            get { return "L" + labelCount++; }
        }

        /// <summary>
        /// Creates a new symbol table given a name
        /// </summary>
        /// <param name="recordName"></param>
        public void CreateSymbolTable(string recordName)
        {
            SymbolTable symbolTable = new SymbolTable(recordName, symbolTableStack.Count);
            symbolTableStack.Push(symbolTable);            
        }

        /// <summary>
        /// Adds an id to a record list
        /// </summary>
        /// <param name="idRecord"></param>
        /// <param name="idRecordList"></param>
        public void AddId(string idRecord,List<string>idRecordList)
        {
            idRecordList.Add(idRecord);
        }

       /// <summary>
       /// finds the correct symbol and sets the attributes of the idRecord
       /// </summary>
       /// <param name="idRecord"></param>
        public void ProcessId (IdentifierRecord idRecord)
        {            
            VariableSymbol symbol;
            foreach(SymbolTable st in symbolTableStack)
            {
                symbol = st.Find(idRecord.lexeme) as VariableSymbol;
                if(symbol != null)
                {
                    idRecord.symbolTable = st;
                    idRecord.symbol = symbol;
                }
            }
            
        }

        /// <summary>
        /// inserts symbols into the symboltable
        /// </summary>
        /// <param name="idRecordList"></param>
        /// <param name="typeRecord"></param>
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
        
        /// <summary>
        /// Generates code for a read statement
        /// </summary>
        /// <param name="idRecord"></param>
        internal void GenerateReadStatement (IdentifierRecord idRecord)
        {
            Console.WriteLine("call       string [mscorlib]System.Console::ReadLine()");
            Console.WriteLine("call       int32 [mscorlib]System.Int32::Parse(string)");
            Console.WriteLine("stloc." + idRecord.symbol.offset);
        }

        /// <summary>
        /// Generates code for write statements
        /// </summary>
        internal void GenerateWriteStatement ()
        {
            Console.WriteLine("call void [mscorlib]System.Console::WriteLine(int32)");
        }

        /// <summary>
        /// Generates code for a push statement for literal type
        /// </summary>
        /// <param name="litRecord"></param>
        /// <param name="factorRecord"></param>
        internal void GenerateLitPush (LiteralRecord litRecord, ref VariableType factorRecord)
        {
            Console.WriteLine("ldc.i4 " + litRecord.lexeme);
            factorRecord = litRecord.type;
        }

        /// <summary>
        /// Generates code for pushing an Identifier
        /// </summary>
        /// <param name="idRecord"></param>
        /// <param name="factorRecord"></param>
        internal void GenerateIdPush (IdentifierRecord idRecord, ref VariableType factorRecord)
        {
            Console.WriteLine("ldloc." + idRecord.symbol.offset);
            factorRecord = idRecord.symbol.variableType;
        }

        /// <summary>
        /// Generates code for arithmetic operations
        /// </summary>
        /// <param name="termTailRecord"></param>
        /// <param name="addOpRecord"></param>
        /// <param name="termRecord"></param>
        /// <param name="resultRecord"></param>
        internal void GenerateArithmetic (VariableType termTailRecord, string addOpRecord, VariableType termRecord, ref VariableType resultRecord)
        {
            switch(termTailRecord)
            {
                case VariableType.Integer:
                    if (addOpRecord.Equals("+"))
                    {
                        Console.WriteLine("add");
                    }
                    else if (addOpRecord.Equals("-"))
                    {
                        Console.WriteLine("sub");
                    }
                    else if (addOpRecord.Equals("*"))
                    {
                        Console.WriteLine("mul");
                    }
                    else if (addOpRecord.Equals("<"))
                    {
                        Console.WriteLine("clt");
                    }
                    else if (addOpRecord.Equals(">"))
                    {
                        Console.WriteLine("cgt");
                    }
                    else if (addOpRecord.Equals("<="))
                    {
                        Console.WriteLine("clt");
                        Console.WriteLine("ldc.i4 0");
                        Console.WriteLine("ceq");
                    }
                    else if (addOpRecord.Equals(">="))
                    {
                        Console.WriteLine("cgt");
                        Console.WriteLine("ldc.i4 0");
                        Console.WriteLine("ceq");
                    }
                    else if (addOpRecord.Equals("<>"))
                    {
                        Console.WriteLine("ceq");
                        Console.WriteLine("ldc.i4 0");
                        Console.WriteLine("ceq");
                    }
                    else if (addOpRecord.Equals("="))
                    {
                        Console.WriteLine("ceq");
                    }
                    resultRecord = VariableType.Integer;
                break;
                default:
                Console.WriteLine("Not Assigned");
                    break;

            }
        }
        /// <summary>
        /// Generates code for initilizing a program
        /// </summary>
        /// <param name="name"></param>
        internal void GenerateProgramInitialize(string name)
        {
            Console.WriteLine(".assembly extern mscorlib {}");
            Console.WriteLine(".assembly " + name + " {}\n");
            Console.WriteLine(".method static void Main()\n{");
            Console.WriteLine(".entrypoint");
        }

        /// <summary>
        /// Generates code for local variable
        /// </summary>
        internal void GenerateLocals()
        {
            SymbolTable table = symbolTableStack.Peek();
            int recordSize = table.activationRecordSize;
            
            if (recordSize > 0)
            {
                int index = 0;
                Console.WriteLine(".maxstack " + recordSize);
                Console.Write(".locals init (");

                foreach (Symbol symbol in table.symbolTable)
                {
                    switch (symbol.symbolType)
                    {                           
                        case SymbolType.VariableSymbol:
                            //write the enum out as a string using the Get
                            Console.Write("[" + index + "] " + Enumerations.GetDescription<VariableType>((symbol as VariableSymbol).variableType) + " " + symbol.name);
                            if (index < table.symbolTable.Count -1)
                            {
                                Console.Write(",");
                            }
                            break;
                    }
                    index++;
                }
                Console.WriteLine(")");
            }
        }
        /// <summary>
        /// Generates code for assignment statements
        /// </summary>
        /// <param name="idRecord"></param>
        /// <param name="expressionRecord"></param>
        internal void GenerateAssign (IdentifierRecord idRecord, VariableType expressionRecord)
        {
            Console.WriteLine("stloc." + idRecord.symbol.offset);
        }

        /// <summary>
        /// Generates code for a label
        /// </summary>
        /// <param name="labelRecord"></param>
        internal void GenerateLabel(ref string labelRecord)
        {
            if (labelRecord.Equals(string.Empty))
            {
                labelRecord = nextLabel;
            }
            Console.WriteLine(labelRecord + ":"); 
        }

        /// <summary>
        /// Generates code for branching
        /// </summary>
        /// <param name="branchLabelRecord"></param>
        /// <param name="branchType"></param>
        internal void GenerateBranch(ref string branchLabelRecord, BranchType branchType)
        {
            branchLabelRecord = nextLabel;
            if (branchLabelRecord.Equals(string.Empty))
            {
                branchLabelRecord = nextLabel;
            }
            switch (branchType)
            {
                case BranchType.br:
                    Console.Write("br.s ");
                    break;
                case BranchType.brfalse:
                    Console.Write("brfalse ");
                    break;
                case BranchType.bgt:
                    Console.Write("bgt ");
                    break;
                default:
                    break;
            }
            Console.WriteLine(branchLabelRecord);
        }
        /// <summary>
        /// Generates code for return statements
        /// </summary>
        internal void GenerateReturn ()
        {
            Console.WriteLine("ret");
            Console.WriteLine("}");
        }

        /// <summary>
        /// Generates code for incrementing in a control structure
        /// </summary>
        /// <param name="controlLabelRecord"></param>
        /// <param name="p"></param>
        internal void GenerateIncrement(ref IdentifierRecord identifierRecord, string addingOperator)
        {
            int offset = identifierRecord.symbol.offset;
            Console.WriteLine("ldloc." + offset);
            Console.WriteLine("ldc.i4 1");
            Console.WriteLine(addingOperator);
            Console.WriteLine("stloc." + offset);
        }
    }
}
