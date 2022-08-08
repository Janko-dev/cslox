using cslox.AST.AST_functions;

namespace cslox
{
    internal interface CsloxCallable
    {
        int arity();
        object call(Interpreter interpreter, List<object> arguments);
    }
}
