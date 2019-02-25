using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    /// <summary>
    /// The possible token types.
    /// </summary>
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

    /// <summary>
    /// Stores a token's type and its possible value.
    /// </summary>
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

    /// <summary>
    /// The lexer. Takes a string and converts it into a list of tokens.
    /// </summary>
    class Lexer
    {
        private List<TokenInfo> tokens = new List<TokenInfo>();

        private int position;

        private string input;

        public List<TokenInfo> GetTokens() => new List<TokenInfo>(tokens);


        public void Handle(string input)
        {
            this.input = input ?? throw new ArgumentNullException("input");
            tokens.Clear();
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

        private bool IsWhitespace(char c) => char.IsWhiteSpace(c);

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
        
        /// <summary>
        /// Finds the index in the input string where the identifier
        /// or constant that begins in the current position ends.
        /// </summary>
        private int FindIdentifierOrConstantEndIndex()
        {
            int index = position;
            char c = input[index];
            bool isConstant = char.IsDigit(c) || c == '.';
            if (isConstant)
                return FindConstantEndIndex();

            while (true)
            {
                if (input.Length == index)
                    return index;

                c = input[index];

                if (IsWhitespace(c) || !char.IsLetterOrDigit(c))
                    return index;

                index++;
            }
        }

        /// <summary>
        /// Finds the index in the input string where the
        /// constant that begins in the current position ends.
        /// </summary>
        private int FindConstantEndIndex()
        {
            int index = position;
            bool dotFound = false;

            while (true)
            {
                if (input.Length == index)
                    return index;

                char c = input[index];

                if (IsWhitespace(c))
                    return index;

                if (!char.IsDigit(c))
                {
                    if (!dotFound && c == '.')
                    {
                        dotFound = true;
                        index++;
                        continue;
                    }

                    return index;
                }

                index++;
            }
        }

        /// <summary>
        /// Gets the next token.
        /// </summary>
        /// <param name="value">The value of the token.</param>
        /// <returns>The type of the token. 
        /// Its possible value is stored in the <paramref name="value"/> parameter.</returns>
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

                    int endIndex = FindIdentifierOrConstantEndIndex();
                    int charCount = endIndex - position;
                    string sub = input.Substring(position, charCount);
                    position += charCount;
                    if (double.TryParse(sub, out double result))
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
