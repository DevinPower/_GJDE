﻿using System;
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

using Microsoft.Win32;

namespace FlatBase.Assistant
{
    public class fieldHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool list { get; set; }

        readonly string[] generics = { "int", "string", "bool", "float", "double" };
        string _name;
        string _type;

        public string Name
        {
            get { return _name; }
            set {
                _name = value;
                PropertyChanged(this, new PropertyChangedEventArgs(""));
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
                PropertyChanged(this, new PropertyChangedEventArgs(""));
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

        public fieldHelper(string n, string t, bool l)
        {
            _name = n;
            _type = t;
            list = l;
        }

        public override string ToString()
        {
            string lDisp = "";

            if (list)
                lDisp = "[]";
            return Name + " " + lDisp + Type;
        }
    }

    public class ConvertedClass
    {
        public string name { get; set; }
        public ObservableCollection<fieldHelper> fields { get; set; }
        public ObservableCollection<string> dependencies = new ObservableCollection<string>();

        public void toDocument()
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
        }
    }

    /// <summary>
    /// Interaction logic for SetupWizard.xaml
    /// </summary>
    public partial class SetupWizard : Window
    {
        public static ObservableCollection<string> loadedClasses = new ObservableCollection<string>();
        public static ObservableCollection<ConvertedClass> inspectedClasses { get; set; }

        public void deleteClass(object sender, RoutedEventArgs e)
        {
            loadedClasses.RemoveAt(loadedScripts.SelectedIndex);
        }

        public SetupWizard()
        {
            InitializeComponent();
            inspectedClasses = new ObservableCollection<ConvertedClass>();

            ContextMenu cm = new ContextMenu();
            MenuItem removeMI = new MenuItem();
            removeMI.Header = "Remove";

            removeMI.Click += (rs, EventArgs) => { deleteClass(rs, EventArgs); };

            cm.Items.Add(removeMI);
            loadedScripts.ContextMenu = cm;
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
                    ret[i] = File.ReadAllText(s);
                    i++;
                }

                return ret;
            }

            return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] code = loadFile();

            if (code == null)
                return;

            foreach (string s in code)
            {
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

                    ObservableCollection<fieldHelper> vO = new ObservableCollection<fieldHelper>();
                    ConvertedClass cc = new ConvertedClass();
                    cc.fields = vO;
                    if (c.BaseList != null)
                    {
                        string clean = c.BaseList.ToString();
                        clean = clean.Replace(" ", "");
                        clean = clean.Replace(":", "");
                        Console.WriteLine("adding " + clean);
                        cc.dependencies.Add(clean);
                    }
                    cc.name = c.Identifier.ToString();
                    inspectedClasses.Add(cc);

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
                        }

                        vO.Add(new fieldHelper(identifierNames[i], tS, l));
                    }
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
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach(ConvertedClass cc in inspectedClasses)
            {
                cc.toDocument();
            }
        }
    }
}