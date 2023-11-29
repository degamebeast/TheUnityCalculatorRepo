//Created by: Deontae Albertie

using UnityEngine;

namespace delib.calculate
{
    [System.Serializable]
    public class ExpressionField
    {
        #region inspector control variables
        [SerializeField]private bool inspectorToggle = true;
        [SerializeField]private bool validCheck = true;
        [SerializeField]private UnityEngine.Object containingClass = null;
        #endregion

        public string expression;

        public Constant Evaluate()
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression);
        }

        public Expression RawFieldExpression
        {
            get
            {
                return new Expression(expression);

            }
        }

    }
}