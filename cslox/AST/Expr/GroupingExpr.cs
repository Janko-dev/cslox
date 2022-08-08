namespace cslox.AST
{
    internal class GroupingExpr : Expr
    {
        public Expr expression;
        public GroupingExpr(Expr expression)
        {
            this.expression = expression;
        }
        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }
}
