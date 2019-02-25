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

        /// <summary>
        /// The start node of the generated syntax tree.
        /// </summary>
        public Node StartNode { get; private set; }

        private List<TokenInfo> tokens;
        private int tokenIndex;
        private Env env;

        public void Parse(string str)
        {
            env.Clear();
            tokenIndex = 0;
            var lexer = new Lexer();
            lexer.Handle(str);
            tokens = lexer.GetTokens();
            StartNode = GetExpr();
            if (GetRemainingTokenCount() > 0)
                throw new ASTException("Expected end of input, got " + GetTokenType());
        }

        private Node GetExpr()
        {
            switch (GetTokenType())
            {
                case Token.Let:
                    VerifyToken(Token.Variable, GetTokenType(1));
                    VerifyToken(Token.EqualitySign, GetTokenType(2));
                    var letNode = new LetNode(env, (string)GetValue(1));
                    ConsumeTokens(3);
                    letNode.VariableValueNode = GetExpr();
                    VerifyToken(Token.In, GetTokenType());
                    ConsumeTokens(1);
                    letNode.In = GetExpr();
                    return letNode;
                default:
                    return GetTerm();
            }
        }

        private Node GetTerm()
        {
            Node left = GetFactor();

            if (GetRemainingTokenCount() == 0)
                return left;

            OperatorNode operatorNode;

            switch (GetTokenType())
            {
                case Token.Plus:
                    operatorNode = new OperatorNode(Operator.Plus);
                    break;
                case Token.Minus:
                    operatorNode = new OperatorNode(Operator.Minus);
                    break;
                case Token.In:
                case Token.ClosingParenthesis:
                    return left;
                default:
                    throw new ASTException("Expected plus or minus, in or closing parenthesis, got " + GetTokenType());
            }

            ConsumeTokens(1);

            operatorNode.Left = left;
            operatorNode.Right = GetTerm();
            return operatorNode;
        }

        private Node GetFactor()
        {
            Node left = GetUnary();

            if (GetRemainingTokenCount() == 0)
                return left;

            OperatorNode operatorNode;

            switch (GetTokenType())
            {
                case Token.Star:
                    operatorNode = new OperatorNode(Operator.Star);
                    break;
                case Token.Slash:
                    operatorNode = new OperatorNode(Operator.Slash);
                    break;
                default:
                    return left;
            }

            ConsumeTokens(1);

            operatorNode.Left = left;
            operatorNode.Right = GetFactor();
            return operatorNode;
        }

        private Node GetUnary(bool reverse = false)
        {
            switch (GetTokenType())
            {
                case Token.Plus:
                    ConsumeTokens(1);
                    return GetUnary();
                case Token.Minus:
                    ConsumeTokens(1);
                    return GetUnary(true);
                default:
                    PrimaryNode node = GetPrimary();
                    node.Reversed = reverse;
                    return node;
            }
        }

        private PrimaryNode GetPrimary()
        {
            switch (GetTokenType())
            {
                case Token.Constant:
                    var constantNode = new ConstantNode((double)GetValue());
                    ConsumeTokens(1);
                    return constantNode;
                case Token.Variable:
                    var varRefNode = new VariableReferenceNode(env, (string)GetValue());
                    ConsumeTokens(1);
                    return varRefNode;
                case Token.OpeningParanthesis:
                    var parenNode = new ParenNode();
                    ConsumeTokens(1);
                    parenNode.InnerNode = GetExpr();
                    VerifyToken(Token.ClosingParenthesis, GetTokenType());
                    ConsumeTokens(1);
                    return parenNode;
                default:
                    throw new ASTException("Expected constant, variable or opening parenthesis, got " + GetTokenType());
            }
        }

        /// <summary>
        /// Gets the type of the current or an upcoming token.
        /// </summary>
        /// <param name="peekLength">How far to peek the token from the current index.</param>
        private Token GetTokenType(int peekLength = 0) => tokens[tokenIndex + peekLength].Token;

        /// <summary>
        /// Gets the value of the current or an upcoming token.
        /// </summary>
        /// <param name="peekLength">How far to peek the token from the current index.</param>
        private object GetValue(int peekLength = 0) => tokens[tokenIndex + peekLength].Value;

        /// <summary>
        /// Consumes a given number of tokens.
        /// </summary>
        /// <param name="count">How many tokens to consume.</param>
        private void ConsumeTokens(int count) => tokenIndex += count;

        /// <summary>
        /// Calculates and returns the number of tokens that haven't been handled by the parser yet.
        /// </summary>
        private int GetRemainingTokenCount() => tokens.Count - tokenIndex;

        /// <summary>
        /// Verifies that a token matches the expected token.
        /// Throws an <see cref="ASTException"/> if the tokens don't match.
        /// </summary>
        /// <param name="expected">The expected token.</param>
        /// <param name="actual">The token to compare to the expected token.</param>
        private void VerifyToken(Token expected, Token actual)
        {
            if (expected != actual)
                throw new ASTException($"Expected {expected}, got {actual}");
        }
    }


    /******************************/
    /** Syntax Tree Node Classes **/
    /******************************/

    abstract class Node
    {
        public Node() { }

        public abstract double GetValue();
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

    abstract class PrimaryNode : Node
    {
        public bool Reversed { get; set; }
    }

    class ParenNode : PrimaryNode
    {
        public Node InnerNode { get; set; }

        public override double GetValue() => Reversed ? -InnerNode.GetValue() : InnerNode.GetValue();
    }

    class ConstantNode : PrimaryNode
    {
        public ConstantNode(double value)
        {
            Value = value;
        }

        private double Value { get; }

        public override double GetValue() => Reversed ? -Value : Value;
    }

    class VariableReferenceNode : PrimaryNode
    {
        public VariableReferenceNode(Env env, string variableName)
        {
            this.env = env;
            VariableName = variableName;
        }

        private Env env;
        private string VariableName { get; }

        public override double GetValue() => Reversed ? -env.GetValue(VariableName) : env.GetValue(VariableName);
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
                case Operator.Plus:
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
