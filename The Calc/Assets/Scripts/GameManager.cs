//Created by: Deontae Albertie

using delib.calculate;
using delib.calculate.unity;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public class TestingClass
    {
        public static TestingClass Instance;
        //public ExpressionField field2 = new ExpressionField();
        public int testIt;
        public int testItProp
        {
            get;
            set;
        }

        public float testItMethod(int x)
        {
            return Mathf.Pow(2, x);
        }

        public TestingClass()
        {
            Instance = this;
        }
    }
    public static GameManager Instance { get; private set; }


    public TMP_InputField expressionInput;

    public TMP_Text outputText;

    public TMP_Text textPreFab;

    [ArgumentName(0, "stuff")]
    public ExpressionField<TestingClass> field;
    public ExpressionField field2;


    [ArgumentName(2, "Dolmm")]
    public ExpressionField<TestingClass, TestingClass, int, GameManager> field3;
    public GameObject VariableMemoryViewContent;

    private Constant test;
    private Constant example;
    private Constant exampleProp { get; set; }
    private float example2;
    private float cobo;


    private Integer inttest;
    private Integer intexample;
    private int intexample2;

    private int rest;

    private Calculator calc;

    private void Awake()
    {
        rest = 3;
        cobo = 10;
        TestingClass classs = new TestingClass();
        classs.testIt = 5;
        classs.testItProp = 6;


        Instance = this;

        test = 9;
        example = 12;
        example2 = 14;

        inttest = 8;
        intexample = 11;
        intexample2 = 13;

        calc = this.GetClassAsCalculator();
        //Expression ex = classs.field2.ContainingClassContextFieldExpression;
        //CalculateExpress(field.RawFieldExpression);
        //CalculateExpress(field.RawFieldExpression);
        Debug.Log(field.Evaluate(classs));
        Debug.Log(field2.Evaluate());
        //CalculateExpress(field.ContainingClassContextFieldExpression);
        //CalculateExpress(classs.field2.ContainingClassContextFieldExpression);
    }

    public Constant exampleMethod(Constant test)
    {
        return test * 2;
    }


    public void CalculateExpress(Expression expr)
    {
        if (!expr.Validate(calc))
        {
            outputText.text = "Error";
            return;
        }
        calc.Calculate(expr);


        float ans = calc.variableMemory["ans"].Value;


        outputText.text = ans.ToString();
    }

    public void EqualsButtonHandler()
    {
        Expression expr = new Expression(expressionInput.text, calc);

        CalculateExpress(expr);
    }



    public void VariablesButtonHandler()
    {

        VariableMemoryViewContent.DestroyChildren();
        foreach(Variable var in calc.GetVariablesInMemory())
        {
            TMP_Text varText = Instantiate(textPreFab, VariableMemoryViewContent.transform);

            varText.text = $" {var.VarName} \t {var.Value} ";
        }
    }

}

public static class ExtensionMethods
{
    public static void DestroyChildren(this GameObject go)
    {
        go.transform.DestroyChildren();
    }
    public static void DestroyChildren(this Transform trans)
    {
        for(int i = trans.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(trans.GetChild(i).gameObject);
        }
    }
}