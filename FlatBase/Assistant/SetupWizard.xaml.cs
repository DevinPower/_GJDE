using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using Microsoft.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

using Newtonsoft.Json;

using Microsoft.Win32;

namespace FlatBase.Assistant
{
    [Serializable]
    public class variantWrapper
    {
        public string display  {get;set;}
        public string markdown { get; set; }

        public variantWrapper(string m, string d)
        {
            display = d;
            markdown = m;
        }
    }

    [Serializable]
    public class fieldHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool list { get; set; }

        readonly string[] generics = { "int", "string", "bool", "float", "double" };


        string _name;
        string _type;
        string _variant;

        public string Name
        {
            get { return _name; }
            set {
                _name = value;
                
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Type"));
            }
        }

        public string Status
        {
            get
            {
                if (SetupWizard.loadedClasses.Contains(_type) || generics.Contains(_type))
                    return "+";
                else
                    return "E";
            }
        }

        public string Variant
        {
            get
            {
                return _variant;
            }
            set
            {
                _variant = value;
                if (PropertyChanged != null)
                {
                    Console.WriteLine("VNOT");
                    
                    PropertyChanged(this, new PropertyChangedEventArgs("Variant"));
                }
            }
        }


        public fieldHelper(string n, string t, bool l)
        {
            _name = n;
            _type = t;
            list = l;
            _variant = "Default";
        }

        public string getMarkdownVariant()
        {
            Console.WriteLine(_variant);
            if (_variant == "Default")
            {
                if (generics.Contains(_type))
                    return _type[0].ToString();
                else
                    return _type;
            }

            foreach (variantWrapper vw in SetupWizard.variants[_type])
            {
                Console.WriteLine(vw.display);
                if (vw.display == _variant)
                {
                    return vw.markdown;
                }
            }

            return _type;
        }

        public override string ToString()
        {
            string lDisp = "";

            if (list)
                lDisp = "A";
            if (generics.Contains(_type))
                return lDisp + getMarkdownVariant() + " " + Name;
            else
                return lDisp + "R " + Name + getMarkdownVariant();

            return "";
        }

