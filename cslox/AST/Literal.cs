namespace cslox.AST
{
    internal class Literal : Expr
    {
        public object? value;
        public Literal(object? value)
        {
            this.value = value;
        }
        public override R Accept<R>(Visitor<R> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }
}
