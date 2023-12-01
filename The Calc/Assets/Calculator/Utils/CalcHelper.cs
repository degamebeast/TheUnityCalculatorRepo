//Created by: Deontae Albertie

using System.Collections;
using System.Collections.Generic;
using System.Reflection;


namespace delib.calculate
{
    public static class CalcHelper
    {
        public static Calculator ConvertClassToCalculator(System.Type obj, params object[] args)
        {
            Calculator classContextCalc = new Calculator(args);



            foreach (FieldInfo fi in obj.GetFields(Library.AllClassVariablesBindingFlag))
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

    public static class CalculatorExtensionMethods
    {
        public static System.Type FindTypeFromPath(this System.Type type, string path)
        {
            string[] argPath = path.Split('.');
            //object arg = argumentMemory[argPath[0]].ObjectValue;

            System.Type curType = type;

            for (int argIndex = 0; argIndex < argPath.Length; argIndex++)
            {
                FieldInfo curFieldInfo = curType.GetField($"{argPath[argIndex]}", Library.AllClassVariablesBindingFlag);
                if (curFieldInfo == null)
                    return null;
                curType = curFieldInfo.FieldType;
            }

            return curType;
        }
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