        /// <summary>
        /// this version of the function returns the category number for arrays
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public string ToString(List<string> categories)
        {
            string lDisp = "";

            int cN = 0;
            foreach(string s in categories)
            {
                if (_type == s)
                {
                    break;
                }
                cN++;
            }

            if (list)
                lDisp = "A";
            if (generics.Contains(_type))
                return lDisp + getMarkdownVariant() + " " + Name;
            else
                return lDisp + "R " + Name + " " + cN;

            return "";
        }
    }

    [Serializable]
    public class ConvertedClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ConvertedClass()
        {
            fields = new ObservableCollection<fieldHelper>();
            fields.CollectionChanged += ContentCollectionChanged; 
        }

        public string name { get; set; }
        public ObservableCollection<fieldHelper> fields { get; set; }
        public string linkedFilePath { get; set; }

        public ObservableCollection<string> dependencies = new ObservableCollection<string>();

        public void itemChange(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("FIELDS CHANGED");
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("fields"));
            }
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (fieldHelper item in e.OldItems)
                {
                    item.PropertyChanged -= itemChange;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (fieldHelper item in e.NewItems)
                {
                    item.PropertyChanged += itemChange;
                }
            }
        }


        public string toDocument(List<string> catText)
        {
            Console.WriteLine(name);
            foreach(string s in dependencies)
            {
                Console.WriteLine("I " + s);
            }

            foreach (fieldHelper fh in fields)
            {
                Console.WriteLine("  -> " + fh.Name + " " + fh.Type + fh.list.ToString());
            }

            string tableDoc = "#This file was auto generated on " + System.DateTime.Today + " at " + System.DateTime.Now;

            tableDoc += "\n";

            foreach (string s in dependencies)
            {
                tableDoc += "I " + s + "\n";
            }

            foreach (fieldHelper fh in fields)
            {
                tableDoc += fh.ToString(catText) + "\n";
            }

            return tableDoc;
        }

        public bool isTopLevel()
        {
            if (dependencies.Count == 0)
                return true;
            return false;
        }

        public ConvertedClass(string n, List<fieldHelper> fhs)
        {
            name = n;
            foreach (fieldHelper fh in fhs)
                fields.Add(fh);
        }
    }

    /// <summary>
    /// Interaction logic for SetupWizard.xaml
    /// </summary>
    public partial class SetupWizard : Window
    {
        public static Dictionary<string, List<variantWrapper>> variants = new Dictionary<string, List<variantWrapper>>();

        public static ObservableCollection<string> loadedClasses = new ObservableCollection<string>();
        public static ObservableCollection<ConvertedClass> inspectedClasses { get; set; }

        ConvertedClass curClass;

        public void deleteClass(object sender, RoutedEventArgs e)
        {
            loadedClasses.RemoveAt(loadedScripts.SelectedIndex);
        }

        MainWindow _mw;

        void rebuildView(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("BUILDING VIEW");
            buildView();
        }


        public void buildView()
        {
            try
            {
                List<string> catText = new List<string>();
                foreach (ConvertedClass cc in inspectedClasses)
                {
                    if (cc.isTopLevel())
                    {
                        catText.Add(cc.name);
                    }
                }

                string curText = curClass.toDocument(catText);
                Console.WriteLine(curText);
                _mw.buildOSView(previewPanel, MainWindow.loadManifest(curText, null, null, true), 0);
            }
            catch
            {
                Console.WriteLine("RENDER ERROR");
            }
        }

        public SetupWizard(MainWindow mw)
        {
            InitializeComponent();
            inspectedClasses = new ObservableCollection<ConvertedClass>();

            ContextMenu cm = new ContextMenu();
            MenuItem removeMI = new MenuItem();
            removeMI.Header = "Remove";

            _mw = mw;

            removeMI.Click += (rs, EventArgs) => { deleteClass(rs, EventArgs); };

            cm.Items.Add(removeMI);
            loadedScripts.ContextMenu = cm;

            if (variants.Count == 0)
                loadVariants();

            if (File.Exists("config/classinspections.txt"))
            {
                Console.WriteLine("LOADING");
                string stored = File.ReadAllText("config/classinspections.txt");
                var deserializeSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                ObservableCollection<ConvertedClass> sv = 
                    JsonConvert.DeserializeObject< ObservableCollection<ConvertedClass>>(stored, deserializeSettings);

                foreach(ConvertedClass cc in sv)
                {
                    inspectedClasses.Add(cc);
                    loadedScripts.Items.Add(cc.name);
                }
            }
        }

        public string[] loadFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;

            string[] ret;
            if (openFileDialog.ShowDialog() == true)
            {
                Console.WriteLine(openFileDialog.FileNames.Count());
                ret = new string[openFileDialog.FileNames.Count()];
                int i = 0;
                foreach (string s in openFileDialog.FileNames)
                {
                    ret[i] = s;
                    i++;
                }

                return ret;
            }

            return null;
        }

        public void loadVariants()
        {
            string[] files = Directory.GetFiles("config/System");

            foreach(string f in files)
            {      
                string[] readData = File.ReadAllLines(f);
                string defaultName = readData[0].Split('|')[1];
                variants.Add(defaultName, new List<variantWrapper>());

                variantWrapper defaultV = new variantWrapper(readData[0].Split('|')[0], "Default");
                variants[defaultName].Add(defaultV);

                foreach (string s in readData)
                {
                    if (s == readData[0])
                        continue;
                    string[] split = s.Split('|');

                    variantWrapper vw = new variantWrapper(split[0], split[1]);

                    variants[defaultName].Add(vw);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] code = loadFile();

            if (code == null)
                return;

            foreach (string p in code)
            {
                string s = File.ReadAllText(p);
                Console.Write("AAA");
                loadedScripts.ItemsSource = loadedClasses;

                var syntaxTree = CSharpSyntaxTree.ParseText(s);

                var classes = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();

                foreach (ClassDeclarationSyntax c in classes)
                {
                    string[] identifierNames = c.DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>().Select(v => v.Identifier.Text)
                    .ToArray();

                    object[] identifierTypes = c.DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>().Select(v => v.Type)
                    .ToArray();
                    //Console.WriteLine(c.BaseList.ToString());
                    loadedClasses.Add(c.Identifier.ToString());

                    ConvertedClass cc = new ConvertedClass();
                    cc.PropertyChanged += rebuildView;

                    if (c.BaseList != null)
                    {
                        string clean = c.BaseList.ToString();
                        clean = clean.Replace(" ", "");
                        clean = clean.Replace(":", "");
                        Console.WriteLine("adding " + clean);
                        cc.dependencies.Add(clean);
                    }
                    cc.name = c.Identifier.ToString();
                    

                    for (int i = 0; i < identifierNames.Count(); i++)
                    {
                        string tS = "";
                        tS = identifierTypes[i].ToString();
                        TypeSyntax t = identifierTypes[i] as TypeSyntax;
                        bool l = false;
                        if (t.GetFirstToken().ToString() == "List")
                        {
                            tS = t.GetFirstToken().GetNextToken().GetNextToken().ToString();
                            l = true;
                            Console.WriteLine("DEFINITELY A LIST");
                        }

                        cc.fields.Add(new fieldHelper(identifierNames[i], tS, l));
                    }

                    cc.linkedFilePath = p;
                    System.Security.Cryptography.MD5 hsh = System.Security.Cryptography.MD5.Create();
                    hsh.ComputeHash(Encoding.ASCII.GetBytes(p));

                    Console.Write(BitConverter.ToString(hsh.Hash));

                    inspectedClasses.Add(cc);
                }
            }
        }

        private void LoadedScripts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Binding items = new Binding();
            items.Converter = new FlatBase.Misc.WizardHierarchyConverter();
            items.Path = new PropertyPath("inspectedClasses["+loadedScripts.SelectedIndex+"]");
            items.Source = this;

            inspector.SetBinding(ListView.ItemsSourceProperty, items);
            curClass = inspectedClasses[loadedScripts.SelectedIndex];

            buildView();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<string> catText = new List<string>();
            foreach(ConvertedClass cc in inspectedClasses)
            {
                string curName = cc.name;
                
                if (!cc.isTopLevel())
                {
                    curName = ">" + curName;
                }
                catText.Add(curName);
            }

            foreach (ConvertedClass cc in inspectedClasses)
            {
                string tableDoc = cc.toDocument(catText);

                File.WriteAllText("config/" + cc.name + ".txt", tableDoc);
            }

            string plainTextCategories = "";
            foreach(string s in catText)
            {
                plainTextCategories += s + "\n";
            }

            File.WriteAllText("config/categories.txt", plainTextCategories);

            var deserializeSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string sv = JsonConvert.SerializeObject(inspectedClasses, deserializeSettings);
            File.WriteAllText("config/classinspections.txt", sv);

            _mw.loadCategories();
            this.Close();
        }
    }
}