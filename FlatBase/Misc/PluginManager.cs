using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FlatBase.Misc
{
    public class Plugin
    {
        public interface exporter
        {
            string export(FlatBase.GJDB database);
        }

    }

    public class PluginManager
    {
        public string path;

        public PluginManager(string p)
        {
            path = p;
        }

        public string run(GJDB database)
        {
            // You must supply a valid fully qualified assembly name.
            Assembly SampleAssembly = Assembly.LoadFile(path);
            // Display all the types contained in the specified assembly.
            try
            {
                foreach (Type oType in SampleAssembly.GetTypes())
                {
                    Plugin exporter = (Plugin)Activator.CreateInstance(oType);
                    
                    foreach (MethodInfo m in oType.GetMethods())
                    {
                        Console.WriteLine("AT: " + m.Name);
                        object[] paramsPass = new object[1];
                        paramsPass[0] = database;
                        if (m.Name == "exportDatabase")
                        {
                            string result = (string)m.Invoke(exporter, paramsPass);
                            Console.WriteLine("received " + result + " from library");
                            return result;
                        }
                    }
                }
            }
            catch(ReflectionTypeLoadException e)
            {
                Console.WriteLine("PLUGIN EXCEPTION-> " + e.Message);
                foreach(Exception se in e.LoaderExceptions)
                {
                    Console.WriteLine(se.Message);
                }
                Console.WriteLine("---------------");
            }

            return "error with exporting plugin";
        }
    }
}
