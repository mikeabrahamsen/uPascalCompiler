using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.LexicalAnalyzer;
using Compiler.Parser;
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

            LexicalAnalyzer.LexicalAnalyzer scanner = new LexicalAnalyzer.LexicalAnalyzer();

            scanner.OpenFile(args[0]);

            Token token = new Token();
            string output;
            
            while(!scanner.Finished)
            {
                scanner.ErrorFound = false;
                token = scanner.GetNextToken();
                if(token.Tag != null)
                {
                    output = string.Format("{0,-20} {1,-5} {2,-5} {3}",
                        token.Tag, scanner.Line, (scanner.Column - token.Lexeme.Length-1), token.Lexeme);
                    Console.WriteLine(output);
                    if(scanner.ErrorFound)
                    {
                       Console.WriteLine(scanner.ErrorMessage);
                    }
                    TokenQueue.Enqueue(token);
                }
            }
            try
            {
                Parser.Parser parser = new Parser.Parser(TokenQueue, scanner, args[0]);
                parser.SystemGoal();
                Console.WriteLine("Program Parsed Correctly");
            }
            catch(SyntaxException e)
            {
                Console.WriteLine(e.ErrorMessage);
            }

            //Added to hold console window open for viewing
            Console.ReadLine();
        }

        
    }
}
