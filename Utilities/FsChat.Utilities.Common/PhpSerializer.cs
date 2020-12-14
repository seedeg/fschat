using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsChat.Utilities.Common
{
    /// <summary>
    /// Serializer Class.
    /// </summary>
    public class PhpSerializer
    {
        //types:
        // N = null
        // s = string
        // i = int
        // d = double
        // a = array (hashtable)
        private Dictionary<SortedDictionary<string, dynamic>, bool> seenSortedDicts; //for serialize (to infinte prevent loops)
        private Dictionary<Hashtable, bool> seenHashtables; //for serialize (to infinte prevent loops)
        private Dictionary<ArrayList, bool> seenArrayLists; //for serialize (to infinte prevent loops) lol

        private int pos; //for unserialize

        public bool XMLSafe = true; //This member tells the serializer wether or not to strip carriage returns from strings when serializing and adding them back in when deserializing
                                    //http://www.w3.org/TR/REC-xml/#sec-line-ends

        public Encoding StringEncoding = new UTF8Encoding();

        private readonly System.Globalization.NumberFormatInfo nfi;

        public PhpSerializer()
        {
            nfi = new System.Globalization.NumberFormatInfo
            {
                NumberGroupSeparator = "",
                NumberDecimalSeparator = "."
            };
        }

        public string Serialize(object obj)
        {
            seenArrayLists = new Dictionary<ArrayList, bool>();
            seenHashtables = new Dictionary<Hashtable, bool>();
            seenSortedDicts = new Dictionary<SortedDictionary<string, dynamic>, bool>();

            return Serialize(obj, new StringBuilder()).ToString();
        }

        private StringBuilder Serialize(object obj, StringBuilder sb)
        {
            if (obj == null)
            {
                return sb.Append("N;");
            }
            else if (obj is string)
            {
                string str = (string)obj;
                if (XMLSafe)
                {
                    str = str.Replace("\r\n", "\n");//replace \r\n with \n
                    str = str.Replace("\r", "\n");//replace \r not followed by \n with a single \n  Should we do this?
                }
                return sb.Append("s:" + StringEncoding.GetByteCount(str) + ":\"" + str + "\";");
            }
            else if (obj is bool)
            {
                return sb.Append("b:" + (((bool)obj) ? "1" : "0") + ";");
            }
            else if (obj is int)
            {
                int i = (int)obj;
                return sb.Append("i:" + i.ToString(nfi) + ";");
            }
            else if (obj is double)
            {
                double d = (double)obj;

                return sb.Append("d:" + d.ToString(nfi) + ";");
            }
            else if (obj is ArrayList)
            {
                if (seenArrayLists.ContainsKey((ArrayList)obj))
                    return sb.Append("N;");//cycle detected
                else
                    seenArrayLists.Add((ArrayList)obj, true);

                ArrayList a = (ArrayList)obj;
                sb.Append("a:" + a.Count + ":{");
                for (int i = 0; i < a.Count; i++)
                {
                    Serialize(i, sb);
                    Serialize(a[i], sb);
                }
                sb.Append("}");
                return sb;
            }
            else if (obj is Hashtable)
            {
                if (seenHashtables.ContainsKey((Hashtable)obj))
                    return sb.Append("N;");//cycle detected
                else
                    seenHashtables.Add((Hashtable)obj, true);

                Hashtable a = (Hashtable)obj;
                sb.Append("a:" + a.Count + ":{");
                foreach (DictionaryEntry entry in a)
                {
                    Serialize(entry.Key, sb);
                    Serialize(entry.Value, sb);
                }
                sb.Append("}");
                return sb;
            }
            else if (obj is SortedDictionary<string, dynamic>)
            {
                if (seenSortedDicts.ContainsKey((SortedDictionary<string, dynamic>)obj))
                    return sb.Append("N;");//cycle detected
                else
                    seenSortedDicts.Add((SortedDictionary<string, dynamic>)obj, true);

                SortedDictionary<string, dynamic> a = (SortedDictionary<string, dynamic>)obj;
                sb.Append("a:" + a.Count + ":{");
                foreach (KeyValuePair<string, dynamic> entry in a)
                {
                    Serialize(entry.Key, sb);
                    Serialize(entry.Value, sb);
                }
                sb.Append("}");
                return sb;
            }
            else
            {
                return sb;
            }
        }

        public object Deserialize(string str)
        {
            pos = 0;
            return DeserializeString(str);
        }

        private object DeserializeString(string str)
        {
            if (str == null || str.Length <= pos)
                return new Object();

            int start, end, length;
            string stLen;
            switch (str[pos])
            {
                case 'N':
                    pos += 2;
                    return null;
                case 'b':
                    char chBool;
                    chBool = str[pos + 2];
                    pos += 4;
                    return chBool == '1';
                case 'i':
                    string stInt;
                    start = str.IndexOf(":", pos) + 1;
                    end = str.IndexOf(";", start);
                    stInt = str.Substring(start, end - start);
                    pos += 3 + stInt.Length;
                    return Int32.Parse(stInt, nfi);
                case 'd':
                    string stDouble;
                    start = str.IndexOf(":", pos) + 1;
                    end = str.IndexOf(";", start);
                    stDouble = str.Substring(start, end - start);
                    pos += 3 + stDouble.Length;
                    return Double.Parse(stDouble, nfi);
                case 's':
                    start = str.IndexOf(":", pos) + 1;
                    end = str.IndexOf(":", start);
                    stLen = str.Substring(start, end - start);
                    int bytelen = Int32.Parse(stLen);
                    length = bytelen;
                    //This is the byte length, not the character length - so we migth  
                    //need to shorten it before usage. This also implies bounds checking
                    if ((end + 2 + length) >= str.Length) length = str.Length - 2 - end;
                    string stRet = str.Substring(end + 2, length);
                    while (StringEncoding.GetByteCount(stRet) > bytelen)
                    {
                        length--;
                        stRet = str.Substring(end + 2, length);
                    }
                    pos += 6 + stLen.Length + length;
                    if (XMLSafe)
                    {
                        stRet = stRet.Replace("\n", "\r\n");
                    }
                    return stRet;
                case 'a':
                    //if keys are ints 0 through N, returns an ArrayList, else returns Hashtable
                    start = str.IndexOf(":", pos) + 1;
                    end = str.IndexOf(":", start);
                    stLen = str.Substring(start, end - start);
                    length = Int32.Parse(stLen);
                    Hashtable htRet = new Hashtable(length);
                    ArrayList alRet = new ArrayList(length);
                    pos += 4 + stLen.Length; //a:Len:{
                    for (int i = 0; i < length; i++)
                    {
                        //read key
                        object key = DeserializeString(str);
                        //read value
                        object val = DeserializeString(str);

                        if (alRet != null)
                        {
                            if (key is int && (int)key == alRet.Count)
                                alRet.Add(val);
                            else
                                alRet = null;
                        }
                        htRet[key] = val;
                    }
                    pos++; //skip the }
                    if (pos < str.Length && str[pos] == ';')//skipping our old extra array semi-colon bug (er... php's weirdness)
                        pos++;
                    if (alRet != null)
                        return alRet;
                    else
                        return htRet;
                default:
                    return "";
            }//switch
        }//unserialzie(object)	
    }//class Serializer
}
