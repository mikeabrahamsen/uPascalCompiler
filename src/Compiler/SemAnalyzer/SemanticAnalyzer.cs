using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.SymbolTbl;
using Compiler.Library;
using System.ComponentModel;
using System.IO;

namespace Compiler.SemAnalyzer
{
    class SemanticAnalyzer
    {
        const int DELEGATE_EXTRA_SPACE = 2;

        public TextWriter cilOutput
        {
            get;
            set;
        }
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
            cilOutput = new StreamWriter("CIL.il");
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
                
                cilOutput.WriteLine("/* " + methodRecord.name + "Delegate" +" delegate function declaration */");
                cilOutput.WriteLine(".class auto ansi sealed nested public " + methodRecord.name 
                    + "Delegate");
            
                cilOutput.WriteLine("extends [mscorlib]System.MulticastDelegate");
                cilOutput.WriteLine("{");
                cilOutput.WriteLine(".method public hidebysig specialname rtspecialname");
                cilOutput.WriteLine("instance void  .ctor(object 'object'," + Environment.NewLine +
                                     "native int 'method') runtime managed");
                cilOutput.WriteLine("{");

                cilOutput.WriteLine("} // end of method " + methodRecord.name + "Delegate" + "::.ctor");
               

                cilOutput.WriteLine(".method public hidebysig newslot virtual " + Environment.NewLine +
                        "instance void  Invoke() runtime managed");
                cilOutput.WriteLine("{");
                cilOutput.WriteLine("} // end of method " + methodRecord.name + "Delegate" + "::Invoke");

                cilOutput.WriteLine(".method public hidebysig newslot virtual");
                cilOutput.WriteLine("\tinstance class [mscorlib]System.IAsyncResult");
                cilOutput.WriteLine("\tBeginInvoke(class [mscorlib]System.AsyncCallback callback,");
                cilOutput.WriteLine("\tobject 'object') runtime managed");
                cilOutput.WriteLine("{");
                cilOutput.WriteLine("} // end of method "+ methodRecord.name+ "Delegate" + "::BeginInvoke");

                cilOutput.WriteLine(".method public hidebysig newslot virtual");
                cilOutput.WriteLine("instance void  EndInvoke(class [mscorlib]System.IAsyncResult result) " 
                    + "runtime managed");
                cilOutput.WriteLine("{");
                cilOutput.WriteLine("} // end of method " +methodRecord.name+ "Delegate"+ "::EndInvoke");
                cilOutput.WriteLine("} // end of class " + methodRecord.name+ "Delegate" + Environment.NewLine);             
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
            cilOutput.WriteLine("  call       string [mscorlib]System.Console::ReadLine()");
            cilOutput.WriteLine("  call       int32 [mscorlib]System.Int32::Parse(string)");
            cilOutput.Write("  stfld\t");
            GenerateFieldLocation(idRecord);
        }

        /// <summary>
        /// Generates code for write statements
        /// </summary>
        internal void GenerateWriteStatement ()
        {
            cilOutput.WriteLine("  call void [mscorlib]System.Console::WriteLine(int32)");
        }

        /// <summary>
        /// Generates code for a push statement for literal type
        /// </summary>
        /// <param name="litRecord"></param>
        /// <param name="factorRecord"></param>
        internal void GenerateLitPush (LiteralRecord litRecord, ref VariableType factorRecord)
        {
            cilOutput.WriteLine("  ldc.i4 " + litRecord.lexeme);
            factorRecord = litRecord.type;
        }

        /// <summary>
        /// Generates code for pushing an Identifier
        /// </summary>
        /// <param name="idRecord"></param>
        /// <param name="factorRecord"></param>
        internal void GenerateIdPush (IdentifierRecord idRecord, ref VariableType factorRecord)
        {
            cilOutput.WriteLine("  ldloc.0");
            cilOutput.Write("  ldfld\t");
            GenerateFieldLocation(idRecord);

            factorRecord = idRecord.symbol.variableType;
        }

