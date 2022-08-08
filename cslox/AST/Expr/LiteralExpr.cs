namespace cslox.AST
{
    internal class LiteralExpr : Expr
    {
        public object? value;
        public LiteralExpr(object? value)
        {
            this.value = value;
        }
        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }
}
