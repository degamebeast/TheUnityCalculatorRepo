//Created by: Deontae Albertie

using System;

namespace delib.calculate
{
    //delegate for all CalculatorFunction's
    public delegate Constant CalculatorFunction(Calculator calc, params Token[] constArgs);

    public class Function
    {
        public string FuncName { get; protected set; }
        public int ParamCount { get; protected set; }

        protected CalculatorFunction calculatorFunction;

        public Function()
        {
            FuncName = null;
            ParamCount = 0;
            calculatorFunction = null;
        }

        public Function(string name, int numArgs, CalculatorFunction val)
        {
            FuncName = name;
            ParamCount = numArgs;
            calculatorFunction = val;
        }

        //will ignore excess parameters passed in
        public virtual Constant Call(Calculator calc, params Token[] args)
        {
            if (calculatorFunction == null)
                return null;

            if (ParamCount != args.Length)
                throw new ArgumentException($"Incorrect number of parameters in Function.Call\nWas given: {args.Length}\nExpected: {ParamCount}", "args");

            if (calc == null)
                calc = new Calculator();


            return calculatorFunction(calc, args);
        }
    }

    //Function class for basic operations (those that take 2 parameters)
    public class OperationFunction : Function
    {
        public OperationFunction()
        {
            FuncName = "Operation";
            ParamCount = 2;
            calculatorFunction = null;
        }
        public OperationFunction(CalculatorFunction operation)
        {
            FuncName = "Operation";
            ParamCount = 2;
            calculatorFunction = operation;
        }
    }


    //Collection of operations for a calculator
    public static class Operations
    {
        public static Constant Multiplication(Calculator calc, params Token[] operands)
        {
            Constant left = operands[0].Value;
            Constant right = operands[1].Value;
            #region handle integer math
            Integer intLeft = operands[0].Value as Integer;
            Integer intRight = operands[1].Value as Integer;

            if (intLeft != null && intRight != null)
                return (Integer)(operands[0].Value) * (Integer)(operands[1].Value);

            if (intLeft != null)
                left = (float)intLeft;
            if (intRight != null)
                right = (float)intRight;
            #endregion


            return left * right;
        }

        public static Constant Division(Calculator calc, params Token[] operands)
        {
            Constant left = operands[0].Value;
            Constant right = operands[1].Value;
            #region handle integer math
            Integer intLeft = operands[0].Value as Integer;
            Integer intRight = operands[1].Value as Integer;

            if (intLeft != null && intRight != null)
                return (Integer)(operands[0].Value) / (Integer)(operands[1].Value);

            if (intLeft != null)
                left = (float)intLeft;
            if (intRight != null)
                right = (float)intRight;
            #endregion


            return left / right;
        }
        public static Integer Modulo(Calculator calc, params Token[] operands)
        {
            return operands[0].Value % operands[1].Value;
        }
        public static Constant Addition(Calculator calc, params Token[] operands)
        {
            Constant left = operands[0].Value;
            Constant right = operands[1].Value;
            #region handle integer math
            Integer intLeft = operands[0].Value as Integer;
            Integer intRight = operands[1].Value as Integer;

            if (intLeft != null && intRight != null)
                return (Integer)(operands[0].Value) + (Integer)(operands[1].Value);

            if (intLeft != null)
                left = (float)intLeft;
            if (intRight != null)
                right = (float)intRight;
            #endregion


            return left + right;
        }
        public static Constant Subtraction(Calculator calc, params Token[] operands)
        {
            Constant left = operands[0].Value;
            Constant right = operands[1].Value;
            #region handle integer math
            Integer intLeft = operands[0].Value as Integer;
            Integer intRight = operands[1].Value as Integer;

            if (intLeft != null && intRight != null)
                return (Integer)(operands[0].Value) - (Integer)(operands[1].Value);

            if (intLeft != null)
                left = (float)intLeft;
            if (intRight != null)
                right = (float)intRight;
            #endregion


            return left - right;
        }
        public static Constant Exponent(Calculator calc, params Token[] operands)
        {
            Constant left = operands[0].Value;
            Constant right = operands[1].Value;
            #region handle integer math
            Integer intLeft = operands[0].Value as Integer;
            Integer intRight = operands[1].Value as Integer;

            if (intLeft != null && intRight != null)
                return MathF.Pow((Integer)(operands[0].Value), (Integer)(operands[1].Value));

            if (intLeft != null)
                left = (float)intLeft;
            if (intRight != null)
                right = (float)intRight;
            #endregion


            return MathF.Pow(left, right);
        }
        public static Constant LeftShift(Calculator calc, params Token[] operands)
        {
            return (int)operands[0].Value << (int)operands[1].Value;
        }
        public static Constant RightShift(Calculator calc, params Token[] operands)
        {
            return (int)operands[0].Value >> (int)operands[1].Value;
        }
    }
}
