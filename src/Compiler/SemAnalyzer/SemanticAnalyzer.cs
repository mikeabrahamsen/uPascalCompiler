using System;
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
        const int DELEGATE_EXTRA_SPACE = 2;

        /// <summary>
        /// Gets and sets the list of delegates
        /// </summary>
        public List<MethodRecord> delegateList
        {
            get;
            set;
        }
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
            delegateList = new List<MethodRecord>();
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
            string cilScope;

            if (symbolTableStack.Count != 0)
            {
                string previousCilScope = symbolTableStack.Peek().cilScope;
                string previousName = symbolTableStack.Peek().name;
                cilScope = previousCilScope + "/c__" + previousName;
            }
            else
            {
                cilScope = "Program";
            }

            SymbolTable symbolTable = new SymbolTable(recordName,cilScope, symbolTableStack.Count);
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
                        break;
                }
                symbolTableStack.Peek().activationRecordSize += size; //increment activation record size
                symbolTableStack.Peek().Insert(symbol);
            }           
        }

        /// <summary>
        /// Generates code for a delegate declaration
        /// </summary>
        internal void GenerateDelegateDeclaration()
        {
            foreach(MethodRecord methodRecord in delegateList)
            {
                Console.WriteLine("/* " + methodRecord.name + "Delegate" +" delegate function declaration */");
                Console.WriteLine(".class auto ansi sealed nested public " + methodRecord.name 
                    + "Delegate");
            
                Console.WriteLine("extends [mscorlib]System.MulticastDelegate");
                Console.WriteLine("{");
                Console.WriteLine(".method public hidebysig specialname rtspecialname");
                Console.WriteLine("instance void  .ctor(object 'object'," + Environment.NewLine +
                                     "native int 'method') runtime managed");
                Console.WriteLine("{");

                Console.WriteLine("} // end of method " + methodRecord.name + "Delegate" + "::.ctor");
               

                Console.WriteLine(".method public hidebysig newslot virtual " + Environment.NewLine +
                        "instance void  Invoke() runtime managed");
                Console.WriteLine("{");
                Console.WriteLine("} // end of method " + methodRecord.name + "Delegate" + "::Invoke");

                Console.WriteLine(".method public hidebysig newslot virtual");
                Console.WriteLine("\tinstance class [mscorlib]System.IAsyncResult");
                Console.WriteLine("\tBeginInvoke(class [mscorlib]System.AsyncCallback callback,");
                Console.WriteLine("\tobject 'object') runtime managed");
                Console.WriteLine("{");
                Console.WriteLine("} // end of method "+ methodRecord.name+ "Delegate" + "::BeginInvoke");

                Console.WriteLine(".method public hidebysig newslot virtual");
                Console.WriteLine("instance void  EndInvoke(class [mscorlib]System.IAsyncResult result) " 
                    + "runtime managed");
                Console.WriteLine("{");
                Console.WriteLine("} // end of method " +methodRecord.name+ "Delegate"+ "::EndInvoke");
                Console.WriteLine("} // end of class " + methodRecord.name+ "Delegate");             
            }
        }
         /// <summary>
        /// inserts symbols into the symboltable
        /// </summary>
        /// <param name="idRecordList"></param>
        /// <param name="typeRecord"></param>
        public void SymbolTableInsert(MethodRecord methodRecord)
        {
            delegateList.Add(methodRecord);

            Symbol symbol;
            switch (methodRecord.symbolType)
            {
                case SymbolType.ProcedureSymbol:
                    symbol = new ProcedureSymbol(methodRecord.name, SymbolType.ProcedureSymbol, nextLabel, methodRecord.parameterList);
                    symbolTableStack.Peek().Insert(symbol);
                    break;
                case SymbolType.FunctionSymbol:
                    break;
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
            Console.Write("stfld\t");
            GenerateFieldLocation(idRecord);
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
            Console.WriteLine("  ldloc.0");
            Console.Write("  ldfld\t");
            GenerateFieldLocation(idRecord);

            factorRecord = idRecord.symbol.variableType;
        }

        /// <summary>
        /// Writes the location and type for field
        /// </summary>
        /// <param name="idRecord"></param>
        internal void GenerateFieldLocation(IdentifierRecord idRecord)
        {
            Console.WriteLine(Enumerations.GetDescription<VariableType>(
                idRecord.symbol.variableType) + " " +
                    idRecord.symbolTable.cilScope + "/c__" + idRecord.symbolTable.name
                        + "::" + idRecord.lexeme + Environment.NewLine);
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
                    else if(addOpRecord.Equals("div"))
                    {
                        Console.WriteLine("div");
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
                        Console.WriteLine("cgt");
                        Console.WriteLine("ldc.i4 0");
                        Console.WriteLine("ceq");
                    }
                    else if (addOpRecord.Equals(">="))
                    {
                        Console.WriteLine("clt");
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
                    else if (addOpRecord.Equals("or"))
                    {
                        Console.WriteLine("or");
                    }
                    else if (addOpRecord.Equals("and"))
                    {
                        Console.WriteLine("and");
                    }
                    else if (addOpRecord.Equals(""))
                    {
                        Console.WriteLine("and");
                    }
                    else if (addOpRecord.Equals("mod"))
                    {
                        Console.WriteLine("rem");
                    }
                    else if (addOpRecord.Equals("not"))
                    {
                        Console.WriteLine("not");
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
            Console.WriteLine(".assembly " + name + " {}" + Environment.NewLine);
            Console.WriteLine(".class private auto ansi beforefieldinit Program");
            Console.WriteLine("\textends [mscorlib]System.Object");
            Console.WriteLine("{" + Environment.NewLine);
        }
        /// <summary>
        /// Generates code for fields
        /// </summary>
        internal void GenerateFields()
        {
            SymbolTable table = symbolTableStack.Peek();
            int recordSize = table.activationRecordSize;

            if (recordSize > 0)
            {
                int index = 0;

                foreach (Symbol symbol in table.symbolTable)
                {
                    switch (symbol.symbolType)
                    {
                        case SymbolType.VariableSymbol:
                            //write the enum out as a string using the Get
                            Console.WriteLine(".field public " + Enumerations.GetDescription<VariableType>(
                                (symbol as VariableSymbol).variableType) + " " + symbol.name);
                            break;
                        case SymbolType.ProcedureSymbol:
                            Console.WriteLine(".field public class Program/" + symbol.name +
                                "\tDelegate D__" + symbol.name);
                            break;
                        case SymbolType.FunctionSymbol:
                            break;
                    }
                    index++;
                }
                Console.WriteLine();
            }

            if (symbolTableStack.Count > 1)
            {
                GeneratePreviousScopeObjects();
            }
        }

        /// <summary>
        /// Generates fields for all of the previous scopes that we may need to 
        /// see from the current scope
        /// </summary>
        internal void GeneratePreviousScopeObjects()
        {
            int count = 0;
            foreach (SymbolTable symbolTable in symbolTableStack)
            {
                if (count > 0)
                {
                    Console.WriteLine(".field public class " + symbolTable.cilScope + "/c__"
                        + symbolTable.name + " c__" + symbolTable.name + "Obj");
                }
                count++;
            }
            
        }
        /*
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
                Console.WriteLine(".maxstack " + (recordSize + 1));
                Console.Write(".locals init (");

                foreach (Symbol symbol in table.symbolTable)
                {
                    switch (symbol.symbolType)
                    {                           
                        case SymbolType.VariableSymbol:
                            //write the enum out as a string using the Get
                            Console.Write("[" + index + "] " + Enumerations.GetDescription<VariableType>(
                                (symbol as VariableSymbol).variableType) + " " + symbol.name);
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
        }*/
        /// <summary>
        /// Generates code for assignment statements
        /// </summary>
        /// <param name="idRecord"></param>
        /// <param name="expressionRecord"></param>
        internal void GenerateAssign (IdentifierRecord idRecord, VariableType expressionRecord)
        {
            Console.WriteLine("stfld\t" + 
                Enumerations.GetDescription<VariableType>(idRecord.symbol.variableType) + " " + 
                    idRecord.symbolTable.cilScope + "/c__" + idRecord.symbolTable.name
                        + "::" + idRecord.lexeme + Environment.NewLine);
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
            
            if (branchLabelRecord.Equals(string.Empty))
            {
                branchLabelRecord = nextLabel;
            }

            Console.Write(Enumerations.GetDescription<BranchType>(branchType));
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
            Console.WriteLine("  ldloc.0");
            Console.WriteLine("  ldloc.0");
            Console.Write("  ldfld\t");
            GenerateFieldLocation(identifierRecord);
            Console.WriteLine("  ldc.i4 1");
            Console.WriteLine("  " + addingOperator);
            Console.Write("  stfld\t"); 
            GenerateFieldLocation(identifierRecord);
        }
        
        /// <summary>
        /// Generates a closing brace
        /// </summary>
        internal void GenerateClosingBrace()
        {
            Console.WriteLine("}");
        }

        /// <summary>
        /// Generates code for a class declaration
        /// </summary>
        /// <param name="identifierRecord"></param>
        internal void GenerateClassDeclaration(string identifierRecord)
        {
            Console.WriteLine("/* c__" + identifierRecord + " Class definition */");
            Console.WriteLine(".class auto ansi sealed nested private beforefieldinit c__" +
                identifierRecord);
            Console.WriteLine("\textends [mscorlib]System.Object");
            Console.WriteLine("{" + Environment.NewLine);
        }

        /// <summary>
        /// Generates code for a class constructor
        /// </summary>
        /// <param name="identifierRecord"></param>
        internal void GenerateClassConstructor(string identifierRecord)
        {
            Console.WriteLine("/* c__" + identifierRecord + " constructor */");
            Console.WriteLine(".method public hidebysig specialname rtspecialname ");
            Console.WriteLine("\tinstance void  .ctor() cil managed ");
            Console.WriteLine("{");
            Console.WriteLine("  .maxstack  8");
            Console.WriteLine("  .ldarg.0");
            Console.WriteLine("  call\tinstance void [mscorlib]System.Object::.ctor()");
            Console.WriteLine("  ret");
            Console.WriteLine("} // end of method c__" + identifierRecord + "::.ctor");
        }

        /// <summary>
        /// Generates code for a method declaration
        /// </summary>
        /// <param name="identifierRecord"></param>
        internal void GenerateMethodDeclaration(string identifierRecord)
        {
            if (symbolTableStack.Count == 1)
            {
                Console.WriteLine(".method private hidebysig static void " + identifierRecord + 
                        "() cil managed");
                Console.WriteLine("{");
                Console.WriteLine(" .entrypoint");
            }
            else
            {
                Console.WriteLine(".method public hidebysig instance void " + identifierRecord + 
                        "() cil managed");
                Console.WriteLine("{");
            }

            int delegateCount = 0;

            Console.WriteLine("  .maxstack " + (delegateCount + DELEGATE_EXTRA_SPACE) + 
                                    Environment.NewLine);

            string cilScope = symbolTableStack.Peek().cilScope;

            Console.WriteLine( "  .locals init ([0] class " + cilScope + "/c__" + identifierRecord +
                " c__" + identifierRecord + "Obj" + ")" + Environment.NewLine);

            Console.WriteLine("  newobj\tvoid " + cilScope + "/c__" + identifierRecord + "::.ctor()");
            Console.WriteLine("  stloc.0" + Environment.NewLine);

        }
        /// <summary>
        /// Generates code for loading an object onto the stack
        /// </summary>
        internal void GenerateLoadObject()
        {
            Console.WriteLine("  ldloc.0");
        }
    }
}
