using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Compiler.Library
{
    public enum BranchType
    {
        br,
        beq,
        bge,
        bgt,
        ble,
        blt,
        bne,
        brfalse,
        brtrue
    }
    
    public enum IOMode
    {
        In = 1,
        InOut
    }
    public enum VariableType
    {
        [Description("Int32")]
        Integer = 1,
        Float,
        String,
        Fixed,
        Null
    }
    public enum SymbolType
    {
        VariableSymbol = 1,
        ProcedureSymbol,
        ParameterSymbol,
        FunctionSymbol
    }
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
        MP_EOF = 3
    }

    /// <summary>
    /// State enumerations for modeling a state machine
    /// </summary>
    public enum States
    {
        S0 = 0,
        S1, S2, S3, S4, S5, S6, S7

    }


    static class Enumerations
    {
        /// <summary>
        /// this method will get the description of an enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerationValue"></param>
        /// <returns></returns>
        public static string GetDescription<T>(this object enumerationValue)
            where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();

        }
    }
}


