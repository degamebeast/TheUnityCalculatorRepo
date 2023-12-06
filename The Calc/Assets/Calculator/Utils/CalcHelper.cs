//Created by: Deontae Albertie

using System.Collections;
using System.Collections.Generic;
using System.Reflection;


namespace delib.calculate
{
    public static class CalcHelper
    {
        //Creates a calculator from the given class type
        public static Calculator ConvertClassToCalculator(System.Type type, params object[] args)
        {
            Calculator classContextCalc = new Calculator(args);



            foreach (FieldInfo fi in type.GetFields(Library.AllClassVariablesBindingFlag))
            {

                classContextCalc.AddVariableToMemory(fi.Name, null);

            }

            return classContextCalc;
        }

        //removes rich text tag pairs from a  string
        //NOTE: needs to be improved doesn't handle edge cases like unclosed tags
        public static string RemoveAllRichText(string str)
        {
            int gtIndex = -1;
            int ltIndex = -1;
            string curTag = "";

            bool tagsRemain = true;

            while (tagsRemain)
            {
                tagsRemain = false;
                for (int curIndex = str.Length - 1; curIndex >= 0; curIndex--)
                {
                    if (str[curIndex] == '>')
                    {
                        gtIndex = curIndex;
                        continue;
                    }
                    if (str[curIndex] == '/')
                    {

                        if (curIndex - 1 < 0) continue;
                        if (str[curIndex - 1] != '<') continue;
                        ltIndex = curIndex - 1;
                        curTag = str.Substring(curIndex + 1, (gtIndex - curIndex) - 1);

                        str = str.Remove(ltIndex, gtIndex - ltIndex + 1);

                        ltIndex = str.IndexOf($"<{curTag}");

                        gtIndex = str.IndexOf('>', ltIndex);

                        str = str.Remove(ltIndex, gtIndex - ltIndex + 1);

                        tagsRemain = true;
                        break;
                    }

                }
            }



            return str;
        }
    }

    public class ClassPathInfo
    {
        public object classObj;
        public string fieldNameInContainer;

        public ClassPathInfo()
        {
            classObj = null;
            fieldNameInContainer = null;
        }
        public ClassPathInfo(object obj, string name)
        {
            classObj = obj;
            fieldNameInContainer = name;
        }
    }
    public static class CalculatorExtensionMethods
    {
        //returns the type of the field located at the end of path or 'null' if path does not exist
        public static ClassPathInfo FindObjectFromPath(this object obj, string path) 
        {

            ClassPathInfo[] discard;
            return FindObjectFromPath(obj,path, out discard); 
        }
        public static ClassPathInfo FindObjectFromPath(this object obj, string path, out ClassPathInfo[] objPathObjects)
        {
            List<ClassPathInfo> fullObjectPathObjects = new List<ClassPathInfo>();
            objPathObjects = null;
            string[] objPath = path.Split('.');

            object curObj = obj;
            System.Type curType = curObj.GetType();
            string curPath = null;
            for (int objIndex = 0; objIndex < objPath.Length; objIndex++)
            {
                fullObjectPathObjects.Add(new ClassPathInfo(curObj, curPath));
                curPath = objPath[objIndex];

                if (curPath == "Array")//detects an array/list
                {
                    curPath = "_items";
                }

                if (curType.IsArray)
                {
                    if (curPath == "_items")//in case where the item is an actual arry we discard the "_item" token
                    {
                        objIndex++;
                        curPath = objPath[objIndex];
                    }
                    int arrayIndex = int.Parse(curPath.Substring(curPath.IndexOf('[') + 1, 1));
                    curObj = ((object[])curObj)[arrayIndex];
                    curType = curObj.GetType();
                    continue;
                }
                FieldInfo curFieldInfo = curType.GetField($"{curPath}", Library.AllClassVariablesBindingFlag);
                if (curFieldInfo == null)
                    return null;
                curObj = curFieldInfo.GetValue(curObj);
                if (curObj == null)//in case the final property in our path is a nullable type and is currently null
                    curType = null;
                else
                    curType = curObj.GetType();
            }

            objPathObjects = fullObjectPathObjects.ToArray();
            return new ClassPathInfo(curObj, curPath);
        }

