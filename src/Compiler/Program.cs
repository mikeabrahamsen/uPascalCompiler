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
            LexicalAnalyzer.LexicalAnalyzer scanner = new LexicalAnalyzer.LexicalAnalyzer();

            scanner.OpenFile(args[0]);

            string output;
            Token token = new Token();
            Parser.Parser parser = new Parser.Parser(scanner);
            /*
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
                }
            }
             * */
            parser.Program();
                Console.WriteLine("yay we made it");
             
            //Added to hold console window open for viewing
            Console.ReadLine();
        }

        
    }
}
