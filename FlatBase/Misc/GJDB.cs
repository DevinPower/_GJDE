﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using System.ComponentModel;
using System.Collections.Specialized;


namespace FlatBase
{
    public class GJDB : INotifyPropertyChanged
    {
        public List<string> tabNames = new List<string>();
        public Assistant.TrelloAssistant trelloIntegration;

        public event PropertyChangedEventHandler PropertyChanged;

        public GJDB()
        {
            data = new ObservableCollection<ObservableCollection<ObjectStructure>>();
            data.CollectionChanged += ContentCollectionChanged;

            if (trelloIntegration == null)
                trelloIntegration = new Assistant.TrelloAssistant();
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("data"));
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public void ContentCollectionChangedsub(object sender, NotifyCollectionChangedEventArgs e)
        {
            /*foreach (Object item in e.NewItems)
            {
                if (item is string)
                    Console.WriteLine(item);
            }*/
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
                /*for (int i = 0; i < data.Count; i++)
                {
                    for (int i2 = 0; i2 < data[i].Count; i2++)
                    {     
                        PropertyChanged(this, new PropertyChangedEventArgs("data[" + i + "]["+i2+"].Name"));
                    }
                }*/
            }
        }

        public void addDB()
        {
            data.Add(new ObservableCollection<ObjectStructure>());
            data.Last().CollectionChanged += ContentCollectionChanged;
        }

        public void addItem(int dbc)
        {
            ObjectStructure o = MainWindow.loadManifest(tabNames[dbc]);
            data[dbc].Add(o);
            o.setObserver(ContentCollectionChangedsub);
        }

        public void addItem(int dbc, ObjectStructure o)
        {
            data[dbc].Add(o);
            o.setObserver(ContentCollectionChangedsub);
        }

        public ObservableCollection<ObservableCollection<ObjectStructure>> data { get; set; }

        public string export(Misc.PluginManager exporter)
        {
            return exporter.run(this);
        }


    }
}
