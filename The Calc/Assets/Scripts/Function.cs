using System;


namespace delib.calculate
{
    //delegate for all CalculatorFunction's
    public delegate Constant CalculatorFunction(Calculator calc, params Token[] constArgs);

    public class Function
    {
        public string FuncName { get; protected set; }
        public int ArgCount { get; protected set; }

        protected CalculatorFunction calculatorFunction;

        public Function()
        {
            FuncName = null;
            ArgCount = 0;
            calculatorFunction = null;
        }

        public Function(string name, int numArgs, CalculatorFunction val)
        {
            FuncName = name;
            ArgCount = numArgs;
            calculatorFunction = val;
        }

        //will ignore excess arguments passed in
        public virtual Constant Call(Calculator calc, params Token[] args)
        {
            if (calculatorFunction == null)
                return null;

            if (ArgCount != args.Length)
                throw new ArgumentException($"Incorrect number of arguments in Function.Call\nWas given: {args.Length}\nExpected: {ArgCount}", "args");

            if (calc == null)
                calc = new Calculator();


            return calculatorFunction(calc, args);
        }
    }

    //Function class for basic operations (those that take 2 arguments)
    public class OperationFunction : Function
    {
        public OperationFunction()
        {
            FuncName = "Operation";
            ArgCount = 2;
            calculatorFunction = null;
        }
        public OperationFunction(CalculatorFunction operation)
        {
            FuncName = "Operation";
            ArgCount = 2;
            calculatorFunction = operation;
        }
    }


    //Collection of operations for a calculator
    public static class Operations
    {
        public static Constant Multiplication(Calculator calc, params Token[] operands)
        {
            return operands[0].Value * operands[1].Value;
        }
        public static Constant Division(Calculator calc, params Token[] operands)
        {
            return operands[0].Value / operands[1].Value;
        }
        public static Constant Modulo(Calculator calc, params Token[] operands)
        {
            return operands[0].Value % operands[1].Value;
        }
        public static Constant Addition(Calculator calc, params Token[] operands)
        {
            return operands[0].Value + operands[1].Value;
        }
        public static Constant Subtraction(Calculator calc, params Token[] operands)
        {
            return operands[0].Value - operands[1].Value;
        }
        public static Constant Exponent(Calculator calc, params Token[] operands)
        {
            return MathF.Pow(operands[0].Value, operands[1].Value);
        }
    }
}
