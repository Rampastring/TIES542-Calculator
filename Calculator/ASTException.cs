using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public class ASTException : Exception
    {
        public ASTException(string message) : base(message)
        {
        }
    }
}
