using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace FlatBase
{
    public class TagField : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string reference { get; set; }
        public ObservableCollection<string> value { get; set; }

        public TagField(string r)
        {
            reference = r;
            value = new ObservableCollection<string>();
            value.CollectionChanged += ContentCollectionChanged;
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("ZZattempted change");
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("value"));
                Console.WriteLine("ZZchanged");
            }
        }
    }

    public class UserTag
    {
        public Color color { get; set; }
        public string name { get; set; }

        public UserTag(Color c, string n)
        {
            color = c;
            name = n;
        }
    }

    class TagManager
    {
        public static Dictionary<string, List<UserTag>> tags = new Dictionary<string, List<UserTag>>();

        public static void loadTag(string name)
        {
            tags.Add(name, new List<UserTag>());
            string[] r = File.ReadAllLines("config/Tags/" + name + ".txt");
            foreach (string s in r)
            {
                string[] split = s.Split(' ');
                
                Color c = Color.FromRgb(Byte.Parse(split[0]), Byte.Parse(split[1]), Byte.Parse(split[2]));

                tags[name].Add(new UserTag(c, split[3]));
            }
        }

        public static bool isTag(string list, string text)
        {
            foreach(UserTag ut in tags[list])
            {
                if (ut.name == text)
                    return true;
            }

            return false;
        }
        public static Color tagColor(string list, string text)
        {
            foreach (UserTag ut in tags[list])
            {
                if (ut.name == text)
                {
                    Console.WriteLine("returned a color");
                    return ut.color;
                }
            }

            return Colors.Red;
        }

        public static void parseAll()
        {
            string[] s = File.ReadAllLines("config/tagsManifest.txt");
            for (int i = 0; i < s.Count(); i++)
            {
                loadTag(s[i]);
            }
        }
    }
}
