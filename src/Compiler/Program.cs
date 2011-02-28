using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.LexicalAnalyzer;

namespace Compiler
{
    /// <summary>
    /// Program driver for testing
    /// </summary>
    class Driver
    {
        static void Main(string[] args)
        {
            Queue<Token> TokenQueue = new Queue<Token>();

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
            Parser.Parser parser = new Parser.Parser(scanner);
            parser.Program();
                Console.WriteLine("Program Parsed Correctly");
             
            //Added to hold console window open for viewing
            Console.ReadLine();
        }

        
    }
}
