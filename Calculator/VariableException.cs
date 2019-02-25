using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public class VariableException : Exception
    {
        public VariableException(string message) : base(message)
        {
        }
    }
}
