using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginBase
{
    public class Plugin
    {
        public interface exporter
        {
            string export(FlatBase.GJDB database);
        }

    }
}
