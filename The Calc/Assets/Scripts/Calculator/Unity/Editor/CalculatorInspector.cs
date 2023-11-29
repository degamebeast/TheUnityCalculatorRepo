//Created by: Deontae Albertie

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace delib.calculate
{

    public class CalculatorInspector
    {

    }


    [CustomPropertyDrawer(typeof(ExpressionField))]
    public class ExpressionFieldDrawer : PropertyDrawer
    {

        // Draw the property inside the given rect
        public override void OnGUI(Rect startPosition, SerializedProperty property, GUIContent label)
        {
            Color origColor = GUI.color;
            //get fields
            SerializedProperty expressionString = property.FindPropertyRelative("expression");
            SerializedProperty toggleBool = property.FindPropertyRelative("inspectorToggle");
            SerializedProperty validCheckBool = property.FindPropertyRelative("validCheck");
            SerializedProperty containingClassObject = property.FindPropertyRelative("containingClass");

            containingClassObject.objectReferenceValue = property.serializedObject.targetObject;



            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(startPosition, label, property);

            GUI.color = new Color(0, 0, 0, 0);
            Rect postLabelPosition = EditorGUI.PrefixLabel(startPosition, label);

            // Draw actual label
            GUI.color = origColor;
            Rect labelPosition = new Rect(startPosition.x, startPosition.y, startPosition.width-postLabelPosition.width, startPosition.height);
            toggleBool.boolValue = EditorGUI.Foldout(labelPosition, toggleBool.boolValue, label,true);
            if(GUI.Button(postLabelPosition, "Validate"))
            {
                Calculator varCheckCalc = new Calculator();
                foreach (FieldInfo fi in property.serializedObject.targetObject.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                {
                    if (fi.FieldType == typeof(Constant))
                    {
                        //Constant val = fi.GetValue(property.serializedObject.targetObject) as Constant;
                        varCheckCalc.AddVariableToMemory(fi.Name);
                    }
                }
                validCheckBool.boolValue = Expression.Validate(expressionString.stringValue, varCheckCalc);
            }

            if (validCheckBool.boolValue)
            {
                GUI.backgroundColor = Color.green;
            }
            else
            {
                GUI.backgroundColor = Color.red;
            }

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            //GUIStyle style = new GUIStyle();

            if (toggleBool.boolValue)
            {
                expressionString.stringValue = EditorGUILayout.TextArea(expressionString.stringValue);
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();

            GUI.color = origColor;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}
