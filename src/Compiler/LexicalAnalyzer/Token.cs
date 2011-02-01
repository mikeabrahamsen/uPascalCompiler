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
        public Token(Tag t)
        {
            Tag = t;
        }

        /// <summary>
        /// Get and Set tag
        /// </summary>
        public Tag Tag
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
