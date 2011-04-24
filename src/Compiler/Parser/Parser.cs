
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Lexer;
using Compiler.Library;
using System.IO;
using Compiler.SemAnalyzer;
using Compiler.SymbolTbl;

namespace Compiler.Parse
{
    class Parser
    {
        /// <summary>
        /// Gets and sets the filename
        /// </summary>
        private string fileName
        {
            get;
            set;
        }
        /// <summary>
        /// Gets and sets the used rules textwriter
        /// </summary>
        private TextWriter UsedRules
        {
            get;
            set;
        }
        /// <summary>
        /// gets and sets the semantic analyzer
        /// </summary>
        private SemanticAnalyzer analyzer
        {
            get;
            set;
        }
        /// <summary>
        /// gets and sets the token queue
        /// </summary>
        private Queue<Token> tokenQueue
        {
            get;
            set;
        }
        /// <summary>
        /// gets and sets the scanner
        /// </summary>
        private LexicalAnalyzer scanner
        {
            get;
            set;
        }
        /// <summary>
        /// gets and sets the lookahead token
        /// </summary>
        private Token lookAheadToken
        {
            get;
            set;
        }
        /// <summary>
        /// Initializes properties and files
        /// </summary>
        /// <param name="TokenQueue"></param>
        /// <param name="scanner"></param>
        /// <param name="fileName"></param>
        public Parser(Queue<Token> TokenQueue, LexicalAnalyzer scanner,
                        string fileName)
        {
            this.tokenQueue = TokenQueue;
            this.scanner = scanner;
            this.fileName = fileName;
            analyzer = new SemanticAnalyzer(fileName);
            UsedRules = new StreamWriter("parse-tree.txt");
            UsedRules.WriteLine(fileName);
            Move();
        }

        /// <summary>
        /// Throws a syntax exception with a given error string
        /// </summary>
        /// <param name="errorString"></param>
        private void Error (string errorString)
        {
            throw new SyntaxException("Error at line: " + lookAheadToken.line + " Column: " + 
                (lookAheadToken.column) + " " + errorString);
        }

        /// <summary>
        /// Matches a lookahead token with a tag
        /// </summary>
        /// <param name="tag"></param>
        private void Match (int tag)
        {
           
            if((int)lookAheadToken.tag == tag)
            {
                Move();
            }
            else
            {
                Error("Syntax Error");
            }
            
        }

        /// <summary>
        /// Remove a token from the queue
        /// </summary>
        private void Move ()
        {
            lookAheadToken = tokenQueue.Dequeue();
        }

        /// <summary>
        /// SystemGoal - This is where parser starts
        /// </summary>
        public void SystemGoal ()
        {
            UsedRules.WriteLine("1");
            Program();
            UsedRules.Close();
            analyzer.cilOutput.Close();
            //If we find the EOF then the parser is done here, if not then there 
            //was an error
            if(lookAheadToken.tag != Tags.MP_EOF)
            {
                Error("Expecting EOF but found " + lookAheadToken.lexeme);
            }
            
        }
        /// <summary>
        /// Parse Program
        /// </summary>
        private void Program ()
        {
            string programIdentifierRecord = string.Empty;

            switch (lookAheadToken.tag)
            {
                case Tags.MP_PROGRAM:
                    UsedRules.WriteLine("2");
                    ProgramHeading(ref programIdentifierRecord);
                    Match(';');
                    Block(programIdentifierRecord);
                    analyzer.symbolTableStack.Pop();
                    Match('.');
                    analyzer.GenerateDelegateDeclaration();
                    analyzer.GenerateClosingBrace();
                    break;
                default:
                    Error("Expecting Program found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse ProgramHeading
        /// </summary>
        /// <param name="programIdentifierRecord"></param>
        private void ProgramHeading(ref string programIdentifierRecord) 
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_PROGRAM:
                    UsedRules.WriteLine("3");
                    Match((int)Tags.MP_PROGRAM);
                    Identifier(ref programIdentifierRecord);
                    analyzer.CreateSymbolTable(programIdentifierRecord);
                    analyzer.GenerateProgramInitialize(programIdentifierRecord);
                    break;
                default:
                    Error("Expecting Program found " + lookAheadToken.lexeme);
                break;
            }
        }
        
        /// <summary>
        /// Parse Block
        /// </summary>
        /// <param name="identifierRecord"></param>
        private void Block (string identifierRecord) 
        {
            switch (lookAheadToken.tag)
            {
                case Tags.MP_VAR:
                case Tags.MP_PROCEDURE:
                case Tags.MP_FUNCTION:
                case Tags.MP_BEGIN:
                    UsedRules.WriteLine("4");
                    analyzer.GenerateClassDeclaration(identifierRecord);
                    VariableDeclarationPart();                    
                    analyzer.GenerateClassConstructor(identifierRecord);
                    ProcedureAndFunctionDeclarationPart();
                    analyzer.GenerateFields();
                    analyzer.GenerateClosingBrace();
                    analyzer.GenerateMethodDeclaration(identifierRecord);
                    StatementPart();
                    analyzer.GenerateReturn();
                    break;
                default:
                    Error("Expecting ProgramBlock found " + lookAheadToken.lexeme);
                    break;
            }

        }

        /// <summary>
        /// Parse VariableDeclarationPart
        /// </summary>
        private void VariableDeclarationPart () 
        {
            switch(lookAheadToken.tag)
            {
                //"var" Variable Declaration ";" VariableDeclationTail
                case Tags.MP_VAR: 
                    UsedRules.WriteLine("5");
                    Match((int)Tags.MP_VAR);
                    VariableDeclaration();
                    Match(';');
                    VariableDeclarationTail();
                    break;
                    //This
                case Tags.MP_PROCEDURE: case Tags.MP_FUNCTION: case Tags.MP_BEGIN: //lambda
                    UsedRules.WriteLine("6");
                    break;
                default:
                    Error("Expecting VariableDeclarationPart but found " + lookAheadToken.lexeme);
                    break;
                
            }
        }

        /// <summary>
        /// Parse ProcedureAndFunctionDeclarationPart
        /// </summary>
        private void ProcedureAndFunctionDeclarationPart () 
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_PROCEDURE: // PeocedureDeclaration ProcedureAndFunctionDeclarationPart
                    UsedRules.WriteLine("12");
                    ProcedureDeclaration();
                    ProcedureAndFunctionDeclarationPart();
                    break;
                case Tags.MP_FUNCTION: // FunctionDeclaration ProcedureAndFunctionDeclarationPart
                    UsedRules.WriteLine("13");
                    FunctionDeclaration();
                    ProcedureAndFunctionDeclarationPart();
                    break;
                case Tags.MP_BEGIN: //lamda
                    UsedRules.WriteLine("14");
                    break;
                default:
                    Error("Expecting ProcedureAndFunctionDeclarationPart but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse VariableDeclarationTail
        /// </summary>
        private void VariableDeclarationTail () 
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_IDENTIFIER: // VariableDeclaration ";" VariableDeclarationTail
                    UsedRules.WriteLine("7");
                    VariableDeclaration();
                    Match(';');
                    VariableDeclarationTail();
                    break;
                case Tags.MP_PROCEDURE:
                case Tags.MP_FUNCTION:
                case Tags.MP_BEGIN:// lamda
                    UsedRules.WriteLine("8");
                    break;
                default:
                    Error("Expecting VariableDeclarationTail but found " + lookAheadToken.lexeme);
                    break;
            }
            
        }

