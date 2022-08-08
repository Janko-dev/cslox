namespace cslox.AST
{
    internal class SuperExpr : Expr
    {
        public Token keyword;
        public Token method;
        public SuperExpr(Token keyword, Token method)
        {
            this.keyword = keyword;
            this.method = method;
        }
        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitSuperExpr(this);
        }
    }
}
