namespace cslox.AST
{
    internal class BinaryExpr : Expr
    {
        public Expr left;
        public Token op;
        public Expr right;
        public BinaryExpr(Expr left, Token op, Expr right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }
        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }
}
