﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox.AST
{
    internal class Parser
    {
        private List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Expr expression() {
            return equality();
        }

        public Expr equality()
        {
            Expr expr = comparison();
            while (match(new[] { TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL })) {
                Token op = previous();
                Expr right = comparison();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        public Expr comparison()
        {
            Expr expr = term();
            while (match(new[] { TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL }))
            {
                Token op = previous();
                Expr right = term();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        public Expr term()
        {
            Expr expr = factor();
            while (match(new[] { TokenType.MINUS, TokenType.PLUS }))
            {
                Token op = previous();
                Expr right = factor();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        public Expr factor()
        {
            Expr expr = unary();
            while (match(new[] { TokenType.SLASH, TokenType.STAR }))
            {
                Token op = previous();
                Expr right = unary();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        public Expr unary() {
            Expr expr;
            if (match(new[] { TokenType.BANG, TokenType.MINUS }))
            {
                Token op = previous();
                Expr right = unary();
                expr = new Unary(op, right);
            }
            else 
            {
                expr = primary();
            }
            return expr;
        }

        public Expr primary() {
            Expr expr;
            if (match(new[] { TokenType.NUMBER, TokenType.STRING, TokenType.TRUE, TokenType.FALSE, TokenType.NIL }))
            {
                string lexeme = previous().lexeme;
                expr = new Literal(lexeme);
            }
            else
            {
                match(new[] { TokenType.LEFT_PAREN });
                expr = new Grouping(expression());
                match(new[] { TokenType.RIGHT_PAREN });
            }

            return expr;
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