using System;
using System.Collections.Generic;

namespace delib.calculate
{
    //class for producing calculations
    public class Calculator
    {
        public Dictionary<string, Function> functionMemory;//all the functions stored in this calculator instance
        public Dictionary<string, Variable> variableMemory;//all the variables stored in this calculator instance

        public Calculator()
        {
            functionMemory = new Dictionary<string, Function>(Library.defaultFunctions);
            variableMemory = new Dictionary<string, Variable>();
            variableMemory.Add("ans", new Variable("ans"));
            variableMemory.Add("NaN", new Variable("NaN", float.NaN));

        }

        public void Calculate(string expr)
        {
            Calculate(new Expression(expr));
        }

        //solves the given expression and then stores the answer inside of the "ans" calculator variable
        public void Calculate(Expression expr)
        {
            Expression[] statements = expr.GetStatements();

            foreach (Expression statementExpr in statements)
            {
                variableMemory["ans"].Set(ExpressResult(ResolveIdentifiers(statementExpr)));
            }

        }

        //resolves the given expression down to a single constant token
        public Constant ExpressResult(Expression expr)
        {

            //starting from ConstantPriority - 1 so that constant values aren't used as operators
            for (int priority = Library.ConstantPriority-1; priority > 0; priority--)
            {
                for (int index = 0; index < expr.Count; index++)
                {
                    if (expr[index].Priority == priority)
                    {
                        index = Resolve(expr, index);
                    }
                }
            }



            return GenerateOutput(expr[1]);
        }

        //returns the value of the passed in token based on it's type
        public float GenerateOutput(Token convert)
        {
            switch (convert.Type.Value)
            {
                case TokenTypeValue.Variable:
                    return variableMemory[convert.ObjectName].Value;
                default:
                    return convert.Value;
            }
        }

        //returns a list of all variables in this calculators memory
        public List<Variable> GetVariablesInMemory()
        {
            List<Variable> vars = new List<Variable>();

            foreach (KeyValuePair<string, Variable> pair in variableMemory)
            {
                vars.Add(pair.Value);
            }

            return vars;
        }


        //resolves all identifiers in an expression down to the related values in this calculator
        //if an identifier is not recognized it gets added to the variable memory (the added variable will be unitialized)
        public Expression ResolveIdentifiers(Expression expr)
        {
            for(int i = 0; i < expr.Count; i++)
            {
                Token cur = expr[i];
                if(cur.Type == TokenTypeValue.Identifier)
                {
                    Function funct;
                    Variable var;
                    if (functionMemory.TryGetValue(cur.ObjectName, out funct))
                    {
                        expr[i] = new Token(cur.ObjectName, TokenTypeValue.Function);
                        continue;
                    }

                    if (!variableMemory.TryGetValue(cur.ObjectName, out var))
                    {
                        variableMemory.Add(cur.ObjectName, new Variable(cur.ObjectName));
                    }
                    expr[i] = new Token(cur.ObjectName, TokenTypeValue.Variable);

                }
            }

            return expr;
        }


        //takes the token at the given 'index' in 'expr' and reolves it using it's left and right nieghbors as operands
        // returns which index the calculator should now be pointing at
        private int Resolve(Expression expr, int index)
        {
            switch (expr[index].Type.Value)
            {
                case TokenTypeValue.Null://if null then just continue looping
                    return index + 1;
                case TokenTypeValue.Variable://if a variable resolve down to it's stored Constant
                    if (expr[index + 1].Type == TokenTypeValue.Assignment)
                        return index + 1;
                    expr[index] = new Token(variableMemory[expr[index].ObjectName].Value);
                    return index;
                case TokenTypeValue.Function:
                    expr[index] = new Token(FunctionCall(this, expr[index], expr[index + 1]));
                    expr.RemoveAt(index + 1);
                    return index;
                case TokenTypeValue.Expression://If an expression calculate the full expression 
                    expr[index] = new Token(ExpressResult(expr[index].Expression));
                    return index;
                default://otherwise assume is an operator and perform the operation
                    Token prev = expr[index - 1];
                    Token symbol = expr[index];
                    Token next = expr[index + 1];
                    Function funct = Library.defaultOperations[new Operation(symbol.Type, new Operands(prev.Type, next.Type))];
                    expr[index - 1] = new Token(funct.Call(this, prev, next));
                    expr.RemoveRange(index, 2);
                    return index - 1;

            }
        }


        #region static functions
        //assigns a variable of name(args[0].ObjectName) to the value stored in args[1] in the given calculator
        //this will overwrite it's current value if the variable already exists
        public static Constant AssignmentOperator(Calculator calc, params Token[] args)
        {
            Variable newVar = new Variable(args[0].ObjectName, args[1].Value);
            if (!calc.variableMemory.TryAdd(newVar.VarName, newVar))
            {
                calc.variableMemory[newVar.VarName] = newVar;
            }

            return newVar.Value;
        }

        //calls the function of name(args[0].ObjectName) in the given calculator and gives it the arguments-expression that is stored in args[1] for it's parameters
        public static Constant FunctionCall(Calculator calc, params Token[] args)
        {
            Function curFunct = null;
            if (calc.functionMemory.TryGetValue(args[0].ObjectName, out curFunct))
            {
                Expression arguments = args[1].Expression.GetResolveArguments();

                for (int i = 0; i < arguments.Count; i++)
                {
                    if (arguments[i].Type == TokenTypeValue.Null) continue;
                    arguments[i] = new Token(calc.ExpressResult(arguments[i].Expression));
                }

                return curFunct.Call(calc, arguments.RemoveNulls().ToArray());
            }

            return null;
        }
        #endregion
    }


    


    //represents a constant mathematical value
    public class Constant
    {
        private float value;

        public Constant()
        {

        }
        public Constant(float initVal)
        {
            value = initVal;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static implicit operator float(Constant constant)
        {
            if (constant == null)
                return float.NaN;
            return constant.value;
        }

        public static implicit operator Constant(float floatValue)
        {
            return new Constant(floatValue);
        }
    }


    //represents a programmatical variable
    public class Variable
    {
        public string VarName { get; protected set; }

        public Constant Value { get; protected set; }
        public bool Assigned
        {
            get
            {
                return Value != null;
            }
        }

        public Variable()
        {
            VarName = null;
            Value = null;
        }

        public Variable(string name, Constant val = null)
        {
            VarName = name;
            Value = val;
        }

        public virtual void Set(Constant value)
        {
            Value = value;
        }
    }

}

