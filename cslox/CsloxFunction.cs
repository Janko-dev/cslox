using cslox.AST.AST_functions;
using cslox.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    internal class CsloxFunction : CsloxCallable
    {
        private FunctionStmt decl;
        private Env closure;
        private bool isinitializer;

        public CsloxFunction(FunctionStmt decl, Env closure, bool isinitializer)
        {
            this.decl = decl;
            this.closure = closure;
            this.isinitializer = isinitializer;
        }

        public int arity()
        {
            return decl.pars.Count;
        }

        public object call(Interpreter interpreter, List<object> arguments)
        {
            Env env = new Env(closure);
            for (int i = 0; i < decl.pars.Count; i++) {
                env.define(decl.pars[i].lexeme, arguments[i]);
            }
            try
            {
                interpreter.executeBlock(decl.body, env);
            }
            catch (Return retVal) {
                if (isinitializer) return closure.getAt(0, "this");
                return retVal.value;
            }

            if (isinitializer) return closure.getAt(0, "this");
            return null;
        }

        internal CsloxFunction bind(CsloxInstance csloxInstance)
        {
            Env env = new Env(closure);
            env.define("this", csloxInstance);
            return new CsloxFunction(decl, env, isinitializer);
        }

        public override string ToString()
        {
            return $"<fn {decl.name.lexeme} >";
        }
    }
}
