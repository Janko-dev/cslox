namespace cslox.AST
{
    internal class TernaryExpr : Expr
    {
        public Expr cond;
        public Expr trueBranch;
        public Expr falseBranch;
        public TernaryExpr(Expr cond, Expr trueBranch, Expr falseBranch)
        {
            this.cond = cond;
            this.trueBranch = trueBranch;
            this.falseBranch = falseBranch;
        }
        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitTernaryExpr(this);
        }
    }
}
