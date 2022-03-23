using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace eP.Text.Json
{
    public class JsonNumber
    {
        private bool _isNegative = false;
        private ulong _value = 0;
        private long _exponent = 0;
        public JsonNumber(bool IsNegative, ulong Value,long Exponent)
        {
            _isNegative = IsNegative;
            _value = Value;
            _exponent = Exponent;
        }

        public override string ToString()
        {
            if (_value == 0)
            {
                _exponent = 0;
                _isNegative = false;
            }
            string _numOut = _value.ToString();
            long _newExp = (_numOut.Length-1) + _exponent;
            string _expOut = _newExp.ToString();
            StringBuilder sb = new StringBuilder(_numOut.Length+_expOut.Length+3);
            if(_isNegative)
                sb.Append('-');
            if(_numOut.Length < 2 || _exponent == 0)
                sb.Append(_numOut);
            else
            {
                sb.Append(_numOut[0]);
                sb.Append('.');
                sb.Append(_numOut.Substring(1));
            }
            if(_newExp != 0 && _exponent != 0)
            {
                sb.Append('E');
                if(_exponent > 0)
                    sb.Append('+');
                sb.Append(_expOut);
            }
            return sb.ToString();
        }
    }

    public class JsonNumberConstructor
    {
        private bool _isNegative = false;
        private ulong _value = 0;
        private long _exponent = 0;
        private bool _exponent_flag = true;
        private long _exponent_parts = 0;
        private bool over_precision()
        {
            return _value >= 10000000000000000;
        }
        public void SetNegative()
        {
            _isNegative = true;
        }
        public void WriteIntegerDigital(uint digital)
        {
            if (over_precision())
            {
                _exponent++;
                return;
            }
            _value *= 10;
            _value += digital;
        }

        public void WriteDecimalDigital(uint digital)
        {
            if (over_precision())
            {
                return;
            }
            _exponent--;
            _value *= 10;
            _value += digital;
        }

        public void SetExponentFlag(bool IsPassive)
        {
            _exponent_flag = IsPassive;
        }

        public void WriteExponentDigital(uint digital)
        {
            _exponent_parts *= 10;
            if(_exponent_flag)
                _exponent_parts += digital;
            else
                _exponent_parts -= digital;
        }

        public JsonNumber GetResult()
        {
            return new JsonNumber(_isNegative,_value, _exponent + _exponent_parts);
        }
    }
}
