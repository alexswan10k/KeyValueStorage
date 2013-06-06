using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Utility
{
    public class CharSubstitutor
    {
        public Dictionary<char, char> SubstitutionTable { get; set; }
        public CharSubstitutor()
        {
            SubstitutionTable = new Dictionary<char, char>();
        }

        public CharSubstitutor(Dictionary<char, char> substitutionTable)
        {
            SubstitutionTable = substitutionTable;
        }

        public string Convert(string input)
        {
            string outString = input;
            foreach (var charKV in SubstitutionTable)
            {
                outString = outString.Replace(charKV.Key, charKV.Value);
            }
            return outString;
        }

        public string ConvertBack(string input)
        {
            string outString = input;
            foreach (var charKV in SubstitutionTable)
            {
                outString = outString.Replace(charKV.Value, charKV.Key);
            }
            return outString;
        }
    }
}
