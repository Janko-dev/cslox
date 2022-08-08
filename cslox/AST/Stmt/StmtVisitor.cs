namespace cslox.AST
{
    internal interface StmtVisitor<R>
    {
        R VisitExpressionStmt(ExpressionStmt stmt);
        R VisitFunctionStmt(FunctionStmt stmt);
        R VisitBlockStmt(BlockStmt stmt);
        R VisitIfStmt(IfStmt stmt);
        R VisitWhileStmt(WhileStmt stmt);
        R VisitPrintStmt(PrintStmt stmt);
        R VisitReturnStmt(ReturnStmt stmt);
        R VisitVarDeclStmt(VarDeclStmt stmt);
        R VisitClassStmt(ClassStmt stmt);
    }
}
