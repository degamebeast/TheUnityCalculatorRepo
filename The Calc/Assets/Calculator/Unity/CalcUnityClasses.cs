//Created by: Deontae Albertie

using UnityEngine;

namespace delib.calculate.unity
{
    [System.Serializable]
    public class ClassFieldInfoHolder
    {
        #region inspector control variables
        public bool inspectorToggle = false;
        public float fieldsHeight = 0;
        #endregion

        public string fieldName = null;
        public System.Type type = null;
        [SerializeReference]public ClassFieldInfoHolder[] fields = null;

        public float GetTotalHeight()
        {
            if (!inspectorToggle)
                return 0;
            float total = 0;
        
            if(fields != null)
            {
                foreach (ClassFieldInfoHolder info in fields)
                {
                    total += info.GetTotalHeight();
                }

            }

            total += fieldsHeight;

            return total;
        }
    }

    #region ExpressionField's
    //This class represents all of the core data needed for an ExpressionField including it's UnityEditor tracking variables
    //Note: all children of ExpressionFieldBase should be made serializeable [System.Serializable] 
    public abstract class ExpressionFieldBase : ISerializationCallbackReceiver
    {
        #region inspector control variables
        //control for whether the inspector field should be collapsed or not
        [SerializeField] private bool inspectorToggle = true;
        //control to determine whether the Valid or Invalid graphic should be displayed
        [SerializeField] private bool validCheck = true;
        //the height of the expression textArea factoring in the current inspector width
        [SerializeField] private float lineHeight = 0;
        [SerializeReference] protected ClassFieldInfoHolder[] argInfos = null;
        #endregion
        //The class that this ExpressionField belongs to
        [SerializeField] protected UnityEngine.Object containingClass = null;//This variable is set through the inspector but, still has purpose in the class instance

        //The expression this class is wrapping
        public string expression;

        //returns 'expression' converted into a standard delib.calculate.Expression
        public Expression RawFieldExpression
        {
            get
            {
                return new Expression(expression);

            }
        }

        public virtual void OnBeforeSerialize()
        {

        }

        public virtual void OnAfterDeserialize()
        {

        }
    }

    //a non-generic ExpressionField
    [System.Serializable]
    public class ExpressionField : ExpressionFieldBase
    {

        public ExpressionField()
        {
            argInfos = new ClassFieldInfoHolder[0];
        }

