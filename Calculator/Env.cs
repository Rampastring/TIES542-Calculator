using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    /// <summary>
    /// Stores and retrieves variables.
    /// </summary>
    class Env
    {
        public Dictionary<string, double> variables = new Dictionary<string, double>();

        public double GetValue(string varName)
        {
            if (variables.TryGetValue(varName, out double value))
                return value;

            throw new VariableException("Variable not found: " + varName);
        }

        public void SetValue(string varName, double value)
        {
            if (variables.ContainsKey(varName))
                throw new VariableException("Variable defined more than once in the same scope: " + varName);

            variables[varName] = value;
        }

        public void ReleaseValue(string varName)
        {
            if (!variables.Remove(varName))
                throw new VariableException("Couldn't find variable to release: " + varName);
        }

        public void Clear()
        {
            variables.Clear();
        }
    }
}
