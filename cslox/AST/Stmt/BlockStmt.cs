namespace cslox.AST
{
    internal class BlockStmt : Stmt
    {
        public List<Stmt> statements;
        public BlockStmt(List<Stmt> statements)
        {
            this.statements = statements;
        }
        public override R Accept<R>(StmtVisitor<R> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }
    }
}
