//Created by: Deontae Albertie

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace delib.calculate
{

    public class CalculatorInspector
    {

    }


    [CustomPropertyDrawer(typeof(ExpressionFieldBase),true)]
    public class ExpressionFieldBaseDrawer : PropertyDrawer
    {

        // Draw the property inside the given rect
        public override void OnGUI(Rect startPosition, SerializedProperty property, GUIContent label)
        {
            System.Type propertyType = property.serializedObject.targetObject.GetType().GetField(property.name, Library.AllClassVariablesBindingFlag).FieldType;
            System.Type[] propertyTypeGenericTypes = propertyType.GetGenericArguments();

            Rect curPosition = startPosition;

            float standardSpacing = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);


            //get fields
            SerializedProperty expressionString = property.FindPropertyRelative("expression");
            SerializedProperty toggleBool = property.FindPropertyRelative("inspectorToggle");
            SerializedProperty validCheckBool = property.FindPropertyRelative("validCheck");
            SerializedProperty containingClassObject = property.FindPropertyRelative("containingClass");
            //SerializedProperty parameterTypesList = property.FindPropertyRelative("parameterTypes");

            containingClassObject.objectReferenceValue = property.serializedObject.targetObject;

            Color origColor = GUI.color;
            Color origBackColor = GUI.backgroundColor;
            Color validColor = validCheckBool.boolValue? Color.green : Color.red;


            if (toggleBool.boolValue)
                curPosition.height = standardSpacing;

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(curPosition, label, property);

            GUI.color = new Color(0, 0, 0, 0);
            Rect postLabelPosition = EditorGUI.PrefixLabel(curPosition, label);

            // Draw actual label
            GUI.color = origColor;


            Rect labelPosition = new Rect(curPosition.x, curPosition.y, curPosition.width-postLabelPosition.width, curPosition.height);
            toggleBool.boolValue = EditorGUI.Foldout(labelPosition, toggleBool.boolValue, label,true);

            GUI.backgroundColor = validColor;
            if(GUI.Button(postLabelPosition, "Validate"))
            {

                Calculator varCheckCalc = property.serializedObject.targetObject.GetClassAsCalculator(propertyTypeGenericTypes);
                validCheckBool.boolValue = Expression.Validate(expressionString.stringValue, varCheckCalc, propertyTypeGenericTypes);
            }
            GUI.backgroundColor = origColor;

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            //GUIStyle style = new GUIStyle();
            GUILayout.BeginVertical();
            if (toggleBool.boolValue)
            {
                float expandedSpacing = (EditorGUIUtility.singleLineHeight * 1.25f + EditorGUIUtility.standardVerticalSpacing);
                if (propertyType.IsGenericType)
                {
                    int arraySize = propertyTypeGenericTypes.Length;
                    if (arraySize == 0) arraySize = 1;
                    expandedSpacing = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * (propertyType.IsGenericType ? (arraySize) : 1);
                }

                curPosition = new Rect(startPosition.x, curPosition.y + standardSpacing, startPosition.width, expandedSpacing);
                for(int i = 0; i < propertyTypeGenericTypes.Length; i++)
                {
                    EditorGUI.LabelField(curPosition, $"arg{i} |\tType: {propertyTypeGenericTypes[i].Name}");
                }


                curPosition = new Rect(startPosition.x, curPosition.y + expandedSpacing, startPosition.width, EditorGUIUtility.singleLineHeight);

                GUI.backgroundColor = validColor;
                expressionString.stringValue = EditorGUI.TextArea(curPosition, expressionString.stringValue);
            }
            GUILayout.EndVertical();
            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();

            GUI.backgroundColor = origBackColor;
            GUI.color = origColor;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            System.Type propertyType = property.serializedObject.targetObject.GetType().GetField(property.name, Library.AllClassVariablesBindingFlag).FieldType;
            System.Type[] propertyTypeGenericTypes = propertyType.GetGenericArguments();

            SerializedProperty expressionString = property.FindPropertyRelative("expression");
            SerializedProperty toggleBool = property.FindPropertyRelative("inspectorToggle");
            SerializedProperty validCheckBool = property.FindPropertyRelative("validCheck");
            SerializedProperty containingClassObject = property.FindPropertyRelative("containingClass");
            //SerializedProperty parameterTypesList = property.FindPropertyRelative("parameterTypes");

            float standardHeight = base.GetPropertyHeight(property, label);

            if (toggleBool.boolValue)
            {
                if (propertyType.IsGenericType)
                {
                    int arraySize = propertyTypeGenericTypes.Length;
                    if (arraySize == 0) arraySize = 1;
                    return standardHeight * 2 + ((arraySize) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing));
                }

                return standardHeight * 2 + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            }
            return standardHeight;
        }
    }
}
