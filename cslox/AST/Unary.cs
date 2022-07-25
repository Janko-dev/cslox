namespace cslox.AST
{
    internal class Unary : Expr
    {
        public Token op;
        public Expr right;
        public Unary(Token op, Expr right)
        {
            this.op = op;
            this.right = right;
        }
        public override R Accept<R>(Visitor<R> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }
}
