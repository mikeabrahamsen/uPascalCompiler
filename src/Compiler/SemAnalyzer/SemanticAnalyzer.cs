using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.SymbolTbl;
using Compiler.Library;
using System.ComponentModel;
using System.IO;
using Compiler.Parse;
using System.Text.RegularExpressions;
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
        public SemanticAnalyzer (string fileName)
        {
            symbolTableStack = new Stack<SymbolTable>();
            delegateList = new List<MethodRecord>();
            SetFileName(fileName);
            labelCount = 0;
        }

        /// <summary>
        /// Sets the filename to a 'filename'.il format
        /// </summary>
        /// <param name="fileName"></param>
        private void SetFileName(string fileName)
        {
            Match match = Regex.Match(fileName, @"[a-zA-Z0-9-]+.");
            //remove the extention and replace with .il
            cilOutput = new StreamWriter( match.ToString() + "il");
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
        public void ProcessId(string idRecord, ref List<string>idRecordList)
        {
            idRecordList.Add(idRecord);
        }

       /// <summary>
       /// finds the correct symbol and sets the attributes of the idRecord
       /// </summary>
       /// <param name="idRecord"></param>
        public void ProcessId (ref IdentifierRecord idRecord)
        {            
            Symbol symbol;
            foreach(SymbolTable st in symbolTableStack)
            {
                symbol = st.Find(idRecord.lexeme);
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
                symbolTableStack.Peek().activationRecordSize += size;
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
                
                cilOutput.WriteLine("/* " + methodRecord.name + "Delegate" +
                    " delegate function declaration */");
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
                cilOutput.WriteLine("} // end of method "+ methodRecord.name+ "Delegate" 
                    + "::BeginInvoke");

                cilOutput.WriteLine(".method public hidebysig newslot virtual");
                cilOutput.WriteLine("instance void  EndInvoke(class [mscorlib]System.IAsyncResult result) " 
                    + "runtime managed");
                cilOutput.WriteLine("{");
                cilOutput.WriteLine("} // end of method " +methodRecord.name+ "Delegate"+ "::EndInvoke");
                cilOutput.WriteLine("} // end of class " + methodRecord.name+ "Delegate" + 
                    Environment.NewLine);             
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
                    symbol = new ProcedureSymbol(methodRecord.name, SymbolType.ProcedureSymbol, 
                        nextLabel, methodRecord.parameterList);
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
            GenerateObjectScope(idRecord.symbolTable);
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
        internal void GenerateArithmetic (VariableType termTailRecord, string addOpRecord,
            VariableType termRecord, ref VariableType resultRecord)
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
                            cilOutput.WriteLine(".field public " + 
                                Enumerations.GetDescription<VariableType>(
                                (symbol as VariableSymbol).variableType) + " " + symbol.name);
                            break;
                        case SymbolType.ProcedureSymbol:
                            cilOutput.WriteLine(".field public class Program/" + symbol.name +
                                "Delegate d__" + symbol.name);
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
            cilOutput.WriteLine("} // end of method c__" + identifierRecord + "::.ctor" + 
                Environment.NewLine);
        }

        /// <summary>
        /// Generates code for a method declaration
        /// </summary>
        /// <param name="identifierRecord"></param>
        internal void GenerateMethodDeclaration(string identifierRecord)
        {

            SymbolType type;
            int symbolCount = 1;
            string parameters = string.Empty;
            foreach (Symbol symbol in symbolTableStack.Peek().symbolTable)
            {
                type = symbol.symbolType;
                if (type == SymbolType.ParameterSymbol)
                {
                    ParameterSymbol pSymbol = symbol as ParameterSymbol;
                    parameters += (Enumerations.GetDescription<VariableType>(
                        pSymbol.variableType));
                    if (pSymbol.parameter.mode == IOMode.InOut)
                    {
                        parameters += ("&");
                    }
                    parameters += (" " + pSymbol.name);
                    symbolCount++;
                    if (symbolCount != symbolTableStack.Peek().symbolTable.Count)
                    {
                        parameters += (", ");
                    }                    
                }
            }

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
                cilOutput.WriteLine("\tb__"+identifierRecord + "(" + parameters +
                    ") cil managed");
                cilOutput.WriteLine("{");
            }

            int delegateCount = 0;

            cilOutput.WriteLine("  .maxstack " + (delegateCount + DELEGATE_EXTRA_SPACE) + 
                                    Environment.NewLine);

            string cilScope = symbolTableStack.Peek().cilScope;

            cilOutput.WriteLine( "  .locals init ([0] class " + cilScope + "/c__" + identifierRecord +
                " c__" + identifierRecord + "Obj" + ")" + Environment.NewLine);

            cilOutput.WriteLine("  newobj\tinstance void " + cilScope + "/c__" + 
                identifierRecord + "::.ctor()");
            cilOutput.WriteLine("  stloc.0" + Environment.NewLine);

            GenerateDelegateInitilization();

            if (symbolTableStack.Count > 1)
            {
                int scopeCount = 0;
                string newScope = symbolTableStack.Peek().cilScope + "/c__" + 
                    symbolTableStack.Peek().name;
                int index = newScope.LastIndexOf("/");
                string oldScope = newScope.Substring(0, index);
                SymbolTable previousScope;
                SymbolTable symbolTable = symbolTableStack.Peek();
                int count = 0;                
                
                for(int i = 0;i <symbolTableStack.Count -1;i++)
                {
                    cilOutput.WriteLine("  ldloc.0");
                    cilOutput.WriteLine("  ldarg.0");

                    //symbolTable = symbolTableStack.ElementAt(count);
                    count++;
                    previousScope = symbolTableStack.ElementAt(count);
                    
                    if (scopeCount > 0)
                    {
                        cilOutput.WriteLine("  ldfld\tclass " + previousScope.cilScope +
                            "/c__" + previousScope.name + " " + oldScope + "::c__" + previousScope.name +
                            "Obj");
                    }

                    cilOutput.WriteLine("  stfld\tclass " + previousScope.cilScope +
                        "/c__" + previousScope.name + " " + newScope + "::c__" + previousScope.name +
                        "Obj" + Environment.NewLine);

                    scopeCount++;
                }
            }
        }

        /// <summary>
        /// Generates code for initilizing a delegate
        /// </summary>
        internal void GenerateDelegateInitilization()
        {
            SymbolTable symbolTable = symbolTableStack.Peek();

            foreach (Symbol symbol in symbolTable.symbolTable)
            {
                if (symbol.symbolType == SymbolType.ProcedureSymbol ||
                    symbol.symbolType == SymbolType.FunctionSymbol)
                {
                    cilOutput.WriteLine("  ldloc.0");
                    cilOutput.WriteLine("  ldloc.0");
                    cilOutput.WriteLine("  ldftn\tinstance void " + symbolTable.cilScope +
                        "/c__" + symbolTable.name + "::b__" + symbol.name + "()");
                    cilOutput.WriteLine("  newobj\tinstance void Program" + "/" + symbol.name +
                        "Delegate::.ctor(object,native int)");
                    cilOutput.WriteLine("  stfld\tclass Program" + "/" + symbol.name + "Delegate " +
                        symbolTable.cilScope + "/c__" + symbolTable.name + "::d__" + symbol.name +
                        Environment.NewLine);
                }
            }
        }


        /// <summary>
        /// Generates code for calling a method
        /// </summary>
        internal void GenerateCallMethod(MethodRecord methodRecord)
        {
            SymbolTable symbolTable = symbolTableStack.Peek();

            cilOutput.WriteLine("  ldloc.0");
            cilOutput.WriteLine("  ldfld      class Program/");
            cilOutput.WriteLine(methodRecord.name + "Delegate " + symbolTable.cilScope +
                "/c__" + symbolTable.name + "::d__" + methodRecord.name);
            cilOutput.WriteLine("  callvirt\tinstance void Program/" + methodRecord.name +
                "Delegate::Invoke()" + Environment.NewLine);
        }

        /// <summary>
        /// Generates code for the object scope
        /// </summary>
        /// <param name="objectTable"></param>
        internal void GenerateObjectScope(SymbolTable objectTable)
        {
            if (symbolTableStack.Count == 1)
            {
                cilOutput.WriteLine("  ldloc.0");
            }
            else
            {
                int nestingLevelDifference = symbolTableStack.Count - 1 - objectTable.nestingLevel;

                if (nestingLevelDifference <= 1)
                {
                    cilOutput.WriteLine("  ldloc.0");
                }
                else
                {
                    cilOutput.WriteLine("  ldarg.0");

                    if (nestingLevelDifference > 1)
                    {
                        cilOutput.WriteLine("  ldfld\tclass " + objectTable.cilScope + "/c__" +
                            objectTable.name + " " + symbolTableStack.Peek().cilScope + "::c__" +
                            objectTable.name + "Obj");
                    }
                }

            }
        }

        /// <summary>
        /// Inserts a ParameterSymbol into the symbol table
        /// </summary>
        /// <param name="list"></param>
        internal void SymbolTableInsert(List<Parameter> list)
        {

            foreach (Parameter parameter in list)
            {
                symbolTableStack.Peek().Insert(new ParameterSymbol(parameter.name,
                    SymbolType.ParameterSymbol,parameter,parameter.variableType));
            }
        }
    }
}
