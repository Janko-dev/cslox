namespace cslox.AST
{
    internal class AssignExpr : Expr
    {
        public Token name;
        public Expr value;
        public AssignExpr(Token name, Expr value)
        {
            this.name = name;
            this.value = value;
        }
        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitAssignExpr(this);
        }
    }
}
