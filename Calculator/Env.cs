using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    class Env
    {
        public Dictionary<string, int> variables = new Dictionary<string, int>();

        public int GetValue(string varName)
        {
            if (variables.TryGetValue(varName, out int value))
                return value;

            throw new Exception("Value not found: " + varName);
        }
    }
}
