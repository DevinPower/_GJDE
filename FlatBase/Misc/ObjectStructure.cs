using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

using System.ComponentModel;
using System.Collections.Specialized;

namespace FlatBase
{
    public class ArrayStructure : INotifyPropertyChanged
    {
        ObjectStructure parent;
        public event PropertyChangedEventHandler PropertyChanged;

        ObservableCollection<ObjectStructure> refVals;
        public string pureName { get; set; }

        public ObservableCollection<ObjectStructure> REFS
        {
            get
            {
                return refVals;
            }
            set
            {
                refVals = value;
            }
        }

        public ArrayStructure(string defVal, ObjectStructure mb)
        {
            refVals = new ObservableCollection<ObjectStructure>();
            refVals.CollectionChanged += ContentCollectionChanged;
            pureName = defVal;
            parent = mb;
            Console.WriteLine(parent.Name + " as parent");
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("REFS"));
                PropertyChanged(this, new PropertyChangedEventArgs("refnames"));
            }
        }

        public void add()
        {
            //ObjectStructure os = new ObjectStructure();
            Console.WriteLine("b new" + parent.Name);

            REFS.Add(MainWindow.loadManifest(pureName, parent));
        }
    }

    [Serializable]
    public class ObjectHeader
    {
        public string Name { get; set; }

        public ObjectHeader(string name)
        {
            Name = name;

        }
    }

    public class intArray
    {
        public int[] values { get; set; }

        public intArray(int length)
        {
            values = new int[length];
            for (int i = 0; i < length; i++)
                values[i] = 25;
        }
    }

    [Serializable]
    public class OReference : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public OReference()
        {
        }

        public int refDB { get; set; }

        public void setRA(int v)
        {
            refDB = v;
        }

        public int getRA()
        {
            return refDB;
        }

        int refVal;
        public int REF
        {
            get
            {
                return refVal;
            }
            set
            {
                refVal = value;
                PropertyChanged(this, new PropertyChangedEventArgs("REF"));
            }
        }
    }

    [Serializable]
    public class OReferenceList : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public OReferenceList()
        {
            refVals = new ObservableCollection<int>();
            refVals.CollectionChanged += ContentCollectionChanged;
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("REFS"));
                PropertyChanged(this, new PropertyChangedEventArgs("refnames"));
            }
        }

        public int refDB { get; set; }

        public void setRA(int v)
        {
            refDB = v;
        }

        public int getRA()
        {
            return refDB;
        }

        ObservableCollection<int> refVals;
        public ObservableCollection<int> REFS
        {
            get
            {
                return refVals;
            }
            set
            {
                refVals = value;
            }
        }
    }

    [Serializable]
    public class ObjectStructure : INotifyPropertyChanged
    {
        public Dictionary<string, Misc.weightpool> weightpools = new Dictionary<string, Misc.weightpool>();
        public event PropertyChangedEventHandler PropertyChanged;

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("FIELDS"));
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        public bool excludeExport { get; set; }

        int nameOverride = -1;
        string _handledName;
        public void setOverride(int v)
        {
            nameOverride = v;
        }

        int indexedValue = -1;
        public ObjectStructure getIndexed(int iv)
        {
            indexedValue = iv;
            return this;
        }

        public bool meetsFilter(ObservableCollection<string> terms)
        {
            foreach (string s in terms)
            {
                if (Name.ToLower().Contains(s))
                    return true;

                foreach (object f in FIELDS)
                {
                    if (f is TagField)
                    {
                        if ((f as TagField).value.Contains(s))
                            return true;
                    }
                }
            }
            return false;
        }

        public string Name
        {
            get
            {
                if (nameOverride != -1)
                    return (string)FIELDS[nameOverride];
                else
                    return _handledName;
            }
            set
            {
                if (nameOverride != -1)
                    FIELDS[nameOverride] = value;
                else
                    _handledName = value;
            }
        }
        ObservableCollection<object> fields { get; set; }
        public List<String> fieldexportnames { get; set; }

        public ObservableCollection<object> FIELDS
        {
            get
            {
                return fields;
            }
            set
            {
                fields = value;
            }
        }

        public ObjectStructure()
        {
            fieldexportnames = new List<string>();
            FIELDS = new ObservableCollection<object>();
            Name = "New Entry";
            fields.CollectionChanged += ContentCollectionChanged;
        }

        public void setObserver(NotifyCollectionChangedEventHandler o)
        {
            fields.CollectionChanged += o;
        }

        public override string ToString()
        {
            return Name;
        }

        public ObjectStructure Copy(ObjectStructure pureObject)
        {
            var deserializeSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            Console.WriteLine(JsonConvert.SerializeObject(this, deserializeSettings));
            ObjectStructure OS = JsonConvert.DeserializeObject<ObjectStructure>(
                JsonConvert.SerializeObject(this), deserializeSettings);

            for (int i = 0; i < OS.fields.Count; i++)
            {
                object o = OS.fields[i];
                if (o is Newtonsoft.Json.Linq.JObject)
                {
                    JsonSerializer serializer = new JsonSerializer();
                    object RO = serializer.Deserialize(new JTokenReader(o as JToken), pureObject.FIELDS[i].GetType());
                    pureObject.FIELDS[i] = RO;
                }
                else
                {
                    pureObject.FIELDS[i] = o;
                }
            }
            return pureObject;
        }

        public string save()
        {
            string json = "{";
            int c = 0;
            foreach (object o in fields)
            {
                if (o is ObjectHeader)
                {
                    c++;
                    continue;
                }

                json += "\"" + fieldexportnames[c] + "\":";

                if (!(o is int))
                    json += "\"";

                json += o.ToString();
                if (!(o is int))
                    json += "\"";

                c++;

                if (c < fields.Count)
                    json += ",";
            }

            json += "}";

            Console.WriteLine(c.ToString() + " fields exported");

            return json;
        }
    }
}
