using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox.AST.AST_functions
{


    internal class Resolver : ExprVisitor<object>, StmtVisitor<object>
    {
        private enum FunctionType
        {
            NONE,
            FUNCTION,
            METHOD,
            INITIALIZER
        }
        private enum ClassType
        {
            NONE,
            CLASS,
            SUBCLASS
        }

        private Interpreter interpreter;
        private Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
        private FunctionType currentFunction = FunctionType.NONE;
        private ClassType currentClass = ClassType.NONE;

        public Resolver(Interpreter interpreter) { 
            this.interpreter = interpreter;
        }

        public void resolve(Expr expr)
        {
            expr.Accept(this);
        }

        public void resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        public void resolve(List<Stmt> statements)
        {
            foreach (Stmt stmt in statements)
            {
                resolve(stmt);
            }
        }

        private void beginScope() {
            scopes.Push(new Dictionary<string, bool>());
        }

        private void endScope()
        {
            scopes.Pop();
        }

        private void declare(Token name) {
            if (scopes.Count == 0) return;
            Dictionary<string, bool> scope = scopes.Peek();
            if (scope.ContainsKey(name.lexeme)) {
                Cslox.error(name, "Already a variable with this name in this scope");
            }
            scope.TryAdd(name.lexeme, false);
        }

        private void define(Token name)
        {
            if (scopes.Count == 0) return;
            scopes.Peek()[name.lexeme] = true;
        }

        private void resolveLocal(Expr expr, Token name) {
            for (int i = scopes.Count - 1; i >= 0; i--) {
                if (scopes.ElementAt(i).ContainsKey(name.lexeme)) {
                    interpreter.resolve(expr, i);
                    return;
                }
            }
        }

        private void resolveFunction(FunctionStmt func, FunctionType type) {
            FunctionType enclosingFunction = currentFunction;
            currentFunction = type;
            
            beginScope();
            foreach(Token param in func.pars){
                declare(param);
                define(param);
            }
            resolve(func.body);
            endScope();
            currentFunction = enclosingFunction;
        }

        public object VisitAssignExpr(AssignExpr expr)
        {
            resolve(expr.value);
            resolveLocal(expr, expr.name);
            return null;
        }

        public object VisitBinaryExpr(BinaryExpr expr)
        {
            resolve(expr.left);
            resolve(expr.right);
            return null;
        }

        public object VisitBlockStmt(BlockStmt stmt)
        {
            beginScope();
            resolve(stmt.statements);
            endScope();
            return null;
        }

        public object VisitCallExpr(CallExpr expr)
        {
            resolve(expr.callee);
            foreach (Expr arg in expr.args) {
                resolve(arg);
            }
            return null;
        }

        public object VisitExpressionStmt(ExpressionStmt stmt)
        {
            resolve(stmt.expression);
            return null;
        }

        public object VisitFunctionStmt(FunctionStmt stmt)
        {
            declare(stmt.name);
            define(stmt.name);
            resolveFunction(stmt, FunctionType.FUNCTION);
            return null;
        }

        public object VisitGroupingExpr(GroupingExpr expr)
        {
            resolve(expr.expression);
            return null;
        }

        public object VisitIfStmt(IfStmt stmt)
        {
            resolve(stmt.cond);
            resolve(stmt.trueBranch);
            if (stmt.falseBranch != null) resolve(stmt.falseBranch);
            return null;
        }

        public object VisitLiteralExpr(LiteralExpr expr)
        {
            return null;
        }

        public object VisitLogicalExpr(LogicalExpr expr)
        {
            resolve(expr.left);
            resolve(expr.right);
            return null;
        }

        public object VisitPrintStmt(PrintStmt stmt)
        {
            resolve(stmt.expression);
            return null;
        }

        public object VisitReturnStmt(ReturnStmt stmt)
        {
            if (currentFunction == FunctionType.NONE) {
                Cslox.error(stmt.keyword, "Can't return from top-level code");
            }
            if (stmt.value != null) {
                if (currentFunction == FunctionType.INITIALIZER) {
                    Cslox.error(stmt.keyword, "Can't return a value from an initializer");
                }
                resolve(stmt.value);
            }
            return null;
        }

        public object VisitTernaryExpr(TernaryExpr expr)
        {
            resolve(expr.cond);
            resolve(expr.trueBranch);
            resolve(expr.falseBranch);
            return null;
        }

        public object VisitUnaryExpr(UnaryExpr expr)
        {
            resolve(expr.right);
            return null;
        }

        public object VisitVarDeclStmt(VarDeclStmt stmt)
        {
            declare(stmt.name);
            if (stmt.initializer != null) {
                resolve(stmt.initializer);
            }
            define(stmt.name);
            return null;
        }

        public object VisitVarExpr(VarExpr expr)
        {
            bool exists;
            if (scopes.Count != 0 && scopes.Peek().TryGetValue(expr.name.lexeme, out exists)) {
                if (!exists) Cslox.error(expr.name, "can't read local variable in its own initializer.");
            }
            resolveLocal(expr, expr.name);
            return null;
        }

        public object VisitWhileStmt(WhileStmt stmt)
        {
            resolve(stmt.cond);
            resolve(stmt.body);
            return null;
        }

        public object VisitClassStmt(ClassStmt stmt)
        {
            ClassType enclosingClass = currentClass;
            currentClass = ClassType.CLASS;
            declare(stmt.name);
            define(stmt.name);
            if (stmt.superclass != null && stmt.name.lexeme.Equals(stmt.superclass.name.lexeme)) {
                Cslox.error(stmt.superclass.name, "A class can't inherit from itself");
            }
            
            if (stmt.superclass != null)
            {
                currentClass = ClassType.SUBCLASS;
                resolve(stmt.superclass);
            }

            if (stmt.superclass != null) {
                beginScope();
                scopes.Peek().TryAdd("super", true);
            }
            beginScope();
            scopes.Peek().TryAdd("this", true);
            foreach (FunctionStmt func in stmt.methods) {
                FunctionType declaration = FunctionType.METHOD;
                if (func.name.lexeme.Equals("init")) declaration = FunctionType.INITIALIZER;
                resolveFunction(func, declaration);
            }
            endScope();
            if (stmt.superclass != null) endScope();
            currentClass = enclosingClass;

            return null;
        }

        public object VisitGetExpr(GetExpr expr)
        {
            resolve(expr.obj);
            return null;
        }

        public object VisitSetExpr(SetExpr expr)
        {
            resolve(expr.value);
            resolve(expr.obj);
            return null;
        }

        public object VisitThisExpr(ThisExpr expr)
        {
            if (currentClass == ClassType.NONE) {
                Cslox.error(expr.keyword, "Can't use 'this' outside of a class");
                return null;
            }
            resolveLocal(expr, expr.keyword);
            return null;
        }

        public object VisitSuperExpr(SuperExpr expr)
        {
            if (currentClass == ClassType.NONE)
            {
                Cslox.error(expr.keyword, "can't use 'super' outside of a class");
            }
            else if (currentClass != ClassType.SUBCLASS) {
                Cslox.error(expr.keyword, "can't use 'super' in a class with no superclass");
            }
            resolveLocal(expr, expr.keyword);
            return null;
        }
    }
}
