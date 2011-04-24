using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.Lexer
{
    /// <summary>
    /// A word is made up of a string and a tag
    /// The string will be the lexeme of the token found
    /// </summary>
    class Word : Token
    {
        /// <summary>
        /// Constructor for Word
        /// </summary>
        /// <param name="s"></param>
        /// <param name="tag"></param>
        /// <param name="line"></param>
        /// <param name="column"></param>
        public Word(string s, int tag,int line,int column) : base(tag,line,column)
        {
            lexeme = s;
        }
        
        /// <summary>
        /// Overrides the tostring to output the lexeme of a token
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return lexeme;
        }


    }
}
