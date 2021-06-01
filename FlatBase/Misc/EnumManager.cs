using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace FlatBase
{
    public class EnumField
    {
        public string reference { get; set; }
        public int selected { get; set; }

        public EnumField(string r)
        {
            reference = r;
            selected = 0;
        }
    }

    public class EnumStructure
    {
        [JsonIgnore]
        public Dictionary<int, string> fields { get; set; }

        public EnumStructure()
        {
            fields = new Dictionary<int, string>();
        }
    }

    public class EnumManager
    {
        public static Dictionary<string, EnumStructure> enums = new Dictionary<string, EnumStructure>();
        public void loadEnum(string name)
        {
            EnumStructure eStruct = new EnumStructure();

            string[] r = File.ReadAllLines("config/Enums/" + name + ".txt");
            foreach(string s in r)
            {
                //TODO: Error catching here
                string[] split = s.Split(' ');
                eStruct.fields.Add(Int32.Parse( split[0] ), split[1]);
            }

            enums.Add(name, eStruct);
        }

        public EnumStructure getEnum(string name)
        {
            /*EnumStructure toRet = new EnumStructure();

            foreach (KeyValuePair<int, string> e in enums[name].fields)
            {
                toRet.fields.Add(e.Key, e.Value);
            }

            return toRet;*/
            return enums[name];

        }

        public void parseAll()
        {
            if (File.Exists("config/enumsManifest.txt"))
            {
                string[] s = File.ReadAllLines("config/enumsManifest.txt");
                for (int i = 0; i < s.Count(); i++)
                {
                    loadEnum(s[i]);
                }
            }
        }
    }
}
