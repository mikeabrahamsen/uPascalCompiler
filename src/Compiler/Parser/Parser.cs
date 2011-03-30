using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Lexer;
using Compiler.Library;
using System.IO;

namespace Compiler.Parse
{
    class Parser
    {
        private Queue<Token> TokenQueue;
        private LexicalAnalyzer scanner;
        private TextWriter UsedRules = new StreamWriter("parse-tree.txt");

        public Parser (Queue<Token> TokenQueue,LexicalAnalyzer scanner, string fileName)
        {
            this.TokenQueue = TokenQueue;
            this.scanner = scanner;
            UsedRules.WriteLine(fileName);
            Move();
        }
        private Token LookAheadToken
        {
            get;
            set;
        }
        private void Error (string errorString)
        {
            throw new SyntaxException("Error at line: " + scanner.Line + " Column: " + 
                (scanner.Column - LookAheadToken.Lexeme.Length-1) + " " + errorString);
        }
        private void Match (int tag)
        {
           
            if((int)LookAheadToken.Tag == tag)
            {
                Move();
            }
            else
            {
                Error("Syntax Error");
            }
            
        }
        private void Move ()
        {
            LookAheadToken = TokenQueue.Dequeue();
        }

        public void SystemGoal ()
        {
            UsedRules.WriteLine("1");
            Program();
            UsedRules.Close();

            //If we find the EOF then the parser is done here, if not then there was an error
            if(LookAheadToken.Tag != Tags.MP_EOF)
            {
                Error("Expecting EOF but found " + LookAheadToken.Lexeme);
            }
            
        }
        private void Program ()
        {
            UsedRules.WriteLine("2");
            ProgramHeading();
            Match(';');            
            Block();
            Match('.');
        }
        private void ProgramHeading () 
        {
            UsedRules.WriteLine("3");
            Match((int)Tags.MP_PROGRAM);
            Identifier();
        }

