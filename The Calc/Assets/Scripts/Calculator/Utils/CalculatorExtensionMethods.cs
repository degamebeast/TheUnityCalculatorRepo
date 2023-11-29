//Created by: Deontae Albertie

using System.Reflection;

namespace delib.calculate
{
    public static class CalculatorExtensionMethods
    {
        public static Calculator GetClassAsCalculator(this System.Object obj)
        {
            Calculator classContextCalc = new Calculator();



            foreach (FieldInfo fi in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {

                if (fi.FieldType == typeof(Constant))
                {
                    Constant val = fi.GetValue(obj) as Constant;
                    classContextCalc.AddVariableToMemory(fi.Name, val);
                }

            }

            return classContextCalc;
        }
    }
}

