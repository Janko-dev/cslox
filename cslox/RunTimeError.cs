using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    internal class RunTimeError : Exception
    {
        public Token token;
        public RunTimeError(Token token, string message)
            : base(message)
        {
            this.token = token;
        }
    }
}
