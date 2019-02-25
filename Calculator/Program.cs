using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set culture to en-US so we use '.' as decimal separator
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            // Some test inputs
            //string program = "let x = let x = (2* ((2+3)*4)) in 2*x in 2+x";
            //string program = "let x = 2*2 in let y = 3*2 in x*y";
            //string program = "let zyx = 2 + 2 in 4*zyx + 24";
            //string program = "let zyx = 2 + 2 in 4.5454*zyx + 24";
            //string program = "let zyx = 2 + 2 in 2*3*4*5+6+7+8*9";
            //string program = "let zyx = 2 + 2 in 2*(3+4)*5";

            Parser parser = new Parser();

            Console.WriteLine("Calculator");
            Console.WriteLine("(C) Rami Pasanen, 2019");
            Console.WriteLine("TIES542 Principles of Programming Languages");

            while (true)
            {
                Console.WriteLine();
                Console.Write("Input: > ");
                string input = Console.ReadLine();
                try
                {
                    parser.Parse(input);
                    Console.WriteLine("Result: " + parser.StartNode.GetValue());
                }
                catch (ASTException ex)
                {
                    Console.WriteLine("ASTException: " + ex.Message);
                }
                catch (VariableException ex)
                {
                    Console.WriteLine("VariableException: " + ex.Message);
                }
            }
        }
    }
}
