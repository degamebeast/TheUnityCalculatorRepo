//Created by: Deontae Albertie

using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace delib.calculate.unity
{

    public class CalculatorInspector
    {

    }

    //PropertyDrawer for drawing the ExpressionField GUI
    [CustomPropertyDrawer(typeof(ExpressionFieldBase),true)]
    public class ExpressionFieldBaseDrawer : PropertyDrawer
    {

        // Draw the property inside the given rect
        public override void OnGUI(Rect startPosition, SerializedProperty property, GUIContent label)
        {
            #region property info
            //general constants for GUI
            float standardSpacing = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            //Get access to the current properties Type information
            System.Type propertyType = property.serializedObject.targetObject.FindObjectFromPath(property.propertyPath).GetType();//.GetField(property.name, Library.AllClassVariablesBindingFlag).FieldType;
            System.Type[] propertyTypeGenericTypes = propertyType.GetGenericArguments();

            //property's internal property variables
            SerializedProperty expressionString = property.FindPropertyRelative("expression");
            SerializedProperty toggleBool = property.FindPropertyRelative("inspectorToggle");
            SerializedProperty validCheckBool = property.FindPropertyRelative("validCheck");
            SerializedProperty containingClassObject = property.FindPropertyRelative("containingClass");
            SerializedProperty lineHeightFloat = property.FindPropertyRelative("lineHeight");
            #endregion

            //assigning the current property position tracker to it's default value
            Rect curPosition = startPosition;


            //storing referencs to the current GUI colors
            Color origColor = GUI.color;
            Color origBackgroundColor = GUI.backgroundColor;
            Color origContentColor = GUI.contentColor;

            //checking the current valid status and assigning values accordingly
            Color validColor = validCheckBool.boolValue? Color.green : Color.red;

            //assign the current value of containing class to the serialized target object
            //SELF NOTE: it may be worth trying to find a more effient way to do this
            containingClassObject.objectReferenceValue = property.serializedObject.targetObject;


            //set cur position height to standard line height since most controlls will be at this height
            curPosition.height = standardSpacing;

            
            //SELF NOTE: Using BeginProperty / EndProperty on the parent property means that, prefab override logic works on the entire property.
            EditorGUI.BeginProperty(curPosition, label, property);
            
            #region first line
            //creating an invisible label in order to get appropriate spacing without messing up UI visuals
            //SELF NOTE: I hope unity has a better way to do this!!
            GUI.color = new Color(0, 0, 0, 0);
            Rect postLabelPosition = EditorGUI.PrefixLabel(curPosition, label);

            // Draw actual label (in this case the label is represented with an editor foldout)
            GUI.color = origColor;
            Rect labelPosition = new Rect(curPosition.x, curPosition.y, curPosition.width-postLabelPosition.width, curPosition.height);
            toggleBool.boolValue = EditorGUI.Foldout(labelPosition, toggleBool.boolValue, label,true);


            //draw validate button in the corresponding color
            GUI.backgroundColor = validColor;
            if(GUI.Button(postLabelPosition, "Validate"))
            {
                Calculator varCheckCalc = property.serializedObject.targetObject.GetClassAsCalculator(propertyTypeGenericTypes);
                validCheckBool.boolValue = Expression.Validate(expressionString.stringValue, varCheckCalc, propertyTypeGenericTypes);
            }
            GUI.backgroundColor = origColor;
            #endregion



            //SELF NOTE: Don't make child fields be indented
            //storing current indentation level
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;


            GUILayout.BeginVertical();
            if (toggleBool.boolValue)
            {
                Calculator propCalc = CalcHelper.ConvertClassToCalculator(propertyType);
                //draw all generic TYpe info
                for (int i = 0; i < propertyTypeGenericTypes.Length; i++)
                {
                    curPosition = new Rect(startPosition.x, curPosition.y + standardSpacing, startPosition.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(curPosition, $"arg{i} |\tType: {propertyTypeGenericTypes[i].Name}");
                }


                //create textArea style
                Font font = Resources.Load<FontAsset>("ExpressionFieldTextAreaFont").sourceFontFile;
                GUIStyle textAreaStyle = new GUIStyle(GUI.skin.textArea);
                textAreaStyle.richText = true;
                textAreaStyle.font = font;

                //create textArea for user input
                int linesNeeded = (int)(CalcUnityHelper.GetTextWidth(expressionString.stringValue, font, textAreaStyle.fontSize) / startPosition.width) + 2;
                lineHeightFloat.floatValue = linesNeeded * standardSpacing;
                curPosition = new Rect(startPosition.x, curPosition.y + standardSpacing, startPosition.width, lineHeightFloat.floatValue);
                string valHolder = EditorGUI.TextArea(curPosition, expressionString.stringValue,textAreaStyle);
                
                //create label directly over textArea to display text with coloring
                GUI.backgroundColor = new Color(0, 0, 0, 0);
                Expression labelExpress = new Expression(valHolder,false);
                propCalc.ResolveIdentifiers(labelExpress);
                labelExpress.AddNullCaps();
                Expression.CondenseDots(labelExpress);
                labelExpress.RemoveAllNulls();

                string labelText = "";
                int textIndex = 0;
                foreach (Token toke in labelExpress)
                {
                    Color col = CalcUnityHelper.TokenInspectorColor[toke.Type];
                    string tokeText = toke.ToString();
                    //removes any differences between what has been typed and what is being presented to the user
                    //NOTE: this for loop is wildly unsafe, will eventually need to improve
/*                    for(int i = 0; i < tokeText.Length;)
                    {
                        if (valHolder[textIndex] != tokeText[i])
                        {
                            tokeText = tokeText.Remove(i, 1);
                            continue;
                        }
                        i++;
                        textIndex++;
                    }*/

                    if (toke.Type == TokenTypeValue.Invalid)
                        labelText += $"<color=\"#{ColorUtility.ToHtmlStringRGBA(col)}\">_</color>";
                    else if (toke.Type == TokenTypeValue.Ignore)
                        labelText += " ";
                    else
                        labelText += $"<color=\"#{ColorUtility.ToHtmlStringRGBA(col)}\">{tokeText}</color>";


                }

                EditorGUI.LabelField(curPosition, labelText, textAreaStyle);


                //assign current value stored in textArea back out to the expression field in property
                expressionString.stringValue = valHolder;
            }
            GUILayout.EndVertical();


            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();

            //restore previous GUI colors before exiting function
            GUI.contentColor = origContentColor;
            GUI.backgroundColor = origBackgroundColor;
            GUI.color = origColor;
        }

        //Inspector function for determining a controls height
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            #region property info

            //general constants for GUI
            float standardSpacing = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            //Get access to the current properties Type information
            System.Type propertyType = property.serializedObject.targetObject.GetType().GetField(property.name, Library.AllClassVariablesBindingFlag).FieldType;
            System.Type[] propertyTypeGenericTypes = propertyType.GetGenericArguments();


            //property's internal property variables
            SerializedProperty expressionString = property.FindPropertyRelative("expression");
            SerializedProperty toggleBool = property.FindPropertyRelative("inspectorToggle");
            SerializedProperty validCheckBool = property.FindPropertyRelative("validCheck");
            SerializedProperty containingClassObject = property.FindPropertyRelative("containingClass");
            SerializedProperty lineHeightFloat = property.FindPropertyRelative("lineHeight");
            #endregion

            float standardHeight = base.GetPropertyHeight(property, label);

            //if the drop down is on then addspace for the 'expression' property to be drawn
            if (toggleBool.boolValue)
            {
                //if 'property' is a generic type then addspace for the generic values info to be drawn
                if (propertyType.IsGenericType)
                {
                    int arraySize = propertyTypeGenericTypes.Length;
                    if (arraySize == 0) arraySize = 1;
                    return standardHeight + (arraySize * standardSpacing) + lineHeightFloat.floatValue;
                }

                return standardHeight + standardSpacing + lineHeightFloat.floatValue;
            }

            return standardHeight;
        }
    }
}
