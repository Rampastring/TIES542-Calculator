using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexer lexer = new Lexer();
            lexer.Handle("let zyx = 2 + 2 in 4*zyx + 24");
            var tokens = lexer.GetTokens();
            foreach (var token in tokens)
            {
                Console.WriteLine(token.ToString());
            }
        }
    }
}
