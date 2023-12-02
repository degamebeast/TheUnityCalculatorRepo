//Created by: Deontae Albertie

using System.Collections.Generic;
using UnityEngine;

namespace delib.calculate.unity
{
    public static class CalcUnityHelper
    {
        public static int GetTextWidth(string text, Font textFont, int size = 0, FontStyle fontStyle = FontStyle.Normal)
        {
            textFont.RequestCharactersInTexture(text, size, fontStyle);

            int textWidth = 0;


            CharacterInfo curInfo = new CharacterInfo();
            for (int stringIndex = 0; stringIndex < text.Length; stringIndex++)
            {
                textFont.GetCharacterInfo(text[stringIndex], out curInfo, size, fontStyle);
                textWidth += curInfo.advance;
            }

            return textWidth;
        }
        //the default color settings for each token type
        //NOTE: every token MUST have an entry in this dictionary
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

