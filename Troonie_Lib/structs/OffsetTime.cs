
namespace Troonie_Lib
{
    public struct OffsetTime
    {
        //private char sign;
        //private char hour1;
        //private char hour2;
        //private char minute1;
        //private char minute2;
        public bool HasValidValue { get; private set; }
        public string Value { get; private set; }
      
        public OffsetTime(string s)
        {            
            HasValidValue = false;
            Value = string.Empty;
            if (s != null &&
                (s[0] == '+' || s[0] == '-') &&
                (s[1] == '0' || s[1] == '1') &&
                (s[2] == '0' || s[2] == '1' || s[2] == '2' || s[2] == '3' || s[2] == '4' ||
                 s[2] == '5' || s[2] == '6' || s[2] == '7' || s[2] == '8' || s[2] == '9') &&
                (s[3] == ':') &&
                (s[4] == '0' || s[4] == '1' || s[4] == '3' || s[4] == '4') &&
                (s[5] == '0' || s[5] == '5'))
            { 
                Value = s;
                HasValidValue = true;
            }
        }


    }
}