        /// <summary>
        /// Writes the location and type for field
        /// </summary>
        /// <param name="idRecord"></param>
        internal void GenerateFieldLocation(IdentifierRecord idRecord)
        {
            cilOutput.WriteLine(Enumerations.GetDescription<VariableType>(
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
                        cilOutput.WriteLine("  add");
                    }
                    else if (addOpRecord.Equals("-"))
                    {
                        cilOutput.WriteLine("  sub");
                    }
                    else if (addOpRecord.Equals("*"))
                    {
                        cilOutput.WriteLine("  mul");
                    }
                    else if(addOpRecord.Equals("div"))
                    {
                        cilOutput.WriteLine("  div");
                    }
                    else if (addOpRecord.Equals("<"))
                    {
                        cilOutput.WriteLine("  clt");
                    }
                    else if (addOpRecord.Equals(">"))
                    {
                        cilOutput.WriteLine("  cgt");
                    }
                    else if (addOpRecord.Equals("<="))
                    {
                        cilOutput.WriteLine("  cgt");
                        cilOutput.WriteLine("  ldc.i4 0");
                        cilOutput.WriteLine("  ceq");
                    }
                    else if (addOpRecord.Equals(">="))
                    {
                        cilOutput.WriteLine("  clt");
                        cilOutput.WriteLine("  ldc.i4 0");
                        cilOutput.WriteLine("  ceq");
                    }
                    else if (addOpRecord.Equals("<>"))
                    {
                        cilOutput.WriteLine("  ceq");
                        cilOutput.WriteLine("  ldc.i4 0");
                        cilOutput.WriteLine("  ceq");
                    }
                    else if (addOpRecord.Equals("="))
                    {
                        cilOutput.WriteLine("  ceq");
                    }
                    else if (addOpRecord.Equals("or"))
                    {
                        cilOutput.WriteLine("  or");
                    }
                    else if (addOpRecord.Equals("and"))
                    {
                        cilOutput.WriteLine("  and");
                    }
                    else if (addOpRecord.Equals(""))
                    {
                        cilOutput.WriteLine("  and");
                    }
                    else if (addOpRecord.Equals("mod"))
                    {
                        cilOutput.WriteLine("  rem");
                    }
                    else if (addOpRecord.Equals("not"))
                    {
                        cilOutput.WriteLine("  not");
                    }
                    resultRecord = VariableType.Integer;
                break;
                default:
                cilOutput.WriteLine("Not Assigned");
                    break;

            }
        }
        /// <summary>
        /// Generates code for initilizing a program
        /// </summary>
        /// <param name="name"></param>
        internal void GenerateProgramInitialize(string name)
        {
            cilOutput.WriteLine(".assembly extern mscorlib {}");
            cilOutput.WriteLine(".assembly " + name + " {}" + Environment.NewLine);
            cilOutput.WriteLine(".class private auto ansi beforefieldinit Program");
            cilOutput.WriteLine("\textends [mscorlib]System.Object");
            cilOutput.WriteLine("{" + Environment.NewLine);
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
                            cilOutput.WriteLine(".field public " + Enumerations.GetDescription<VariableType>(
                                (symbol as VariableSymbol).variableType) + " " + symbol.name);
                            break;
                        case SymbolType.ProcedureSymbol:
                            cilOutput.WriteLine(".field public class Program/" + symbol.name +
                                "\tDelegate D__" + symbol.name);
                            break;
                        case SymbolType.FunctionSymbol:
                            break;
                    }
                    index++;
                }
                cilOutput.WriteLine();
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
                    cilOutput.WriteLine(".field public class " + symbolTable.cilScope + "/c__"
                        + symbolTable.name + " c__" + symbolTable.name + "Obj");
                }
                count++;
            }
            
        }
        /// <summary>
        /// Generates code for assignment statements
        /// </summary>
        /// <param name="idRecord"></param>
        /// <param name="expressionRecord"></param>
        internal void GenerateAssign (IdentifierRecord idRecord, VariableType expressionRecord)
        {
            cilOutput.WriteLine("  stfld\t" + 
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
            cilOutput.WriteLine(labelRecord + ":"); 
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

            cilOutput.Write(Enumerations.GetDescription<BranchType>(branchType));
            cilOutput.WriteLine(branchLabelRecord);
        }
        /// <summary>
        /// Generates code for return statements
        /// </summary>
        internal void GenerateReturn ()
        {
            cilOutput.WriteLine("  ret");
            cilOutput.WriteLine("}");
        }

        /// <summary>
        /// Generates code for incrementing in a control structure
        /// </summary>
        /// <param name="controlLabelRecord"></param>
        /// <param name="p"></param>
        internal void GenerateIncrement(ref IdentifierRecord identifierRecord, string addingOperator)
        {
            cilOutput.WriteLine("  ldloc.0");
            cilOutput.WriteLine("  ldloc.0");
            cilOutput.Write("  ldfld\t");
            GenerateFieldLocation(identifierRecord);
            cilOutput.WriteLine("  ldc.i4 1");
            cilOutput.WriteLine("  " + addingOperator);
            cilOutput.Write("  stfld\t"); 
            GenerateFieldLocation(identifierRecord);
        }
        
        /// <summary>
        /// Generates a closing brace
        /// </summary>
        internal void GenerateClosingBrace()
        {
            cilOutput.WriteLine("}");
        }

        /// <summary>
        /// Generates code for a class declaration
        /// </summary>
        /// <param name="identifierRecord"></param>
        internal void GenerateClassDeclaration(string identifierRecord)
        {
            cilOutput.WriteLine("/* c__" + identifierRecord + " Class definition */");
            cilOutput.WriteLine(".class auto ansi sealed nested private beforefieldinit c__" +
                identifierRecord);
            cilOutput.WriteLine("\textends [mscorlib]System.Object");
            cilOutput.WriteLine("{" + Environment.NewLine);
        }

        /// <summary>
        /// Generates code for a class constructor
        /// </summary>
        /// <param name="identifierRecord"></param>
        internal void GenerateClassConstructor(string identifierRecord)
        {
            cilOutput.WriteLine("/* c__" + identifierRecord + " constructor */");
            cilOutput.WriteLine(".method public hidebysig specialname rtspecialname ");
            cilOutput.WriteLine("\tinstance void  .ctor() cil managed ");
            cilOutput.WriteLine("{");
            cilOutput.WriteLine("  .maxstack  8");
            cilOutput.WriteLine("  ldarg.0");
            cilOutput.WriteLine("  call\tinstance void [mscorlib]System.Object::.ctor()");
            cilOutput.WriteLine("  ret");
            cilOutput.WriteLine("} // end of method c__" + identifierRecord + "::.ctor" + Environment.NewLine);
        }

        /// <summary>
        /// Generates code for a method declaration
        /// </summary>
        /// <param name="identifierRecord"></param>
        internal void GenerateMethodDeclaration(string identifierRecord)
        {
            if (symbolTableStack.Count == 1)
            {
                cilOutput.WriteLine(".method private hidebysig static void ");
                cilOutput.WriteLine(identifierRecord + "() cil managed");
                cilOutput.WriteLine("{");
                cilOutput.WriteLine(" .entrypoint");
            }
            else
            {
                cilOutput.WriteLine(".method public hidebysig instance void ");
                cilOutput.WriteLine("\tb__"+identifierRecord + "() cil managed");
                cilOutput.WriteLine("{");
            }

            int delegateCount = 0;

            cilOutput.WriteLine("  .maxstack " + (delegateCount + DELEGATE_EXTRA_SPACE) + 
                                    Environment.NewLine);

            string cilScope = symbolTableStack.Peek().cilScope;

            cilOutput.WriteLine( "  .locals init ([0] class " + cilScope + "/c__" + identifierRecord +
                " c__" + identifierRecord + "Obj" + ")" + Environment.NewLine);

            cilOutput.WriteLine("  newobj\tinstance void " + cilScope + "/c__" + identifierRecord + "::.ctor()");
            cilOutput.WriteLine("  stloc.0" + Environment.NewLine);

        }
        /// <summary>
        /// Generates code for loading an object onto the stack
        /// </summary>
        internal void GenerateLoadObject()
        {
            cilOutput.WriteLine("  ldloc.0");
        }
    }
}
