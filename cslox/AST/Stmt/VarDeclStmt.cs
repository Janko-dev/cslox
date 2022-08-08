namespace cslox.AST
{
    internal class VarDeclStmt : Stmt
    {
        public Token name;
        public Expr initializer;
        public VarDeclStmt(Token name, Expr initializer)
        {
            this.name = name;
            this.initializer = initializer;
        }
        public override R Accept<R>(StmtVisitor<R> visitor)
        {
            return visitor.VisitVarDeclStmt(this);
        }
    }
}
