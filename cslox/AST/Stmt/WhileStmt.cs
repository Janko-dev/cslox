namespace cslox.AST
{
    internal class WhileStmt : Stmt
    {
        public Expr cond;
        public Stmt body;
        public WhileStmt(Expr cond, Stmt body)
        {
            this.cond = cond;
            this.body = body;
        }
        public override R Accept<R>(StmtVisitor<R> visitor)
        {
            return visitor.VisitWhileStmt(this);
        }
    }
}
