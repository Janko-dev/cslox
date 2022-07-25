namespace cslox.AST
{
    internal class Binary : Expr
    {
        public Expr left;
        public Token op;
        public Expr right;
        public Binary(Expr left, Token op, Expr right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }
        public override R Accept<R>(Visitor<R> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }
}
