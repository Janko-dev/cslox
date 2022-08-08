namespace ASTGenerator
{
    internal class Program
    {
        private static string basepath = @"C:\Users\janba\source\repos\cslox\cslox\AST\";

        private static List<string> exprRules = new List<string>()
        {
            "AssignExpr   : Token name, Expr value",
            "BinaryExpr   : Expr left, Token op, Expr right",
            "CallExpr     : Expr callee, Token paren, List<Expr> args",
            "GetExpr      : Expr obj, Token name",
            "SetExpr      : Expr obj, Token name, Expr value",
            "ThisExpr     : Token keyword",
            "SuperExpr    : Token keyword, Token method",
            "TernaryExpr  : Expr cond, Expr trueBranch, Expr falseBranch",
            "GroupingExpr : Expr expression",
            "LiteralExpr  : object? value",
            "UnaryExpr    : Token op, Expr right",
            "VarExpr      : Token name",
            "LogicalExpr  : Expr left, Token op, Expr right",
        };

        private static List<string> stmtRules = new List<string>()
        {
            "ExpressionStmt : Expr expression",
            "FunctionStmt   : Token name, List<Token> pars, List<Stmt> body",
            "BlockStmt      : List<Stmt> statements",
            "IfStmt         : Expr cond, Stmt trueBranch, Stmt falseBranch",
            "WhileStmt      : Expr cond, Stmt body",
            "PrintStmt      : Expr expression",
            "ReturnStmt     : Token keyword, Expr value",
            "VarDeclStmt    : Token name, Expr initializer",
            "ClassStmt      : Token name, VarExpr superclass, List<FunctionStmt> methods"
        };

        static void Main(string[] args)
        {

            defineAST("Expr", exprRules, @"Expr");
            defineAST("Stmt", stmtRules, @"Stmt");

            defineVisitorInterface("Expr", exprRules, @"Expr");
            defineVisitorInterface("Stmt", stmtRules, @"Stmt");
        }

        private static void defineVisitorInterface(string extends, List<string> types, string path)
        {

            using (StreamWriter writer = new StreamWriter(Path.Combine(basepath, path, $"{extends}Visitor.cs")))
            {
                writer.WriteLine("namespace cslox.AST");
                writer.WriteLine("{");
                writer.WriteLine($"    internal interface {extends}Visitor<R>");
                writer.WriteLine("    {");
                foreach (string type in types) {
                    string name = type.Split(":")[0].Trim();
                    writer.WriteLine($"        R Visit{name}({name} {extends.ToLower()});");
                }
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }
        }

        private static void defineAST(string extends, List<string> types, string path)
        {
            if (!Directory.Exists(Path.Combine(basepath, path))){ 
                Directory.CreateDirectory(Path.Combine(basepath, path));
            }
            using (StreamWriter writer = new StreamWriter(Path.Combine(basepath, path, extends + ".cs")))
            {
                writer.WriteLine("namespace cslox.AST");
                writer.WriteLine("{");
                writer.WriteLine($"    internal abstract class {extends}");
                writer.WriteLine("    {");
                writer.WriteLine($"        public abstract R Accept<R>({extends}Visitor<R> visitor);");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            foreach (string entry in types)
            {
                string className = entry.Split(":")[0].Trim();
                string fieldString = entry.Split(":")[1].Trim();
                using (StreamWriter writer = new StreamWriter(Path.Combine(basepath, path, className + ".cs")))
                {
                    writer.WriteLine("namespace cslox.AST");
                    writer.WriteLine("{");
                    writer.WriteLine($"    internal class {className} : {extends}");
                    writer.WriteLine("    {");
                    string[] fields = fieldString.Split(", ");
                    foreach (string field in fields)
                    {
                        writer.WriteLine($"        public {field};");
                    }
                    writer.WriteLine($"        public {className}({fieldString})");
                    writer.WriteLine("        {");
                    foreach (string field in fields)
                    {
                        string name = field.Split(" ")[1];
                        writer.WriteLine($"            this.{name} = {name};");
                    }
                    writer.WriteLine("        }");

                    writer.WriteLine($"        public override R Accept<R>({extends}Visitor<R> visitor)");
                    writer.WriteLine("        {");
                    writer.WriteLine($"            return visitor.Visit{className}(this);");
                    writer.WriteLine("        }");
                    writer.WriteLine("    }");
                    writer.WriteLine("}");
                }
            }
        }
    }
}