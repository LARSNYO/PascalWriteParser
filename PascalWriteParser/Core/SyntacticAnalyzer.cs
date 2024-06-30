using PascalWriteParser.Core;
using System;
using System.Collections.Generic;


namespace PascalWriteParser
{
    public class SyntaxNode
    {
        public string Name { get; }
        public List<SyntaxNode> Children { get; }

        public SyntaxNode(string name)
        {
            Name = name;
            Children = new List<SyntaxNode>();
        }

        public void AddChild(SyntaxNode child)
        {
            Children.Add(child);
        }

        public override string ToString()
        {
            return Name;
        }
    }
    public class SyntacticAnalyzer
    {
        private readonly LexicalAnalyzer _lexer;
        private Token _currentToken;

        public SyntacticAnalyzer(LexicalAnalyzer lexer)
        {
            _lexer = lexer;
            _currentToken = _lexer.GetNextToken();
        }

        public SyntaxNode Parse()
        {
            return S();
        }

        private SyntaxNode S()
        {
            var node = new SyntaxNode("S");
            node.AddChild(WriteStatement());
            Match(TokenType.Semicolon);
            return node;
        }

        private SyntaxNode WriteStatement()
        {
            var node = new SyntaxNode("WriteStatement");
            Match(TokenType.Write);
            node.AddChild(new SyntaxNode("write"));
            Match(TokenType.OpenParen);
            node.AddChild(new SyntaxNode("("));
            node.AddChild(ArgList());
            Match(TokenType.CloseParen);
            node.AddChild(new SyntaxNode(")"));
            return node;
        }

        private SyntaxNode ArgList()
        {
            var node = new SyntaxNode("ArgList");
            node.AddChild(Arg());
            while (_currentToken.Type == TokenType.Comma)
            {
                Match(TokenType.Comma);
                node.AddChild(new SyntaxNode(","));
                node.AddChild(Arg());
            }
            return node;
        }

        private SyntaxNode Arg()
        {
            var node = new SyntaxNode("Arg");
            if (_currentToken.Type == TokenType.String)
            {
                node.AddChild(new SyntaxNode(_currentToken.Value));
                Match(TokenType.String);
            }
            else if (_currentToken.Type == TokenType.Number)
            {
                node.AddChild(new SyntaxNode(_currentToken.Value));
                Match(TokenType.Number);
            }
            else
            {
                throw new Exception($"Unexpected token: {_currentToken}");
            }
            return node;
        }

        private void Match(TokenType expectedType)
        {
            if (_currentToken.Type == expectedType)
            {
                _currentToken = _lexer.GetNextToken();
            }
            else
            {
                throw new Exception($"Expected token {expectedType} but got {_currentToken.Type}");
            }
        }
    }
}
