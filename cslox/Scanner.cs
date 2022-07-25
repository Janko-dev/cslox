using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    internal class Scanner
    {
        private string source;
        private List<Token> tokens;
        private int start = 0;
        private int current = 0;
        private int line = 1;

        private static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>() {
            { "and", TokenType.AND },
            { "or",  TokenType.OR },
            { "print", TokenType.PRINT },
            { "if", TokenType.IF },
            { "else", TokenType.ELSE },
            { "true", TokenType.TRUE },
            { "false", TokenType.FALSE },
            { "nil", TokenType.NIL },
            { "for", TokenType.FOR },
            { "while", TokenType.WHILE },
            { "fun", TokenType.FUN },
            { "return", TokenType.RETURN },
            { "class", TokenType.CLASS },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "var", TokenType.VAR }
        };

        public Scanner(string source) { 
            this.source = source;
            tokens = new List<Token>();
        }

        public List<Token> scanTokens() {
            while (!atEnd()) {
                start = current;
                scanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        private void scanToken()
        {
            char c = advance();
            switch (c) { 
                case '(': addToken(TokenType.LEFT_PAREN); break;
                case ')': addToken(TokenType.RIGHT_PAREN); break;
                case '{': addToken(TokenType.LEFT_BRACE); break;
                case '}': addToken(TokenType.RIGHT_BRACE); break;
                case ',': addToken(TokenType.COMMA); break;
                case '.': addToken(TokenType.DOT); break;
                case '-': addToken(TokenType.MINUS); break;
                case '+': addToken(TokenType.PLUS); break;
                case ';': addToken(TokenType.SEMICOLON); break;
                case '*': addToken(TokenType.STAR); break;
                case '!': addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '=': addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '<': addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '>': addToken(match('=') ? TokenType.GREATER_EQUAL: TokenType.GREATER); break;
                case '/': {
                        if (match('/'))
                        {
                            // this is a comment lexeme
                            for (; peek() != '\n' && !atEnd(); advance()) ;
                        }
                        else if (match('*')) {
                            // this is a multiline comment lexeme
                            for (; peek() != '*' && peekNext() != '/' && !atEnd(); advance()) ;
                            advance();
                            advance();
                        } else addToken(TokenType.SLASH);

                    }; break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n': line++; break;

                case '"': addString(); break;

                default:
                    {
                        if (isDigit(c))
                        {
                            addNumber();
                        }
                        else if (isAlpha(c)) {
                            addIdentifier();
                        }
                        else
                        {
                            Cslox.error(line, "Unexpected character.");
                        }
                    }; break;
            }
        }

        private bool isDigit(char c) { 
            return c >= '0' && c <= '9';
        }

        private bool isAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                    c == '_';
        }

        private bool isAlphaNumeric(char c)
        {
            return isDigit(c) || isAlpha(c);
        }

        private bool match(char expected)
        {
            if (atEnd()) return false;
            if (source.ElementAt(current) != expected) return false;

            current++;
            return true;
        }

        private char peek() { 
            if (atEnd()) return '\0';
            return source.ElementAt(current);
        }

        private char peekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source.ElementAt(current + 1);
        }

        private void addToken(TokenType t)
        {
            addToken(t, null);
        }

        private void addToken(TokenType t, object? literal)
        {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(t, text, literal, line));
        }

        private void addString() {
            while (peek() != '"' && !atEnd()) { 
                if (peek() == '\n') line++;
                advance();
            }

            if (atEnd()) {
                Cslox.error(line, "Unterminated string.");
                return;
            }

            advance();

            string value = source.Substring(start+1, current - start - 2);
            addToken(TokenType.STRING, value);
        }

        private void addNumber() {
            while (isDigit(peek())) advance();

            if (peek() == '.' && isDigit(peekNext())) {
                advance();
                while (isDigit(peek())) advance();
            }

            addToken(TokenType.NUMBER, Double.Parse(source.Substring(start, current - start), System.Globalization.CultureInfo.InvariantCulture));
        }

        private void addIdentifier()
        {
            while (isAlphaNumeric(peek())) advance();
            string text = source.Substring(start, current - start);
            TokenType type;
            keywords.TryGetValue(text, out type);

            addToken(type);
        }

        private char advance()
        {
            return source.ElementAt(current++);
        }

        private bool atEnd() {
            return current >= source.Length;
        }
    }
}
