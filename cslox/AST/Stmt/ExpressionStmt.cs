namespace cslox.AST
{
    internal class ExpressionStmt : Stmt
    {
        public Expr expression;
        public ExpressionStmt(Expr expression)
        {
            this.expression = expression;
        }
        public override R Accept<R>(StmtVisitor<R> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }
}
