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

(Source of the grammar for people living in Finland: https://tim.jyu.fi/view/kurssit/tie/okp/2017/content_notes/The-anatomy-of-a-programming-language)
