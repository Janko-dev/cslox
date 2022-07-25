namespace cslox.AST
{
    internal interface Visitor<R>
    {
        R VisitBinaryExpr(Binary expr);
        R VisitTernaryExpr(Ternary expr);
        R VisitGroupingExpr(Grouping expr);
        R VisitLiteralExpr(Literal expr);
        R VisitUnaryExpr(Unary expr);
    }
}
