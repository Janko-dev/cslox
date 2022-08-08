namespace cslox.AST
{
    internal class CallExpr : Expr
    {
        public Expr callee;
        public Token paren;
        public List<Expr> args;
        public CallExpr(Expr callee, Token paren, List<Expr> args)
        {
            this.callee = callee;
            this.paren = paren;
            this.args = args;
        }
        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitCallExpr(this);
        }
    }
}
