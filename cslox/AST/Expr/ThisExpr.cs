namespace cslox.AST
{
    internal class ThisExpr : Expr
    {
        public Token keyword;
        public ThisExpr(Token keyword)
        {
            this.keyword = keyword;
        }
        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitThisExpr(this);
        }
    }
}
