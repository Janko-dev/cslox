namespace cslox.AST
{
    internal abstract class Expr
    {
        public abstract R Accept<R>(Visitor<R> visitor);
    }
}
