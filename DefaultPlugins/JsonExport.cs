using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using FlatBase;

namespace DefaultPlugins
{
    public class JsonExport : FlatBase.Misc.Plugin
    {
        public string exportDatabase(FlatBase.GJDB database)
        {
            string json = "{\n";
            int i = 0;

            foreach (ObservableCollection<ObjectStructure> collection in database.data)
            {
                string catName = database.tabNames[i];
                json += "\"" + catName + "\" : [\n";

                int c = 0;
                foreach (ObjectStructure os in collection)
                {
                    json += exportObjectStructure(database, os, catName, c);
                    c++;

                    if (c < collection.Count)
                        json += ",";
                }


                json += "]";

                i++;
                if (i < database.tabNames.Count)
                    json += ",";

            }

            json += "}";

            return json;
        }

        public string exportArray(FlatBase.GJDB database, FlatBase.ArrayStructure AS)
        {
            string toreturn = "";

            foreach (ObjectStructure o in AS.REFS)
            {
                if (o.FIELDS.Count == 1)
                {
                    if (o.FIELDS[0] is int)
                        toreturn += o.FIELDS[0].ToString() + ",";
                    else if (o.FIELDS[0] is OReference)
                    {
                        toreturn += "{" + saveReference(database, (o.FIELDS[0] as OReference)) + "},";
                    }
                    else
                        toreturn += "\"" + o.FIELDS[0].ToString() + "\",";
                }
            }

            return toreturn;
        }

        public string saveReference(FlatBase.GJDB database, FlatBase.OReference oref)
        {
            return "\"$ref\" : \"" + database.tabNames[oref.refDB] + oref.REF.ToString() + "\",";
        }

        public string exportObjectStructure(FlatBase.GJDB database, FlatBase.ObjectStructure os, string catName, int id)
        {
            string json = "{";
            json += "\"$id\" : \"" + catName + id.ToString() + "\",";
            int c = 0;
            foreach (object o in os.FIELDS)
            {
                if (o is ObjectHeader)
                {
                    c++;
                    continue;
                }

                json += "\"" + os.fieldexportnames[c] + "\":";

                if (!(o is int) && !(o is ArrayStructure))
                    json += "\"";

                if (o is OReference)
                {
                    json += (o as OReference).REF;
                }
                else if (o is ArrayStructure)
                {
                    json += "[";
                    ArrayStructure AS = o as ArrayStructure;
                    //json += AS.save(database);
                    json += exportArray(database, AS);
                    /*foreach (ObjectStructure aos in AS.REFS)
                    {
                        //json += aos.save() + ",";
                    }*/
                    json += "]";
                }
                else
                {
                    json += o.ToString();
                }
                if (!(o is int) && !(o is ArrayStructure))
                    json += "\"";

                c++;

                if (c < os.FIELDS.Count)
                    json += ",";
            }

            json += "}";

            Console.WriteLine(c.ToString() + " fields exported");

            return json;
        }
    }
}
