namespace cslox.AST.AST_functions
{
    internal class CsloxInstance
    {

        private CsloxClass csloxClass;
        private Dictionary<string, object> fields = new Dictionary<string, object>();

        public CsloxInstance(CsloxClass csloxClass)
        {
            this.csloxClass = csloxClass;
        }

        public override string ToString()
        {
            return csloxClass.name + " instance";
        }

        public object get(Token name)
        {
            if (fields.ContainsKey(name.lexeme)) { 
                return fields[name.lexeme];
            }

            CsloxFunction method = csloxClass.findMethod(name.lexeme);
            if (method != null) return method.bind(this);

            throw new RunTimeError(name, $"Undefined property '{name.lexeme}'");
        }

        internal void set(Token name, object value)
        {
            if (fields.ContainsKey(name.lexeme)) fields[name.lexeme] = value;
            else fields.Add(name.lexeme, value);
        }
    }
}