        private void Block () 
        {
            UsedRules.WriteLine("4");
            VariableDeclarationPart();
            ProcedureAndFunctionDeclarationPart();
            StatementPart();
        }
        private void VariableDeclarationPart () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.MP_VAR: //"var" Variable Declaration ";" VariableDeclationTail
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
                    Error("Expecting VariableDeclarationPart but found " + LookAheadToken.Lexeme);
                    break;
                
            }
        }
        private void ProcedureAndFunctionDeclarationPart () 
        {
            switch(LookAheadToken.Tag)
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
                    Error("Expecting ProcedureAndFunctionDeclarationPart but found " + LookAheadToken.Lexeme);
                    break;
            }
        }

        private void VariableDeclarationTail () 
        {
            switch(LookAheadToken.Tag)
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
                    Error("Expecting VariableDeclarationTail but found " + LookAheadToken.Lexeme);
                    break;
            }
            
        }
        private void VariableDeclaration () 
        {
            UsedRules.WriteLine("9");
            IdentifierList();
            Match(':');
            Type();
        }
        private void Type () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.MP_INTEGER: // Integer
                    UsedRules.WriteLine("10");
                    Match((int)Tags.MP_INTEGER);
                    break;
                case Tags.MP_FLOAT: // Float
                    UsedRules.WriteLine("11");
                    Match((int)Tags.MP_FLOAT);
                    break;
                default:
                    Error("Expecting Type but found " + LookAheadToken.Lexeme);
                    break;
            }
        }
        private void ProcedureDeclaration () 
        {
            UsedRules.WriteLine("15");
            ProcedureHeading();
            Match(';');
            Block();
            Match(';');
        }
        private void FunctionDeclaration () 
        {
            UsedRules.WriteLine("16");
            FunctionHeading();
            Match(';');
            Block();
            Match(';');
        }
        private void ProcedureHeading () 
        {
            UsedRules.WriteLine("17");
            Match((int)Tags.MP_PROCEDURE);
            Identifier();
            OptionalFormalParameterList();

        }
        private void FunctionHeading () 
        {
            UsedRules.WriteLine("18");
            Match((int)Tags.MP_FUNCTION);
            Identifier();
            OptionalFormalParameterList();
            Type();
        }
        private void OptionalFormalParameterList () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.MP_LPAREN: // "(" FormalParameterSection FormalParameterSectionTail ")"
                    UsedRules.WriteLine("19");
                    Match('(');
                    FormalParameterSection();
                    FormalParameterSectionTail();
                    Match(')');
                    break;
                case Tags.MP_SCOLON:
                case Tags.MP_INTEGER:
                case Tags.MP_FLOAT:// lamda
                    UsedRules.WriteLine("20");
                    break;
                default:
                    Error("Expecting OptionalFormalParameterList but found " + LookAheadToken.Lexeme);
                    break;
            }
        }
        private void FormalParameterSectionTail () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.MP_SCOLON: // ";" FormalParameterSection FormalParameterSectionTail
                    UsedRules.WriteLine("21");
                    Match(';');
                    FormalParameterSection();
                    FormalParameterSectionTail();
                    break;
                case Tags.MP_RPAREN: // lamda
                    UsedRules.WriteLine("22");
                    break;
                default:
                    Error("Expecting FormalParameterSectionTail but found " + LookAheadToken.Lexeme);
                    break;
            }
        }
        private void FormalParameterSection () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.MP_IDENTIFIER: // ValueParameterSection
                    UsedRules.WriteLine("23");
                    ValueParameterSection();
                    break;
                case Tags.MP_VAR: // VariableParameterSection
                    UsedRules.WriteLine("24");
                    VariableParameterSection();
                    break;
                default:
                    Error("Expecting FormalParameterSection but found " + LookAheadToken.Lexeme);
                    break;
            }
        }
        private void ValueParameterSection () 
        {
            UsedRules.WriteLine("25");
            IdentifierList();
            Match(':');
            Type();
        }
        private void VariableParameterSection () 
        {
            UsedRules.WriteLine("26");
            Match((int)Tags.MP_VAR);
            IdentifierList();
            Match(':');
            Type();
        }
        private void StatementPart ()
        {
            UsedRules.WriteLine("27");
            CompoundStatement();
        }
        private void CompoundStatement () 
        {
            UsedRules.WriteLine("28");
            Match((int)Tags.MP_BEGIN);
            StatementSequence();
            Match((int)Tags.MP_END);
        }
        private void StatementSequence () 
        {
            UsedRules.WriteLine("29");
            Statement();
            StatementTail();
        }
        private void StatementTail () 
        {
            switch (LookAheadToken.Tag)
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
                    Error("Expecting StatementTail but found " + LookAheadToken.Lexeme);
                    break;
            }
        }
        private void Statement () 
        {
            switch (LookAheadToken.Tag)
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
                    if(TokenQueue.Peek().Tag == Tags.MP_ASSIGN)
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
                    Error("Expecting Statement but found " + LookAheadToken.Lexeme);
                    break;
            }
        }

        private void EmptyStatement () 
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_SCOLON:
                case Tags.MP_END:
                case Tags.MP_ELSE:
                case Tags.MP_UNTIL://lambda
                    UsedRules.WriteLine("42");
                    break;
                default:
                    Error("Expecting EmptyStatement but found " + LookAheadToken.Lexeme);
                    break;
            }
        }
        private void ReadStatement () 
        {
            UsedRules.WriteLine("43");
            Match((int)Tags.MP_READ);
            Match((int)Tags.MP_LPAREN);
            ReadParameter();
            ReadParameterTail();
            Match((int)Tags.MP_RPAREN);
        }
        private void ReadParameterTail()
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_COMMA:  //"," ReadParameter ReadParameterTail
                    UsedRules.WriteLine("44");
                    Match((int)Tags.MP_COMMA);
                    ReadParameter();
                    ReadParameterTail();
                    break;

                case Tags.MP_RPAREN: //lambda
                    UsedRules.WriteLine("45");
                    break;
                default:
                    Error("Expecting ReadParameterTail but found " + LookAheadToken.Lexeme);
                    break;
            }
        }
        private void ReadParameter() 
        {
            UsedRules.WriteLine("46");
            Identifier();
        }
        private void WriteStatement () 
        {
            UsedRules.WriteLine("47");
            Match((int)Tags.MP_WRITE);
            Match((int)Tags.MP_LPAREN);
            WriteParameter();
            WriteParameterTail();
            Match((int)Tags.MP_RPAREN);
        }
        private void WriteParameterTail () 
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_COMMA:  //"," WriteParameter WriteParameterTail
                    UsedRules.WriteLine("48");
                    Match((int)Tags.MP_COMMA);
                    WriteParameter();
                    WriteParameterTail();
                    break;

                case Tags.MP_RPAREN: //lambda
                    UsedRules.WriteLine("49");
                    break;
                default:
                    Error("Expecting WriteParameterTail but found " + LookAheadToken.Lexeme);
                    break;
            }
        }
        private void WriteParameter() 
        {
            UsedRules.WriteLine("50");
            OrdinalExpression();
        }
        private void AssignmentStatement () 
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_IDENTIFIER: // Identifier ":=" Expression
                    UsedRules.WriteLine("51");
                    Identifier();
                    Match((int)Tags.MP_ASSIGN);
                    Expression();
                    break;
                default:
                    Error("Expecting AssignmentStatement but found " + LookAheadToken.Lexeme);
                    break;
            }
        }
        private void IfStatement() 
        {
            UsedRules.WriteLine("52");
            Match((int)Tags.MP_IF);
            BooleanExpression();
            Match((int)Tags.MP_THEN);
            Statement();
            OptionalElsePart();
        }
        private void OptionalElsePart() 
        {
            switch (LookAheadToken.Tag)
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
                    Error("Expecting OptionalElsePart but found " + LookAheadToken.Lexeme);
                    break;
            }
        }
        private void RepeatStatement() 
        {
            UsedRules.WriteLine("55");
            Match((int)Tags.MP_REPEAT);
            StatementSequence();
            Match((int)Tags.MP_UNTIL);
            BooleanExpression();
        }
        private void WhileStatement () 
        {
            UsedRules.WriteLine("56");
            Match((int)Tags.MP_WHILE);
            BooleanExpression();
            Match((int)Tags.MP_DO);
            Statement();

        }
        private void ForStatement () 
        {
            UsedRules.WriteLine("57");
            Match((int)Tags.MP_FOR);
            ControlVariable();
            Match((int)Tags.MP_ASSIGN);
            InitialValue();
            StepValue();
            FinalValue();
            Match((int)Tags.MP_DO);
            Statement();
        }
        private void ControlVariable () 
        {
            UsedRules.WriteLine("58");
            Identifier();
        }
        private void InitialValue () 
        {
            UsedRules.WriteLine("59");
            OrdinalExpression();
        }
        private void StepValue() 
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_TO: // "to"
                    UsedRules.WriteLine("60");
                    Match((int)Tags.MP_TO);
                    break;
                case Tags.MP_DOWNTO: //"downto"
                    UsedRules.WriteLine("61");
                    Match((int)Tags.MP_DOWNTO);
                    break;
                default:
                    Error("Expecting StepValue but found " + LookAheadToken.Lexeme);
                    break;
            }
        }
        private void FinalValue () 
        {
            UsedRules.WriteLine("62");
            OrdinalExpression();
        }
        private void ProcedureStatement() 
        {
            UsedRules.WriteLine("63");
            Identifier();
            OptionalActualParameterList();
        }

        private void OptionalActualParameterList() 
        {
            switch (LookAheadToken.Tag)
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
                    Error("Expecting OptionalActualParameterList but found " + LookAheadToken.Lexeme);
                    break;
            }
        }
        private void Identifier ()
        {
            Match((int)Tags.MP_IDENTIFIER);
        }        

        private void ActualParameterTail()
        {
            switch(LookAheadToken.Tag)
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
                    Error("Expecting ActualParameterTail but found " + LookAheadToken.Lexeme);
                    break;
            }
        }

        private void ActualParameter()
        {
            UsedRules.WriteLine("68");
            OrdinalExpression();
        }

        private void Expression()
        {
            UsedRules.WriteLine("69");
            SimpleExpression();
            OptionalRelationalPart();
        }

        private void OptionalRelationalPart()
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_EQUAL:
                case Tags.MP_LTHAN:
                case Tags.MP_GTHAN:
                case Tags.MP_LEQUAL:
                case Tags.MP_GEQUAL:
                case Tags.MP_NEQUAL://RelationalOperator SimpleExpression 
                    UsedRules.WriteLine("70");
                    RelationalOperator();
                    SimpleExpression();
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
                    Error("Expecting OptionalRelationalPart but found " + LookAheadToken.Lexeme);
                    break;

            }
        }
        
        private void RelationalOperator()
        {
            switch (LookAheadToken.Tag)
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
                    Error("Expecting RelationalOperator but found " + LookAheadToken.Lexeme);
                    break;

            }
        }

        private void SimpleExpression()
        {
            UsedRules.WriteLine("78");
            OptionalSign();
            Term();
            TermTail();
        }

        private void TermTail()
        {
            switch(LookAheadToken.Tag)
            {
                //AddingOperator Term TermTail
                case Tags.MP_PLUS:
                case Tags.MP_MINUS:
                case Tags.MP_OR:
                    UsedRules.WriteLine("79");
                    AddingOperator();
                    Term();
                    TermTail();
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
                    Error("Expecting TermTail but found " + LookAheadToken.Lexeme);
                    break;
            }
        }
        private void OptionalSign()
        {
            switch(LookAheadToken.Tag)
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
                    Error("Expecting OptionalSign but found " + LookAheadToken.Lexeme);
                    break;
            }
        }

        private void AddingOperator()
        {
            switch (LookAheadToken.Tag)
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
                    Error("Expecting AddingOperator but found " + LookAheadToken.Lexeme);
                    break;
            }
        }

        private void Term()
        {
            UsedRules.WriteLine("87");
            Factor();
            FactorTail();
        }

        private void MultiplyingOperator()
        {
            switch(LookAheadToken.Tag)
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
                    Error("Expecting MultiplyingOperator but found " + LookAheadToken.Lexeme);
                    break;
            }
        }

        private void FactorTail()
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_TIMES:
                case Tags.MP_DIV:
                case Tags.MP_MOD:
                case Tags.MP_AND:
                    UsedRules.WriteLine("88");
                    MultiplyingOperator();
                    Factor();
                    FactorTail();
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
                    Error("Expecting FactorTail but found " + LookAheadToken.Lexeme);
                    break;
            }
        }

        private void Factor()
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_INTEGER_LIT:
                    UsedRules.WriteLine("94");
                    Match((int)Tags.MP_INTEGER_LIT);
                    break;
                case Tags.MP_NOT:
                    UsedRules.WriteLine("95");
                    Match((int)Tags.MP_NOT);
                    Factor();
                    break;
                case Tags.MP_LPAREN:
                    UsedRules.WriteLine("96");
                    Match((int)Tags.MP_LPAREN);
                    Expression();
                    Match((int)Tags.MP_RPAREN);
                    break;                    
                case Tags.MP_IDENTIFIER:
                    UsedRules.WriteLine("97");
                    Identifier();
                    OptionalActualParameterList();
                    break;
                default:
                    Error("Expecting Factor but found " + LookAheadToken.Lexeme);
                    break;
            }
        }

        private void BooleanExpression()
        {
            UsedRules.WriteLine("98");
            Expression();
        }
        private void OrdinalExpression()
        {
            UsedRules.WriteLine("99");
            Expression();
        }

        private void IdentifierList()
        {
            UsedRules.WriteLine("100");
            Identifier();
            IdentifierTail();
        }

        private void IdentifierTail()
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_COMMA:
                    UsedRules.WriteLine("101");
                    Match((int)Tags.MP_COMMA);
                    Identifier();
                    IdentifierTail();
                    break;
                case Tags.MP_COLON: //lambda 
                    UsedRules.WriteLine("102");
                    break;
                default:
                    Error("Expecting IdentifierTail but found " + LookAheadToken.Lexeme);
                    break;
            }
        }
    }
}
