namespace cslox.AST
{
    internal class LogicalExpr : Expr
    {
        public Expr left;
        public Token op;
        public Expr right;
        public LogicalExpr(Expr left, Token op, Expr right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }
        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitLogicalExpr(this);
        }
    }
}
