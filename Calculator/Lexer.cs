using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public enum Token
    {
        Plus,
        Minus,
        Star,
        Slash,
        Constant,
        Variable,
        Let,
        In,
        OpeningParanthesis,
        ClosingParenthesis,
        EqualitySign
    }

    public struct TokenInfo
    {
        public TokenInfo(Token token, object value = null)
        {
            Token = token;
            Value = value;
        }

        public Token Token { get; }
        public object Value { get; }

        public override string ToString()
        {
            return Value == null ? Token.ToString() : Token.ToString() + $" ({Value.ToString()})"; 
        }
    }

    class Lexer
    {
        private List<TokenInfo> tokens;

        private int position;

        private string input;

        public List<TokenInfo> GetTokens() => new List<TokenInfo>(tokens);



        public void Handle(string input)
        {
            this.input = input ?? throw new ArgumentNullException("input");
            tokens = new List<TokenInfo>();
            position = 0;

            while (true)
            {
                SkipWhitespace();

                if (input.Length == position)
                    break;

                Token token = GetNextToken(out object value);
                tokens.Add(new TokenInfo(token, value));
            }
        }

        private bool IsWhitespace(char c) => c == ' ' || c == '\n' || c == '\r';

        private void SkipWhitespace()
        {
            while (true)
            {
                if (input.Length > position && IsWhitespace(input[position]))
                    position++;
                else
                    break;
            }
        }
        
        private int FindIdentifierOrVariableEndIndex()
        {
            int index = position;
            while (true)
            {
                if (input.Length == index || IsWhitespace(input[index]))
                    return index;
                index++;
            }
        }


        private Token GetNextToken(out object value)
        {
            char c = input[position];

            value = null;

            switch (c)
            {
                case '+':
                    position++;
                    return Token.Plus;
                case '-':
                    position++;
                    return Token.Minus;
                case '*':
                    position++;
                    return Token.Star;
                case '/':
                    position++;
                    return Token.Slash;
                case '(':
                    position++;
                    return Token.OpeningParanthesis;
                case ')':
                    position++;
                    return Token.ClosingParenthesis;
                case '=':
                    position++;
                    return Token.EqualitySign;
                default:
                    if (input.Length - position > 3 && input.Substring(position, 3) == "let")
                    {
                        position += 3;
                        return Token.Let;
                    } 
                     
                    if (input.Length - position > 2 && input.Substring(position, 2) == "in")
                    {
                        position += 2;
                        return Token.In;
                    }

                    int endIndex = FindIdentifierOrVariableEndIndex();
                    int charCount = endIndex - position;
                    string sub = input.Substring(position, charCount);
                    position += charCount;
                    if (int.TryParse(sub, out int result))
                    {
                        value = result;
                        return Token.Constant;
                    }

                    value = sub;
                    return Token.Variable;
            }
        }
    }
}
