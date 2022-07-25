using cslox.AST;
using System.Text;

namespace cslox
{
    internal class Cslox
    {

        private static bool hadError = false;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1) runFile(args[0]);
            else runPrompt();

            //// pretty printer test
            //Expr expression = new Binary(
            //    new Unary(
            //        new Token(TokenType.MINUS, "-", null, 1),
            //        new Literal(123)),
            //    new Token(TokenType.STAR, "*", null, 1),
            //    new Grouping(
            //        new Literal(45.231))
            //);

            //Console.WriteLine(new AstPrinter().print(expression));

            //// Reverse polish notation printer test
            //Expr expression2 = new Binary(
            //    new Binary(
            //        new Literal(1),
            //        new Token(TokenType.PLUS, "+", null, 1),
            //        new Literal(2)),
            //    new Token(TokenType.STAR, "*", null, 1),
            //    new Binary(
            //        new Literal(4),
            //        new Token(TokenType.MINUS, "-", null, 1),
            //        new Literal(3)));

            //Console.WriteLine(new RPNPrinter().print(expression2));
        }

        private static void runFile(string path) {
            byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
            Console.WriteLine($"Path: {path}");
            run(Encoding.Default.GetString(bytes));
            if (hadError) Environment.Exit(65);
        }

        private static void runPrompt()
        {
            Console.WriteLine("Welcome to the REPL (Read, Evaluate, Print, Loop) environment");
            while (true) {
                Console.Write("> ");
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                string line = Console.ReadLine();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (line == null) break;
                run(line);
                hadError = false;
            }
        }

        private static void run(string source) {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.scanTokens();

            foreach (Token tok in tokens) {
                Console.WriteLine(tok.toString());
            }
            
            Parser parser = new Parser(tokens);
            Expr expr = parser.expression();
            Console.WriteLine(new AstPrinter().print(expr));
        }

        public static void error(int line, string message) {
            report(line, "", message);
        }

        private static void report(int line, string where, string message) {
            Console.WriteLine($"[Line {line}] Error {where}: {message}");
            hadError = true;
        }
    }
}