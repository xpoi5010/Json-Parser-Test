using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace json_parser_test_
{
    public class JSONObject:IEnumerable<JSONKeyPair>
    {
        JSONKeyPair firstElement;
        JSONKeyPair endElement;
        public int Count { get; private set; } = 0;
        public JSONObject()
        {

        }

        public void Add(string Key, object Value)
        {
            JSONKeyPair newElement = new JSONKeyPair()
            {
                Key = Key,
                Value = Value
            };
            if (firstElement is null)
                firstElement = endElement = newElement;
            else
            {
                endElement.Next = newElement;
                endElement = newElement;
            }
            Count++;
        }

        public IEnumerator<JSONKeyPair> GetEnumerator()
        {
            return new JSONKeyPairEnumerator(firstElement);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new JSONKeyPairEnumerator(firstElement);
        }

        public class JSONKeyPairEnumerator : IEnumerator<JSONKeyPair>
        {
            private JSONKeyPair topObj;
            private JSONKeyPair firstObj;
            private int position = -1;
            public JSONKeyPairEnumerator(JSONKeyPair topObj)
            {
                this.topObj = this.firstObj = topObj;
            }

            public JSONKeyPair Current => topObj;

            object IEnumerator.Current => topObj;

            public void Dispose()
            {
                firstObj = topObj = null;
            }

            public bool MoveNext()
            {
                if (topObj is null || topObj.Next is null)
                    return false;
                if(position >= 0)
                    topObj = topObj.Next;
                position++;
                return true;
            }

            public void Reset()
            {
                topObj = firstObj;
                position = -1;
            }
        }

        public override string ToString()
        {
            return $"JsonObject: Count:{Count}";
        }
    }

    public class JSONArray : IEnumerable<object>
    {
        JSONArrayObject firstElement;
        JSONArrayObject endElement;
        public int Count { get; private set; } = 0;
        public JSONArray()
        {

        }
        public void Add(object Value)
        {
            JSONArrayObject newElement = new JSONArrayObject()
            {
                Value = Value
            };
            if (firstElement is null)
                firstElement = endElement = newElement;
            else
            {
                endElement.Next = newElement;
                endElement = newElement;
            }
            Count++;
        }

        public IEnumerator<object> GetEnumerator()
        {
            return new JSONArrayEnumlator(firstElement);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new JSONArrayEnumlator(firstElement);
        }

        public class JSONArrayEnumlator : IEnumerator<object>, IEnumerator
        {
            private JSONArrayObject topObj;
            private JSONArrayObject firstObj;
            private int position = -1;
            public JSONArrayEnumlator(JSONArrayObject topObj)
            {
                this.topObj = this.firstObj = topObj;
            }
            public object Current => topObj.Value;

            public void Dispose()
            {
                firstObj = topObj = null;
            }

            public bool MoveNext()
            {
                if (topObj is null || topObj.Next is null)
                    return false;
                if(position >= 0)
                    topObj = topObj.Next;
                position++;
                return true;
            }

            public void Reset()
            {
                topObj = firstObj;
                position = -1;
            }
        }

        public override string ToString()
        {
            return $"Array: Count:{Count}";
        }
    }
    public class JSONArrayObject
    {
        public object Value { get; set; }
        public JSONArrayObject Next { get; set; }

    }

    public class JSONKeyPair
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public JSONKeyPair Next { get; set; }

        public override string ToString()
        {
            return $"[{Key},{Value}]";
        }
    }
    
}
