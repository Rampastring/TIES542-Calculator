﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    class Parser
    {
        public Parser()
        {
            env = new Env();
        }

        public Node StartNode;

        private Node currentNode;
        private List<TokenInfo> tokens;
        private int tokenIndex = 0;
        private Env env;

        public void Parse(string str)
        {
            Lexer l = new Lexer();
            l.Handle(str);
            tokens = l.GetTokens();
        }

        public Node GetExpr()
        {
            switch (GetTokenType())
            {
                case Token.Let:
                    VerifyToken(Token.Variable, GetTokenType(1));
                    VerifyToken(Token.EqualitySign, GetTokenType(2));
                    LetNode letNode = new LetNode(currentNode, (string)tokens[tokenIndex + 1].Value);
                    ConsumeTokens(2);
                    //AddNode(letNode);
                    GetExpr();
                    VerifyToken(Token.In, GetTokenType());
                    ConsumeTokens(1);
                    GetExpr();
                    return letNode;
                case Token.Constant:
                    // get factor and term
                    //ConstantNode cNode = new ConstantNode(currentNode, (int)GetValue());
                    //AddNode(cNode);
                    //ConsumeTokens(1);
                    //break;
                case Token.Variable:
                    // get factor and term
                    //VariableReferenceNode varRefNode = new VariableReferenceNode(currentNode, env, (string)GetValue());
                    //AddNode(varRefNode);
                    //ConsumeTokens(1);
                    //break;
                case Token.OpeningParanthesis:
                    throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        private void GetArith()
        {
            Node left = GetSubArith();
            switch (GetTokenType())
            {

            }
        }

        private Node GetSubArith()
        {
            Node left;

            if (GetRemainingTokenCount() == 1)
            {
                return EvalConstOrVariable();
            }

            switch (GetTokenType())
            {
                case Token.OpeningParanthesis:
                    left = GetParenthesesNode();
                    break;
                case Token.Constant:
                case Token.Variable:
                    left = EvalConstOrVariable();
                    break;
            }


            Node center;

            switch (GetTokenType())
            {
                case Token.Plus:
                    center = new OperatorNode(Operator.Sum);
                    break;
                case Token.Minus:
                    center = new OperatorNode(Operator.Minus);
                    break;
                case Token.Star:
                    center = new OperatorNode(Operator.Star);
                    break;
                case Token.Slash:
                    center = new OperatorNode(Operator.Slash);
                    break;
            }

            throw new NotImplementedException();
        }

        private Node EvalConstOrVariable()
        {
            switch (GetTokenType())
            {
                case Token.Constant:
                    var cNode = new ConstantNode(null, (int)GetValue());
                    ConsumeTokens(1);
                    return cNode;
                case Token.Variable:
                    var varRefNode = new VariableReferenceNode(null, env, (string)GetValue());
                    ConsumeTokens(1);
                    return varRefNode;
                default:
                    throw new Exception("Expected constant or variable, got " + GetTokenType());
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

        private void AddNode(Node node)
        {
            if (StartNode == null)
            {
                StartNode = node;
            }

            currentNode = node;

            // TODO handle parents
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
                throw new Exception($"expected {expected}, got {actual}");
        }
    }

    class Node
    {
        public Node() { }

        public Node(Node parentNode)
        {
            ParentNode = parentNode;
        }

        public Node ParentNode { get; }

        public virtual int GetValue() { throw new NotImplementedException(); }
    }

    class LetNode : Node
    {
        public LetNode(Node parentNode, string variableName) : base(parentNode)
        {
            VariableName = variableName;
        }

        public string VariableName { get; }

        public Node VariableValueNode { get; }
        public Node In { get; }
    }

    class ConstantNode : Node
    {
        public ConstantNode(Node parentNode, int value) : base(parentNode)
        {
            Value = value;
        }

        private int Value { get; }

        public override int GetValue() => Value;
    }

    class ParenNode : Node
    {
        public Node InnerNode { get; set; }

        public override int GetValue() => InnerNode.GetValue();
    }

    class VariableReferenceNode : Node
    {
        public VariableReferenceNode(Node parentNode, Env env, string variableName) : base(parentNode)
        {
            this.env = env;
            VariableName = variableName;
        }

        private Env env;
        private string VariableName { get; }

        public override int GetValue() => env.GetValue(VariableName);
    }

    class OperatorNode : Node
    {
        public OperatorNode(Operator op)
        {
            this.op = op;
        }

        private Operator op;

        public Node Left { get; private set; }
        public Node Right { get; private set; }

        public override int GetValue()
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
                    throw new Exception("Unknown operator type");
            }
        }
    }
}
