using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.LexicalAnalyzer.Library;

namespace Compiler.LexicalAnalyzer
{
    /// <summary>
    /// A token to associate a tag to
    /// </summary>
    class Token
    {
        public Token ()
        { 
        }

        /// <summary>
        /// Constructor for Token
        /// </summary>
        /// <param name="t"></param>
        public Token(int? t)
        {
            if(t == null)
            {
                Tag = null;
            }
            else
            {
                Tag = (Tags)t;
            }
            Lexeme = Convert.ToChar(t).ToString();
        }

        /// <summary>
        /// Get and Set tag
        /// </summary>
        public Tags? Tag
        {
            get;
            set;
        }
        /// <summary>
        /// Get and set the Lexeme
        /// </summary>
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
