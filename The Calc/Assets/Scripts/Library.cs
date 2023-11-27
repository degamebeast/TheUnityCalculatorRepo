using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace delib.calculate
{
    public static class Library
    {
        //characters that act as seperators for each token value
        //extra and or excess delimiters are ignored
        public static List<char> Delimiters = new List<char>()
        {
            ' '
        };

        //which characters represent which token types
        public static Dictionary<char, TokenTypeValue> SymbolToTokenType = new Dictionary<char, TokenTypeValue>()
        {
            {',' , TokenTypeValue.Seperator },
            {';' , TokenTypeValue.End_Statement },
            {'=' , TokenTypeValue.Assignment },
            {'(' , TokenTypeValue.Open_Paren },
            {')' , TokenTypeValue.Close_Paren },
            {'*' , TokenTypeValue.Multiplication },
            {'/' , TokenTypeValue.Division },
            {'%' , TokenTypeValue.Modulo },
            {'+' , TokenTypeValue.Addition },
            {'-' , TokenTypeValue.Subtraction},
            {'.' , TokenTypeValue.Decimal_Point },
            {'^' , TokenTypeValue.Exponent }

        };

        public const int ConstantPriority = 7;//this value represents the current priority level of the constant token type. This value should always be the HIGHEST priority in the list

        //EVERY TOKEN TYPE IN THE ENUM MUST ALSO BE GIVEN AN ENTRY IN HERE
        //The operation execution priority for each token
        public static Dictionary<TokenTypeValue, uint> TokenPriority = new Dictionary<TokenTypeValue, uint>()
        {
            {TokenTypeValue.Null, 0},
            {TokenTypeValue.Operation, 0},
            {TokenTypeValue.Decimal_Point, 0},
            {TokenTypeValue.Seperator, 0},
            {TokenTypeValue.End_Statement, 0},
            {TokenTypeValue.Open_Paren, 0},
            {TokenTypeValue.Close_Paren, 0},


            {TokenTypeValue.Arguments, 7},
            {TokenTypeValue.Invalid, 7},
            {TokenTypeValue.Identifier, 7},
            {TokenTypeValue.Constant, 7},

            {TokenTypeValue.Function, 6},

            {TokenTypeValue.Expression, 5},
            {TokenTypeValue.Variable, 5},



            {TokenTypeValue.Exponent, 4},


            {TokenTypeValue.Multiplication, 3},
            {TokenTypeValue.Division, 3},
            {TokenTypeValue.Modulo, 3},

            {TokenTypeValue.Addition, 2},
            {TokenTypeValue.Subtraction, 2},

            {TokenTypeValue.Assignment, 1},

        };

        //lists of equatable types of the given token
        public static Dictionary<TokenTypeValue, List<TokenTypeValue>> ResolvingTypes = new Dictionary<TokenTypeValue, List<TokenTypeValue>>()
        {
            {
                TokenTypeValue.Any,
                new List<TokenTypeValue>()
                {
                    TokenTypeValue.Null,
                    TokenTypeValue.Invalid,
                    TokenTypeValue.Any,
                    TokenTypeValue.Identifier,
                    TokenTypeValue.Constant,
                    TokenTypeValue.Variable,
                    TokenTypeValue.Expression,
                    TokenTypeValue.Function,
                    TokenTypeValue.Arguments,
                    TokenTypeValue.Decimal_Point,
                    TokenTypeValue.Open_Paren,
                    TokenTypeValue.Close_Paren,
                    TokenTypeValue.Seperator,
                    TokenTypeValue.End_Statement,
                    TokenTypeValue.Operation,
                    TokenTypeValue.Exponent,
                    TokenTypeValue.Multiplication,
                    TokenTypeValue.Division,
                    TokenTypeValue.Modulo,
                    TokenTypeValue.Addition,
                    TokenTypeValue.Subtraction,
                    TokenTypeValue.Assignment,
                }
            },
            {
                TokenTypeValue.Constant,
                new List<TokenTypeValue>()
                {
                    TokenTypeValue.Constant,
                    TokenTypeValue.Variable,
                    TokenTypeValue.Expression,
                    TokenTypeValue.Arguments
                }
            },
            {
                TokenTypeValue.Variable,
                new List<TokenTypeValue>()
                {
                    TokenTypeValue.Variable,
                    TokenTypeValue.Identifier
                }
            },
            {
                TokenTypeValue.Expression,
                new List<TokenTypeValue>()
                {
                    TokenTypeValue.Expression,
                    TokenTypeValue.Arguments
                }
            },
            {
                TokenTypeValue.Operation,
                new List<TokenTypeValue>()
                {
                    TokenTypeValue.Assignment,
                    TokenTypeValue.Function,
                    TokenTypeValue.Exponent,
                    TokenTypeValue.Multiplication,
                    TokenTypeValue.Division,
                    TokenTypeValue.Modulo,
                    TokenTypeValue.Addition,
                    TokenTypeValue.Subtraction
                }
            }

        };

        //lists of valid operands for the given token types
        public static Dictionary<TokenTypeValue, List<Operands>> ValidOperands = new Dictionary<TokenTypeValue, List<Operands>>()
        {
            {
                TokenTypeValue.Multiplication,
                new List<Operands>()
                {
                    new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant),
                    new Operands(TokenTypeValue.Constant, TokenTypeValue.Operation),
                    new Operands(TokenTypeValue.Operation, TokenTypeValue.Constant),
                    new Operands(TokenTypeValue.Operation, TokenTypeValue.Operation),
                }
            },
            {
                TokenTypeValue.Division,
                new List<Operands>()
                {
                    new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant),
                    new Operands(TokenTypeValue.Constant, TokenTypeValue.Operation),
                    new Operands(TokenTypeValue.Operation, TokenTypeValue.Constant),
                    new Operands(TokenTypeValue.Operation, TokenTypeValue.Operation),
                }
            },
            {
                TokenTypeValue.Modulo,
                new List<Operands>()
                {
                    new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant),
                    new Operands(TokenTypeValue.Constant, TokenTypeValue.Operation),
                    new Operands(TokenTypeValue.Operation, TokenTypeValue.Constant),
                    new Operands(TokenTypeValue.Operation, TokenTypeValue.Operation),
                }
            },
            {
                TokenTypeValue.Addition,
                new List<Operands>()
                {
                    new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant),
                    new Operands(TokenTypeValue.Constant, TokenTypeValue.Operation),
                    new Operands(TokenTypeValue.Operation, TokenTypeValue.Constant),
                    new Operands(TokenTypeValue.Operation, TokenTypeValue.Operation),
                }
            },
            {
                TokenTypeValue.Subtraction,
                new List<Operands>()
                {
                    new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant),
                    new Operands(TokenTypeValue.Constant, TokenTypeValue.Operation),
                    new Operands(TokenTypeValue.Operation, TokenTypeValue.Constant),
                    new Operands(TokenTypeValue.Operation, TokenTypeValue.Operation),
                }
            },
            {
                TokenTypeValue.Exponent,
                new List<Operands>()
                {
                    new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant),
                    new Operands(TokenTypeValue.Constant, TokenTypeValue.Operation),
                    new Operands(TokenTypeValue.Operation, TokenTypeValue.Constant),
                    new Operands(TokenTypeValue.Operation, TokenTypeValue.Operation),
                }
            },
            {
                TokenTypeValue.Assignment,
                new List<Operands>()
                {
                    new Operands(TokenTypeValue.Variable, TokenTypeValue.Constant),
                    new Operands(TokenTypeValue.Variable, TokenTypeValue.Operation),
                }
            },
            {
                TokenTypeValue.Function,
                new List<Operands>()
                {
                    new Operands(TokenTypeValue.Any, TokenTypeValue.Arguments),
                }
            }
        };

        //token types that can be represented with a name
        public static List<TokenTypeValue> NamedTypes = new List<TokenTypeValue>()
        {
            TokenTypeValue.Identifier,
            TokenTypeValue.Variable,
            TokenTypeValue.Function
        };

        //token type's that represent some form of expression
        public static List<TokenTypeValue> ExpressiveTypes = new List<TokenTypeValue>()
        {
            TokenTypeValue.Expression,
            TokenTypeValue.Arguments
        };

        //basic calculator operations
        public static Dictionary<Operation, Function> defaultOperations = new Dictionary<Operation, Function>()
        {
            {
                new Operation(TokenTypeValue.Multiplication, new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Multiplication)
            },
            {
                new Operation(TokenTypeValue.Division, new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Division)
            },
            {
                new Operation(TokenTypeValue.Modulo, new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Modulo)
            },
            {
                new Operation(TokenTypeValue.Addition, new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Addition)
            },
            {
                new Operation(TokenTypeValue.Subtraction, new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Subtraction)
            },
            {
                new Operation(TokenTypeValue.Exponent, new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Exponent)
            },
            {
                new Operation(TokenTypeValue.Assignment, new Operands(TokenTypeValue.Variable, TokenTypeValue.Constant)),
                new Function("assignment", 2, Calculator.AssignmentOperator)
            },
            {
                new Operation(TokenTypeValue.Assignment, new Operands(TokenTypeValue.Identifier, TokenTypeValue.Constant)),
                new Function("assignment", 2, Calculator.AssignmentOperator)
            }
        };

        //basic calculator functions
        public static Dictionary<string, Function> defaultFunctions = new Dictionary<string, Function>()
        {
            {
                "abs",
                new Function("abs", 1, (calc, args) =>
                {
                    return MathF.Abs(args[0].Value);
                })
            },
            {
                "max",
                new Function("max", 2, (calc, args) =>
                {
                    return MathF.Max(args[0].Value, args[1].Value);
                })
            }
        };
    }
}