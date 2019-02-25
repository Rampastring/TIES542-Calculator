# Calculator

A calculator created as an exercise for the University of Jyväskylä course TIES542 Principles of Programming languages.

## Compiling

The calculator can be compiled with Visual Studio 2017 on Windows. .NET Framework 4.6.1 is required to run the compiled program.
I personally used VS2017 version 15.9.3, but older versions of VS2017 will likely work too, as long as they support modern C# syntax.

## Usage

When run, the calculator will ask you for input and solve given expressions. The grammar used it as follows:


    <expression> ::= <term>
                   | let <variable name> = <expression> in <expression>
    
    <term> ::= <factor>
             | <term> + <factor>
             | <term> - <factor>
             
    <factor> ::= <unary expression>
               | <factor> * <unary expression>
               | <factor> / <unary expression>
    
    <unary expression> ::= <primary expression>
                         | + <unary expression>
                         | - <unary expression>
    
    <primary expression> ::= <numeric constant>
                           | <variable name>
                           | ( <expression> )

Some example inputs:

    (2+3)*4

    let x = let x = (2* ((2+3)*4)) in 2*x in 2+x

	let x = 2*2 in let y = 3*2 in x*y

	let zyx = 2 + 2 in 2*3*4*5+6+7+8*9

Source of the grammar (accessible only to people living in Finland): https://tim.jyu.fi/view/kurssit/tie/okp/2017/content_notes/The-anatomy-of-a-programming-language
