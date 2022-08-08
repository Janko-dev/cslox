namespace cslox.AST
{
    internal abstract class Expr
    {
        public abstract R Accept<R>(ExprVisitor<R> visitor);
    }
}
