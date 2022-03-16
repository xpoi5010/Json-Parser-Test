using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace json_parser_test_
{
    public static class JsonParser
    {
        public static object JSONDeserialize(string text)
        {
            Stack<ObjectParseState> stateStack = new Stack<ObjectParseState>();
            stateStack.Push(ObjectParseState.ObjectParse);
            Stack<object> objStack = new Stack<object>();
            CharReader reader = new CharReader(text);
            char readChar = '0';
            bool isNegative = false;
            while (!reader.IsEnd())
            {
                readChar = reader.Peek();
                if (stateStack.Count == 0)
                    break;
                ObjectParseState currentState = stateStack.Peek();
                switch (currentState)
                {
                    case ObjectParseState.ObjectParse:
                        if (char.IsWhiteSpace(readChar) || char.IsControl(readChar)) //Skip Space Characters and Control Characters
                        {
                            reader.Accept();
                        }
                        else if (readChar == '{') //Json Object Start
                        {
                            stateStack.Pop();
                            stateStack.Push(ObjectParseState.JsonObject);
                            objStack.Push(new JSONObject());
                            reader.Accept();
                        }
                        else if (readChar == '[') //Array Start
                        {
                            stateStack.Pop();
                            stateStack.Push(ObjectParseState.Array);
                            objStack.Push(new JSONArray());
                            reader.Accept();
                        }
                        else if (readChar == '"') //String Start
                        {
                            stateStack.Pop();
                            stateStack.Push(ObjectParseState.String);
                            objStack.Push(new StringBuilder(128));
                            reader.Accept();
                        }
                        else if (char.IsDigit(readChar) || readChar == '-') //Number Start
                        {
                            stateStack.Pop();
                            stateStack.Push(ObjectParseState.Number);
                            isNegative = readChar == '-';
                            if (isNegative)
                                reader.Accept();
                            objStack.Push(new BigInteger(0));
                        }
                        else if (readChar == '}') //Dictionary End
                        {
                            stateStack.Pop(); // If there is no more elements in dictionary, it will backs to dictionary state.
                        }
                        else if (readChar == ']') //Array End: When it has no element.
                        {
                            stateStack.Pop();
                            if(stateStack.Peek() != ObjectParseState.Array)
                                throw new Exception($"unexpected {readChar}. Offset: {reader.Offset}");
                        }
                        else if (reader.AcceptString("null"))
                        {
                            objStack.Push(null);
                            stateStack.Pop();
                        }
                        else if (reader.AcceptString("true"))
                        {
                            objStack.Push(true);
                            stateStack.Pop();
                        }
                        else if (reader.AcceptString("false"))
                        {
                            objStack.Push(false);
                            stateStack.Pop();
                        }
                        else
                        {
                            throw new Exception($"unexpected {readChar}. Offset: {reader.Offset}");
                        }
                        break;
                    case ObjectParseState.JsonObject:
                        if (char.IsWhiteSpace(readChar) || char.IsControl(readChar)) //Skip Space Characters and Control Characters
                        {
                            reader.Accept();
                        }
                        else if(readChar == '}')
                        {
                            if(objStack.Peek() is KeyValuePair<string, object>)
                            {
                                KeyValuePair<string,object> obj = (KeyValuePair<string, object>)objStack.Pop();
                                if (objStack.Peek() is not JSONObject)
                                    throw new Exception("unknown error: ObjNotJsonObj[1].");
                                JSONObject outDict = (JSONObject)objStack.Peek();
                                outDict.Add(obj.Key, obj.Value);
                            }
                            if(objStack.Peek() is not JSONObject)
                                throw new Exception("unknown error: ObjNotDict[2].");
                            stateStack.Pop();
                            reader.Accept();
                        }
                        else if(readChar == '"') //Dict obj starts
                        {
                            stateStack.Push(ObjectParseState.JsonObj_Key);
                            stateStack.Push(ObjectParseState.ObjectParse);
                        }
                        else
                        {
                            throw new Exception($"expected \" instead of {readChar}. Offset: {reader.Offset}");
                        }
                        break;
                    case ObjectParseState.JsonObj_Key:
                        if(readChar == ':')// to switch next state
                        {
                            object topObj = objStack.Peek();
                            if (topObj is not string)
                                throw new Exception("Type of key is required of string. Offset: {reader.Offset}");

                            stateStack.Pop();
                            stateStack.Push(ObjectParseState.JsonObj_Value);
                            stateStack.Push(ObjectParseState.ObjectParse);

                            reader.Accept();
                        }
                        else if (char.IsWhiteSpace(readChar) || char.IsControl(readChar)) //Skip Space Characters and Control Characters
                        {
                            reader.Accept();
                        }
                        else
                        {
                            throw new Exception($"expected : instead of {readChar}.");
                        }
                        break;
                    case ObjectParseState.JsonObj_Value:
                        if (char.IsWhiteSpace(readChar) || char.IsControl(readChar)) //Skip Space Characters and Control Characters
                        {
                            reader.Accept();
                        }
                        else if (readChar == ',' || readChar == '}')// next JsonObj
                        {
                            stateStack.Pop();
                            object valueObj = objStack.Pop();
                            object keyObj = objStack.Pop();
                            JSONObject baseDict = (JSONObject)objStack.Peek();
                            if (keyObj is not string)
                                throw new Exception("unknown error: KEY_TYPE_NOT_STR.");
                            baseDict.Add((string)keyObj, valueObj);
                            if(readChar == ',')
                                reader.Accept();
                        }
                        else
                        {
                            throw new Exception($"expected , or }} instead of {readChar}. Offset: {reader.Offset}");
                        }
                        break;
                    case ObjectParseState.String:
                        if(readChar == '"')
                        {
                            object stringBuilder = objStack.Pop();
                            if (stringBuilder is not StringBuilder)
                                throw new Exception("unknown error: STR_BUILDER_NOT_FOUND");
                            string outStr = ((StringBuilder)stringBuilder).ToString();
                            objStack.Push(outStr);
                            stateStack.Pop();
                            reader.Accept();
                        }
                        else if(readChar == '\\')
                        {
                            stateStack.Push(ObjectParseState.Str_Escape);
                            reader.Accept();
                        }
                        else
                        {
                            StringBuilder sbb = (StringBuilder)objStack.Peek();
                            sbb.Append(readChar);
                            reader.Accept();
                        }
                        break;
                    case ObjectParseState.Str_Escape:
                        StringBuilder sb = (StringBuilder)objStack.Peek();
                        if (readChar == 't') sb.Append('\t');
                        else if (readChar == 'r') sb.Append('\r');
                        else if (readChar == 'n') sb.Append('\n');
                        else if (readChar == '\\') sb.Append('\\');
                        else if (readChar == 'b') sb.Append('\b');
                        else if (readChar == '/') sb.Append('/');
                        else if (readChar == '"') sb.Append('"');
                        else if (readChar == 'f') sb.Append('\f');
                        else if(readChar == 'u')
                        {
                            int utf16Char = 0;
                            for(int i = 0; i < 4; i++)
                            {
                                utf16Char <<= 8;
                                char c = (char)reader.Read();
                                if (c == -1)
                                    throw new Exception("arrived the end of file.");
                                if (c is >= '0' and <= '9')
                                    utf16Char += (c - '0');
                                else if(c is >= 'A' and <= 'F')
                                    utf16Char += (c - 'A' + 0xA);
                                else if (c is >= 'a' and <= 'f')
                                    utf16Char += (c - 'a' + 0xA);
                                else
                                    throw new Exception("it is not hex char.");
                            }
                            sb.Append((char)utf16Char);
                        }
                        reader.Accept();
                        stateStack.Pop();
                        break;
                    case ObjectParseState.Number:
                        if (char.IsDigit(readChar))
                        {
                            int num = readChar - '0';
                            BigInteger integer = (BigInteger)objStack.Pop();
                            integer *= 10;
                            if (isNegative)
                                integer -= num;
                            else
                                integer += num;
                            objStack.Push(integer);
                            reader.Accept();
                        }
                        else
                        {
                            stateStack.Pop();
                        }
                        break;
                    case ObjectParseState.Array:
                        if (char.IsWhiteSpace(readChar) || char.IsControl(readChar)) //Skip Space Characters and Control Characters
                        {
                            reader.Accept();
                        }
                        else if (readChar == ']')
                        {
                            stateStack.Pop();
                            reader.Accept();
                        }
                        else
                        {
                            stateStack.Push(ObjectParseState.Array_Object);
                            stateStack.Push(ObjectParseState.ObjectParse);
                        }
                        break;
                    case ObjectParseState.Array_Object:
                        if (char.IsWhiteSpace(readChar) || char.IsControl(readChar)) //Skip Space Characters and Control Characters
                        {
                            reader.Accept();
                        }
                        else if (readChar == ']' || readChar == ',')
                        {
                            object obj = objStack.Pop();
                            JSONArray arr = (JSONArray)objStack.Peek();
                            arr.Add(obj);
                            stateStack.Pop();
                            if (readChar == ',')
                                reader.Accept();
                        }
                        else
                        {
                            throw new Exception($"expected ] or , instead of {readChar}. Offset: {reader.Offset}");
                        }
                        break;
                }
            }
            if (objStack.Count > 1)
                throw new Exception("");
            return objStack.Pop();
        }

        private enum ObjectParseState
        {
            ObjectParse, JsonObject, JsonObj_Key, JsonObj_Value, Array, Array_Object ,String, Str_Escape, Number
        }
    }
}
