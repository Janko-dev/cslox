using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox.AST.AST_functions
{
    internal class AstPrinter : Visitor<string>
    {

        public string print(Expr expression)
        {
            return expression.Accept(this);
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

        public string VisitBinaryExpr(Binary expr)
        {
            return parenthesize(expr.op.lexeme, new[] { expr.left, expr.right });
        }

        public string VisitGroupingExpr(Grouping expr)
        {
            return parenthesize("group", new[] { expr.expression });
        }

        public string VisitLiteralExpr(Literal expr)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return expr.value == null ? "nil" : expr.value.ToString();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public string VisitUnaryExpr(Unary expr)
        {
            return parenthesize(expr.op.lexeme, new[] { expr.right });
        }

        public string VisitTernaryExpr(Ternary expr)
        {
            return parenthesize("ternary", new[] { expr.cond, expr.trueBranch, expr.falseBranch });
        }
    }
}
