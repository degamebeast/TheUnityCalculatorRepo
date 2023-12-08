//Created by: Deontae Albertie

using System;
using System.Collections.Generic;
using System.Reflection;

namespace delib.calculate
{
    //class for producing calculations
    public class Calculator
    {
        public const int argLimit = 8;

        public Dictionary<string, Function> functionMemory;//all the functions stored in this calculator instance
        public Dictionary<string, Variable> variableMemory;//all the variables stored in this calculator instance
        protected Dictionary<string, Argument> argumentMemory;//the arguments stored in this calculator instance (from the most recent call to calculate)

        public Calculator(params object[] initArgs)
        {
            functionMemory = new Dictionary<string, Function>(Library.defaultFunctions);
            variableMemory = new Dictionary<string, Variable>();
            variableMemory.Add("ans", new Variable("ans"));
            variableMemory.Add("NaN", new Variable("NaN", float.NaN));
            argumentMemory = new Dictionary<string, Argument>();

            SetArguments(initArgs);

        }

        public Constant Calculate(string expr, params object[] args)
        {
            return Calculate(new Expression(expr),args);
        }

        //solves the given expression and then stores the answer inside of the "ans" calculator variable
        public Constant Calculate(Expression expr, params object[] args)
        {
            SetArguments(args);
            Expression[] statements = expr.GetStatements();

            foreach (Expression statementExpr in statements)
            {
                variableMemory["ans"].Set(ExpressResult(ResolveIdentifiers(statementExpr,true)));
            }

            return variableMemory["ans"].Value;
        }

        public void SetArguments(params object[] args)
        {
            argumentMemory.Clear();

            if (args.Length > argLimit)
                throw new ArgumentException($"to many args where passed into calculator SetArgument:\tThe limit is {argLimit}");

            for (int i = 0; i < argLimit; i++)
            {
                    string argName = $"arg{i}";
                if(i < args.Length)
                {
                    argumentMemory.Add(argName, new Argument(argName, args[i]));
                }
                else
                {
                    argumentMemory.Add(argName, null);
                }
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
                case TokenTypeValue.Integer:
                    return (Integer)(convert.Value);
                default:
                    return convert.Value;
            }
        }

        //returns true if the given variable was successfully added to memory and false otherwise
        public bool AddFunctionToMemory(Function funct)
        {
            return functionMemory.TryAdd(funct.FuncName, funct);
        }

        public bool AddFunctionToMemory(string funcName)//creates empty entry
        {
            return AddFunctionToMemory(new Function(funcName, 0, null));
        }

        public bool AddFunctionToMemory(string funcName, int argCount, CalculatorFunction funct)
        {
            return AddFunctionToMemory(new Function(funcName, argCount, funct ));
        }

