using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.Library
{
    /// <summary>
    /// Enum values for all of the tokens
    /// </summary>
    public enum Tags
    {
        MP_AND = 256,
        MP_BEGIN, MP_DIV, MP_DO, MP_DOWNTO,
        MP_ELSE, MP_END, MP_FIXED, MP_FLOAT, MP_FOR,
        MP_FUNCTION, MP_IF, MP_INTEGER, MP_MOD, MP_NOT,
        MP_OR, MP_PROCEDURE, MP_PROGRAM, MP_READ, MP_REPEAT,
        MP_THEN, MP_TO, MP_UNTIL, MP_VAR, MP_WHILE,
        MP_WRITE, MP_WRITELN, MP_IDENTIFIER,
        MP_GEQUAL, MP_LEQUAL,
        MP_NEQUAL, MP_ASSIGN,
        MP_INTEGER_LIT,
        MP_FIXED_LIT, MP_FLOAT_LIT, MP_STRING_LIT,
        MP_ERROR, MP_RUN_COMMENT, MP_RUN_STRING,
        MP_COLON = 58,
        MP_PERIOD = 46,
        MP_COMMA = 44,
        MP_SCOLON = 59,
        MP_LPAREN = 40,
        MP_RPAREN = 41,
        MP_EQUAL = 61,
        MP_GTHAN = 62,
        MP_LTHAN = 60,
        MP_PLUS = 43,
        MP_MINUS = 45,
        MP_TIMES = 42,
        MP_EOF = 3,
        DUMMYTAG1 = 999,
        DUMMYTAG2,
        DUMMYTAG3,
        DUMMYTAG4,
        DUMMYTAG5,
        DUMMYTAG6,
        DUMMYTAG7,
        DUMMYTAG8,
        DUMMYTAG9,
        DUMMYTAG10
    }

    /// <summary>
    /// State enumerations for modeling a state machine
    /// </summary>
    public enum States
    {
        S0 = 0,
        S1, S2, S3, S4, S5, S6, S7

    }
}
