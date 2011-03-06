using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.LexicalAnalyzer;
using Compiler.LexicalAnalyzer.Library;
using System.IO;

namespace Compiler.Parser
{
    class Parser
    {
        private Queue<Token> TokenQueue;
        private LexicalAnalyzer.LexicalAnalyzer scanner;
        private TextWriter UsedRules = new StreamWriter("UsedRules.txt");

        public Parser (Queue<Token> TokenQueue,LexicalAnalyzer.LexicalAnalyzer scanner)
        {
            this.TokenQueue = TokenQueue;
            this.scanner = scanner;
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
                if(LookAheadToken.Tag != Tags.MP_EOF)
                {
                    Move();
                }
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
            UsedRules.WriteLine("Rule 1");
            Program();
            Match((int)Tags.MP_EOF);
            UsedRules.Close();
        }
        private void Program ()
        {
            UsedRules.WriteLine("Rule 2");
            ProgramHeading();
            Match(';');
            Block();
            Match('.');
        }
        private void ProgramHeading () 
        {
            UsedRules.WriteLine("Rule 3");
            Match((int)Tags.MP_PROGRAM);
            Identifier();
        }

        private void Block () 
        {
            UsedRules.WriteLine("Rule 4:");
            VariableDeclarationPart();
            ProcedureAndFunctionDeclarationPart();
            StatementPart();
        }
        private void VariableDeclarationPart () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.MP_VAR: //"var" Variable Declaration ";" VariableDeclationTail
                    UsedRules.WriteLine("Rule 5");
                    Match((int)Tags.MP_VAR);
                    VariableDeclaration();
                    Match(';');
                    VariableDeclarationTail();
                    break;
                    //This
                case Tags.MP_PROCEDURE: case Tags.MP_FUNCTION: case Tags.MP_BEGIN: //lambda
                    UsedRules.WriteLine("Rule 6");
                    break;
                default:
                    Error("SyntaxError");
                    break;
                
            }
        }
        private void ProcedureAndFunctionDeclarationPart () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.MP_PROCEDURE: // PeocedureDeclaration ProcedureAndFunctionDeclarationPart
                    UsedRules.WriteLine("Rule 12");
                    ProcedureDeclaration();
                    ProcedureAndFunctionDeclarationPart();
                    break;
                case Tags.MP_FUNCTION: // FunctionDeclaration ProcedureAndFunctionDeclarationPart
                    UsedRules.WriteLine("Rule 13");
                    FunctionDeclaration();
                    ProcedureAndFunctionDeclarationPart();
                    break;
                case Tags.MP_BEGIN: //lamda
                    UsedRules.WriteLine("Rule 14");
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }

        private void VariableDeclarationTail () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.MP_IDENTIFIER: // VariableDeclaration ";" VariableDeclarationTail
                    UsedRules.WriteLine("Rule 7");
                    VariableDeclaration();
                    Match(';');
                    VariableDeclarationTail();
                    break;
                case Tags.MP_PROCEDURE:
                case Tags.MP_FUNCTION:
                case Tags.MP_BEGIN:// lamda
                    UsedRules.WriteLine("Rule 8");
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
            
        }
        private void VariableDeclaration () 
        {
            UsedRules.WriteLine("Rule 9");
            IdentifierList();
            Match(':');
            Type();
        }
        private void Type () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.MP_INTEGER: // Integer
                    UsedRules.WriteLine("Rule 10");
                    Match((int)Tags.MP_INTEGER);
                    break;
                case Tags.MP_FLOAT: // Float
                    UsedRules.WriteLine("Rule 11");
                    Match((int)Tags.MP_FLOAT);
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }
        private void ProcedureDeclaration () 
        {
            UsedRules.WriteLine("Rule 15");
            ProcedureHeading();
            Match(';');
            Block();
            Match(';');
        }
        private void FunctionDeclaration () 
        {
            UsedRules.WriteLine("Rule 16");
            FunctionHeading();
            Match(';');
            Block();
            Match(';');
        }
        private void ProcedureHeading () 
        {
            UsedRules.WriteLine("Rule 17");
            Match((int)Tags.MP_PROCEDURE);
            Identifier();
            OptionalFormalParameterList();

        }
        private void FunctionHeading () 
        {
            UsedRules.WriteLine("Rule 18");
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
                    UsedRules.WriteLine("Rule 19");
                    Match('(');
                    FormalParameterSection();
                    FormalParameterSectionTail();
                    Match(')');
                    break;
                case Tags.MP_SCOLON:
                case Tags.MP_INTEGER:
                case Tags.MP_FLOAT:// lamda
                    UsedRules.WriteLine("Rule 20");
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }
        private void FormalParameterSectionTail () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.MP_SCOLON: // ";" FormalParameterSection FormalParameterSectionTail
                    UsedRules.WriteLine("Rule 21");
                    Match(';');
                    FormalParameterSection();
                    FormalParameterSectionTail();
                    break;
                case Tags.MP_RPAREN: // lamda
                    UsedRules.WriteLine("Rule 22");
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }
        private void FormalParameterSection () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.MP_IDENTIFIER: // ValueParameterSection
                    UsedRules.WriteLine("Rule 23");
                    ValueParameterSection();
                    break;
                case Tags.MP_VAR: // VariableParameterSection
                    UsedRules.WriteLine("Rule 24");
                    VariableParameterSection();
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }
        private void ValueParameterSection () 
        {
            UsedRules.WriteLine("Rule 25");
            IdentifierList();
            Match(':');
            Type();
        }
        private void VariableParameterSection () 
        {
            UsedRules.WriteLine("Rule 26");
            Match((int)Tags.MP_VAR);
            IdentifierList();
            Match(':');
            Type();
        }
        private void StatementPart ()
        {
            UsedRules.WriteLine("Rule 27");
            CompoundStatement();
        }
        private void CompoundStatement () 
        {
            UsedRules.WriteLine("Rule 28");
            Match((int)Tags.MP_BEGIN);
            StatementSequence();
            Match((int)Tags.MP_END);
        }
        private void StatementSequence () 
        {
            UsedRules.WriteLine("Rule 29");
            Statement();
            StatementTail();
        }
        private void StatementTail () 
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_SCOLON:  //";" Statement StatementTail
                    UsedRules.WriteLine("Rule 30");
                    Match((int)Tags.MP_SCOLON);
                    Statement();
                    StatementTail();
                    break;

                case Tags.MP_END:
                case Tags.MP_UNTIL://lambda
                    UsedRules.WriteLine("Rule 31");
                    break;
                default:
                    Error("SyntaxError");
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
                    UsedRules.WriteLine("Rule 32");
                    EmptyStatement();
                    break;
                case Tags.MP_BEGIN: // CompoundStatement
                    UsedRules.WriteLine("Rule 33");
                    CompoundStatement();
                    break;
                case Tags.MP_READ: //ReadStatement
                    UsedRules.WriteLine("Rule 34");
                    ReadStatement();
                    break;
                case Tags.MP_WRITE: //WriteStatement
                    UsedRules.WriteLine("Rule 35");
                    WriteStatement();
                    break;
                case Tags.MP_IDENTIFIER: //AssignmentStatement
                    if(TokenQueue.Peek().Tag == Tags.MP_ASSIGN)
                    {
                        UsedRules.WriteLine("Rule 36");
                        AssignmentStatement();
                    }
                    else
                    {
                        UsedRules.WriteLine("Rule 41");
                        ProcedureStatement();
                    }
                    break;
                case Tags.MP_IF: //IfStatement
                    UsedRules.WriteLine("Rule 7");
                    IfStatement();
                    break;
                case Tags.MP_WHILE: //WhileStatement
                    UsedRules.WriteLine("Rule 38");
                    WhileStatement();
                    break;
                case Tags.MP_REPEAT: //RepeatStatement
                    UsedRules.WriteLine("Rule 39");
                    RepeatStatement();
                    break;
                case Tags.MP_FOR: //ForStatement
                    UsedRules.WriteLine("Rule 40");
                    ForStatement();
                    break;                    
                default:
                    Error("SyntaxError");
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
                    UsedRules.WriteLine("Rule 42");
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }
        private void ReadStatement () 
        {
            UsedRules.WriteLine("Rule 43");
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
                    UsedRules.WriteLine("Rule 44");
                    Match((int)Tags.MP_COMMA);
                    ReadParameter();
                    ReadParameterTail();
                    break;

                case Tags.MP_RPAREN: //lambda
                    UsedRules.WriteLine("Rule 45");
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }
        private void ReadParameter() 
        {
            UsedRules.WriteLine("Rule 46");
            Identifier();
        }
        private void WriteStatement () 
        {
            UsedRules.WriteLine("Rule 47");
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
                    UsedRules.WriteLine("Rule 48");
                    Match((int)Tags.MP_COMMA);
                    WriteParameter();
                    WriteParameterTail();
                    break;

                case Tags.MP_RPAREN: //lambda
                    UsedRules.WriteLine("Rule 49");
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }
        private void WriteParameter() 
        {
            UsedRules.WriteLine("Rule 50");
            OrdinalExpression();
        }
        private void AssignmentStatement () 
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_IDENTIFIER: // Identifier ":=" Expression
                    UsedRules.WriteLine("Rule 51");
                    Identifier();
                    Match((int)Tags.MP_ASSIGN);
                    Expression();
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }
        private void IfStatement() 
        {
            UsedRules.WriteLine("Rule 52");
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
                    Match((int)Tags.MP_ELSE);
                    Statement();
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }
        private void RepeatStatement() 
        {
            Match((int)Tags.MP_REPEAT);
            StatementSequence();
            Match((int)Tags.MP_UNTIL);
            BooleanExpression();
        }
        private void WhileStatement () 
        {
            Match((int)Tags.MP_WHILE);
            BooleanExpression();
            Match((int)Tags.MP_DO);
            Statement();

        }
        private void ForStatement () 
        {
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
            Identifier();
        }
        private void InitialValue () 
        {
            OrdinalExpression();
        }
        private void StepValue() 
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_TO: // "to"
                    Match((int)Tags.MP_TO);
                    break;
                case Tags.MP_DOWNTO: //"downto"
                    Match((int)Tags.MP_DOWNTO);
                    break;
                case Tags.DUMMYTAG1: // Lambda
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }
        private void FinalValue () 
        {
            OrdinalExpression();
        }
        private void ProcedureStatement() 
        {
            Identifier();
            OptionalActualParameterList();
        }

        private void OptionalActualParameterList() 
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_LPAREN: // "(" ActualParameter ActualParameterTail ")" 
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
                    break;
                default:
                    Error("SyntaxError");
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
                    Match((int)Tags.MP_COMMA);
                    ActualParameter(); 

                    ActualParameterTail();
                    break;

                case Tags.MP_RPAREN: //lambda
                    
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }

        private void ActualParameter()
        {
            OrdinalExpression();
        }

        private void Expression()
        {
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

                    break;
                default:
                    Error("SyntaxError");
                    break;

            }
        }
        
        private void RelationalOperator()
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_EQUAL:
                    Match('=');
                    break;

                case Tags.MP_LTHAN:
                    Match('<');
                    break;

                case Tags.MP_GTHAN:
                    Match('>');
                    break;
                
                case Tags.MP_LEQUAL:
                    Match((int)Tags.MP_LEQUAL);                    
                    break;
                case Tags.MP_GEQUAL:
                    Match((int)Tags.MP_GEQUAL);
                    break;
                case Tags.MP_NEQUAL:
                    Match((int)Tags.MP_NEQUAL);
                    break;
                default:
                    Error("SyntaxError");
                    break;

            }
        }

        private void SimpleExpression()
        {
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
                    //lambda                    
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }
        private void OptionalSign()
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.MP_PLUS:
                    Match('+');
                    break;
                case Tags.MP_MINUS:
                    Match('-');
                    break;
                case Tags.MP_LPAREN: //lambda
                case Tags.MP_INTEGER_LIT:
                case Tags.MP_NOT:
                case Tags.MP_IDENTIFIER:
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }

        private void AddingOperator()
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_PLUS:
                    Match('+');
                    break;
                case Tags.MP_MINUS:
                    Match('-');
                    break;
                case Tags.MP_OR:
                    Match((int)Tags.MP_OR);
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }

        private void Term()
        {
            Factor();
            FactorTail();
        }

        private void MultiplyingOperator()
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.MP_TIMES:
                    Match('*');
                    break;
                case Tags.MP_DIV:
                    Match((int)Tags.MP_DIV);
                    break;
                case Tags.MP_MOD:
                    Match((int)Tags.MP_MOD);
                    break;
                case Tags.MP_AND: 
                    Match((int)Tags.MP_AND);
                    break;                
                default:
                    Error("SyntaxError");
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
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }

        private void Factor()
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_INTEGER_LIT:
                    Match((int)Tags.MP_INTEGER_LIT);
                    break;
                case Tags.MP_NOT:
                    Match((int)Tags.MP_NOT);
                    Factor();
                    break;
                case Tags.MP_LPAREN:
                    Match((int)Tags.MP_LPAREN);
                    Expression();
                    Match((int)Tags.MP_RPAREN);
                    break;                    
                case Tags.MP_IDENTIFIER:
                    Identifier();
                    OptionalActualParameterList();
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }

        private void BooleanExpression()
        {
            Expression();
        }
        private void OrdinalExpression()
        {
            Expression();
        }

        private void IdentifierList()
        {
            Identifier();
            IdentifierTail();
        }

        private void IdentifierTail()
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_COMMA:
                    Match((int)Tags.MP_COMMA);
                    Identifier();
                    IdentifierTail();
                    break;
                case Tags.MP_COLON: //lambda 
                    break;
                default:
                    Error("SyntaxError");
                    break;
            }
        }
    }
}
