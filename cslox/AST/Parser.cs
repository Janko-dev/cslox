using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox.AST
{
    internal class Parser
    {
        private class ParseError : Exception { }

        private List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Expr? parse() {
            try
            {
                return expression();
            }
            catch (ParseError)
            {
                return null;
            }
        }

        private Expr? expression()
        {
            Expr? expr = equality();
            while (match(new[] { TokenType.QMARK }))
            {
                Expr? trueBranch = expression();
                consume(TokenType.COLON, $"Expected ':' but got '{peek().lexeme}'");
                Expr? falseBranch = expression();
                expr = new Ternary(expr, trueBranch, falseBranch);
            }
            return expr;
        }

        //private Expr expression() {
        //    Expr expr = equality();
        //    while (match(new[] { TokenType.COMMA }))
        //    {
        //        Token op = previous();
        //        Expr right = equality();
        //        expr = new Binary(expr, op, right);
        //    }
        //    return expr;
        //}

        private Expr? equality()
        {
            Expr? expr = comparison();
            while (match(new[] { TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL })) {
                Token op = previous();
                Expr? right = comparison();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        private Expr? comparison()
        {
            Expr? expr = term();
            while (match(new[] { TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL }))
            {
                Token op = previous();
                Expr? right = term();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        private Expr? term()
        {
            Expr? expr = factor();
            while (match(new[] { TokenType.MINUS, TokenType.PLUS }))
            {
                Token op = previous();
                Expr? right = factor();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        private Expr? factor()
        {
            Expr? expr = unary();
            while (match(new[] { TokenType.SLASH, TokenType.STAR }))
            {
                Token op = previous();
                Expr? right = unary();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        private Expr? unary() {
            
            if (match(new[] { TokenType.BANG, TokenType.MINUS }))
            {
                Token op = previous();
                Expr? right = unary();
                return new Unary(op, right);
            }
            else 
            {
                return primary();
            }
        }

        private Expr? primary() {
            if (match(new[] { TokenType.FALSE })) return new Literal(false);
            if (match(new[] { TokenType.TRUE })) return new Literal(true);
            if (match(new[] { TokenType.NIL })) return new Literal(null);
            
            if (match(new[] { TokenType.NUMBER, TokenType.STRING }))
            {
                return new Literal(previous().literal);
            }

            if (match(new[] { TokenType.LEFT_PAREN }))
            {
                Expr expr = expression();
                consume(TokenType.RIGHT_PAREN, $"Expected ')' after expression, but got {peek().lexeme}");
                return new Grouping(expr);
            }
            
            error(peek(), $"Unhandled value, expected expression, but got '{peek().lexeme}'");
            return null;
        }

        private void synchronize() {
            advance();
            while (!atEnd()) {
                if (previous().type == TokenType.SEMICOLON) return;

                switch (peek().type) {
                    case TokenType.CLASS:
                    case TokenType.FOR:
                    case TokenType.FUN:
                    case TokenType.IF:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                    case TokenType.VAR:
                    case TokenType.WHILE:
                        return;
                }
                advance();
            }
        }

        private bool match(TokenType[] types) {
            foreach (TokenType t in types)
            {
                if (check(t)) {
                    advance();
                    return true;
                }
            }
            return false;
        }

        private Token consume(TokenType type, string message) {
            if (check(type)) return advance();
            throw error(peek(), message);
        }

        private ParseError error(Token token, string message) {
            Cslox.error(token, message);
            return new ParseError();
        }

        private Token advance() { 
            if (!atEnd()) current++;
            return previous();
        }

        private bool check(TokenType t)
        {
            if (atEnd()) return false;
            return peek().type == t;
        }

        private Token peek() { 
            return tokens[current];
        }

        private bool atEnd() {
            return peek().type == TokenType.EOF;
        }

        private Token previous() {
            return tokens[current - 1];
        }
    }
}
