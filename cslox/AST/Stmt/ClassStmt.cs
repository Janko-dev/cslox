namespace cslox.AST
{
    internal class ClassStmt : Stmt
    {
        public Token name;
        public VarExpr superclass;
        public List<FunctionStmt> methods;
        public ClassStmt(Token name, VarExpr superclass, List<FunctionStmt> methods)
        {
            this.name = name;
            this.superclass = superclass;
            this.methods = methods;
        }
        public override R Accept<R>(StmtVisitor<R> visitor)
        {
            return visitor.VisitClassStmt(this);
        }
    }
}
