namespace cslox.AST
{
    internal class PrintStmt : Stmt
    {
        public Expr expression;
        public PrintStmt(Expr expression)
        {
            this.expression = expression;
        }
        public override R Accept<R>(StmtVisitor<R> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }
}
