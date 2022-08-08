namespace cslox.AST
{
    internal class VarExpr : Expr
    {
        public Token name;
        public VarExpr(Token name)
        {
            this.name = name;
        }
        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitVarExpr(this);
        }
    }
}