        //Returnd the result of callinf Calculate() on the stored 'expression' variable within the scope of the 'containingClass'
        public Constant Evaluate()
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression);
        }

    }

    //a generic ExpressionField that takes 1 argument
    [System.Serializable]
    public class ExpressionField<T0> : ExpressionFieldBase
    {
        public ExpressionField()
        {
            argInfos = new ClassFieldInfoHolder[1];
        }

        //Returnd the result of callinf Calculate() on the stored 'expression' variable within the scope of the 'containingClass'
        //also sends 1 additional argument into the Calculate() call
        public Constant Evaluate(T0 param1)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1);
        }

        public override void OnBeforeSerialize()
        {
            if(argInfos == null)
                argInfos = new ClassFieldInfoHolder[1];
            if (argInfos[0] == null)
                argInfos[0] = new ClassFieldInfoHolder();

            argInfos[0].type = this.GetType().GenericTypeArguments[0];
        }
    }

    //a generic ExpressionField that takes 2 argument
    [System.Serializable]
    public class ExpressionField<T0, T1> : ExpressionFieldBase
    {

        public ExpressionField()
        {
            argInfos = new ClassFieldInfoHolder[2];
        }
        //Returnd the result of callinf Calculate() on the stored 'expression' variable within the scope of the 'containingClass'
        //also sends 2 additional argument into the Calculate() call
        public Constant Evaluate(T0 param1, T1 param2)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1, param2);
        }

        public override void OnBeforeSerialize()
        {
            if (argInfos == null)
                argInfos = new ClassFieldInfoHolder[2];
            if (argInfos[0] == null)
                argInfos[0] = new ClassFieldInfoHolder();
            if (argInfos[1] == null)
                argInfos[1] = new ClassFieldInfoHolder();

            argInfos[0].type = this.GetType().GenericTypeArguments[0];
            argInfos[0].type = this.GetType().GenericTypeArguments[1];
        }
    }

    //a generic ExpressionField that takes 3 argument
    [System.Serializable]
    public class ExpressionField<T0, T1, T2> : ExpressionFieldBase
    {

        public ExpressionField()
        {
            argInfos = new ClassFieldInfoHolder[3];
        }
        //Returnd the result of callinf Calculate() on the stored 'expression' variable within the scope of the 'containingClass'
        //also sends 3 additional argument into the Calculate() call
        public Constant Evaluate(T0 param1, T1 param2, T2 param3)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1, param2, param3);
        }

        public override void OnBeforeSerialize()
        {
            if (argInfos == null)
                argInfos = new ClassFieldInfoHolder[3];
            if (argInfos[0] == null)
                argInfos[0] = new ClassFieldInfoHolder();
            if (argInfos[1] == null)
                argInfos[1] = new ClassFieldInfoHolder();
            if (argInfos[2] == null)
                argInfos[2] = new ClassFieldInfoHolder();

            argInfos[0].type = this.GetType().GenericTypeArguments[0];
            argInfos[1].type = this.GetType().GenericTypeArguments[1];
            argInfos[2].type = this.GetType().GenericTypeArguments[2];
        }
    }

    //a generic ExpressionField that takes 4 argument
    [System.Serializable]
    public class ExpressionField<T0, T1, T2, T3> : ExpressionFieldBase
    {

        public ExpressionField()
        {
            argInfos = new ClassFieldInfoHolder[4];

        }
        //Returnd the result of callinf Calculate() on the stored 'expression' variable within the scope of the 'containingClass'
        //also sends 4 additional argument into the Calculate() call
        public Constant Evaluate(T0 param1, T1 param2, T2 param3, T3 param4)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1, param2, param3, param4);
        }

        public override void OnBeforeSerialize()
        {
            if (argInfos == null)
                argInfos = new ClassFieldInfoHolder[4];
            if (argInfos[0] == null)
                argInfos[0] = new ClassFieldInfoHolder();
            if (argInfos[1] == null)
                argInfos[1] = new ClassFieldInfoHolder();
            if (argInfos[2] == null)
                argInfos[2] = new ClassFieldInfoHolder();
            if (argInfos[3] == null)
                argInfos[3] = new ClassFieldInfoHolder();

            argInfos[0].type = this.GetType().GenericTypeArguments[0];
            argInfos[1].type = this.GetType().GenericTypeArguments[1];
            argInfos[2].type = this.GetType().GenericTypeArguments[2];
            argInfos[3].type = this.GetType().GenericTypeArguments[3];
        }
    }

    //a generic ExpressionField that takes 5 argument
    [System.Serializable]
    public class ExpressionField<T0, T1, T2, T3, T4> : ExpressionFieldBase
    {

        public ExpressionField()
        {
            argInfos = new ClassFieldInfoHolder[5];

        }
        //Returnd the result of callinf Calculate() on the stored 'expression' variable within the scope of the 'containingClass'
        //also sends 5 additional argument into the Calculate() call
        public Constant Evaluate(T0 param1, T1 param2, T2 param3, T3 param4, T4 param5)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1, param2, param3, param4, param5);
        }

        public override void OnBeforeSerialize()
        {
            if (argInfos == null)
                argInfos = new ClassFieldInfoHolder[5];
            if (argInfos[0] == null)
                argInfos[0] = new ClassFieldInfoHolder();
            if (argInfos[1] == null)
                argInfos[1] = new ClassFieldInfoHolder();
            if (argInfos[2] == null)
                argInfos[2] = new ClassFieldInfoHolder();
            if (argInfos[3] == null)
                argInfos[3] = new ClassFieldInfoHolder();
            if (argInfos[4] == null)
                argInfos[4] = new ClassFieldInfoHolder();

            argInfos[0].type = this.GetType().GenericTypeArguments[0];
            argInfos[1].type = this.GetType().GenericTypeArguments[1];
            argInfos[2].type = this.GetType().GenericTypeArguments[2];
            argInfos[3].type = this.GetType().GenericTypeArguments[3];
            argInfos[4].type = this.GetType().GenericTypeArguments[4];
        }
    }

    //a generic ExpressionField that takes 6 argument
    [System.Serializable]
    public class ExpressionField<T0, T1, T2, T3, T4, T5> : ExpressionFieldBase
    {

        public ExpressionField()
        {
            argInfos = new ClassFieldInfoHolder[1];

        }
        //Returnd the result of callinf Calculate() on the stored 'expression' variable within the scope of the 'containingClass'
        //also sends 6 additional argument into the Calculate() call
        public Constant Evaluate(T0 param1, T1 param2, T2 param3, T3 param4, T4 param5, T5 param6)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1, param2, param3, param4, param5, param6);
        }

        public override void OnBeforeSerialize()
        {
            if (argInfos == null)
                argInfos = new ClassFieldInfoHolder[6];
            if (argInfos[0] == null)
                argInfos[0] = new ClassFieldInfoHolder();
            if (argInfos[1] == null)
                argInfos[1] = new ClassFieldInfoHolder();
            if (argInfos[2] == null)
                argInfos[2] = new ClassFieldInfoHolder();
            if (argInfos[3] == null)
                argInfos[3] = new ClassFieldInfoHolder();
            if (argInfos[4] == null)
                argInfos[4] = new ClassFieldInfoHolder();
            if (argInfos[5] == null)
                argInfos[5] = new ClassFieldInfoHolder();

            argInfos[0].type = this.GetType().GenericTypeArguments[0];
            argInfos[1].type = this.GetType().GenericTypeArguments[1];
            argInfos[2].type = this.GetType().GenericTypeArguments[2];
            argInfos[3].type = this.GetType().GenericTypeArguments[3];
            argInfos[4].type = this.GetType().GenericTypeArguments[4];
            argInfos[5].type = this.GetType().GenericTypeArguments[5];
        }
    }

    //a generic ExpressionField that takes 7 argument
    [System.Serializable]
    public class ExpressionField<T0, T1, T2, T3, T4, T5, T6> : ExpressionFieldBase
    {

        public ExpressionField()
        {
            argInfos = new ClassFieldInfoHolder[7];

        }
        //Returnd the result of callinf Calculate() on the stored 'expression' variable within the scope of the 'containingClass'
        //also sends 7 additional argument into the Calculate() call
        public Constant Evaluate(T0 param1, T1 param2, T2 param3, T3 param4, T4 param5, T5 param6, T6 param7)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1, param2, param3, param4, param5, param6, param7);
        }

        public override void OnBeforeSerialize()
        {
            if (argInfos == null)
                argInfos = new ClassFieldInfoHolder[7];
            if (argInfos[0] == null)
                argInfos[0] = new ClassFieldInfoHolder();
            if (argInfos[1] == null)
                argInfos[1] = new ClassFieldInfoHolder();
            if (argInfos[2] == null)
                argInfos[2] = new ClassFieldInfoHolder();
            if (argInfos[3] == null)
                argInfos[3] = new ClassFieldInfoHolder();
            if (argInfos[4] == null)
                argInfos[4] = new ClassFieldInfoHolder();
            if (argInfos[5] == null)
                argInfos[5] = new ClassFieldInfoHolder();
            if (argInfos[6] == null)
                argInfos[6] = new ClassFieldInfoHolder();


            argInfos[0].type = this.GetType().GenericTypeArguments[0];
            argInfos[1].type = this.GetType().GenericTypeArguments[1];
            argInfos[2].type = this.GetType().GenericTypeArguments[2];
            argInfos[3].type = this.GetType().GenericTypeArguments[3];
            argInfos[4].type = this.GetType().GenericTypeArguments[4];
            argInfos[5].type = this.GetType().GenericTypeArguments[5];
            argInfos[6].type = this.GetType().GenericTypeArguments[6];
        }
    }

    //a generic ExpressionField that takes 8 argument
    [System.Serializable]
    public class ExpressionField<T0, T1, T2, T3, T4, T5, T6, T7> : ExpressionFieldBase
    {

        public ExpressionField()
        {
            argInfos = new ClassFieldInfoHolder[8];
        }
        //Returnd the result of callinf Calculate() on the stored 'expression' variable within the scope of the 'containingClass'
        //also sends 8 additional argument into the Calculate() call
        public Constant Evaluate(T0 param1, T1 param2, T2 param3, T3 param4, T4 param5, T5 param6, T6 param7, T7 param8)
        {
            Calculator classContextCalc = containingClass.GetClassAsCalculator();
            return classContextCalc.Calculate(expression, param1, param2, param3, param4, param5, param6, param7, param8);
        }

        public override void OnBeforeSerialize()
        {
            if (argInfos == null)
                argInfos = new ClassFieldInfoHolder[8];
            if (argInfos[0] == null)
                argInfos[0] = new ClassFieldInfoHolder();
            if (argInfos[1] == null)
                argInfos[1] = new ClassFieldInfoHolder();
            if (argInfos[2] == null)
                argInfos[2] = new ClassFieldInfoHolder();
            if (argInfos[3] == null)
                argInfos[3] = new ClassFieldInfoHolder();
            if (argInfos[4] == null)
                argInfos[4] = new ClassFieldInfoHolder();
            if (argInfos[5] == null)
                argInfos[5] = new ClassFieldInfoHolder();
            if (argInfos[6] == null)
                argInfos[6] = new ClassFieldInfoHolder();
            if (argInfos[7] == null)
                argInfos[7] = new ClassFieldInfoHolder();

            argInfos[0].type = this.GetType().GenericTypeArguments[0];
            argInfos[1].type = this.GetType().GenericTypeArguments[1];
            argInfos[2].type = this.GetType().GenericTypeArguments[2];
            argInfos[3].type = this.GetType().GenericTypeArguments[3];
            argInfos[4].type = this.GetType().GenericTypeArguments[4];
            argInfos[5].type = this.GetType().GenericTypeArguments[5];
            argInfos[6].type = this.GetType().GenericTypeArguments[6];
            argInfos[7].type = this.GetType().GenericTypeArguments[7];
        }
    }

    #endregion




}