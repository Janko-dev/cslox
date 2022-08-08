using cslox.AST;
using cslox.AST.AST_functions;
using System.Text;

namespace cslox
{
    internal class Cslox
    {

        private static Interpreter interpreter = new Interpreter();
        private static bool hadError = false;
        private static bool hadRunTimeError = false;

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
            if (hadRunTimeError) Environment.Exit(70);
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

            //foreach (Token tok in tokens) {
            //    Console.WriteLine(tok.toString());
            //}
            
            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.parse();
            if (hadError) return;

            Resolver resolver = new Resolver(interpreter);
            resolver.resolve(statements);
            if (hadError) return;

            //new AstPrinter().print(expr);
            interpreter.interpret(statements);
        }

        public static void runTimeError(RunTimeError err)
        {
            Console.WriteLine($"{err.Message}\n[Line {err.token.line}]");
            hadRunTimeError = true;
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