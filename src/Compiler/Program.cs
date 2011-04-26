using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Lexer;
using Compiler.Parse;
using Compiler.SymbolTbl;
using Compiler.Library;


namespace Compiler
{
    /// <summary>
    /// Program driver for testing
    /// </summary>
    class Driver
    {
        public static Queue<Token> TokenQueue;
        static void Main(string[] args)
        {
            TokenQueue = new Queue<Token>();

            LexicalAnalyzer scanner = new LexicalAnalyzer();

            scanner.OpenFile(args[0]);

            Token token = new Token();
            string output;            
            
            try
            {
                bool done = false;
                while (!done)
                {

                    token = scanner.GetNextToken();
                    if (token.tag != null)
                    {
                        output = string.Format("{0,-20} {1,-5} {2,-5} {3}",
                            token.tag, scanner.line, (scanner.column - token.lexeme.Length - 1),
                            token.lexeme);
                        TokenQueue.Enqueue(token);
                    }
                    if(token.tag.Equals(Tags.MP_EOF))
                    {
                        done = true;
                    }                
                }
                Parser parser = new Parser(TokenQueue, scanner, args[0]);
                parser.SystemGoal();
                Console.WriteLine("Program Parsed Correctly");
            }
            catch(SyntaxException e)
            {
                Console.WriteLine(e.ErrorMessage);
            }
            Console.WriteLine("Press Any Key To Exit");
            Console.Read();
        }
    } 
}


