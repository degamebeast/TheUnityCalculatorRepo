//Created by: Deontae Albertie

using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

namespace delib.calculate
{
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
/*            if (parameters != null)
            {
                if (parameterTypes == null)
                    throw new System.ArgumentException("Function did not expect arguments and but, recieved some");

                if (parameters.Length != parameterTypes.Length)
                    throw new System.ArgumentException("Incorrect number of arguments");
            }
            else if (parameterTypes != null)
                throw new System.ArgumentException("Function expected arguments and didn't recieve any");

            for (int index = 0; index < parameters.Length; index++)
            {
                object curParam = parameters[index];
                System.Type curType = System.Type.GetType(parameterTypes[index]);
                if (curParam.GetType() != curType)
                    throw new System.ArgumentException($"Argument index '{index}' type did not match expected type");


            }*/

            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1);
        }
    }



}