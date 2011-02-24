using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.LexicalAnalyzer;
using Compiler.LexicalAnalyzer.Library;

namespace Compiler.Parser
{
    class Parser
    {
        private LexicalAnalyzer.LexicalAnalyzer scanner;

        public Parser (LexicalAnalyzer.LexicalAnalyzer scanner)
        {
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
                Move();                                               
            }
            else
            {
                Error("Syntax Error");
            }
            //else throw error
        }
        private void Move ()
        {
            LookAheadToken = scanner.GetNextToken();
        }
        public void Program () 
        {
            ProgramHeading();
            Match(';');
            Block();
            Match('.');
        }
        private void ProgramHeading () 
        {
            Match((int)Tags.MP_PROGRAM);
            ProgramIdentifier();
        }

        private void Block () 
        {
            VariableDeclarationPart();
            ProcedureAndFunctionDeclarationPart();
            StatementPart();
        }
        private void VariableDeclarationPart () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.MP_VAR: //"var" Variable Declaration ";" VariableDeclationTail
                    Match((int)Tags.MP_VAR);
                    VariableDeclaration();
                    Match(';');
                    VariableDeclarationTail();
                    break;
                    //This
                case Tags.MP_PROCEDURE: case Tags.MP_FUNCTION: case Tags.MP_BEGIN: //lambda
                    //this will be null????
                    break;
                default:
                    //throw error
                    break;
                
            }
        }
        private void ProcedureAndFunctionDeclarationPart () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.DUMMYTAG1: // PeocedureDeclaration ProcedureAndFunctionDeclarationPart
                    ProcedureDeclaration();
                    ProcedureAndFunctionDeclarationPart();
                    break;
                case Tags.DUMMYTAG2: // FunctionDeclaration ProcedureAndFunctionDeclarationPart
                    FunctionDeclaration();
                    ProcedureAndFunctionDeclarationPart();
                    break;
                case Tags.DUMMYTAG3: //lamda
                    break;
            }
        }
        private void StatementPart () 
        {
            CompoundStatement();
        }
        private void VariableDeclarationTail () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.DUMMYTAG1: // VariableDeclaration ";" VariableDeclarationTail
                    VariableDeclaration();
                    Match(';');
                    VariableDeclarationTail();
                    break;
                case Tags.DUMMYTAG2: // lamda
                    break;
            }
            
        }
        private void VariableDeclaration () 
        {
            IdentifierList();
            Match(':');
            Type();
        }
        private void Type () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.DUMMYTAG1: // Integer
                    Match((int)Tags.MP_INTEGER);
                    break;
                case Tags.DUMMYTAG2: // Float
                    Match((int)Tags.MP_FLOAT);
                    break;
            }
        }
        private void ProcedureDeclaration () 
        {
            ProcedureHeading();
            Match(';');
            Block();
            Match(';');
        }
        private void FunctionDeclaration () 
        {
            FunctionHeading();
            Match(';');
            Block();
            Match(';');
        }
        private void ProcedureHeading () 
        {
            Match((int)Tags.MP_PROCEDURE);
            ProcedureIdentifier();
            OptionalFormalParameterList();

        }
        private void FunctionHeading () 
        {
            Match((int)Tags.MP_FUNCTION);
            FunctionIdentifier();
            OptionalFormalParameterList();
            Type();
        }
        private void OptionalFormalParameterList () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.DUMMYTAG1: // "(" FormalParameterSection FormalParameterSectionTail ")"
                    Match('(');
                    FormalParameterSection();
                    FormalParameterSectionTail();
                    Match(')');
                    break;
                case Tags.DUMMYTAG2: // lamda
                    break;
            }
        }
        private void FormalParameterList () { }
        private void FormalParameterSectionTail () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.DUMMYTAG1: // ";" FormalParameterSection FormalParameterSectionTail
                    Match(';');
                    FormalParameterSection();
                    FormalParameterSectionTail();
                    break;
                case Tags.DUMMYTAG2: // lamda
                    break;
            }
        }
        private void FormalParameterSection () 
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.DUMMYTAG1: // ValueParameterSection
                    ValueParameterSection();
                    break;
                case Tags.DUMMYTAG2: // VariableParameterSection
                    VariableParameterSection();
                    break;
            }
        }
        private void ValueParameterSection () 
        {
            IdentifierList();
            Match(':');
            Type();
        }
        private void VariableParameterSection () 
        {
            Match((int)Tags.MP_VAR);
            IdentifierList();
            Match(':');
            Type();
        }
        private void CompoundStatement () 
        {
            Match((int)Tags.MP_BEGIN);
            StatementSequence();
            Match((int)Tags.MP_END);
        }
        private void StatementSequence () 
        {
            Statement();
            StatementTail();
        }
        private void StatementTail () 
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_SCOLON:  //";" Statement StatementTail
                    Match((int)Tags.MP_SCOLON);
                    Statement();
                    StatementTail();
                    break;

                case Tags.DUMMYTAG1: //lambda
                    break;

                default:
                    //throw error
                    break;
            }
        }
        private void Statement () 
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.DUMMYTAG1: // EmptyStatement
                    EmptyStatement();
                    break;
                case Tags.DUMMYTAG2: // CompoundStatement
                    CompoundStatement();
                    break;
                case Tags.DUMMYTAG3: //ReadStatement
                    ReadStatement();
                    break;
                case Tags.DUMMYTAG4: //WriteStatement
                    WriteStatement();
                    break;
                case Tags.DUMMYTAG5: //AssignmentStatement
                    AssignmentStatement();
                    break;
                case Tags.DUMMYTAG6: //IfStatement
                    IfStatement();
                    break;
                case Tags.DUMMYTAG7: //WhileStatement
                    WhileStatement();
                    break;
                case Tags.DUMMYTAG8: //RepeatStatement
                    RepeatStatement();
                    break;
                case Tags.DUMMYTAG9: //ForStatement
                    ForStatement();
                    break;
                case Tags.DUMMYTAG10://ProcedureStatement
                    ProcedureStatement();
                    break;
            }

        }

        private void EmptyStatement () 
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.DUMMYTAG1: //lambda
                    break;

                default:
                    //throw error
                    break;
            }
        }
        private void ReadStatement () 
        {
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
                    Match((int)Tags.MP_COMMA);
                    ReadParameter();
                    ReadParameterTail();
                    break;

                case Tags.DUMMYTAG1: //lambda
                    break;

                default:
                    //throw error
                    break;
            }
        }
        private void ReadParameter() 
        {
            VariableIdentifier();
        }
        private void WriteStatement () 
        {
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
                    Match((int)Tags.MP_COMMA);
                    WriteParameter();
                    WriteParameterTail();
                    break;

                case Tags.DUMMYTAG1: //lambda
                    break;

                default:
                    //throw error
                    break;
            }
        }
        private void WriteParameter() 
        {
            OrdinalExpression();
        }
        private void AssignmentStatement () 
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.DUMMYTAG1: // VariableIdentifier ":=" Expression
                    VariableIdentifier();
                    Match((int)Tags.MP_ASSIGN);
                    Expression();
                    break;
                case Tags.DUMMYTAG2: // FunctionIdentifier ":=" Expression
                    FunctionIdentifier();
                    Match((int)Tags.MP_ASSIGN);
                    Expression();
                    break;
            }
        }
        private void IfStatement() 
        {
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
                case Tags.DUMMYTAG1: // Lambda
                    break;
                default:
                    //throw error
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
            VariableIdentifier();
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
                    //throw error
                    break;
            }
        }
        private void FinalValue () 
        {
            OrdinalExpression();
        }
        private void ProcedureStatement() 
        {
            ProcedureIdentifier();
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
                case Tags.DUMMYTAG1: // Lambda
                    break;
                default:
                    //throw error
                    break;
            }
        }
        private void Identifier ()
        {
            Match((int)Tags.MP_IDENTIFIER);
            //throw new NotImplementedException();
        }        

        //austen's additions
        private void UnsignedInteger() { }

        private void ActualParameterTail()
        {
            switch(LookAheadToken.Tag)
            {
                case Tags.MP_COMMA: //"," ActualParameter ActualParameterTail
                    Match((int)Tags.MP_COMMA);
                    ActualParameter(); 

                    ActualParameterTail();
                    break;

                case Tags.DUMMYTAG1: //lambda
                    
                    break;
                default:
                    //throw error
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
                case Tags.DUMMYTAG1: //RelationalOperator SimpleExpression 
                    RelationalOperator();
                    SimpleExpression();
                    break;

                case Tags.DUMMYTAG2: //lambda

                    break;
                default:
                    //throw error
                    break;

            }
        }
        /// <summary>
        /// *******************************************************************************************************************
        /// THIS IS WHERE I LEFT OFF - (AUSTEN) - GOING UP FROM HERE. WILL FINISH TOMORROW
        /// *******************************************************************************************************************
        /// </summary>
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
                    //throw error
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
                // NOW JUST WTF IS COLUMN AE IN THE SPREADSHEET AGAIN? **********************************************************************************************************
                case Tags.MP_LTHAN:
                case Tags.MP_GTHAN:
                case Tags.MP_LEQUAL:
                case Tags.MP_GEQUAL:
                case Tags.MP_NEQUAL:
                    //lambda                    
                    break;
                default:
                    //throw error
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
                case Tags.MP_LPAREN:
                case Tags.MP_INTEGER: // IS THIS UNSIGNED INTEGER? *******************************************************************************************************************
                case Tags.MP_NOT:
                case Tags.MP_IDENTIFIER:
                    //lambda
                    break;
                default:
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
                case Tags.MP_OR: //is this one right??
                    Match((int)Tags.MP_OR);
                    break;
                default:
                    break;
            }
        }

        private void Term()
        {
            Factor();
            FactorTail();
        }

        //HAD TO ADD THIS AGAIN IT WAS HERE \/ \/ \/ \/ \/ \|/ ************************************************ <3 Austen
        private void MultiplyingOperator()
        {
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
                // DOUBLE CHECK DUNNO WHAT IS SUPPOSED TO BE IN COLUMN AE on FinallLL1Table Gdoc  **************************************************************************************
                case Tags.MP_LTHAN:
                case Tags.MP_GTHAN:
                case Tags.MP_LEQUAL:
                case Tags.MP_GEQUAL:
                case Tags.MP_NEQUAL:
                case Tags.MP_PLUS:
                case Tags.MP_MINUS:
                case Tags.MP_OR:
                    //lambda case!
                    break;
                default:
                    break;
            }
        }

        private void Factor()
        {
            switch (LookAheadToken.Tag)
            {
                case Tags.MP_INTEGER: //is this UNSIGNED INT? *******************************************************************
                    UnsignedInteger();
                    break;
                case Tags.MP_IDENTIFIER:
                    VariableIdentifier();
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
                    /*
                case Tags.MP_IDENTIFIER:   ///////// WE WILL HAVE TO RESOLVE CONFLICT // USE DOUBLE LOOKAHEAD HERE PROBABALY
                    FunctionIdentifier();
                    OptionalActualParameterList();
                    break;*/
                default:
                    break;
            }
        }

        private void ProgramIdentifier()
        {
            Identifier();
        }
        private void VariableIdentifier()
        {
            Identifier();
        }
        private void ProcedureIdentifier()
        {
            Identifier();
        }
        private void FunctionIdentifier()
        {
            Identifier();
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
                case Tags.MP_COLON: //lambda //Don't know if this is correct... doesn't seem right?
                    //shouldn't it just be lambda?
                    break;
                default:
                    //RETURN ERROR line # col # unexpected char?
                    break;
            }
        }
    }
}
