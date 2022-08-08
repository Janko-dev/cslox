using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    internal class Env
    {
        public Env enclosing;
        private Dictionary<string, object> values = new Dictionary<string, object>();

        public Env() { enclosing = null; }

        public Env(Env enclosing)
        {
            this.enclosing = enclosing;
        }

        public void define(string name, object value) {
            if (values.ContainsKey(name))
            {
                values[name] = value;
            }
            else values.Add(name, value);
        }

        public object get(Token name) {
            if (values.ContainsKey(name.lexeme)) { 
                return values[name.lexeme];
            }

            if (enclosing != null) return enclosing.get(name);

            throw new RunTimeError(name, $"Undefined variable '{name.lexeme}'");
        }

        public object getAt(int dist, string name) {
            Env e = ancestor(dist);
            if (e.values.ContainsKey(name)) {
                return e.values[name];
            }
            return null;
        }

        public Env ancestor(int dist) {
            Env e = this;
            for (int i = 0; i < dist; i++) {
                e = e.enclosing;
            }
            return e;
        }

        public void assign(Token name, object value) {
            if (values.ContainsKey(name.lexeme))
            {
                values[name.lexeme] = value;
                return;
            }

            if (enclosing != null) { 
                enclosing.assign(name, value);
                return;
            }

            throw new RunTimeError(name, $"Undefined variable '{name.lexeme}'");
        }

        internal void assignsAt(int dist, Token name, object value)
        {
            //Console.WriteLine($"dist: {dist}  name: {name.lexeme}");
            Env e = ancestor(dist);
            if (e.values.ContainsKey(name.lexeme)) {
                e.values[name.lexeme] = value;
            }
        }
    }
}
