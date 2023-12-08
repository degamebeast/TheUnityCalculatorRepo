//Created by: Deontae Albertie

using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using System.Reflection;
using System.Collections.Generic;

namespace delib.calculate.unity
{

    public class CalculatorInspector
    {

    }
    //PropertyDrawer for drawing the ExpressionField GUI
    [CustomPropertyDrawer(typeof(ExpressionFieldBase), true)]
    public class ExpressionFieldBaseDrawer : PropertyDrawer
    {
        public Rect DrawClassDef(Rect position, ClassFieldInfoHolder argInfo, System.Type argType, string argName, int indentLevel)
        {
            float standardSpacing = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
/*            SerializedProperty argToggleBool = argInfo.FindPropertyRelative("inspectorToggle");
            SerializedProperty argFieldsArray = argInfo.FindPropertyRelative("fields");
            SerializedProperty argFieldsHeightFloat = argInfo.FindPropertyRelative("fieldsHeight");*/

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indentLevel;



            //ClassFieldInfoHolder argInfoHold = (ClassFieldInfoHolder)argInfo.boxedValue;
            argInfo.type = argType;
            argInfo.inspectorToggle = EditorGUI.Foldout(position, argInfo.inspectorToggle, $"{argName} |\tType: {argInfo.type.Name}", true);
            /*            ClassPathInfo[] argObjectPath = null;
                        ClassPathInfo argArray = argFieldsArray.serializedObject.targetObject.FindObjectFromPath(argFieldsArray.propertyPath, out argObjectPath);*/
            //if (argObjectPath.Length > 0)

            /*            object argArrayContainer = argObjectPath[argObjectPath.Length - 1].classObj;
                        FieldInfo argArrayFI = argArrayContainer.GetType().GetField(argArray.fieldNameInContainer);*/

            //Debug.Log(argInfo.inspectorToggle);
            if (argInfo.inspectorToggle)
            {
                if (indentLevel > 8)
                {
                    Debug.LogWarning("Unity internal Serialization limit reached. You will not be able to delve deeper");
                    argInfo.inspectorToggle = false;
                    return position;
                }
                //EditorGUI.indentLevel = indentLevel+1;



                //if (argObjectPath.Length > 0)
                {
                    List<ClassFieldInfoHolder> argFieldsList = new List<ClassFieldInfoHolder>();
                    FieldInfo[] argInfos = argInfo.type.GetFields();
                    PropertyInfo[] argPropInfos = argInfo.type.GetProperties();
                    argInfo.fieldsHeight = standardSpacing * (argInfos.Length + argPropInfos.Length);
                    foreach (FieldInfo fi in argInfos)
                    {
                        ClassFieldInfoHolder field = new ClassFieldInfoHolder();
                        field.type = fi.FieldType;
                        field.fieldName = fi.Name;
                        argFieldsList.Add(field);
                    }

                    foreach (PropertyInfo pi in argPropInfos)
                    {
                        ClassFieldInfoHolder field = new ClassFieldInfoHolder();
                        field.type = pi.PropertyType;
                        field.fieldName = pi.Name;
                        argFieldsList.Add(field);
                    }

                    ClassFieldInfoHolder[] curArray = argInfo.fields; //(ClassFieldInfoHolder[])argArrayFI.GetValue(argArrayContainer);
                    if (curArray != null && curArray.Length == argFieldsList.Count)
                    {
                        for (int i = 0; i < curArray.Length; i++)
                        {
                            argFieldsList[i].inspectorToggle = curArray[i].inspectorToggle;
                            argFieldsList[i].fieldsHeight = curArray[i].fieldsHeight;
                            argFieldsList[i].fields = curArray[i].fields;
                        }
                    }
                    //argArrayFI.SetValue(argArrayContainer, argFieldsList.ToArray());
                    argInfo.fields = argFieldsList.ToArray();

                }

                //ClassFieldInfoHolder[] infos = (ClassFieldInfoHolder[])argArray.classObj;
                ClassFieldInfoHolder[] infos = argInfo.fields; //(ClassFieldInfoHolder[])argArray.classObj;
                if (infos != null)
                    for (int i = 0; i < infos.Length; i++)
                    {
                        ClassFieldInfoHolder infoHolder = infos[i];
                        if (infoHolder.type == null) continue;
                        position = new Rect(position.x, position.y + standardSpacing, position.width, standardSpacing);
                        //EditorGUI.LabelField(position, infoHolder.type.Name);
                        position = DrawClassDef(position, argInfo.fields[i], infoHolder.type, infoHolder.fieldName, indentLevel + 1);

                    }
            }
            else
            {
                //argArrayFI.SetValue(argArrayContainer, null);
                argInfo.fields = null;
            }


            EditorGUI.indentLevel = indent;

            return position;
        }
        // Draw the property inside the given rect
        public override void OnGUI(Rect startPosition, SerializedProperty property, GUIContent label)
        {
            #region property info
            //general constants for GUI
            float standardSpacing = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            //Get access to the current properties Type information
            ClassPathInfo propertyClassInfo = property.serializedObject.targetObject.FindObjectFromPath(property.propertyPath);
            ExpressionFieldBase propertyObj = (ExpressionFieldBase)propertyClassInfo.classObj;//.GetField(property.name, Library.AllClassVariablesBindingFlag).FieldType;
            System.Attribute[] propertyAttributes = propertyClassInfo.attributes;
            System.Type propertyType = propertyObj.GetType();//.GetField(property.name, Library.AllClassVariablesBindingFlag).FieldType;
            System.Type[] propertyTypeGenericTypes = propertyType.GetGenericArguments();

            Calculator containingCalc = CalcHelper.ConvertClassToCalculator(property.serializedObject.targetObject.GetType(), propertyTypeGenericTypes);

            //property's internal property variables
            SerializedProperty expressionString = property.FindPropertyRelative("expression");
            SerializedProperty toggleBool = property.FindPropertyRelative("inspectorToggle");
            SerializedProperty validCheckBool = property.FindPropertyRelative("validCheck");
            SerializedProperty containingClassObject = property.FindPropertyRelative("containingClass");
            SerializedProperty lineHeightFloat = property.FindPropertyRelative("lineHeight");
            SerializedProperty argInfosRefArray = property.FindPropertyRelative("argInfos");
            #endregion

            Dictionary<string, string> argToName = new Dictionary<string, string>();
            Dictionary<string, string> nameToArg = new Dictionary<string, string>();

            //assigning the current property position tracker to it's default value
            Rect curPosition = startPosition;

            //storing referencs to the current GUI colors
            Color origColor = GUI.color;
            Color origBackgroundColor = GUI.backgroundColor;
            Color origContentColor = GUI.contentColor;

            //checking the current valid status and assigning values accordingly
            Color validColor = validCheckBool.boolValue ? Color.green : Color.red;

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
            Rect labelPosition = new Rect(curPosition.x, curPosition.y, curPosition.width - postLabelPosition.width, curPosition.height);
            toggleBool.boolValue = EditorGUI.Foldout(labelPosition, toggleBool.boolValue, label, true);


            //draw validate button in the corresponding color
            GUI.backgroundColor = validColor;
            if (GUI.Button(postLabelPosition, "Validate"))
            {
                Calculator varCheckCalc = property.serializedObject.targetObject.GetClassAsCalculator(propertyTypeGenericTypes);
                validCheckBool.boolValue = Expression.Validate(expressionString.stringValue, varCheckCalc, propertyTypeGenericTypes);
            }
            GUI.backgroundColor = origColor;
            #endregion



            //storing current indentation level
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;


            GUILayout.BeginVertical();
            if (toggleBool.boolValue)
            {
                EditorGUI.indentLevel = 1;
                //Calculator propCalc = CalcHelper.ConvertClassToCalculator(propertyType);

                ArgumentNameAttribute[] argAttributes = new ArgumentNameAttribute[propertyTypeGenericTypes.Length];
                string[] argNames = new string[propertyTypeGenericTypes.Length];

                for(int i = 0; i < propertyTypeGenericTypes.Length; i++)
                {
                    argNames[i] = $"arg{i}";
                }

                foreach(System.Attribute atr in propertyClassInfo.attributes)
                {
                    ArgumentNameAttribute argAtr = atr as ArgumentNameAttribute;

                    if (argAtr == null) continue;
                    if (argAtr.ArgNum >= propertyTypeGenericTypes.Length) continue;


                    argNames[argAtr.ArgNum] = argAtr.ArgName;
                }

                for (int i = 0; i < propertyTypeGenericTypes.Length; i++)
                {
                    argToName.Add($"arg{i}", argNames[i]);
                    nameToArg.Add(argNames[i], $"arg{i}");
                }

                //draw all generic TYpe info
                for (int i = 0; i < propertyTypeGenericTypes.Length; i++)
                {
                    curPosition = new Rect(startPosition.x, curPosition.y + standardSpacing, startPosition.width, EditorGUIUtility.singleLineHeight);
                    SerializedProperty argInfo = argInfosRefArray.GetArrayElementAtIndex(i);
                    ClassFieldInfoHolder info = (ClassFieldInfoHolder)argInfo.managedReferenceValue;




                    curPosition = DrawClassDef(curPosition, info, propertyTypeGenericTypes[i], argNames[i], 1);
                }

                EditorGUI.indentLevel = 0;

                //create textArea style
                Font font = Resources.Load<FontAsset>("ExpressionFieldTextAreaFont").sourceFontFile;
                GUIStyle textAreaStyle = new GUIStyle(GUI.skin.textArea);
                textAreaStyle.richText = true;
                textAreaStyle.font = font;

                //create textArea for user input
                int linesNeeded = (int)(CalcUnityHelper.GetTextWidth(expressionString.stringValue, font, textAreaStyle.fontSize) / startPosition.width) + 2;
                //int linesNeeded = (int)(100 / startPosition.width) + 2;
                if (Event.current.type == EventType.Repaint)
                    lineHeightFloat.floatValue = linesNeeded * standardSpacing;
                curPosition = new Rect(startPosition.x, curPosition.y + standardSpacing, startPosition.width, lineHeightFloat.floatValue);
                string valHolder = expressionString.stringValue;

                //create label directly over textArea to display text with coloring
                GUI.backgroundColor = new Color(0, 0, 0, 0);
                Expression labelExpress = new Expression(valHolder, false);
                containingCalc.ResolveIdentifiers(labelExpress);
                labelExpress.AddNullCaps();
                Expression.CondenseDots(labelExpress);
                labelExpress.RemoveAllNulls();

                string labelText = "";
                string inputText = "";
                int textIndex = 0;
                foreach (Token toke in labelExpress)
                {
                    Color col = CalcUnityHelper.TokenInspectorColor[toke.Type];
                    string tokeText = toke.ToString();


                    //removes any differences between what has been typed and what is being presented to the user
                    //NOTE: this for loop is wildly unsafe, will eventually need to improve
                    for (int i = 0; i < tokeText.Length;)
                    {
                        if (valHolder[textIndex] != tokeText[i])
                        {
                            tokeText = tokeText.Remove(i, 1);
                            continue;
                        }
                        i++;
                        textIndex++;
                    }

                    if (toke.Type == TokenTypeValue.Invalid)
                    {
                        labelText += $"<color=\"#{ColorUtility.ToHtmlStringRGBA(col)}\">_</color>";
                        inputText += $"{valHolder[textIndex]}";
                        textIndex++;
                    }
                    else if (toke.Type == TokenTypeValue.Ignore)
                    {
                        labelText += " ";
                        inputText += " ";
                        textIndex++;
                    }
                    else if (toke.Type == TokenTypeValue.Argument)
                    {
                        string[] argSplit = tokeText.Split('.');
                        string argFinal = argSplit[0];

                        if (argToName.ContainsKey(argSplit[0]))
                            argFinal = argToName[argSplit[0]];


                        labelText += $"<color=\"#{ColorUtility.ToHtmlStringRGBA(col)}\">{argFinal}</color>";
                        inputText += $"{argFinal}";
                    }
                    else
                    {
                        labelText += $"<color=\"#{ColorUtility.ToHtmlStringRGBA(col)}\">{tokeText}</color>";
                        inputText += $"{tokeText}";
                    }


                }

                valHolder = EditorGUI.TextArea(curPosition, inputText, textAreaStyle);
                EditorGUI.LabelField(curPosition, labelText, textAreaStyle);

                labelExpress = new Expression(valHolder, false);//reusing variable
                //propCalc.ResolveIdentifiers(labelExpress);
                //labelExpress.AddNullCaps();
                //Expression.CondenseDots(labelExpress);
                labelExpress.RemoveAllNulls();

                inputText = "";//reusing input text variable
                textIndex = 0;
                for( int i = 0; i < labelExpress.Count; i++)
                {
                    Token toke = labelExpress[i];
                    if (toke.Type == TokenTypeValue.Invalid)
                    {

                        inputText += $"{valHolder[textIndex]}";
                        textIndex++;
                    }
                    else if (toke.Type == TokenTypeValue.Ignore)
                    {
                        inputText += " ";
                        textIndex++;
                    }
                    else if (toke.Type == TokenTypeValue.Identifier)
                    {
                        if (nameToArg.ContainsKey(toke.ObjectName))
                        {
                            textIndex += toke.ObjectName.Length;
                            labelExpress[i] = new Token(nameToArg[toke.ObjectName]);
                        }
                        inputText += labelExpress[i].ToString();
                    }
                    else
                    {
                        string text = labelExpress[i].ToString();
                        inputText += text;
                        textIndex += text.Length;
                    }
                }

                //assign current value stored in textArea back out to the expression field in property
                expressionString.stringValue = inputText;
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
            System.Type propertyType = property.serializedObject.targetObject.FindObjectFromPath(property.propertyPath).classObj.GetType();
            System.Type[] propertyTypeGenericTypes = propertyType.GetGenericArguments();


            //property's internal property variables
            SerializedProperty expressionString = property.FindPropertyRelative("expression");
            SerializedProperty toggleBool = property.FindPropertyRelative("inspectorToggle");
            SerializedProperty validCheckBool = property.FindPropertyRelative("validCheck");
            SerializedProperty containingClassObject = property.FindPropertyRelative("containingClass");
            SerializedProperty lineHeightFloat = property.FindPropertyRelative("lineHeight");
            SerializedProperty argInfosRefArray = property.FindPropertyRelative("argInfos");
            #endregion

            float standardHeight = base.GetPropertyHeight(property, label);

            //if the drop down is on then addspace for the 'expression' property to be drawn
            if (toggleBool.boolValue)
            {
                //if 'property' is a generic type then addspace for the generic values info to be drawn
                if (propertyType.IsGenericType)
                {
                    int arraySize = propertyTypeGenericTypes.Length;

                    float fieldHeightNeeded = 0;
                    for (int i = 0; i < arraySize; i++)
                    {
                        SerializedProperty argInfo = argInfosRefArray.GetArrayElementAtIndex(i);
                        ClassFieldInfoHolder info = (ClassFieldInfoHolder)argInfo.managedReferenceValue;
                        if (info == null) continue;

                        fieldHeightNeeded += info.GetTotalHeight();
                    }

                    if (arraySize == 0) arraySize = 1;
                    return standardHeight + (arraySize * standardSpacing) + lineHeightFloat.floatValue + fieldHeightNeeded;
                }

                return standardHeight + standardSpacing + lineHeightFloat.floatValue;
            }

            return standardHeight;
        }
    }
}
