using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

using System.ComponentModel;
using System.Collections.Specialized;

namespace FlatBase
{
    public class ArrayStructure
    {
    }


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

        /*[JsonIgnore]
        public ObservableCollection<string> refnames
        {
            get
            {
                ObservableCollection<string> toret = new ObservableCollection<string>();
                foreach(int i in REFS)
                {
                   //toret.Add(i.ToString("000") + ": " + MainWindow.database[refDB][i].Name);
                }

                return toret;
            }
        }*/
    }

    [Serializable]
    public class ObjectStructure : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("FIELDS"));
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        int nameOverride = -1;
        string _handledName;
        public void setOverride(int v)
        {
            nameOverride = v;
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

        public ObjectStructure Copy()
        {
            //TODO: Fix this bad boy up
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<ObjectStructure>(JsonConvert.SerializeObject(this), deserializeSettings);
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
