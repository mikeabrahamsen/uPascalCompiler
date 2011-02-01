using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LA = Compiler.LexicalAnalyzer;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            LA.LexicalAnalyzer scanner = new LA.LexicalAnalyzer();
            
            //Added to hold console window open for viewing
            Console.ReadLine();
        }

        
    }
}
