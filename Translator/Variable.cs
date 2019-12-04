using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Translator
{
    class Variable : IEquatable<Variable>
    {

        public string iD { get; set; }        
        public string type { get; set; }
        public string name { get; set; }
        public string value { get; set; }        

        public bool Equals(Variable other)
        {
            throw new NotImplementedException();
        }
    }
}