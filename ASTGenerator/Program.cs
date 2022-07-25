namespace ASTGenerator
{
    internal class Program
    {
        private static string basepath = @"C:\Users\janba\source\repos\cslox\cslox\AST\";

        private static List<string> rules = new List<string>()
        {
            "Binary   : Expr left, Token op, Expr right",
            "Ternary  : Expr cond, Expr trueBranch, Expr falseBranch",
            "Grouping : Expr expression",
            "Literal  : object? value",
            "Unary    : Token op, Expr right",
        };

        static void Main(string[] args)
        {

            defineAST("Expr", rules);

            defineVisitorInterface("Expr", rules);
        }

        private static void defineVisitorInterface(string extends, List<string> types)
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(basepath, "Visitor.cs")))
            {
                writer.WriteLine("namespace cslox.AST");
                writer.WriteLine("{");
                writer.WriteLine("    internal interface Visitor<R>");
                writer.WriteLine("    {");
                foreach (string type in types) {
                    string name = type.Split(":")[0].Trim();
                    writer.WriteLine($"        R Visit{name}{extends}({name} {extends.ToLower()});");
                }
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }
        }

        private static void defineAST(string extends, List<string> types)
        {

            using (StreamWriter writer = new StreamWriter(Path.Combine(basepath, extends + ".cs")))
            {
                writer.WriteLine("namespace cslox.AST");
                writer.WriteLine("{");
                writer.WriteLine($"    internal abstract class {extends}");
                writer.WriteLine("    {");
                writer.WriteLine("        public abstract R Accept<R>(Visitor<R> visitor);");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            foreach (string entry in types)
            {
                string className = entry.Split(":")[0].Trim();
                string fieldString = entry.Split(":")[1].Trim();
                using (StreamWriter writer = new StreamWriter(Path.Combine(basepath, className + ".cs")))
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

                    writer.WriteLine($"        public override R Accept<R>(Visitor<R> visitor)");
                    writer.WriteLine("        {");
                    writer.WriteLine($"            return visitor.Visit{className}{extends}(this);");
                    writer.WriteLine("        }");
                    writer.WriteLine("    }");
                    writer.WriteLine("}");
                }
            }
        }
    }
}