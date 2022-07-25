namespace cslox.AST
{
    internal class Ternary : Expr
    {
        public Expr cond;
        public Expr trueBranch;
        public Expr falseBranch;
        public Ternary(Expr cond, Expr trueBranch, Expr falseBranch)
        {
            this.cond = cond;
            this.trueBranch = trueBranch;
            this.falseBranch = falseBranch;
        }
        public override R Accept<R>(Visitor<R> visitor)
        {
            return visitor.VisitTernaryExpr(this);
        }
    }
}
