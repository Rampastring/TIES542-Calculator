using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    /// <summary>
    /// The parser. Builds a syntax tree from a given input string by 
    /// making use of the <see cref="Lexer"/>.
    /// </summary>
    class Parser
    {
        public Parser()
        {
            env = new Env();
        }

        public Node StartNode;

        private List<TokenInfo> tokens;
        private int tokenIndex;
        private Env env;

        public void Parse(string str)
        {
            env.Clear();
            tokenIndex = 0;
            Lexer l = new Lexer();
            l.Handle(str);
            tokens = l.GetTokens();
            StartNode = GetExpr();
        }

        private Node GetExpr()
        {
            switch (GetTokenType())
            {
                case Token.Let:
                    VerifyToken(Token.Variable, GetTokenType(1));
                    VerifyToken(Token.EqualitySign, GetTokenType(2));
                    LetNode letNode = new LetNode(env, (string)tokens[tokenIndex + 1].Value);
                    ConsumeTokens(3);
                    letNode.VariableValueNode = GetExpr();
                    VerifyToken(Token.In, GetTokenType());
                    ConsumeTokens(1);
                    letNode.In = GetExpr();
                    return letNode;
                case Token.Constant:
                case Token.Variable:
                case Token.OpeningParanthesis:
                    return GetArith();
                default:
                    throw new ASTException("Expected let, constant, variable or opening parentheses, got " + GetTokenType());
            }
        }

        private Node GetArith()
        {
            Node left = GetSubArith();

            if (GetRemainingTokenCount() == 0)
                return left;

            OperatorNode center;

            switch (GetTokenType())
            {
                case Token.Plus:
                    center = new OperatorNode(Operator.Sum);
                    break;
                case Token.Minus:
                    center = new OperatorNode(Operator.Minus);
                    break;
                case Token.In:
                case Token.ClosingParenthesis:
                    return left;
                default:
                    throw new ASTException("Expected plus or minus, in or closing parenthesis, got " + GetTokenType());
            }

            ConsumeTokens(1);
            center.Left = left;
            center.Right = GetArith();
            return center;
        }

        private Node GetSubArith()
        {
            Node left;

            if (GetRemainingTokenCount() == 0)
                throw new ASTException("Expected opening parenthesis, constant or variable, got end of input");

            if (GetRemainingTokenCount() == 1)
            {
                return GetConstOrVariable();
            }

            switch (GetTokenType())
            {
                case Token.OpeningParanthesis:
                    left = GetParenthesesNode();
                    if (GetRemainingTokenCount() == 0)
                        return left;
                    break;
                case Token.Constant:
                case Token.Variable:
                    left = GetConstOrVariable();
                    break;
                default:
                    throw new ASTException("Expected opening parenthesis, constant or variable, got " + GetTokenType());
            }

            OperatorNode center;

            switch (GetTokenType())
            {
                case Token.Plus:
                case Token.Minus:
                    return left;
                case Token.Star:
                    center = new OperatorNode(Operator.Star);
                    break;
                case Token.Slash:
                    center = new OperatorNode(Operator.Slash);
                    break;
                case Token.In:
                case Token.ClosingParenthesis:
                    return left;
                default:
                    throw new ASTException("Expected operator, in or closing parenthesis, got " + GetTokenType());
            }

            ConsumeTokens(1);

            center.Left = left;
            center.Right = GetSubArith();

            return center;
        }

        private Node GetConstOrVariable()
        {
            switch (GetTokenType())
            {
                case Token.Constant:
                    var cNode = new ConstantNode((double)GetValue());
                    ConsumeTokens(1);
                    return cNode;
                case Token.Variable:
                    var varRefNode = new VariableReferenceNode(env, (string)GetValue());
                    ConsumeTokens(1);
                    return varRefNode;
                default:
                    throw new ASTException("Expected constant or variable, got " + GetTokenType());
            }
        }

        private Node GetParenthesesNode()
        {
            VerifyToken(Token.OpeningParanthesis, GetTokenType());
            ConsumeTokens(1);
            ParenNode parenNode = new ParenNode();
            Node innerNode = GetExpr();
            VerifyToken(Token.ClosingParenthesis, GetTokenType());
            ConsumeTokens(1);
            parenNode.InnerNode = innerNode;
            return parenNode;
        }

        /// <summary>
        /// Gets the type of the next token.
        /// </summary>
        /// <returns></returns>
        private Token GetTokenType() => tokens[tokenIndex].Token;

        /// <summary>
        /// Peeks the type of an upcoming token.
        /// </summary>
        /// <param name="peekLength">How far to peek the token from the current index.</param>
        private Token GetTokenType(int peekLength) => tokens[tokenIndex + peekLength].Token;

        private object GetValue() => tokens[tokenIndex].Value;

        private object GetValue(int peekLength) => tokens[tokenIndex + peekLength].Value;

        private void ConsumeTokens(int count) => tokenIndex += count;

        private int GetRemainingTokenCount() => tokens.Count - tokenIndex;

        private void VerifyToken(Token expected, Token actual)
        {
            if (expected != actual)
                throw new ASTException($"expected {expected}, got {actual}");
        }
    }

    class Node
    {
        public Node() { }

        public virtual double GetValue() { throw new NotImplementedException(); }
    }

    class LetNode : Node
    {
        public LetNode(Env env, string variableName)
        {
            this.env = env;
            VariableName = variableName;
        }

        public string VariableName { get; }

        public Node VariableValueNode { get; set; }
        public Node In { get; set; }

        private Env env;

        public override double GetValue()
        {
            env.SetValue(VariableName, VariableValueNode.GetValue());
            double value = In.GetValue();
            env.ReleaseValue(VariableName);
            return value;
        }
    }

    class ConstantNode : Node
    {
        public ConstantNode(double value)
        {
            Value = value;
        }

        private double Value { get; }

        public override double GetValue() => Value;
    }

    class ParenNode : Node
    {
        public Node InnerNode { get; set; }

        public override double GetValue() => InnerNode.GetValue();
    }

    class VariableReferenceNode : Node
    {
        public VariableReferenceNode(Env env, string variableName)
        {
            this.env = env;
            VariableName = variableName;
        }

        private Env env;
        private string VariableName { get; }

        public override double GetValue() => env.GetValue(VariableName);
    }

    class OperatorNode : Node
    {
        public OperatorNode(Operator op)
        {
            this.op = op;
        }

        private Operator op;

        public Node Left { get; set; }
        public Node Right { get; set; }

        public override double GetValue()
        {
            switch (op)
            {
                case Operator.Sum:
                    return Left.GetValue() + Right.GetValue();
                case Operator.Minus:
                    return Left.GetValue() - Right.GetValue();
                case Operator.Star:
                    return Left.GetValue() * Right.GetValue();
                case Operator.Slash:
                    return Left.GetValue() / Right.GetValue();
                default:
                    throw new ASTException("Unknown operator type");
            }
        }
    }
}
