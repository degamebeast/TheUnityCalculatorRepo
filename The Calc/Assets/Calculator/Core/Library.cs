//Created by: Deontae Albertie

using System.Collections.Generic;
using System.Reflection;

namespace delib.calculate
{
    public static class Library
    {

        //System.Reflection BindingFlag that grants access to all field variables
        public const BindingFlags AllClassVariablesBindingFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        //characters that act as seperators for each token value
        //extra and or excess delimiters are ignored
        public static List<char> Delimiters = new List<char>()
        {
            ' '
        };

        //which characters are paired with which token types
        public static BiDictionary<char, TokenTypeValue> SymbolTokenTypePairs = new BiDictionary<char, TokenTypeValue>()
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
            {'.' , TokenTypeValue.Dot },
            {'^' , TokenTypeValue.Exponent },
            {'<' , TokenTypeValue.Left_Shift },
            {'>' , TokenTypeValue.Right_Shift },

        };

        public const int ConstantPriority = 7;//this value represents the current priority level of the constant token type. This value should always be the HIGHEST priority in the list

        //EVERY TOKEN TYPE IN THE ENUM MUST ALSO BE GIVEN AN ENTRY IN HERE
        //The operation execution priority for each token
        public static Dictionary<TokenTypeValue, uint> TokenPriority = new Dictionary<TokenTypeValue, uint>()
        {
            {TokenTypeValue.Null, 0},
            {TokenTypeValue.Ignore, 0},
            {TokenTypeValue.Operation, 0},
            {TokenTypeValue.Dot, 0},
            {TokenTypeValue.Seperator, 0},
            {TokenTypeValue.End_Statement, 0},
            {TokenTypeValue.Open_Paren, 0},
            {TokenTypeValue.Close_Paren, 0},


            {TokenTypeValue.Parameters, 7},
            {TokenTypeValue.Invalid, 7},
            {TokenTypeValue.Identifier, 7},
            {TokenTypeValue.Integer, 7},
            {TokenTypeValue.Constant, 7},

            {TokenTypeValue.ArgumentFunction, 5},
            {TokenTypeValue.Function, 6},
            {TokenTypeValue.Argument, 6},

            {TokenTypeValue.Expression, 5},
            {TokenTypeValue.Variable, 5},



            {TokenTypeValue.Left_Shift, 4},
            {TokenTypeValue.Right_Shift, 4},
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
                    TokenTypeValue.Ignore,
                    TokenTypeValue.Any,
                    TokenTypeValue.Identifier,
                    TokenTypeValue.Keyword,
                    TokenTypeValue.Constant,
                    TokenTypeValue.Integer,
                    TokenTypeValue.Variable,
                    TokenTypeValue.Expression,
                    TokenTypeValue.Function,
                    TokenTypeValue.Parameters,
                    TokenTypeValue.Argument,
                    TokenTypeValue.ArgumentFunction,
                    TokenTypeValue.Dot,
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
                    TokenTypeValue.Left_Shift,
                    TokenTypeValue.Right_Shift,
                }
            },
            {
                TokenTypeValue.Constant,
                new List<TokenTypeValue>()
                {
                    TokenTypeValue.Constant,
                    TokenTypeValue.Integer,
                    TokenTypeValue.Variable,
                    //TokenTypeValue.Function,
                    TokenTypeValue.ArgumentFunction,
                    TokenTypeValue.Argument,
                    TokenTypeValue.Expression,
                    TokenTypeValue.Parameters
                }
            },
            {
                TokenTypeValue.Variable,
                new List<TokenTypeValue>()
                {
                    TokenTypeValue.Variable,
                    TokenTypeValue.Argument,
                    TokenTypeValue.Identifier
                }
            },
            {
                TokenTypeValue.Expression,
                new List<TokenTypeValue>()
                {
                    TokenTypeValue.Expression,
                    TokenTypeValue.Parameters
                }
            },
            {
                TokenTypeValue.Identifier,
                new List<TokenTypeValue>()
                {
                    TokenTypeValue.Identifier,
                    TokenTypeValue.Variable,
                    TokenTypeValue.ArgumentFunction,
                    TokenTypeValue.Function,
                    TokenTypeValue.Argument
                }
            },
            {
                TokenTypeValue.Operation,
                new List<TokenTypeValue>()
                {
                    TokenTypeValue.Assignment,
                    TokenTypeValue.ArgumentFunction,
                    TokenTypeValue.Function,
                    TokenTypeValue.Exponent,
                    TokenTypeValue.Multiplication,
                    TokenTypeValue.Division,
                    TokenTypeValue.Modulo,
                    TokenTypeValue.Addition,
                    TokenTypeValue.Subtraction,
                    TokenTypeValue.Left_Shift,
                    TokenTypeValue.Right_Shift,
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
                TokenTypeValue.Left_Shift,
                new List<Operands>()
                {
                    new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant),
                    new Operands(TokenTypeValue.Constant, TokenTypeValue.Operation),
                    new Operands(TokenTypeValue.Operation, TokenTypeValue.Constant),
                    new Operands(TokenTypeValue.Operation, TokenTypeValue.Operation),
                }
            },
            {
                TokenTypeValue.Right_Shift,
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
                    new Operands(TokenTypeValue.Any, TokenTypeValue.Parameters),
                }
            },
            {
                TokenTypeValue.ArgumentFunction,
                new List<Operands>()
                {
                    new Operands(TokenTypeValue.Argument, TokenTypeValue.Parameters),
                }
            }
        };

        //token types that can be represented with a name
        public static List<TokenTypeValue> NamedTypes = new List<TokenTypeValue>()
        {
            TokenTypeValue.Identifier,
            TokenTypeValue.Variable,
            TokenTypeValue.ArgumentFunction,
            TokenTypeValue.Argument,
            TokenTypeValue.Function,
            TokenTypeValue.Keyword
        };

        //token type's that represent some form of expression
        public static List<TokenTypeValue> ExpressiveTypes = new List<TokenTypeValue>()
        {
            TokenTypeValue.Expression,
            TokenTypeValue.Parameters
        };

        //list of all Types that are considered valid Constant inputs
        public static List<System.Type> CaclulatorConstantTypes = new List<System.Type>()
        {
            typeof(Constant),
            typeof(Integer),
            typeof(float),
            typeof(int),
        };

        //basic calculator operations
        public static Dictionary<Operation, Function> defaultOperations = new Dictionary<Operation, Function>()
        {

            {
                new Operation(TokenTypeValue.Multiplication, new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Multiplication)
            },
            {
                new Operation(TokenTypeValue.Multiplication, new Operands(TokenTypeValue.Integer, TokenTypeValue.Integer)),
                new OperationFunction(Operations.Multiplication)
            },
            {
                new Operation(TokenTypeValue.Multiplication, new Operands(TokenTypeValue.Integer, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Multiplication)
            },
            {
                new Operation(TokenTypeValue.Multiplication, new Operands(TokenTypeValue.Constant, TokenTypeValue.Integer)),
                new OperationFunction(Operations.Multiplication)
            },
            {
                new Operation(TokenTypeValue.Division, new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Division)
            },
            {
                new Operation(TokenTypeValue.Division, new Operands(TokenTypeValue.Integer, TokenTypeValue.Integer)),
                new OperationFunction(Operations.Division)
            },
            {
                new Operation(TokenTypeValue.Division, new Operands(TokenTypeValue.Integer, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Division)
            },
            {
                new Operation(TokenTypeValue.Division, new Operands(TokenTypeValue.Constant, TokenTypeValue.Integer)),
                new OperationFunction(Operations.Division)
            },
            {
                new Operation(TokenTypeValue.Modulo, new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Modulo)
            },
            {
                new Operation(TokenTypeValue.Modulo, new Operands(TokenTypeValue.Integer, TokenTypeValue.Integer)),
                new OperationFunction(Operations.Modulo)
            },
            {
                new Operation(TokenTypeValue.Modulo, new Operands(TokenTypeValue.Integer, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Modulo)
            },
            {
                new Operation(TokenTypeValue.Modulo, new Operands(TokenTypeValue.Constant, TokenTypeValue.Integer)),
                new OperationFunction(Operations.Modulo)
            },
            {
                new Operation(TokenTypeValue.Addition, new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Addition)
            },
            {
                new Operation(TokenTypeValue.Addition, new Operands(TokenTypeValue.Integer, TokenTypeValue.Integer)),
                new OperationFunction(Operations.Addition)
            },
            {
                new Operation(TokenTypeValue.Addition, new Operands(TokenTypeValue.Integer, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Addition)
            },
            {
                new Operation(TokenTypeValue.Addition, new Operands(TokenTypeValue.Constant, TokenTypeValue.Integer)),
                new OperationFunction(Operations.Addition)
            },
            {
                new Operation(TokenTypeValue.Subtraction, new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Subtraction)
            },
            {
                new Operation(TokenTypeValue.Subtraction, new Operands(TokenTypeValue.Integer, TokenTypeValue.Integer)),
                new OperationFunction(Operations.Subtraction)
            },
            {
                new Operation(TokenTypeValue.Subtraction, new Operands(TokenTypeValue.Integer, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Subtraction)
            },
            {
                new Operation(TokenTypeValue.Subtraction, new Operands(TokenTypeValue.Constant, TokenTypeValue.Integer)),
                new OperationFunction(Operations.Subtraction)
            },
            {
                new Operation(TokenTypeValue.Exponent, new Operands(TokenTypeValue.Constant, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Exponent)
            },
            {
                new Operation(TokenTypeValue.Exponent, new Operands(TokenTypeValue.Integer, TokenTypeValue.Integer)),
                new OperationFunction(Operations.Exponent)
            },
            {
                new Operation(TokenTypeValue.Exponent, new Operands(TokenTypeValue.Integer, TokenTypeValue.Constant)),
                new OperationFunction(Operations.Exponent)
            },
            {
                new Operation(TokenTypeValue.Exponent, new Operands(TokenTypeValue.Constant, TokenTypeValue.Integer)),
                new OperationFunction(Operations.Exponent)
            },
            {
                new Operation(TokenTypeValue.Left_Shift, new Operands(TokenTypeValue.Constant, TokenTypeValue.Integer)),
                new OperationFunction(Operations.LeftShift)
            },
            {
                new Operation(TokenTypeValue.Right_Shift, new Operands(TokenTypeValue.Integer, TokenTypeValue.Integer)),
                new OperationFunction(Operations.RightShift)
            },
            {
                new Operation(TokenTypeValue.Assignment, new Operands(TokenTypeValue.Variable, TokenTypeValue.Constant)),
                new Function("assignment", 2, Calculator.AssignmentOperator)
            },
            {
                new Operation(TokenTypeValue.Assignment, new Operands(TokenTypeValue.Identifier, TokenTypeValue.Constant)),
                new Function("assignment", 2, Calculator.AssignmentOperator)
            },
            {
                new Operation(TokenTypeValue.Assignment, new Operands(TokenTypeValue.Variable, TokenTypeValue.Integer)),
                new Function("assignment", 2, Calculator.AssignmentOperator)
            },
            {
                new Operation(TokenTypeValue.Assignment, new Operands(TokenTypeValue.Identifier, TokenTypeValue.Integer)),
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
                    return System.MathF.Abs(args[0].Value);
                })
            },
            {
                "max",
                new Function("max", 2, (calc, args) =>
                {
                    return System.MathF.Max(args[0].Value, args[1].Value);
                })
            }
        };

        public static List<string> Keywords = new List<string>()
        {
            "local",
            "static",
        };
    }
}