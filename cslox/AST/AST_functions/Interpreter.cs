namespace cslox.AST.AST_functions
{

    internal class ClockFun : CsloxCallable
    {
        public int arity()
        {
            return 0;
        }

        public object call(Interpreter interpreter, List<object> arguments)
        {
            return DateTime.Now.Ticks;
        }
    }

    class Interpreter : ExprVisitor<object>, StmtVisitor<object>
    {
        public static Env globals = new Env(); 
        private Env env = globals;
        private Dictionary<Expr, int> locals = new Dictionary<Expr, int>();

        public Interpreter() {
            globals.define("clock", new ClockFun());
        }

        public void interpret(List<Stmt> statements) {
            try
            {
                foreach (Stmt stmt in statements)
                    execute(stmt);
            }
            catch (RunTimeError error) {
                Cslox.runTimeError(error);
            }
        }

        public void resolve(Expr expr, int depth)
        {
            locals.Add(expr, depth);
        }

        private void execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private object evaluate(Expr expr) {
            return expr.Accept(this);
        }

        private object lookUpVar(Token name, Expr expr) {
            int dist;
            if (locals.ContainsKey(expr))
            {   
                dist = locals[expr];
                return env.getAt(dist, name.lexeme);
            }
            else {
                return globals.get(name);
            }
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

        private void checkNumberOperand(Token op, object operand) {
            if (operand.GetType() == typeof(double)) return;
            throw new RunTimeError(op, "Operand must be a number");
        }

        private void checkNumberOperands(Token op, object left, object right)
        {
            if (left?.GetType() == typeof(double) && right?.GetType() == typeof(double)) return;
            throw new RunTimeError(op, "Operands must be a number");
        }

        public object VisitBinaryExpr(BinaryExpr expr)
        {
            object left = evaluate(expr.left);
            object right = evaluate(expr.right);

            switch (expr.op.type) {
                case TokenType.GREATER:
                    checkNumberOperands(expr.op, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    checkNumberOperands(expr.op, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS: 
                    checkNumberOperands(expr.op, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL: 
                    checkNumberOperands(expr.op, left, right);
                    return (double)left <= (double)right;
                case TokenType.EQUAL_EQUAL:return isEqual(left, right);
                case TokenType.BANG_EQUAL: return !isEqual(left, right);
                case TokenType.MINUS: 
                    checkNumberOperands(expr.op, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS: { 
                        if (left.GetType() == typeof(double) && right.GetType() == typeof(double))
                            return (double)left + (double)right;
                        if (left.GetType() == typeof(string) && right.GetType() == typeof(string))
                            return (string)left + (string)right;
                        throw new RunTimeError(expr.op, "Operands must be two strings or two numbers");
                    };
                case TokenType.SLASH: 
                    checkNumberOperands(expr.op, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR: 
                    checkNumberOperands(expr.op, left, right);
                    return (double)left * (double)right;
            }

            return null;
        }

        public object VisitGroupingExpr(GroupingExpr expr)
        {
            return evaluate(expr.expression);
        }

        public object VisitLiteralExpr(LiteralExpr expr)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return expr.value;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public object VisitTernaryExpr(TernaryExpr expr)
        {
            object cond = evaluate(expr.cond);
            if (cond.GetType() != typeof(bool)) throw new RunTimeError(null, "Conditional part of ternary operator must be a boolean");
            if ((bool)cond) return evaluate(expr.trueBranch);
            else return evaluate(expr.falseBranch);
        }

        public object VisitUnaryExpr(UnaryExpr expr)
        {
            object right = evaluate(expr.right);
            switch (expr.op.type) {
                case TokenType.MINUS:
                    checkNumberOperand(expr.op, right);
                    return -(double)right;
                case TokenType.BANG:  return !isTruthy(right);
            }

#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }


        public object VisitVarExpr(VarExpr expr)
        {
            return lookUpVar(expr.name, expr);
        }

        public object VisitAssignExpr(AssignExpr expr)
        {
            object value = evaluate(expr.value);

            int dist;
            if (locals.TryGetValue(expr, out dist))
            {
                env.assignsAt(dist, expr.name, value);
            }
            else
            {
                globals.assign(expr.name, value);
            }
            
            return value;
        }

        public object VisitLogicalExpr(LogicalExpr expr)
        {
            object left = evaluate(expr.left);
            if (expr.op.type == TokenType.OR)
            {
                if (isTruthy(left)) return left;
            }
            else {
                if (!isTruthy(left)) return left;
            }
            return evaluate(expr.right);
        }

        public object VisitCallExpr(CallExpr expr)
        {
            object callee = evaluate(expr.callee);
            List<object> args = new List<object>();
            foreach (Expr arg in expr.args) { 
                args.Add(evaluate(arg));
            }
            if (callee is not CsloxCallable) {
                throw new RunTimeError(expr.paren, "Can only call functions and classes");
            }
            CsloxCallable function = (CsloxCallable)callee;
            if (args.Count != function.arity()) {
                throw new RunTimeError(expr.paren, $"Expected {function.arity()} arguments but got {args.Count} arguments");
            }
            return function.call(this, args);
        }

        public object VisitExpressionStmt(ExpressionStmt stmt)
        {
            evaluate(stmt.expression);
            return null;
        }

        public object VisitPrintStmt(PrintStmt stmt)
        {
            object value = evaluate(stmt.expression);
            if (value == null) Console.WriteLine("nil");
            else Console.WriteLine(value);
            return null;
        }

        public object VisitVarDeclStmt(VarDeclStmt stmt)
        {
            object value = null;
            if (stmt.initializer != null) { 
                value = evaluate(stmt.initializer);
            }
            env.define(stmt.name.lexeme, value);
            return null;
        }


        public object VisitBlockStmt(BlockStmt stmt)
        {
            executeBlock(stmt.statements, new Env(env));
            return null;
        }

        public void executeBlock(List<Stmt> statements, Env env) {
            Env previous = this.env;
            try
            {
                this.env = env;
                foreach (Stmt stmt in statements)
                {
                    execute(stmt);
                }
            }
            finally
            {
                this.env = previous;
            }
        }

        public object VisitIfStmt(IfStmt stmt)
        {
            if (isTruthy(evaluate(stmt.cond))){
                execute(stmt.trueBranch);
            } else if (stmt.falseBranch != null) {
                execute(stmt.falseBranch);
            }
            return null;
        }

        public object VisitWhileStmt(WhileStmt stmt)
        {
            while (isTruthy(evaluate(stmt.cond))){
                execute(stmt.body);
            }
            return null;
        }

        public object VisitFunctionStmt(FunctionStmt stmt)
        {
            CsloxFunction fn = new CsloxFunction(stmt, env, false);
            env.define(stmt.name.lexeme, fn);
            return null;
        }

        public object VisitReturnStmt(ReturnStmt stmt)
        {
            object value = null;
            if (stmt.value != null) value = evaluate(stmt.value);
            throw new Return(value);
        }

        public object VisitClassStmt(ClassStmt stmt)
        {
            object superclass = null;
            if (stmt.superclass != null) { 
                superclass = evaluate(stmt.superclass);
                if (superclass.GetType() != typeof(CsloxClass)) {
                    throw new RunTimeError(stmt.superclass.name, "Superclass must be a class");
                }
            }
            env.define(stmt.name.lexeme, null);
            if (stmt.superclass != null) {
                env = new Env(env);
                env.define("super", superclass);
            }
            Dictionary<string, CsloxFunction> methods = new Dictionary<string, CsloxFunction>();
            foreach (FunctionStmt method in stmt.methods) {
                CsloxFunction func = new CsloxFunction(method, env, method.name.lexeme.Equals("init"));
                methods.Add(method.name.lexeme, func);
            }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            CsloxClass csClass = new CsloxClass(stmt.name.lexeme, (CsloxClass)superclass, methods);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            if (superclass != null) env = env.enclosing;
            env.assign(stmt.name, csClass);
            return null;
        }

        public object VisitGetExpr(GetExpr expr) {
            object obj = evaluate(expr.obj);
            if (obj.GetType() == typeof(CsloxInstance)) {
                return ((CsloxInstance)obj).get(expr.name);
            }

            throw new RunTimeError(expr.name, "Only instances have properties");
        }

        public object VisitSetExpr(SetExpr expr)
        {
            object obj = evaluate(expr.obj);
            if (obj.GetType() != typeof(CsloxInstance)) {
                throw new RunTimeError(expr.name, "Only instances have fields");
            }
            object value = evaluate(expr.value);
            ((CsloxInstance)obj).set(expr.name, value);
            return value;
        }

        public object VisitThisExpr(ThisExpr expr)
        {
            return lookUpVar(expr.keyword, expr);
        }

        public object VisitSuperExpr(SuperExpr expr)
        {
            if (locals.ContainsKey(expr)) { 
                int dist = locals[expr];
                CsloxClass superclass = (CsloxClass)env.getAt(dist, "super");
                CsloxInstance obj = (CsloxInstance)env.getAt(dist - 1, "this");
                CsloxFunction method = superclass.findMethod(expr.method.lexeme);
                if (method == null) {
                    throw new RunTimeError(expr.method, $"Undefined property '{expr.method.lexeme}'");
                }
                return method.bind(obj);
            }
            throw new RunTimeError(expr.method, $"Undefined property '{expr.method.lexeme}'");
        }
    }
}
