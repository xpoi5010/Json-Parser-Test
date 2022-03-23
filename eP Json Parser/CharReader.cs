using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eP.Text
{
    public unsafe class CharReader
    {
        private string BaseString;
        private int length = 0;
        private int offset = 0;
        private int pending = 0;
        public int Offset => offset;
        public CharReader(string str)
        {
            BaseString = str;
            length = str.Length;
            offset = 0;
            pending = 0;
        }
        public bool IsEnd()
        {
            return length <= offset;
        }
        public char Read()
        {
            if (length <= offset) throw new Exception("Over range.");
            return BaseString[offset++];
        }
        public char Peek()
        {
            if (length <= offset) throw new Exception("Over range.");
            pending = 1;
            return BaseString[offset];
        }
        public bool CompareString(string comparedString)
        {
            if (comparedString.Length > (length - offset))
                return false;
            return string.Compare(BaseString, offset, comparedString, 0, comparedString.Length) == 0;
        }

        public bool AcceptString(string comparedString)
        {
            bool result = CompareString(comparedString);
            if (result)
            {
                offset += comparedString.Length;
                pending = 0;
            }
            return result;
        }

        public void Accept()
        {
            offset += pending;
            pending = 0;
        }
    }
}
