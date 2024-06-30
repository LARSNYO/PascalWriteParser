namespace PascalWriteParser.Core;


using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;




public enum TokenType
{
    Write,
    OpenParen,
    CloseParen,
    Comma,
    Semicolon,
    String,
    Number,
    EOF,
    Unknown
}


public class Token
{
    public TokenType Type { get; }
    public string Value { get; }


    public Token(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }


    public override string ToString()
    {
        return $"{Type}: {Value}";
    }
}


public class LexicalAnalyzer
{
    private readonly string _input;
    private int _position;


    private static readonly Dictionary<string, TokenType> _keywords = new()
       {
           { "write", TokenType.Write }
       };


    public LexicalAnalyzer(string input)
    {
        _input = input;
        _position = 0;
    }


    public Token GetNextToken()
    {
        SkipWhitespace();


        if (_position >= _input.Length)
        {
            return new Token(TokenType.EOF, string.Empty);
        }


        char currentChar = _input[_position];


        if (currentChar == '(')
        {
            _position++;
            return new Token(TokenType.OpenParen, "(");
        }


        if (currentChar == ')')
        {
            _position++;
            return new Token(TokenType.CloseParen, ")");
        }


        if (currentChar == ',')
        {
            _position++;
            return new Token(TokenType.Comma, ",");
        }


        if (currentChar == ';')
        {
            _position++;
            return new Token(TokenType.Semicolon, ";");
        }


        if (char.IsDigit(currentChar))
        {
            return Number();
        }


        if (currentChar == '\'')
        {
            return String();
        }


        if (char.IsLetter(currentChar))
        {
            return Identifier();
        }


        _position++;
        return new Token(TokenType.Unknown, currentChar.ToString());
    }


    private void SkipWhitespace()
    {
        while (_position < _input.Length && char.IsWhiteSpace(_input[_position]))
        {
            _position++;
        }
    }


    private Token Number()
    {
        int start = _position;
        while (_position < _input.Length && (char.IsDigit(_input[_position]) || _input[_position] == '.'))
        {
            _position++;
        }
        return new Token(TokenType.Number, _input[start.._position]);
    }


    private Token String()
    {
        int start = _position++;
        while (_position < _input.Length && _input[_position] != '\'')
        {
            _position++;
        }


        _position++; // Skip closing quote
        return new Token(TokenType.String, _input[start.._position]);
    }


    private Token Identifier()
    {
        int start = _position;
        while (_position < _input.Length && char.IsLetterOrDigit(_input[_position]))
        {
            _position++;
        }


        string value = _input[start.._position];
        if (_keywords.ContainsKey(value))
        {
            return new Token(_keywords[value], value);
        }


        return new Token(TokenType.Unknown, value);
    }
}
