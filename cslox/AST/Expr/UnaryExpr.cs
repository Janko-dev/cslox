namespace cslox.AST
{
    internal class UnaryExpr : Expr
    {
        public Token op;
        public Expr right;
        public UnaryExpr(Token op, Expr right)
        {
            this.op = op;
            this.right = right;
        }
        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }
}
