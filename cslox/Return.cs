using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    internal class Return : Exception
    {
        public object value;
        public Return(object value) 
        { 
            this.value = value;
        }
    }
}
