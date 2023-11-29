//Created by: Deontae Albertie

using System;

namespace delib.calculate
{
    public enum TokenTypeValue { Null, Invalid, Any, Identifier, Constant, Integer, Variable, Expression, Function, Arguments, Decimal_Point, Open_Paren, Close_Paren, Seperator, End_Statement, Operation, Exponent, Multiplication, Division, Modulo, Addition, Subtraction, Assignment, Left_Shift, Right_Shift }

    //The key class for creating expression
    //represents a singular piece of an expression Ex. '6' '^' '+' '34' 'var'
    public class Token
    {
        public TokenType Type { get; protected set; }

        public string ObjectName { get; protected set; }

        public Constant Value { get; protected set; }
        public Expression Expression { get; protected set; }

        //the calculation priority of this token's type
        public uint Priority
        {
            get
            {
                return Library.TokenPriority[Type];
            }
        }

        #region static properties
        //returns a token representing the 'Null' type
        public static Token NullToken
        {
            get
            {
                return new Token(TokenTypeValue.Null);
            }
        }

        //returns a token representing the 'Invalid' type
        public static Token InvalidToken
        {
            get
            {
                return new Token(TokenTypeValue.Invalid);
            }
        }
        #endregion

        public Token()
        {
            Type = new TokenType(TokenTypeValue.Null);
            Value = null;
            ObjectName = null;
            Expression = null;
        }

        public Token(TokenTypeValue type)
        {
            Type = new TokenType(type);
            Value = null;
            ObjectName = null;
            Expression = null;
        }

        public Token(Constant constant)
        {

            Type = new TokenType(TokenTypeValue.Constant);
            Value = constant;
            ObjectName = null;
            Expression = null;

            switch (constant)
            {
                case Integer:
                    Type = new TokenType(TokenTypeValue.Integer);
                    break;
            }
        }

        public Token(Integer integer)
        {
            Type = new TokenType(TokenTypeValue.Integer);
            Value = integer;
            ObjectName = null;
            Expression = null;
        }

        public Token(Expression expr, TokenTypeValue expressiveType = TokenTypeValue.Expression)
        {
            Type = new TokenType(expressiveType);
            Value = null;
            ObjectName = null;
            Expression = expr;
        }

        public Token(string identity, TokenTypeValue namedType = TokenTypeValue.Identifier)
        {
            if (!IsNamedType(namedType))
                throw new ArgumentException("Argument was not a valid token type for the given constructor", "namedType");
            Type = new TokenType(namedType);
            Value = null;
            ObjectName = identity;
            Expression = null;
        }





        #region static functions

        //returns true if the token type is a type that can be represented with a name
        public static bool IsNamedType(TokenTypeValue token)
        {
            return Library.NamedTypes.Contains(token);
        }

        //returns true if the token type is a type that represents some form of expression
        public static bool IsExpressiveType(TokenTypeValue token)
        {
            return Library.ExpressiveTypes.Contains(token);
        }

        //returns true if 'current' can be resolved to the type passed in as 'conversion'
        public static bool ResolvesTo(TokenTypeValue current, TokenTypeValue conversion)
        {
            if (!Library.ResolvingTypes.ContainsKey(conversion)) return current == conversion;

            return Library.ResolvingTypes[conversion].Contains(current);
        }
        #endregion
    }
}