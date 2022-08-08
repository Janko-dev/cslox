using System.Text;

namespace cslox.AST.AST_functions
{
    internal class AstPrinter : ExprVisitor<string>, StmtVisitor<string>
    {

        public void print(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt stmt in statements)
                    Console.WriteLine(stmt.Accept(this));
            }
            catch (RunTimeError error)
            {
                Cslox.runTimeError(error);
            }
        }

        private string parenthesize(string name, Expr[] exprs)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(").Append(name);
            foreach (Expr e in exprs)
            {
                sb.Append(" ");
                sb.Append(e.Accept(this));
            }
            sb.Append(")");
            return sb.ToString();
        }

        public string VisitBinaryExpr(BinaryExpr expr)
        {
            return parenthesize(expr.op.lexeme, new[] { expr.left, expr.right });
        }

        public string VisitGroupingExpr(GroupingExpr expr)
        {
            return parenthesize("group", new[] { expr.expression });
        }

        public string VisitLiteralExpr(LiteralExpr expr)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return expr.value == null ? "nil" : expr.value.ToString();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public string VisitUnaryExpr(UnaryExpr expr)
        {
            return parenthesize(expr.op.lexeme, new[] { expr.right });
        }

        public string VisitTernaryExpr(TernaryExpr expr)
        {
            return parenthesize("ternary", new[] { expr.cond, expr.trueBranch, expr.falseBranch });
        }

        public string VisitVarExpr(VarExpr expr)
        {
            return $"var {expr.name.lexeme}";
        }

        public string VisitAssignExpr(AssignExpr expr)
        {
            return parenthesize($"{expr.name.lexeme} =", new[] { expr.value });
        }

        public string VisitExpressionStmt(ExpressionStmt stmt)
        {
            return parenthesize($"ExprStmt", new[] { stmt.expression });
        }

        public string VisitPrintStmt(PrintStmt stmt)
        {
            return parenthesize($"PrintStmt", new[] { stmt.expression });
        }

        public string VisitVarDeclStmt(VarDeclStmt stmt)
        {
            return parenthesize($"VarDeclStmt {stmt.name.lexeme}", new[] { stmt.initializer });
        }

        public string VisitBlockStmt(BlockStmt stmt)
        {
            throw new NotImplementedException();
        }

        public string VisitIfStmt(IfStmt stmt)
        {
            throw new NotImplementedException();
        }

        public string VisitLogicalExpr(LogicalExpr expr)
        {
            throw new NotImplementedException();
        }

        public string VisitWhileStmt(WhileStmt stmt)
        {
            throw new NotImplementedException();
        }

        public string VisitCallExpr(CallExpr expr)
        {
            throw new NotImplementedException();
        }

        public string VisitFunctionStmt(FunctionStmt stmt)
        {
            throw new NotImplementedException();
        }

        public string VisitReturnStmt(ReturnStmt stmt)
        {
            throw new NotImplementedException();
        }

        public string VisitClassStmt(ClassStmt stmt)
        {
            throw new NotImplementedException();
        }

        public string VisitGetExpr(GetExpr expr)
        {
            throw new NotImplementedException();
        }

        public string VisitSetExpr(SetExpr expr)
        {
            throw new NotImplementedException();
        }

        public string VisitThisExpr(ThisExpr expr)
        {
            throw new NotImplementedException();
        }

        public string VisitSuperExpr(SuperExpr expr)
        {
            throw new NotImplementedException();
        }
    }
}
