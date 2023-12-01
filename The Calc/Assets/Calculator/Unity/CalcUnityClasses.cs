//Created by: Deontae Albertie

using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

namespace delib.calculate
{
    #region ExpressionField's
    //[System.Serializable] all children should be made serializeable
    public abstract class ExpressionFieldBase
    {
        #region inspector control variables
        [SerializeField] private bool inspectorToggle = true;
        [SerializeField] private bool validCheck = true;
        //[SerializeField] private string[] parameterTypes = null;
        #endregion
        [SerializeField] protected UnityEngine.Object containingClass = null;//This variable is set through the inspector but, still has purpose in the class instance

        public string expression;

        public Expression RawFieldExpression
        {
            get
            {
                return new Expression(expression);

            }
        }
    }

    [System.Serializable]
    public class ExpressionField : ExpressionFieldBase
    {

        public ExpressionField()
        {

        }

        public Constant Evaluate()
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression);
        }

    }

    [System.Serializable]
    public class ExpressionField<T0> : ExpressionFieldBase
    {

        public ExpressionField()
        {

        }

        public Constant Evaluate(T0 param1)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1);
        }
    }

    [System.Serializable]
    public class ExpressionField<T0, T1> : ExpressionFieldBase
    {

        public ExpressionField()
        {

        }

        public Constant Evaluate(T0 param1, T1 param2)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1, param2);
        }
    }

    [System.Serializable]
    public class ExpressionField<T0, T1, T2> : ExpressionFieldBase
    {

        public ExpressionField()
        {

        }

        public Constant Evaluate(T0 param1, T1 param2, T2 param3)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1, param2, param3);
        }
    }

    [System.Serializable]
    public class ExpressionField<T0, T1, T2, T3> : ExpressionFieldBase
    {

        public ExpressionField()
        {

        }

        public Constant Evaluate(T0 param1, T1 param2, T2 param3, T3 param4)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1, param2, param3, param4);
        }
    }

    [System.Serializable]
    public class ExpressionField<T0, T1, T2, T3, T4> : ExpressionFieldBase
    {

        public ExpressionField()
        {

        }

        public Constant Evaluate(T0 param1, T1 param2, T2 param3, T3 param4, T4 param5)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1, param2, param3, param4, param5);
        }
    }
    [System.Serializable]
    public class ExpressionField<T0, T1, T2, T3, T4, T5> : ExpressionFieldBase
    {

        public ExpressionField()
        {

        }

        public Constant Evaluate(T0 param1, T1 param2, T2 param3, T3 param4, T4 param5, T5 param6)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1, param2, param3, param4, param5, param6);
        }
    }
    [System.Serializable]
    public class ExpressionField<T0, T1, T2, T3, T4, T5, T6> : ExpressionFieldBase
    {

        public ExpressionField()
        {

        }

        public Constant Evaluate(T0 param1, T1 param2, T2 param3, T3 param4, T4 param5, T5 param6, T6 param7)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1, param2, param3, param4, param5, param6, param7);
        }
    }
    [System.Serializable]
    public class ExpressionField<T0, T1, T2, T3, T4, T5, T6, T7> : ExpressionFieldBase
    {

        public ExpressionField()
        {

        }

        public Constant Evaluate(T0 param1, T1 param2, T2 param3, T3 param4, T4 param5, T5 param6, T6 param7, T7 param8)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1, param2, param3, param4, param5, param6, param7, param8);
        }
    }

    #endregion




}