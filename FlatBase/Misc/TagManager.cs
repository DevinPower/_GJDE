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
using System.Windows.Media.Imaging;

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
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("value"));
            }
        }
    }

    [Serializable]
    public class UserTag : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        Color _color;
        string _name;
        string _iconPath;

        public ImageBrush icon
        {
            get
            {
                if (iconPath == "")
                    return null;
                return new ImageBrush(new BitmapImage(new Uri(iconPath)));
            }
        }

        public Color color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("color"));
                    PropertyChanged(this, new PropertyChangedEventArgs("displayColor"));
                }
            }
        }

        public Brush displayColor
        {
            get
            {
                return new SolidColorBrush(color);
            }
        }

        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("name"));
            }
        }
        public string iconPath
        {
            get
            {
                return _iconPath;
            }
            set
            {
                _iconPath = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("iconPath"));
                    PropertyChanged(this, new PropertyChangedEventArgs("icon"));
                }
            }
        }

        public UserTag(Color c, string n, string i)
        {
            color = c;
            name = n;
            iconPath = i;
        }
    }

    class TagManager
    {
        public static Dictionary<string, ObservableCollection<UserTag>> tags = new Dictionary<string, ObservableCollection<UserTag>>();

        public static void updateTags()
        {
            foreach(string k in tags.Keys)
            {
                string data = "";
                foreach(UserTag u in tags[k])
                {
                    data += u.color.R + " " + u.color.G + " " + u.color.B + " " + u.name + "|" + u.iconPath +  "\n";
                }

                File.WriteAllText("config/Tags/" + k + ".txt", data);
            }
        }

        public static void loadTag(string name)
        {
            tags.Add(name, new ObservableCollection<UserTag>());
            string[] r = File.ReadAllLines("config/Tags/" + name + ".txt");
            foreach (string s in r)
            {
                string[] imgSplit = s.Split('|');
                string[] split = imgSplit[0].Split(' ');

                Color c = Color.FromRgb(Byte.Parse(split[0]), Byte.Parse(split[1]), Byte.Parse(split[2]));

                tags[name].Add(new UserTag(c, split[3], imgSplit[1]));
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

        public static ImageBrush tagImage(string list, string text)
        {
            foreach (UserTag ut in tags[list])
            {
                if (ut.name == text)
                {
                    return ut.icon;
                }
            }

            return null;
        }

        public static void parseAll()
        {
            if (!File.Exists("config/tagsManifest.txt"))
                return;
            string[] s = File.ReadAllLines("config/tagsManifest.txt");
            for (int i = 0; i < s.Count(); i++)
            {
                Console.WriteLine("LOADED" + s[i]);
                loadTag(s[i]);
            }
        }
    }
}
