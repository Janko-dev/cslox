using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    enum TokenType {
        // literals
        IDENTIFIER, STRING, NUMBER,

        // single-character tokens
        LEFT_PAREN, RIGHT_PAREN,
        LEFT_BRACE, RIGHT_BRACE,
        COMMA, DOT, MINUS, PLUS,
        SEMICOLON, SLASH, STAR,
        
        // one or two character tokens
        BANG, BANG_EQUAL, EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL, LESS, LESS_EQUAL,

        // keywords
        AND, OR, PRINT, IF, ELSE, TRUE, FALSE, NIL,
        FOR, WHILE, FUN, RETURN, CLASS, SUPER, THIS, VAR,
        
        EOF
    }
}
