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

                if (fi.FieldType == typeof(Constant))
                {
                    Constant val = fi.GetValue(type) as Constant;
                    classContextCalc.AddVariableToMemory(fi.Name, val);
                }
                else if (fi.FieldType == typeof(float))
                {
                    float val = (float)fi.GetValue(type);
                    classContextCalc.AddVariableToMemory(fi.Name, val);
                }
                else if (fi.FieldType == typeof(Integer))
                {
                    Integer val = fi.GetValue(type) as Integer;
                    classContextCalc.AddVariableToMemory(fi.Name, val);
                }
                else if (fi.FieldType == typeof(int))
                {
                    int val = (int)fi.GetValue(type);
                    classContextCalc.AddVariableToMemory(fi.Name, val);
                }

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

    public static class CalculatorExtensionMethods
    {
        //returns the type of the field located at the end of path or 'null' if path does not exist
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
        //returns true if the path exists and false otherwise
        public static bool FieldPathIsValid(this System.Type type, string path)
        {
            return type.FindTypeFromPath(path) != null;
        }

        public static Calculator GetClassAsCalculator(this System.Object obj, params object[] args)
        {
            return CalcHelper.ConvertClassToCalculator(obj.GetType(),args);
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

