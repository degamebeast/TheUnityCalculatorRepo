//Created by: Deontae Albertie

using System.Collections.Generic;
using UnityEngine;

namespace delib.calculate.unity
{
    public static class CalcUnityHelper
    {
        public static Dictionary<TokenTypeValue, Color> TokenInspectorColor = new Dictionary<TokenTypeValue, Color>()
        {
            {TokenTypeValue.Null, Color.white},
            {TokenTypeValue.Ignore, Color.white},
            {TokenTypeValue.Operation, Color.red},
            {TokenTypeValue.Dot, Color.red},
            {TokenTypeValue.Seperator, Color.red},
            {TokenTypeValue.End_Statement, Color.red},
            {TokenTypeValue.Open_Paren, Color.red},
            {TokenTypeValue.Close_Paren, Color.red},


            {TokenTypeValue.Parameters, Color.white},
            {TokenTypeValue.Invalid, Color.red},
            {TokenTypeValue.Identifier, Color.cyan},
            {TokenTypeValue.Integer, Color.green},
            {TokenTypeValue.Constant, Color.green},

            {TokenTypeValue.Function, Color.yellow},

            {TokenTypeValue.Expression, Color.white},
            {TokenTypeValue.Argument, Color.magenta},
            {TokenTypeValue.Variable, Color.cyan},



            {TokenTypeValue.Left_Shift, Color.red},
            {TokenTypeValue.Right_Shift, Color.red},
            {TokenTypeValue.Exponent, Color.red},


            {TokenTypeValue.Multiplication, Color.red},
            {TokenTypeValue.Division, Color.red},
            {TokenTypeValue.Modulo, Color.red},

            {TokenTypeValue.Addition, Color.red},
            {TokenTypeValue.Subtraction, Color.red},

            {TokenTypeValue.Assignment, Color.red},

        };
    }

    public static class CalculatorUnityExtensionMethods
    {

    }
}

