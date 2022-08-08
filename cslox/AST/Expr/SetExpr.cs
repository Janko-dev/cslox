namespace cslox.AST
{
    internal class SetExpr : Expr
    {
        public Expr obj;
        public Token name;
        public Expr value;
        public SetExpr(Expr obj, Token name, Expr value)
        {
            this.obj = obj;
            this.name = name;
            this.value = value;
        }
        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitSetExpr(this);
        }
    }
}
