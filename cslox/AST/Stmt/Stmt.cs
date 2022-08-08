namespace cslox.AST
{
    internal abstract class Stmt
    {
        public abstract R Accept<R>(StmtVisitor<R> visitor);
    }
}
