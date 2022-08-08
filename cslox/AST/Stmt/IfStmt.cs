namespace cslox.AST
{
    internal class IfStmt : Stmt
    {
        public Expr cond;
        public Stmt trueBranch;
        public Stmt falseBranch;
        public IfStmt(Expr cond, Stmt trueBranch, Stmt falseBranch)
        {
            this.cond = cond;
            this.trueBranch = trueBranch;
            this.falseBranch = falseBranch;
        }
        public override R Accept<R>(StmtVisitor<R> visitor)
        {
            return visitor.VisitIfStmt(this);
        }
    }
}
