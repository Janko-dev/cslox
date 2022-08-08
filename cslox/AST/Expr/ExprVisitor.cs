namespace cslox.AST
{
    internal interface ExprVisitor<R>
    {
        R VisitAssignExpr(AssignExpr expr);
        R VisitBinaryExpr(BinaryExpr expr);
        R VisitCallExpr(CallExpr expr);
        R VisitGetExpr(GetExpr expr);
        R VisitSetExpr(SetExpr expr);
        R VisitThisExpr(ThisExpr expr);
        R VisitSuperExpr(SuperExpr expr);
        R VisitTernaryExpr(TernaryExpr expr);
        R VisitGroupingExpr(GroupingExpr expr);
        R VisitLiteralExpr(LiteralExpr expr);
        R VisitUnaryExpr(UnaryExpr expr);
        R VisitVarExpr(VarExpr expr);
        R VisitLogicalExpr(LogicalExpr expr);
    }
}
