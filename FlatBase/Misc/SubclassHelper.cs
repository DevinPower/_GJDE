using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;

namespace FlatBase.Misc
{
    public class SubclassHelper
    {
        public string name { get; set; }

        public SubclassHelper(string n)
        {
            name = n;
        }
    }

    public class SubclassDict : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        Dictionary<string, ObservableCollection<Misc.SubclassHelper>> sc;
        public Dictionary<string, ObservableCollection<Misc.SubclassHelper>> subclasses
        {
            get
            {
                return sc;
            }
            set
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("subclasses"));
                sc = value;
            }
        }

        public SubclassDict()
        {
            subclasses = new Dictionary<string, ObservableCollection<Misc.SubclassHelper>>();
            
        }

        public void addTable(string s)
        {
            if (!subclasses.ContainsKey(s))
            {
                subclasses.Add(s, new ObservableCollection<SubclassHelper>());
                subclasses[s].CollectionChanged += ContentCollectionChanged;
            }
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("subclasses"));
            }
        }
    }
}