        //returns the type of the field located at the end of path or 'null' if path does not exist
        //NOTE: works for fields and properties
        public static System.Type FindTypeFromPath(this System.Type type, string path)
        {
            string[] argPath = path.Split('.');
            //object arg = argumentMemory[argPath[0]].ObjectValue;

            System.Type curType = type;

            for (int argIndex = 0; argIndex < argPath.Length; argIndex++)
            {
                FieldInfo curFieldInfo = curType.GetField($"{argPath[argIndex]}", Library.AllClassVariablesBindingFlag);
                PropertyInfo curPropertyInfo = curType.GetProperty($"{argPath[argIndex]}", Library.AllClassVariablesBindingFlag);

                if (curPropertyInfo != null)
                {
                    curType = curPropertyInfo.PropertyType;
                    continue;
                }

                if (curFieldInfo == null)
                    return null;
                curType = curFieldInfo.FieldType;
            }

            return curType;
        }

        //returns true if the path exists and ends in a valid constant number type, false otherwise
        //NOTE: works for fields and properties
        public static bool FieldPathIsConstantNumber(this System.Type type, string path)
        {
            System.Type t = type.FindTypeFromPath(path);
            if (t == null)
                return false;
            return Library.CaclulatorConstantTypes.Contains(t);
        }

        //returns true if the path exists and false otherwise
        //NOTE: works for fields and properties
        public static bool FieldPathIsValid(this System.Type type, string path)
        {
            return type.FindTypeFromPath(path) != null;
        }

        public static Calculator GetClassAsCalculator(this System.Object obj, params object[] args)
        {
            Calculator classContextCalc = new Calculator(args);



            foreach (FieldInfo fi in obj.GetType().GetFields(Library.AllClassVariablesBindingFlag))
            {

                if (fi.FieldType == typeof(Constant))
                {
                    Constant val = fi.GetValue(obj) as Constant;
                    classContextCalc.AddVariableToMemory(fi.Name, val);
                }
                else if (fi.FieldType == typeof(float))
                {
                    float val = (float)fi.GetValue(obj);
                    classContextCalc.AddVariableToMemory(fi.Name, val);
                }
                else if (fi.FieldType == typeof(Integer))
                {
                    Integer val = fi.GetValue(obj) as Integer;
                    classContextCalc.AddVariableToMemory(fi.Name, val);
                }
                else if (fi.FieldType == typeof(int))
                {
                    int val = (int)fi.GetValue(obj);
                    classContextCalc.AddVariableToMemory(fi.Name, val);
                }

            }

            return classContextCalc;
        }
    }

    //a 2 way dictionary that allows for the 'key' and 'value' to be mutually paired
    public class BiDictionary<TKey, TValue> : IEnumerable
    {
        private Dictionary<TKey, TValue> dict;
        private Dictionary<TValue, TKey> dictInv;

        public BiDictionary()
        {
            dict = new Dictionary<TKey, TValue>();
            dictInv = new Dictionary<TValue, TKey>();
        }

        public void Add(TKey key, TValue value)
        {
            dict.Add(key, value);
            dictInv.Add(value, key);
        }
        public void Add(TValue key, TKey value)
        {
            dict.Add(value, key);
            dictInv.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            dictInv.Remove(dict[key]);
            return dict.Remove(key);
        }
        public bool Remove(TValue key)
        {
            dict.Remove(dictInv[key]);
            return dictInv.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dict.TryGetValue(key, out value);
        }

        public bool TryGetValue(TValue key, out TKey value)
        {
            return dictInv.TryGetValue(key, out value);
        }

        public bool ContainsKey(TKey key)
        {
            return dict.ContainsKey(key);
        }

        public bool ContainsKey(TValue key)
        {
            return dictInv.ContainsKey(key);
        }

        public TValue this[TKey key]
        {
            get
            {
                return dict[key];
            }
        }

        public TKey this[TValue value]
        {
            get
            {
                return dictInv[value];
            }
        }


        public IEnumerator GetEnumerator()
        {
            return dict.GetEnumerator();
        }
    }



}