        //returns true if the given variable was successfully added to memory and false otherwise
        public bool AddVariableToMemory(Variable var)
        {
            return variableMemory.TryAdd(var.VarName, var);
        }
        public bool AddVariableToMemory(string varName, Constant val = null)
        {
            return AddVariableToMemory(new Variable(varName, val));
        }
        //special overloads for adding integer variables
        //this way they can be type distiguished later
        public bool AddVariableToMemory(string varName, int val)
        {
            return AddVariableToMemory(new Variable(varName, new Integer(val)));
        }
        public bool AddVariableToMemory(string varName, Integer val)
        {
            return AddVariableToMemory(new Variable(varName, val));
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
        public Expression ResolveIdentifiers(Expression expr, bool addVariableIfNotInMemory = false)
        {
            for(int i = 0; i < expr.Count; i++)
            {
                Token cur = expr[i];
                if (cur.Type == TokenTypeValue.Expression)
                {
                    ResolveIdentifiers(cur.Expression, addVariableIfNotInMemory);
                }
                else if (cur.Type == TokenTypeValue.Identifier)
                {
                    Function funct;
                    Variable var;
                    Argument arg;
                    if (functionMemory.TryGetValue(cur.ObjectName, out funct))
                    {
                        expr[i] = new Token(cur.ObjectName, TokenTypeValue.Function);
                        continue;
                    }

                    string[] argSplit = cur.ObjectName.Split('.');
                    if (argumentMemory.TryGetValue(argSplit[0], out arg))
                    {
                        expr[i] = new Token(cur.ObjectName, TokenTypeValue.Argument);
                        continue;
                    }


                    if (variableMemory.TryGetValue(cur.ObjectName, out var))
                    {
                        expr[i] = new Token(cur.ObjectName, TokenTypeValue.Variable);
                    }
                    else if(addVariableIfNotInMemory)
                    {
                        variableMemory.Add(cur.ObjectName, new Variable(cur.ObjectName));
                        expr[i] = new Token(cur.ObjectName, TokenTypeValue.Variable);
                    }


                }
            }

            return expr;
        }

        public Constant ResolveArgument(string resArg)
        {
            string[] argPath = resArg.Split('.');
            object arg = argumentMemory[argPath[0]].ObjectValue;

            object curField = arg;

            for (int argIndex = 1; argIndex < argPath.Length; argIndex++)
            {
                System.Type curType = curField.GetType();
                FieldInfo curFieldInfo = curType.GetField($"{argPath[argIndex]}", Library.AllClassVariablesBindingFlag);
                PropertyInfo curPropertyInfo = curType.GetProperty($"{argPath[argIndex]}", Library.AllClassVariablesBindingFlag);


                if (curPropertyInfo != null)
                {
                    curField = curPropertyInfo.GetValue(curField);
                    continue;
                }

                if (curFieldInfo == null)
                    return null;
                curField = curFieldInfo.GetValue(curField);
            }

            Constant resolvedField = null;
            switch (curField)
            {
                case Integer:
                    resolvedField = curField as Integer;
                    break;
                case int:
                    resolvedField = new Integer((int)curField);
                    break;
                case Constant:
                    resolvedField = curField as Constant;
                    break;
                case float:
                    resolvedField = new Constant((float)curField);
                    break;
            }

            return resolvedField;
        }


        //takes the token at the given 'index' in 'expr' and reolves it using it's left and right nieghbors as operands
        // returns which index the calculator should now be pointing at
        private int Resolve(Expression expr, int index)
        {
            switch (expr[index].Type.Value)
            {
                case TokenTypeValue.Null://if null then just continue looping
                    return index + 1;
                case TokenTypeValue.Argument://---
                    expr[index] = new Token(ResolveArgument(expr[index].ObjectName));
                    return index;
                case TokenTypeValue.Variable://if a variable, resolve down to it's stored Constant
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
                    Operation currentOp = new Operation(symbol.Type, new Operands(prev.Type, next.Type));
                    //UnityEngine.Debug.Log(currentOp);

                    Function funct = Library.defaultOperations[currentOp];
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
            bool intCheck = args[1].Value is Integer;
            Variable newVar = intCheck? new Variable(args[0].ObjectName, args[1].Value as Integer) : new Variable(args[0].ObjectName, args[1].Value);
            if (!calc.variableMemory.TryAdd(newVar.VarName, newVar))
            {
                calc.variableMemory[newVar.VarName] = newVar;
            }

            return newVar.Value;
        }

        //calls the function of name(args[0].ObjectName) in the given calculator and gives it the parameters-expression that is stored in args[1] for it's parameters
        public static Constant FunctionCall(Calculator calc, params Token[] args)
        {
            Function curFunct = null;
            if (calc.functionMemory.TryGetValue(args[0].ObjectName, out curFunct))
            {
                if (args[1].Expression == null) return float.NaN;
                

                Expression parameters = args[1].Expression.GetResolveParameters();

                for (int i = 0; i < parameters.Count; i++)
                {
                    if (parameters[i].Type == TokenTypeValue.Null) continue;
                    parameters[i] = new Token(calc.ExpressResult(parameters[i].Expression));
                }

                return curFunct.Call(calc, parameters.RemoveAllNulls().ToArray());
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
            value = float.NaN;
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

    //Wrapper class for c# int that extends the logic from the Constant class
    //Constant class 'value' member will always be equal to float.NaN when checking from an Integer class instance
    public class Integer : Constant
    {
        private int intValue;

        public Integer() : base()
        {

        }
        public Integer(int initVal) : base(float.NaN)
        {
            intValue = initVal;
        }

        public override string ToString()
        {
            return intValue.ToString();
        }

        public static implicit operator float(Integer integer)
        {
            if (integer == null)
                return float.NaN;
            return integer.intValue;
        }

        public static implicit operator Integer(float floatValue)
        {
            return new Integer((int)floatValue);
        }

        //will always convert down to 0 if integer object is null
        public static implicit operator int(Integer integer)
        {
            if (integer == null)
                return 0;
            return integer.intValue;
        }

        public static implicit operator Integer(int intValue)
        {
            return new Integer(intValue);
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

    //represents an evaluation argument
    public class Argument
    {
        public string ArgName { get; protected set; }

        public System.Type ArgType { get; set; }
        public object ObjectValue { get; set; }
        public bool Assigned
        {
            get
            {
                return ObjectValue != null;
            }
        }

        public Argument()
        {
            ArgName = null;
            ArgType = null;
            ObjectValue = null;
        }

        public Argument(string name, object obj = null)
        {
            ArgName = name;
            ArgType = obj.GetType();
            ObjectValue = obj;

            if (obj is System.Type)
                ArgType = (System.Type)obj;
        }
    }

}

