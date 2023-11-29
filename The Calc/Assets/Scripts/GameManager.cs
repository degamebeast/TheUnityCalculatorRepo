//Created by: Deontae Albertie

using delib.calculate;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public class TestingClass
    {
        //public ExpressionField field2 = new ExpressionField();
    }
    public static GameManager Instance { get; private set; }


    public TMP_InputField expressionInput;

    public TMP_Text outputText;

    public TMP_Text textPreFab;

    public ExpressionField field;
    public ExpressionField field2;
    public GameObject VariableMemoryViewContent;

    private Constant test;
    private Constant example;
    private float example2;
    private float cobo;

    private Integer inttest;
    private Integer intexample;
    private int intexample2;

    private Calculator calc;

    private void Awake()
    {
        cobo = 10;
        TestingClass classs = new TestingClass();
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
        Debug.Log(field.Evaluate());
        Debug.Log(field2.Evaluate());
        //CalculateExpress(field.ContainingClassContextFieldExpression);
        //CalculateExpress(classs.field2.ContainingClassContextFieldExpression);
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