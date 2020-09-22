using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

using System.ComponentModel;
using System.Collections.Specialized;

namespace FlatBase.Misc
{
    class TupleStructure
    {
        public List<ObjectStructure> fields { get; set; }

        public TupleStructure(List<ObjectStructure> f)
        {
            fields = f;
        }
    }
}
