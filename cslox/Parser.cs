using cslox.AST;

namespace cslox
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

        public List<Stmt> parse() { 
            List<Stmt> statements = new List<Stmt> ();
            while (!atEnd()) { 
                statements.Add(declaration());
            }
            return statements;
        }

        private Stmt declaration() {
            try
            {
                if (match(new[] { TokenType.CLASS })) return classDeclaration();
                if (match(new[] { TokenType.FUN })) return function("function");
                if (match(new[] { TokenType.VAR })) return varDecl();
                return statement();

            }
            catch (ParseError)
            {
                synchronize();
                return null;
            }
        }

        private Stmt classDeclaration() {
            Token name = consume(TokenType.IDENTIFIER, "Expected class name");
            VarExpr superClass = null;
            if (match(new[] { TokenType.LESS })) {
                consume(TokenType.IDENTIFIER, "Expected superclass name");
                superClass = new VarExpr(previous());
            }
            consume(TokenType.LEFT_BRACE, "Expected '{' after class name");
            List<FunctionStmt> methods = new List<FunctionStmt>();
            while (!check(TokenType.RIGHT_BRACE) && !atEnd()) {
                methods.Add((FunctionStmt)function("method"));
            }
            consume(TokenType.RIGHT_BRACE, "Expected '}' To close class declaration");
            return new ClassStmt(name, superClass, methods);
        }

        private Stmt function(string kind) {
            Token name = consume(TokenType.IDENTIFIER, $"Expected {kind} name");
            consume(TokenType.LEFT_PAREN, $"Expected '(' after {kind} name");
            List<Token> pars = new List<Token>();
            if (!check(TokenType.RIGHT_PAREN)) {
                do
                {
                    if (pars.Count >= 255)
                    {
                        error(peek(), "Can't have more than 255 parameters");
                    }
                    pars.Add(consume(TokenType.IDENTIFIER, "Expected parameter name"));
                } while (match(new[] { TokenType.COMMA }));
            }
            consume(TokenType.RIGHT_PAREN, "Expected ')' after parameters");
            consume(TokenType.LEFT_BRACE, $"Expected '{{' before {kind} body");
            List<Stmt> body = block();
            return new FunctionStmt(name, pars, body);
        }

        private Stmt varDecl() {

            Token id = consume(TokenType.IDENTIFIER, $"Expected identifier after 'var'");
            Expr initializer = null;
            if (match(new[] { TokenType.EQUAL }))
            {
                initializer = expression();
            }
            consume(TokenType.SEMICOLON, $"Expected ';' at the end of variable declaration");
            return new VarDeclStmt(id, initializer);
        }

        private Stmt statement() {
            if (match(new[] { TokenType.PRINT }))
            {
                Expr expr = expression();
                consume(TokenType.SEMICOLON, $"Expected ';' at the end of print statement");
                return new PrintStmt(expr);
            }
            else if (match(new[] { TokenType.LEFT_BRACE })) return new BlockStmt(block());
            else if (match(new[] { TokenType.IF })) return ifStatement();
            else if (match(new[] { TokenType.RETURN })) return returnStatement();
            else if (match(new[] { TokenType.FOR })) return forStatement();
            else if (match(new[] { TokenType.WHILE })) return whileStatement();
            else
            {
                Expr expr = expression();
                consume(TokenType.SEMICOLON, $"Expected ';' at the end of expression statement");
                return new ExpressionStmt(expr);
            }
        }

        private Stmt returnStatement() {
            Token keyword = previous();
            Expr value = null;
            if (!check(TokenType.SEMICOLON)) { 
                value = expression();
            }
            consume(TokenType.SEMICOLON, "Expected ';' after return value");
            return new ReturnStmt(keyword, value);
        }

        private Stmt whileStatement()
        {
            consume(TokenType.LEFT_PAREN, "Expected '(' after while keyword");
            Expr cond = expression();
            consume(TokenType.RIGHT_PAREN, "Expected matching ')' after expression");
            Stmt body = statement();
            return new WhileStmt(cond, body);
        }

        private Stmt ifStatement()
        {
            consume(TokenType.LEFT_PAREN, "Expected '(' after if keyword");
            Expr cond = expression();
            consume(TokenType.RIGHT_PAREN, "Expected matching ')' after expression");
            Stmt trueBranch = statement();
            Stmt falseBranch = null;
            if (match(new[] { TokenType.ELSE })) {
                falseBranch = statement();
            }
            return new IfStmt(cond, trueBranch, falseBranch);
        }

        private Stmt forStatement() {
            consume(TokenType.LEFT_PAREN, "Expected '(' after for keyword");
            Stmt init;
            if (match(new[] { TokenType.SEMICOLON })) init = null;
            else if (match(new[] { TokenType.VAR })) init = varDecl();
            else {
                Expr expr = expression();
                consume(TokenType.SEMICOLON, $"Expected ';' at the end of expression statement");
                init = new ExpressionStmt(expr);
            }

            Expr cond = null;
            if (!check(TokenType.SEMICOLON)) {
                cond = expression();
            }
            consume(TokenType.SEMICOLON, "Expected ';' after for statement initializer");

            Expr incr = null;
            if (!check(TokenType.SEMICOLON))
            {
                incr = expression();
            }
            consume(TokenType.RIGHT_PAREN, "Expected matching ')' after for header");

            Stmt body = statement();

            if (incr != null)
            {
                body = new BlockStmt(new List<Stmt>(){
                    body,
                    new ExpressionStmt(incr),
                });
            }
            
            if (cond == null) cond = new LiteralExpr(true);
            body = new WhileStmt(cond, body);
            
            if (init != null)
            {
                body = new BlockStmt(new List<Stmt>(){
                    init,
                    body,
                });
            }
            return body;
        }

        private List<Stmt> block() {
            List<Stmt> statements = new List<Stmt>();
            while (!check(TokenType.RIGHT_BRACE) && !atEnd())
            {
                statements.Add(declaration());
            }
            consume(TokenType.RIGHT_BRACE, "Expected '}' after block statement");
            return statements;
        }

        private Expr expression()
        {
            Expr expr = or();

            if (match(new[] { TokenType.EQUAL }))
            {
                Token equal = previous();
                Expr value = expression();
                if (expr.GetType() == typeof(VarExpr))
                {
                    Token name = ((VarExpr)expr).name;
                    return new AssignExpr(name, value);
                }
                else if (expr.GetType() == typeof(GetExpr)) {
                    GetExpr get = (GetExpr)expr;
                    return new SetExpr(get.obj, get.name, value);
                }

                error(equal, "Invalid assignment target");
            }
            else {
                while (match(new[] { TokenType.QMARK }))
                {
                    Expr trueBranch = expression();
                    consume(TokenType.COLON, $"Expected ':' but got '{peek().lexeme}'");
                    Expr falseBranch = expression();
                    expr = new TernaryExpr(expr, trueBranch, falseBranch);
                }
            }
            return expr;
        }

        private Expr or() {
            Expr expr = and();
            while (match(new[] { TokenType.OR }))
            {
                Token op = previous();
                Expr right = and();
                expr = new LogicalExpr(expr, op, right);
            }
            return expr;
        }

        private Expr and()
        {
            Expr expr = equality();
            while (match(new[] { TokenType.AND }))
            {
                Token op = previous();
                Expr right = equality();
                expr = new LogicalExpr(expr, op, right);
            }
            return expr;
        }

        private Expr equality()
        {
            Expr expr = comparison();
            while (match(new[] { TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL }))
            {
                Token op = previous();
                Expr right = comparison();
                expr = new BinaryExpr(expr, op, right);
            }
            return expr;
        }

        private Expr comparison()
        {
            Expr expr = term();
            while (match(new[] { TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL }))
            {
                Token op = previous();
                Expr right = term();
                expr = new BinaryExpr(expr, op, right);
            }
            return expr;
        }

        private Expr term()
        {
            Expr expr = factor();
            while (match(new[] { TokenType.MINUS, TokenType.PLUS }))
            {
                Token op = previous();
                Expr right = factor();
                expr = new BinaryExpr(expr, op, right);
            }
            return expr;
        }

        private Expr factor()
        {
            Expr expr = unary();
            while (match(new[] { TokenType.SLASH, TokenType.STAR }))
            {
                Token op = previous();
                Expr right = unary();
                expr = new BinaryExpr(expr, op, right);
            }
            return expr;
        }

        private Expr unary()
        {

            if (match(new[] { TokenType.BANG, TokenType.MINUS }))
            {
                Token op = previous();
                Expr right = unary();
                return new UnaryExpr(op, right);
            }
            else
            {
                return call();
            }
        }

        private Expr call() { 
            Expr expr = primary();
            while (true)
            {
                if (match(new[] { TokenType.LEFT_PAREN }))
                {
                    expr = finishCall(expr);
                }
                else if (match(new[] { TokenType.DOT })) {
                    Token name = consume(TokenType.IDENTIFIER, "Expected property name after '.'");
                    expr = new GetExpr(expr, name);
                } else
                {
                    break;
                }
            }
            return expr;
        }

        private Expr finishCall(Expr callee) {
            List<Expr> args = new List<Expr>();
            if (!check(TokenType.RIGHT_PAREN)){
                do
                {
                    if (args.Count >= 255) {
                        error(peek(), "Can't have more than 255 arguments");
                    }
                    args.Add(expression());
                } while (match(new[] { TokenType.COMMA }));
            }
            Token paren = consume(TokenType.RIGHT_PAREN, "Expected ')' after arguments");
            return new CallExpr(callee, paren, args);
        }

        private Expr primary()
        {
            if (match(new[] { TokenType.FALSE })) return new LiteralExpr(false);
            if (match(new[] { TokenType.TRUE })) return new LiteralExpr(true);
            if (match(new[] { TokenType.NIL })) return new LiteralExpr(null);
            if (match(new[] { TokenType.THIS })) return new ThisExpr(previous());

            if (match(new[] { TokenType.NUMBER, TokenType.STRING }))
            {
                return new LiteralExpr(previous().literal);
            }

            if (match(new[] { TokenType.SUPER })) {
                Token keyword = previous();
                consume(TokenType.DOT, "Expected '.' after 'super'");
                Token method = consume(TokenType.IDENTIFIER, "Expected superclass method name");
                return new SuperExpr(keyword, method);
            }

            if (match(new[] { TokenType.IDENTIFIER }))
            {
                return new VarExpr(previous());
            }

            if (match(new[] { TokenType.LEFT_PAREN }))
            {
                Expr expr = expression();
                consume(TokenType.RIGHT_PAREN, $"Expected ')' after expression, but got {peek().lexeme}");
                return new GroupingExpr(expr);
            }

            error(peek(), $"Unhandled value, expected expression, but got '{peek().lexeme}'");
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        private void synchronize()
        {
            advance();
            while (!atEnd())
            {
                if (previous().type == TokenType.SEMICOLON) return;

                switch (peek().type)
                {
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

        private bool match(TokenType[] types)
        {
            foreach (TokenType t in types)
            {
                if (check(t))
                {
                    advance();
                    return true;
                }
            }
            return false;
        }

        private Token consume(TokenType type, string message)
        {
            if (check(type)) return advance();
            throw error(peek(), message);
        }

        private ParseError error(Token token, string message)
        {
            Cslox.error(token, message);
            return new ParseError();
        }

        private Token advance()
        {
            if (!atEnd()) current++;
            return previous();
        }

        private bool check(TokenType t)
        {
            if (atEnd()) return false;
            return peek().type == t;
        }

        private Token peek()
        {
            return tokens[current];
        }

        private bool atEnd()
        {
            return peek().type == TokenType.EOF;
        }

        private Token previous()
        {
            return tokens[current - 1];
        }
    }
}
