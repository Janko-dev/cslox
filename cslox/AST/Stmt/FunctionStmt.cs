namespace cslox.AST
{
    internal class FunctionStmt : Stmt
    {
        public Token name;
        public List<Token> pars;
        public List<Stmt> body;
        public FunctionStmt(Token name, List<Token> pars, List<Stmt> body)
        {
            this.name = name;
            this.pars = pars;
            this.body = body;
        }
        public override R Accept<R>(StmtVisitor<R> visitor)
        {
            return visitor.VisitFunctionStmt(this);
        }
    }
}
