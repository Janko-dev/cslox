using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox.AST.AST_functions
{
    internal class Interpreter : Visitor<object>
    {

        private object evaluate(Expr expr) {
            return expr.Accept(this);
        }

        private bool isTruthy(object obj) {
            if (obj == null) return false;
            if (obj.GetType() == typeof(bool)) return (bool)obj;
            return true;
        }

        private bool isEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;
            return a.Equals(b);
        }

        public object VisitBinaryExpr(Binary expr)
        {
            object left = evaluate(expr.left);
            object right = evaluate(expr.right);

            switch (expr.op.type) {
                case TokenType.GREATER: return (double)left > (double)right;
                case TokenType.GREATER_EQUAL: return (double)left >= (double)right;
                case TokenType.LESS: return (double)left < (double)right;
                case TokenType.LESS_EQUAL: return (double)left <= (double)right;
                case TokenType.EQUAL_EQUAL: return isEqual(left, right);
                case TokenType.BANG_EQUAL: return !isEqual(left, right);
                case TokenType.MINUS: return (double)left - (double)right;
                case TokenType.PLUS: { 
                        if (left.GetType() == typeof(double) && right.GetType() == typeof(double))
                            return (double)left + (double)right;
                        if (left.GetType() == typeof(string) && right.GetType() == typeof(string))
                            return (string)left + (string)right;
                    }; break;
                case TokenType.SLASH: return (double)left / (double)right;
                case TokenType.STAR: return (double)left * (double)right;
            }

            return null;
        }

        public object VisitGroupingExpr(Grouping expr)
        {
            return evaluate(expr.expression);
        }

        public object VisitLiteralExpr(Literal expr)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return expr.value;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public object VisitTernaryExpr(Ternary expr)
        {
            throw new NotImplementedException();
        }

        public object VisitUnaryExpr(Unary expr)
        {
            object right = evaluate(expr.right);
            switch (expr.op.type) {
                case TokenType.MINUS: return -(double)right;
                case TokenType.BANG:  return !isTruthy(right);
            }

#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
