using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Library;

namespace Compiler.Lexer
{
    /// <summary>
    /// A token to associate a tag to
    /// </summary>
    class Token
    {
        public Token ()
        {
            tag = null;
        }

        /// <summary>
        /// Constructor for Token
        /// </summary>
        /// <param name="t"></param>
        public Token(int t,int line,int column)
        {
            tag = (Tags)t;
            lexeme = Convert.ToChar(t).ToString();
            this.line = line;
            this.column = column;
            
        }
        /// <summary>
        /// Gets and sets the column
        /// </summary>
        public int column
        {
            get;
            set;
        }
        /// <summary>
        /// Gets and sets the column
        /// </summary>
        public int line
        {
            get;
            set;
        }
        /// <summary>
        /// Get and Set tag
        /// </summary>
        public Tags? tag
        {
            get;
            set;
        }
        /// <summary>
        /// Get and set the Lexeme
        /// </summary>
        public string lexeme
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
            return tag.ToString();
        }
    }
}
