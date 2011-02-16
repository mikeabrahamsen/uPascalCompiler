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
        
        private Token LookAheadToken
        {
            get;
            set;
        }
        private void Match (int tag)
        {
            if((int)LookAheadToken.Tag == tag)
            {
                Move();
            }
            //else throw error
        }
        private void Move ()
        {
            LookAheadToken = scanner.GetNextToken();
        }
        private void Program () 
        {
            ProgramHeading();
            Match(';');
            Block();
            Match('.');
        }
        private void ProgramHeading () 
        {
            Match((int)Tags.MP_PROGRAM);
            Identifier();
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
                case Tags.DUMMYTAG1: //lambda
                    
                    break;
                default:
                    //throw error
                    break;
                
            }
        }
        private void ProcedureAndFunctionDeclarationPart () { }
        private void StatementPart () { }
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
        private void VariableDeclaration () { }
        private void Type () { }
        private void ProcedureDeclaration () { }
        private void FunctionDeclaration () { }
        private void ProcedureHeading () { }
        private void FuctionHeading () { }
        private void OptionalFormalParameterList () { }
        private void FormalParameterList () { }
        private void FormalParameterSectionTail () { }
        private void FormalParameterSection () { }
        private void ValueParameterSection () { }
        private void VariableParameterSection () { }
        private void CompoundStatement () { }
        private void StatementSequence () { }
        private void StatementTail () { }
        private void Statement () { }
        //private void SimpleStatement () { }
        //private void StructuredStatement () { }
        //private void ConditionalStatement () { }
        //private void RepetitiveStatement () { }
        private void EmptyStatement () { }
        private void ReadStatement () { }
        private void ReadParameterTail () { }
        private void WriteStatement () { }
        private void WriteParameterTail () { }
        private void AssignmentStatement () { }
        private void ProcedureStatement () { }
        private void IfStatement () { }
        private void RepeatStatement () { }
        private void WhileStatement () { }
        private void ForStatement () { }
        private void ControlVariable () { }
        private void InitialValue () { }
        private void FinalValue () { }
        private void Expression () { }
        private void SimpleExpression () { }
        private void Term () { }
        private void Factor () { }
        private void RelationalOperator () { }
        private void AddingOperator () { }
        private void MultiplyingOperator () { }
        private void FunctionDesignator () { }
        private void Variable () { }
        private void ActualParameterList () { }
        private void ActualParameter () { }
        private void ReadParameterList () { }
        private void ReadParameter () { }
        private void WriteParamaterList () { }
        private void WriteParameter () { }
        private void BooleanExpression () { }
        private void OrdinalExpression () { }
        private void VariableIdentifier () { }
        private void ProcedureIdentifier () { }
        private void IdentifierList () { }
        private void Identifier ()
        {
            throw new NotImplementedException();
        }

    }
}
