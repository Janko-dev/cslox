using cslox.AST;
using cslox.AST.AST_functions;
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
                string? line = Console.ReadLine();
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
            Expr expr = parser.parse();
            if (hadError) return;
                
            Console.WriteLine(new AstPrinter().print(expr));
        }

        public static void error(int line, string message) {
            report(line, "", message);
        }

        public static void error(Token token, string message)
        {
            if (token.type == TokenType.EOF)
            {
                report(token.line, "at end", message);
            }
            else {
                report(token.line, $"at '{token.lexeme}'", message);
            }
        }

        private static void report(int line, string where, string message) {
            Console.WriteLine($"[Line {line}] Error {where}: {message}");
            hadError = true;
        }
    }
}