        /// <summary>
        /// Parse VariableDeclaration
        /// </summary>
        private void VariableDeclaration () 
        {
            List<string> identifierRecordList = new List<string>();
            TypeRecord typeRecord = new TypeRecord(SymbolType.VariableSymbol, VariableType.Null);
            switch(lookAheadToken.tag)
            {
                case Tags.MP_IDENTIFIER:
                    UsedRules.WriteLine("9");
                    IdentifierList(ref identifierRecordList);
                    Match(':');
                    Type(ref typeRecord);
                    analyzer.SymbolTableInsert(identifierRecordList, typeRecord);
                    break;
                default:
                    Error("Expecting VariableDeclaration but found " + lookAheadToken.lexeme);
                    break;
            }
            
        }

        /// <summary>
        /// Parse Type
        /// </summary>
        /// <param name="typeRecord"></param>
        private void Type (ref TypeRecord typeRecord) 
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_INTEGER: // Integer
                    UsedRules.WriteLine("10");
                    Match((int)Tags.MP_INTEGER);
                    typeRecord.variableType = VariableType.Integer;
                    break;
                case Tags.MP_FLOAT: // Float
                    UsedRules.WriteLine("11");
                    Match((int)Tags.MP_FLOAT);
                    typeRecord.variableType = VariableType.Float;
                    break;
                default:
                    Error("Expecting Type but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse ProcedureDeclaration
        /// </summary>
        private void ProcedureDeclaration () 
        {
            MethodRecord procedureIdentifierRecord = new MethodRecord(SymbolType.ProcedureSymbol);

            switch(lookAheadToken.tag)
            {
                case Tags.MP_PROCEDURE:
                    UsedRules.WriteLine("15");
                    ProcedureHeading(procedureIdentifierRecord);
                    Match(';');
                    Block(procedureIdentifierRecord.name);
                    analyzer.symbolTableStack.Pop();
                    Match(';');
                    break;
                default:
                    Error("Expecting ProcedureDeclaration but found " + lookAheadToken.lexeme);
                    break;

            }
        }

        /// <summary>
        /// Parse FunctionDeclaration
        /// </summary>
        private void FunctionDeclaration () 
        {
            MethodRecord functionIdentifierRecord = new MethodRecord(SymbolType.FunctionSymbol);
            switch (lookAheadToken.tag)
            {
                case Tags.MP_FUNCTION:
                    UsedRules.WriteLine("16");
                    FunctionHeading(functionIdentifierRecord);
                    Match(';');
                    Block(functionIdentifierRecord.name);
                    analyzer.symbolTableStack.Pop();
                    Match(';');
                    break;
                default:
                    Error("Expecting FunctionDeclaration but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse ProcedureHeading
        /// </summary>
        /// <param name="procedureRecord"></param>
        private void ProcedureHeading (MethodRecord procedureRecord) 
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_PROCEDURE:                    
                    UsedRules.WriteLine("17");
                    Match((int)Tags.MP_PROCEDURE);
                    Identifier(procedureRecord);
                    OptionalFormalParameterList(procedureRecord.parameterList);
                    analyzer.SymbolTableInsert(procedureRecord);
                    analyzer.CreateSymbolTable(procedureRecord.name);
                    break;
                default:
                    Error("Expecting ProcedureHeading but found " + lookAheadToken.lexeme);
                    break;
            }
            

        }

        /// <summary>
        /// Parse FunctionHeading
        /// </summary>
        /// <param name="functionRecord"></param>
        private void FunctionHeading (MethodRecord functionRecord) 
        {
            TypeRecord typeRecord = new TypeRecord(SymbolType.FunctionSymbol, VariableType.Null);

            switch (lookAheadToken.tag)
            {
                case Tags.MP_FUNCTION:
                    UsedRules.WriteLine("18");
                    Match((int)Tags.MP_FUNCTION);
                    Identifier(functionRecord);
                    OptionalFormalParameterList(functionRecord.parameterList);
                    Type(ref typeRecord);
                    functionRecord.returnType = typeRecord.variableType;
                    //add a record into the current symbol table for the function
                    analyzer.SymbolTableInsert(functionRecord);
                    analyzer.CreateSymbolTable(functionRecord.name);
                    break;
                default:
                    Error("Expecting FunctionHeading but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse OptionalFormalParameterList
        /// </summary>
        /// <param name="parameterList"></param>
        private void OptionalFormalParameterList (List<Parameter> parameterList) 
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_LPAREN: // "(" FormalParameterSection FormalParameterSectionTail ")"
                    UsedRules.WriteLine("19");
                    Match('(');
                    FormalParameterSection(parameterList);
                    FormalParameterSectionTail(parameterList);
                    Match(')');
                    break;
                case Tags.MP_SCOLON:
                case Tags.MP_INTEGER:
                case Tags.MP_FLOAT:// lamda
                    UsedRules.WriteLine("20");
                    break;
                default:
                    Error("Expecting OptionalFormalParameterList but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse FormalParameterSectionTail
        /// </summary>
        /// <param name="parameters"></param>
        private void FormalParameterSectionTail (List<Parameter> parameters) 
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_SCOLON: // ";" FormalParameterSection FormalParameterSectionTail
                    UsedRules.WriteLine("21");
                    Match(';');
                    FormalParameterSection(parameters);
                    FormalParameterSectionTail(parameters);
                    break;
                case Tags.MP_RPAREN: // lamda
                    UsedRules.WriteLine("22");
                    break;
                default:
                    Error("Expecting FormalParameterSectionTail but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse FormalParameterSection
        /// </summary>
        /// <param name="parameters"></param>
        private void FormalParameterSection (List<Parameter> parameters) 
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_IDENTIFIER: // ValueParameterSection
                    UsedRules.WriteLine("23");
                    ValueParameterSection(parameters);
                    break;
                case Tags.MP_VAR: // VariableParameterSection
                    UsedRules.WriteLine("24");
                    VariableParameterSection(parameters);
                    break;
                default:
                    Error("Expecting FormalParameterSection but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse ValueParameterSection
        /// </summary>
        /// <param name="parameters"></param>
        private void ValueParameterSection (List<Parameter> parameters) 
        {
            TypeRecord typeRecord = new TypeRecord(SymbolType.ParameterSymbol, VariableType.Null);
            List<string> identifierList = new List<string>();

             switch(lookAheadToken.tag)
            {
                 case Tags.MP_IDENTIFIER:
                    UsedRules.WriteLine("25");
                    IdentifierList(ref identifierList);
                    Match(':');
                    Type(ref typeRecord);

                     //TODO: must make the list of parameters
                     break;
                 default:
                     Error("Expecting ValueParameterSection but found " + lookAheadToken.lexeme);
                     break;
            }
        }

        /// <summary>
        /// Parse VariableParameterSection
        /// </summary>
        /// <param name="parameters"></param>
        private void VariableParameterSection (List<Parameter> parameters) 
        {
            TypeRecord typeRecord = new TypeRecord(SymbolType.ParameterSymbol, VariableType.Null);
            List<string> identifierList = new List<string>();

            switch (lookAheadToken.tag)
            {
                case Tags.MP_VAR:
                    UsedRules.WriteLine("26");
                    Match((int)Tags.MP_VAR);
                    IdentifierList(ref identifierList);
                    Match(':');
                    Type(ref typeRecord);
                    //TODO: must make the list of parameters
                    break;
                default:
                    Error("Expecting VariableParameterSection but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse StatementPart
        /// </summary>
        private void StatementPart ()
        {
            switch (lookAheadToken.tag)
            {
                case Tags.MP_BEGIN:
                    UsedRules.WriteLine("27");
                    CompoundStatement();
                    break;
                default:
                    Error("Expecting StatementPart but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse CompoundStatement
        /// </summary>
        private void CompoundStatement () 
        {
            switch (lookAheadToken.tag)
            {
                case Tags.MP_BEGIN:
                    UsedRules.WriteLine("28");
                    Match((int)Tags.MP_BEGIN);
                    StatementSequence();
                    Match((int)Tags.MP_END);
                    break;
                default:
                    Error("Expecting CompoundStatement but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse Statement Sequence
        /// </summary>
        private void StatementSequence () 
        {
            switch (lookAheadToken.tag)
            {
                case Tags.MP_SCOLON:
                case Tags.MP_BEGIN:
                case Tags.MP_END:
                case Tags.MP_READ:
                case Tags.MP_WRITE:
                case Tags.MP_IF:
                case Tags.MP_ELSE:
                case Tags.MP_REPEAT:
                case Tags.MP_UNTIL:
                case Tags.MP_WHILE:
                case Tags.MP_FOR:
                case Tags.MP_IDENTIFIER:
                    UsedRules.WriteLine("29");
                    Statement();
                    StatementTail();
                    break;
                default:
                    Error("Expecting Statementsequence but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse StatementTail
        /// </summary>
        private void StatementTail () 
        {
            switch (lookAheadToken.tag)
            {
                case Tags.MP_SCOLON:  //";" Statement StatementTail
                    UsedRules.WriteLine("30");
                    Match((int)Tags.MP_SCOLON);
                    Statement();
                    StatementTail();
                    break;

                case Tags.MP_END:
                case Tags.MP_UNTIL://lambda
                    UsedRules.WriteLine("31");
                    break;
                default:
                    Error("Expecting StatementTail but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse Statement
        /// </summary>
        private void Statement () 
        {
            switch (lookAheadToken.tag)
            {
                case Tags.MP_SCOLON:
                case Tags.MP_END:
                case Tags.MP_ELSE:
                case Tags.MP_UNTIL:// EmptyStatement
                    UsedRules.WriteLine("32");
                    EmptyStatement();
                    break;
                case Tags.MP_BEGIN: // CompoundStatement
                    UsedRules.WriteLine("33");
                    CompoundStatement();
                    break;
                case Tags.MP_READ: //ReadStatement
                    UsedRules.WriteLine("34");
                    ReadStatement();
                    break;
                case Tags.MP_WRITE: //WriteStatement
                    UsedRules.WriteLine("35");
                    WriteStatement();
                    break;
                case Tags.MP_IDENTIFIER: //AssignmentStatement
                    if(tokenQueue.Peek().tag == Tags.MP_ASSIGN)
                    {
                        UsedRules.WriteLine("36");
                        AssignmentStatement();
                    }
                    else
                    {
                        UsedRules.WriteLine("41");
                        ProcedureStatement();
                    }
                    break;
                case Tags.MP_IF: //IfStatement
                    UsedRules.WriteLine("7");
                    IfStatement();
                    break;
                case Tags.MP_WHILE: //WhileStatement
                    UsedRules.WriteLine("38");
                    WhileStatement();
                    break;
                case Tags.MP_REPEAT: //RepeatStatement
                    UsedRules.WriteLine("39");
                    RepeatStatement();
                    break;
                case Tags.MP_FOR: //ForStatement
                    UsedRules.WriteLine("40");
                    ForStatement();
                    break;                    
                default:
                    Error("Expecting Statement but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse Empty Statement
        /// </summary>
        private void EmptyStatement () 
        {
            switch (lookAheadToken.tag)
            {
                case Tags.MP_SCOLON:
                case Tags.MP_END:
                case Tags.MP_ELSE:
                case Tags.MP_UNTIL://lambda
                    UsedRules.WriteLine("42");
                    break;
                default:
                    Error("Expecting EmptyStatement but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse ReadStatement
        /// </summary>
        private void ReadStatement () 
        {
            IdentifierRecord idRecord = new IdentifierRecord();
            switch(lookAheadToken.tag)
            {
                case Tags.MP_READ:
                    UsedRules.WriteLine("43");
                    Match((int)Tags.MP_READ);
                    Match((int)Tags.MP_LPAREN);
                    ReadParameter(idRecord);
                    analyzer.GenerateLoadObject();
                    analyzer.GenerateReadStatement(idRecord);
                    ReadParameterTail();
                    Match((int)Tags.MP_RPAREN);
                    break;
                default:
                    Error("Expecting Read but found " + lookAheadToken.tag);
                    break;
            }
                    
        }

        /// <summary>
        /// Parse ReadParameterTail
        /// </summary>
        private void ReadParameterTail()
        {
            IdentifierRecord idRecord = new IdentifierRecord();
            switch (lookAheadToken.tag)
            {
                case Tags.MP_COMMA:  //"," ReadParameter ReadParameterTail
                    UsedRules.WriteLine("44");
                    Match((int)Tags.MP_COMMA);
                    ReadParameter(idRecord);
                    analyzer.GenerateLoadObject();
                    analyzer.GenerateReadStatement(idRecord);
                    ReadParameterTail();
                    break;

                case Tags.MP_RPAREN: //lambda
                    UsedRules.WriteLine("45");
                    break;
                default:
                    Error("Expecting ReadParameterTail but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse ReadParameter
        /// </summary>
        /// <param name="idRecord"></param>
        private void ReadParameter (IdentifierRecord idRecord) 
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_IDENTIFIER:
                    string idRecName = null;
                    UsedRules.WriteLine("46");
                    Identifier(ref idRecName);
                    idRecord.lexeme = idRecName;
                    analyzer.ProcessId(idRecord);
                    break;
                default:
                    Error("Expecting ID found " + lookAheadToken.tag);
                    break;
             }
        }

        /// <summary>
        /// Parse WriteStatement
        /// </summary>
        private void WriteStatement () 
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_WRITE:
                    UsedRules.WriteLine("47");
                    Match((int)Tags.MP_WRITE);
                    Match((int)Tags.MP_LPAREN);
                    WriteParameter();
                    analyzer.GenerateWriteStatement();
                    WriteParameterTail();
                    Match((int)Tags.MP_RPAREN);
                break;
                default:
                    Error("Expecting Write, found " + lookAheadToken.tag);
                break;
            }
        }

        /// <summary>
        /// Parse WriteParameterTail
        /// </summary>
        private void WriteParameterTail () 
        {
            switch (lookAheadToken.tag)
            {
                case Tags.MP_COMMA:  //"," WriteParameter WriteParameterTail
                    UsedRules.WriteLine("48");
                    Match((int)Tags.MP_COMMA);
                    WriteParameter();
                    analyzer.GenerateWriteStatement();
                    WriteParameterTail();
                    break;
                case Tags.MP_RPAREN: //lambda
                    UsedRules.WriteLine("49");
                    break;
                default:
                    Error("Expecting WriteParameterTail but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse WriteParameter
        /// </summary>
        private void WriteParameter() 
        {
            VariableType ordinalExpressionRecord = VariableType.Null;
            
            switch(lookAheadToken.tag)
            {
                case Tags.MP_LPAREN:
                case Tags.MP_PLUS:
                case Tags.MP_MINUS:
                case Tags.MP_INTEGER_LIT:
                case Tags.MP_NOT:
                case Tags.MP_IDENTIFIER:
                    UsedRules.WriteLine("50");
                    OrdinalExpression(ref ordinalExpressionRecord);
                break;
                default:
                    Error("Expecting WriteParameter but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse AssignmentStatement
        /// </summary>
        private void AssignmentStatement () 
        {
            IdentifierRecord idRecord = new IdentifierRecord();
            VariableType expressionRecord = VariableType.Null;
            string idRecName = null;
            switch (lookAheadToken.tag)
            {
                case Tags.MP_IDENTIFIER: // Identifier ":=" Expression
                    UsedRules.WriteLine("51");
                    Identifier(ref idRecName);
                    idRecord.lexeme = idRecName;
                    analyzer.ProcessId(idRecord);
                    Match((int)Tags.MP_ASSIGN);
                    analyzer.GenerateLoadObject();
                    Expression(ref expressionRecord);
                    analyzer.GenerateAssign(idRecord, expressionRecord);
                    break;
                default:
                    Error("Expecting AssignmentStatement but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse IfStatement
        /// </summary>
        private void IfStatement() 
        {
            string ifLabelRecord = string.Empty, elseLabelRecord = string.Empty;

            switch(lookAheadToken.tag)
            {
                case Tags.MP_IF:
                    UsedRules.WriteLine("52");
                    Match((int)Tags.MP_IF);
                    BooleanExpression();
                    analyzer.GenerateBranch(ref ifLabelRecord, BranchType.brfalse);
                    Match((int)Tags.MP_THEN);
                    Statement();
                    analyzer.GenerateBranch(ref elseLabelRecord, BranchType.br);
                    analyzer.GenerateLabel(ref ifLabelRecord);
                    OptionalElsePart();
                    analyzer.GenerateLabel(ref elseLabelRecord);
                    break;
                default:
                    Error("Expecting IfStatement but foud " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse OptionalElsePart
        /// </summary>
        private void OptionalElsePart() 
        {
            switch (lookAheadToken.tag)
            {
                case Tags.MP_ELSE: // "else" Statement  
                    UsedRules.WriteLine("53");
                    Match((int)Tags.MP_ELSE);
                    Statement();
                    break;
                case Tags.MP_SCOLON: case Tags.MP_END: case Tags.MP_UNTIL:
                    UsedRules.WriteLine("54");
                    break;
                default:
                    Error("Expecting OptionalElsePart but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse RepeatStatement
        /// </summary>
        private void RepeatStatement() 
        {
            string startLoopLabel = string.Empty;

            switch(lookAheadToken.tag)
            {
                case Tags.MP_REPEAT:
                    UsedRules.WriteLine("55");
                    Match((int)Tags.MP_REPEAT);
                    analyzer.GenerateLabel(ref startLoopLabel);
                    StatementSequence();
                    Match((int)Tags.MP_UNTIL);                    
                    BooleanExpression();  
                    analyzer.GenerateBranch(ref startLoopLabel, BranchType.brfalse);
                    break;
                default:
                    Error("Expecting RepeatStatement but found " + lookAheadToken.lexeme);
                    break;
            }
            
        }

        /// <summary>
        /// Parse WhileStatement
        /// </summary>
        private void WhileStatement () 
        {
            string controlLabelRecord = string.Empty;
            string loopLabel = string.Empty;

            switch(lookAheadToken.tag)
            {
                case Tags.MP_WHILE:
                    UsedRules.WriteLine("56");
                    Match((int)Tags.MP_WHILE);
                    analyzer.GenerateLabel(ref controlLabelRecord);
                    BooleanExpression();
                    Match((int)Tags.MP_DO);
                    analyzer.GenerateBranch(ref loopLabel, BranchType.brfalse);
                    Statement();
                    analyzer.GenerateBranch(ref controlLabelRecord, BranchType.br);
                    analyzer.GenerateLabel(ref loopLabel);
                    break;
                default:
                    Error("Expecting WhileStatement but found " + lookAheadToken.lexeme);
                    break;

            }
        }

        /// <summary>
        /// Parse ForStatement
        /// </summary>
        private void ForStatement () 
        {
            IdentifierRecord controlVariableRecord = new IdentifierRecord();
            VariableType finalValueRecord = VariableType.Null;
            VariableType controlStatementRecord = VariableType.Null;
            string controlLabelRecord = string.Empty;
            string loopLabel = string.Empty;
            ControlRecord stepValueRecord = new ControlRecord();

            switch(lookAheadToken.tag)
            {
                case Tags.MP_FOR:
                    UsedRules.WriteLine("57");
                    Match((int)Tags.MP_FOR);
                    ControlVariable(ref controlVariableRecord);
                    Match((int)Tags.MP_ASSIGN);
                    InitialValue(ref controlVariableRecord);
                    analyzer.GenerateLabel(ref controlLabelRecord);
                    analyzer.GenerateIdPush(controlVariableRecord,ref controlStatementRecord);                      
                    StepValue(ref stepValueRecord);
                    FinalValue(ref finalValueRecord);
                    analyzer.GenerateBranch(ref loopLabel, stepValueRecord.branchType);
                    Match((int)Tags.MP_DO);
                    Statement();
                    analyzer.GenerateIncrement(ref controlVariableRecord, stepValueRecord.addingOperator);
                    analyzer.GenerateBranch(ref controlLabelRecord, BranchType.br);
                    analyzer.GenerateLabel(ref loopLabel);
                    break;
                default:
                    Error("Expecting ForStatement found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse Control Variable
        /// </summary>
        /// <param name="controlVariableRecord"></param>
        private void ControlVariable (ref IdentifierRecord controlVariableRecord) 
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_IDENTIFIER:
                    string procedureIdentifier = null;
                    UsedRules.WriteLine("58");
                    Identifier(ref procedureIdentifier);
                    controlVariableRecord.lexeme = procedureIdentifier;
                    analyzer.ProcessId(controlVariableRecord);
                    break;
                default:
                    Error("Expecting ControlVariable but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse InitialValue
        /// </summary>
        /// <param name="initialValueRecord"></param>
        private void InitialValue (ref IdentifierRecord initialValueRecord) 
        {
            VariableType ordinalExpressionRecord = VariableType.Null;

            switch (lookAheadToken.tag)
            {
                case Tags.MP_LPAREN:
                case Tags.MP_PLUS:
                case Tags.MP_MINUS:
                case Tags.MP_INTEGER_LIT:
                case Tags.MP_NOT:
                case Tags.MP_IDENTIFIER:
                    UsedRules.WriteLine("59");
                    analyzer.GenerateLoadObject();
                    OrdinalExpression(ref ordinalExpressionRecord);
                    analyzer.GenerateAssign(initialValueRecord, ordinalExpressionRecord);
                    break;
                default:
                    Error("Expecting InitialValue but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse StepValue
        /// </summary>
        /// <param name="stepValueRecord"></param>
        private void StepValue(ref ControlRecord stepValueRecord) 
        {
            switch (lookAheadToken.tag)
            {
                case Tags.MP_TO: // "to"
                    UsedRules.WriteLine("60");
                    stepValueRecord.addingOperator = "add";
                    stepValueRecord.branchType = BranchType.bgt;
                    Match((int)Tags.MP_TO);
                    break;
                case Tags.MP_DOWNTO: //"downto"
                    UsedRules.WriteLine("61");
                    stepValueRecord.addingOperator = "sub";
                    stepValueRecord.branchType = BranchType.blt;
                    Match((int)Tags.MP_DOWNTO);
                    break;
                default:
                    Error("Expecting StepValue but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse FinalValue
        /// </summary>
        /// <param name="ordinalExpressionRecord"></param>
        private void FinalValue (ref VariableType ordinalExpressionRecord) 
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_LPAREN:
                case Tags.MP_PLUS:
                case Tags.MP_MINUS:
                case Tags.MP_INTEGER_LIT:
                case Tags.MP_NOT:
                case Tags.MP_IDENTIFIER:
                    UsedRules.WriteLine("62");
                    OrdinalExpression(ref ordinalExpressionRecord);
                    break;
                default:
                Error("Expecting FinalValue but found " + lookAheadToken.lexeme);
                break;
            }
        }

        /// <summary>
        /// Parse ProcedureStatement
        /// </summary>
        private void ProcedureStatement() 
        {
            string procedureIdentifier = null;
            switch (lookAheadToken.tag)
            {
                case Tags.MP_IDENTIFIER:
                    UsedRules.WriteLine("63");
                    Identifier(ref procedureIdentifier);
                    OptionalActualParameterList();
                    break;
                default:
                    Error("Expecting ProcedureStatement but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse OptionalActualParameterList
        /// </summary>
        private void OptionalActualParameterList() 
        {
            switch (lookAheadToken.tag)
            {
                case Tags.MP_LPAREN: // "(" ActualParameter ActualParameterTail ")" 
                    UsedRules.WriteLine("64");
                    Match((int)Tags.MP_LPAREN);
                    ActualParameter();
                    ActualParameterTail();
                    Match((int)Tags.MP_RPAREN);
                    break;
                case Tags.MP_SCOLON:
                case Tags.MP_RPAREN:
                case Tags.MP_END:
                case Tags.MP_COMMA:
                case Tags.MP_THEN:
                case Tags.MP_ELSE:
                case Tags.MP_UNTIL:
                case Tags.MP_DO:
                case Tags.MP_TO:
                case Tags.MP_DOWNTO:
                case Tags.MP_EQUAL:
                case Tags.MP_LTHAN:
                case Tags.MP_GTHAN:
                case Tags.MP_LEQUAL:
                case Tags.MP_GEQUAL:
                case Tags.MP_NEQUAL:
                case Tags.MP_PLUS:
                case Tags.MP_MINUS:
                case Tags.MP_OR:
                case Tags.MP_TIMES:
                case Tags.MP_DIV:
                case Tags.MP_MOD:
                case Tags.MP_AND:// Lambda
                    UsedRules.WriteLine("65");
                    break;
                default:
                    Error("Expecting OptionalActualParameterList but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse Identifier
        /// </summary>
        /// <param name="programIdenentifierRecord"></param>
        private void Identifier (ref string programIdenentifierRecord)
        {
            programIdenentifierRecord = lookAheadToken.lexeme;
            Match((int)Tags.MP_IDENTIFIER);            
        }

        /// <summary>
        /// Parse Identifier
        /// </summary>
        /// <param name="programIdenentifierRecord"></param>
        private void Identifier(MethodRecord programIdenentifierRecord)
        {
            programIdenentifierRecord.name = lookAheadToken.lexeme;
            Match((int)Tags.MP_IDENTIFIER);
        } 

        /// <summary>
        /// Parse ActualParameterTail
        /// </summary>
        private void ActualParameterTail()
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_COMMA: //"," ActualParameter ActualParameterTail
                    UsedRules.WriteLine("66");
                    Match((int)Tags.MP_COMMA);
                    ActualParameter();
                    ActualParameterTail();
                    break;

                case Tags.MP_RPAREN: //lambda
                    UsedRules.WriteLine("67");
                    break;
                default:
                    Error("Expecting ActualParameterTail but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse ActualParameter
        /// </summary>
        private void ActualParameter()
        {
            VariableType ordinalExpressionRecord = VariableType.Null;

            switch (lookAheadToken.tag)
            {
                case Tags.MP_LPAREN:
                case Tags.MP_PLUS:
                case Tags.MP_MINUS:
                case Tags.MP_INTEGER_LIT:
                case Tags.MP_NOT:
                case Tags.MP_IDENTIFIER:
                    UsedRules.WriteLine("68");
                    OrdinalExpression(ref ordinalExpressionRecord);
                    break;
                default:
                    Error("Expecting ActualParameter but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse Expression
        /// </summary>
        /// <param name="expressionRecord"></param>
        private void Expression(ref VariableType expressionRecord)
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_LPAREN:
                case Tags.MP_PLUS:
                case Tags.MP_MINUS:
                case Tags.MP_INTEGER_LIT:
                case Tags.MP_NOT:
                case Tags.MP_IDENTIFIER:
                    UsedRules.WriteLine("69");
                    SimpleExpression(ref expressionRecord);
                    OptionalRelationalPart(ref expressionRecord);
                break;
                default:
                    Error("Expecting Expression Args found " + lookAheadToken.lexeme);
                break;
            }
        }

        /// <summary>
        /// Parse OptionalRelationalPart
        /// </summary>
        /// <param name="expressionRecord"></param>
        private void OptionalRelationalPart(ref VariableType expressionRecord)
        {
            VariableType simpleExpressionRecord = VariableType.Null;
            VariableType resultRecord = VariableType.Null;
            string relationalOpRecord = string.Empty;

            switch (lookAheadToken.tag)
            {
                case Tags.MP_EQUAL:
                case Tags.MP_LTHAN:
                case Tags.MP_GTHAN:
                case Tags.MP_LEQUAL:
                case Tags.MP_GEQUAL:
                case Tags.MP_NEQUAL://RelationalOperator SimpleExpression 
                    UsedRules.WriteLine("70");
                    RelationalOperator(ref relationalOpRecord);
                    SimpleExpression(ref simpleExpressionRecord);
                    analyzer.GenerateArithmetic(expressionRecord, relationalOpRecord, 
                        simpleExpressionRecord, ref resultRecord);
                    break;

                case Tags.MP_SCOLON:
                case Tags.MP_RPAREN:
                case Tags.MP_END:
                case Tags.MP_COMMA:
                case Tags.MP_THEN:
                case Tags.MP_ELSE:
                case Tags.MP_UNTIL:
                case Tags.MP_DO:
                case Tags.MP_TO:
                case Tags.MP_DOWNTO://lambda
                    UsedRules.WriteLine("71");
                    break;
                default:
                    Error("Expecting OptionalRelationalPart but found " + lookAheadToken.lexeme);
                    break;

            }
        }

        /// <summary>
        /// Parse RelationalOperator
        /// </summary>
        /// <param name="relationalOpRecord"></param>
        private void RelationalOperator(ref string relationalOpRecord)
        {
            relationalOpRecord = lookAheadToken.lexeme;
            switch (lookAheadToken.tag)
            {
                case Tags.MP_EQUAL:
                    UsedRules.WriteLine("72");
                    Match('=');
                    break;

                case Tags.MP_LTHAN:
                    UsedRules.WriteLine("73");
                    Match('<');
                    break;

                case Tags.MP_GTHAN:
                    UsedRules.WriteLine("74");
                    Match('>');
                    break;
                
                case Tags.MP_LEQUAL:
                    UsedRules.WriteLine("75");
                    Match((int)Tags.MP_LEQUAL);                    
                    break;
                case Tags.MP_GEQUAL:
                    UsedRules.WriteLine("76");
                    Match((int)Tags.MP_GEQUAL);
                    break;
                case Tags.MP_NEQUAL:
                    UsedRules.WriteLine("77");
                    Match((int)Tags.MP_NEQUAL);
                    break;
                default:
                    Error("Expecting RelationalOperator but found " + lookAheadToken.lexeme);
                    break;

            }
        }

        /// <summary>
        /// Parse SimpleExpression
        /// </summary>
        /// <param name="simpleExpressionRecord"></param>
        private void SimpleExpression(ref VariableType simpleExpressionRecord)
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_LPAREN:
                case Tags.MP_PLUS:
                case Tags.MP_MINUS:
                case Tags.MP_INTEGER_LIT:
                case Tags.MP_NOT:
                case Tags.MP_IDENTIFIER:
                    UsedRules.WriteLine("78");
                    OptionalSign();
                    Term(ref simpleExpressionRecord);
                    TermTail(ref simpleExpressionRecord);
                break;
                default:
                    Error("Expecting Simple Expression found " + lookAheadToken.lexeme);
                break;
            }
        }

        /// <summary>
        /// Parse TermTail
        /// </summary>
        /// <param name="termTailRecord"></param>
        private void TermTail(ref VariableType termTailRecord)
        {
            string addOpRecord = null;
            VariableType termRecord = VariableType.Null;
            VariableType resultRecord = VariableType.Null;

            switch(lookAheadToken.tag)
            {
                //AddingOperator Term TermTail
                case Tags.MP_PLUS:
                case Tags.MP_MINUS:
                case Tags.MP_OR:
                    UsedRules.WriteLine("79");
                    AddingOperator(ref addOpRecord);
                    Term(ref termRecord);
                    analyzer.GenerateArithmetic(termTailRecord, addOpRecord, termRecord, 
                                                    ref resultRecord);
                    TermTail(ref resultRecord);
                    break;

                case Tags.MP_SCOLON: 
                case Tags.MP_RPAREN:
                case Tags.MP_END:
                case Tags.MP_COMMA:
                case Tags.MP_THEN:
                case Tags.MP_ELSE:
                case Tags.MP_UNTIL:
                case Tags.MP_DO:
                case Tags.MP_TO:
                case Tags.MP_DOWNTO:
                case Tags.MP_EQUAL:
                case Tags.MP_LTHAN:
                case Tags.MP_GTHAN:
                case Tags.MP_LEQUAL:
                case Tags.MP_GEQUAL:
                case Tags.MP_NEQUAL:
                    UsedRules.WriteLine("80");
                    //lambda                    
                    break;
                default:
                    Error("Expecting TermTail but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse OptionalSign
        /// </summary>
        private void OptionalSign()
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_PLUS:
                    UsedRules.WriteLine("81");
                    Match('+');
                    break;
                case Tags.MP_MINUS:
                    UsedRules.WriteLine("82");
                    Match('-');
                    break;
                case Tags.MP_LPAREN: //lambda
                case Tags.MP_INTEGER_LIT:
                case Tags.MP_NOT:
                case Tags.MP_IDENTIFIER:
                    UsedRules.WriteLine("83");
                    break;
                default:
                    Error("Expecting OptionalSign but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse AddingOperator
        /// </summary>
        /// <param name="addOpRecord"></param>
        private void AddingOperator(ref string addOpRecord)
        {
            addOpRecord = lookAheadToken.lexeme;
            switch (lookAheadToken.tag)
            {
                case Tags.MP_PLUS:
                    UsedRules.WriteLine("84");
                    Match('+');
                    break;
                case Tags.MP_MINUS:
                    UsedRules.WriteLine("85");
                    Match('-');
                    break;
                case Tags.MP_OR:
                    UsedRules.WriteLine("86");
                    Match((int)Tags.MP_OR);
                    break;
                default:
                    Error("Expecting AddingOperator but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse Term
        /// </summary>
        /// <param name="termRecord"></param>
        private void Term(ref VariableType termRecord)
        {
            switch(lookAheadToken.tag)
            {
                case Tags.MP_LPAREN:
                case Tags.MP_IDENTIFIER:
                case Tags.MP_INTEGER_LIT:
                case Tags.MP_NOT:
                    UsedRules.WriteLine("87");
                    Factor(ref termRecord);
                    FactorTail(ref termRecord);
                break;
                default:
                    Error("Expecting Term found " + lookAheadToken.tag);
                break;
            }
        }

        /// <summary>
        /// Parse MultiplyingOperator
        /// </summary>
        /// <param name="mulOpRecord"></param>
        private void MultiplyingOperator(ref string mulOpRecord)
        {

            mulOpRecord = lookAheadToken.lexeme;
            switch(lookAheadToken.tag)
            {
                case Tags.MP_TIMES:
                    UsedRules.WriteLine("90");
                    Match('*');
                    break;
                case Tags.MP_DIV:
                    UsedRules.WriteLine("91");
                    Match((int)Tags.MP_DIV);
                    break;
                case Tags.MP_MOD:
                    UsedRules.WriteLine("92");
                    Match((int)Tags.MP_MOD);
                    break;
                case Tags.MP_AND:
                    UsedRules.WriteLine("93");
                    Match((int)Tags.MP_AND);
                    break;                
                default:
                    Error("Expecting MultiplyingOperator but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse FactorTail
        /// </summary>
        /// <param name="factorTailRecord"></param>
        private void FactorTail(ref VariableType factorTailRecord)
        {
            string mulOpRecord = null;
            VariableType factorRecord = VariableType.Null, resultRecord = VariableType.Null;
            switch (lookAheadToken.tag)
            {
                case Tags.MP_TIMES:
                case Tags.MP_DIV:
                case Tags.MP_MOD:
                case Tags.MP_AND:
                    UsedRules.WriteLine("88");
                    MultiplyingOperator(ref mulOpRecord);
                    Factor(ref factorTailRecord);
                    analyzer.GenerateArithmetic(factorTailRecord, mulOpRecord, factorRecord,
                                                    ref resultRecord);
                    FactorTail(ref factorTailRecord);
                    break;

                case Tags.MP_SCOLON://lambda case!
                case Tags.MP_RPAREN:
                case Tags.MP_END:
                case Tags.MP_COMMA:
                case Tags.MP_THEN:
                case Tags.MP_ELSE:
                case Tags.MP_UNTIL:
                case Tags.MP_DO:
                case Tags.MP_TO:
                case Tags.MP_DOWNTO:
                case Tags.MP_EQUAL:
                case Tags.MP_LTHAN:
                case Tags.MP_GTHAN:
                case Tags.MP_LEQUAL:
                case Tags.MP_GEQUAL:
                case Tags.MP_NEQUAL:
                case Tags.MP_PLUS:
                case Tags.MP_MINUS:
                case Tags.MP_OR:
                    UsedRules.WriteLine("89");
                    break;
                default:
                    Error("Expecting FactorTail but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse Factor
        /// </summary>
        /// <param name="factorRecord"></param>
        private void Factor(ref VariableType factorRecord)
        {
            IdentifierRecord idRecord = new IdentifierRecord();
            LiteralRecord litRecord = new LiteralRecord();
            string idRecName = null;
            switch (lookAheadToken.tag)
            {
                case Tags.MP_INTEGER_LIT:
                    UsedRules.WriteLine("94");
                    litRecord.lexeme = lookAheadToken.lexeme;
                    litRecord.type = VariableType.Integer;
                    Match((int)Tags.MP_INTEGER_LIT);                    
                    analyzer.GenerateLitPush(litRecord, ref factorRecord);
                    break;
                case Tags.MP_NOT:
                    UsedRules.WriteLine("95");
                    Match((int)Tags.MP_NOT);
                    Factor(ref factorRecord);
                    break;
                case Tags.MP_LPAREN:
                    UsedRules.WriteLine("96");
                    Match((int)Tags.MP_LPAREN);
                    Expression(ref factorRecord);
                    Match((int)Tags.MP_RPAREN);
                    break;                    
                case Tags.MP_IDENTIFIER:
                    UsedRules.WriteLine("97");
                    Identifier(ref idRecName);
                    idRecord.lexeme = idRecName;
                    analyzer.ProcessId(idRecord);
                    analyzer.GenerateIdPush(idRecord, ref factorRecord);
                    OptionalActualParameterList();
                    break;
                default:
                    Error("Expecting Factor but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse BooleanExpression
        /// </summary>
        private void BooleanExpression()
        {
            switch (lookAheadToken.tag)
            {
                case Tags.MP_LPAREN:
                case Tags.MP_PLUS:
                case Tags.MP_MINUS:
                case Tags.MP_INTEGER:
                case Tags.MP_NOT:
                case Tags.MP_IDENTIFIER:
                    VariableType expressionRecord = VariableType.Null;
                    UsedRules.WriteLine("98");
                    Expression(ref expressionRecord);
                    break;
                default:
                Error("Expecting BooleanExpression but found " + lookAheadToken.lexeme);
                break;
            }
        }

        /// <summary>
        /// Parse OrdinalExpression
        /// </summary>
        /// <param name="expressionRecord"></param>
        private void OrdinalExpression(ref VariableType expressionRecord)
        {
            switch (lookAheadToken.tag)
            {
                case Tags.MP_LPAREN:
                case Tags.MP_PLUS:
                case Tags.MP_MINUS:
                case Tags.MP_IDENTIFIER:
                case Tags.MP_NOT:
                case Tags.MP_INTEGER_LIT:
                    UsedRules.WriteLine("99");
                    Expression(ref expressionRecord);
                    break;
                default:
                    Error("Expecting OridinalExpression but found " + lookAheadToken.lexeme);
                    break;
            }
            
        }

        /// <summary>
        /// Parse IdentifierList
        /// </summary>
        /// <param name="identifierRecordList"></param>
        private void IdentifierList (ref List<string> identifierRecordList)
        {            
            string identifierRecord = null;

            switch (lookAheadToken.tag)
            {
                case Tags.MP_IDENTIFIER:
                    UsedRules.WriteLine("100");
                    Identifier(ref identifierRecord);
                    analyzer.AddId(identifierRecord, identifierRecordList);
                    IdentifierTail(ref identifierRecordList);
                    break;
                default:
                    Error("Expecting OridinalExpression but found " + lookAheadToken.lexeme);
                    break;
            }
        }

        /// <summary>
        /// Parse IdentifierTail
        /// </summary>
        /// <param name="identifierRecordList"></param>
        private void IdentifierTail(ref List<string>identifierRecordList)
        {
            string identifierRecord = null;
            switch (lookAheadToken.tag)
            {
                case Tags.MP_COMMA:
                    UsedRules.WriteLine("101");
                    Match((int)Tags.MP_COMMA);
                    Identifier(ref identifierRecord);
                    analyzer.AddId(identifierRecord, identifierRecordList);
                    IdentifierTail(ref identifierRecordList);
                    break;
                case Tags.MP_COLON: //lambda 
                    UsedRules.WriteLine("102");
                    break;
                default:
                    Error("Expecting IdentifierTail but found " + lookAheadToken.lexeme);
                    break;
            }
        }
    }
}
