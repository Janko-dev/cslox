namespace cslox.AST.AST_functions
{
    internal class CsloxClass : CsloxCallable
    {

        public string name;
        private CsloxClass superclass;
        private Dictionary<string, CsloxFunction> methods;

        public CsloxClass(string name, CsloxClass superclass, Dictionary<string, CsloxFunction> methods)
        {
            this.name = name;
            this.superclass = superclass;
            this.methods = methods;
        }

        public int arity()
        {
            CsloxFunction initializer = findMethod("init");
            if (initializer == null) return 0;
            return initializer.arity();
            
        }

        public object call(Interpreter interpreter, List<object> arguments)
        {
            CsloxInstance instance = new CsloxInstance(this);
            CsloxFunction initializer = findMethod("init");
            if (initializer != null) { 
                initializer.bind(instance).call(interpreter, arguments);
            }
            return instance;
        }

        public override string ToString()
        {
            return name;
        }

        internal CsloxFunction findMethod(string lexeme)
        {
            if (methods.ContainsKey(lexeme)) { 
                return methods[lexeme];
            }
            if (superclass != null) { 
                return superclass.findMethod(lexeme);
            }
            return null;
        }
    }
}