namespace cslox.AST
{
    internal class GetExpr : Expr
    {
        public Expr obj;
        public Token name;
        public GetExpr(Expr obj, Token name)
        {
            this.obj = obj;
            this.name = name;
        }
        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitGetExpr(this);
        }
    }
}
