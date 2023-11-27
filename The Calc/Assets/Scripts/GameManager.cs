using delib.calculate;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public TMP_InputField expressionInput;

    public TMP_Text outputText;

    public GameObject VariableMemoryViewContent;

    public TMP_Text textPreFab;

    private Calculator calc;

    private void Awake()
    {
        Instance = this;

        calc = new Calculator();

    }

    public void EqualsButtonHandler()
    {
        Expression expr = new Expression(expressionInput.text, calc);

        if(!expr.Validate())
        {
            outputText.text = "Error";
            return;
        }
        calc.Calculate(expr);


        float ans = calc.variableMemory["ans"].Value;


        outputText.text = ans.ToString();
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