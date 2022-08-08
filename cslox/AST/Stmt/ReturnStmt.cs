namespace cslox.AST
{
    internal class ReturnStmt : Stmt
    {
        public Token keyword;
        public Expr value;
        public ReturnStmt(Token keyword, Expr value)
        {
            this.keyword = keyword;
            this.value = value;
        }
        public override R Accept<R>(StmtVisitor<R> visitor)
        {
            return visitor.VisitReturnStmt(this);
        }
    }
}
