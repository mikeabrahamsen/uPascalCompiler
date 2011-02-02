using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.LexicalAnalyzer
{
    /// <summary>
    /// A token to associate a tag to
    /// </summary>
    class Token
    {
        /// <summary>
        /// Constructor for tag
        /// </summary>
        /// <param name="t"></param>
        public Token(int t)
        {
            Tag = (Tags)t;
            Lexeme = Convert.ToChar(t).ToString();
        }

        /// <summary>
        /// Get and Set tag
        /// </summary>
        public Tags Tag
        {
            get;
            set;
        }
        public string Lexeme
        {
            get;
            set;
        }
        /// <summary>
        /// Print out the tag
        /// </summary>
        /// <returns>Tag</returns>
        public override string ToString()
        {
            return Tag.ToString();
        }
    }
}
