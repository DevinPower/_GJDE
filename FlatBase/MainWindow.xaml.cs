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
using MaterialDesignThemes.Wpf;
using System.Windows.Controls.Primitives;
using System.IO;
using Newtonsoft.Json;

using System.Collections.ObjectModel;

namespace FlatBase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GJDB database;
        static EnumManager em = new EnumManager();

        public List<ObjectStructure> objectTemplates = new List<ObjectStructure>();
        public Dictionary<string, ObservableCollection<string>> subclasses { get; set; }
        List<StackPanel> stackProperties = new List<StackPanel>();
        
        public void loadDatabase(string fileName)
        {
            string file = File.ReadAllText(fileName);
            GJDB tempList = 
                (GJDB)JsonConvert.DeserializeObject(file, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            database.data.Clear();
            foreach (ObservableCollection<ObjectStructure> oc in tempList.data)
            {
                database.addDB();
                foreach (object o in oc)
                {
                    database.data.Last().Add((ObjectStructure)o);
                }
            }
        }

        public void saveDatabase(string fileName)
        {
            string json = JsonConvert.SerializeObject(database, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            File.WriteAllText(fileName, json);
        }

        public MainWindow()
        {
            subclasses = new Dictionary<string, ObservableCollection<string>>();
            database = new GJDB(); 
        }

        private void Entry_Copy(object sender, RoutedEventArgs e, ListView lv, int index)
        {
            int ind = tabMain.SelectedIndex;
            string n = ((TabItem)tabMain.Items[ind]).Header.ToString();
            database.data[index].Add(((ObjectStructure)database.data[index][lv.SelectedIndex]).Copy(loadManifest(n)));
            lv.Items.Refresh();
        }

        private void Entry_Delete(object sender, RoutedEventArgs e, ListView lv, int index)
        {
            database.data[index].RemoveAt(lv.SelectedIndex);
            lv.SelectedIndex = -1;
        }

        private void Entry_Exclude(object sender, RoutedEventArgs e, ListView lv, int index)
        {
            ((ObjectStructure)database.data[index][lv.SelectedIndex]).excludeExport = true;
        }

        private void SelectionChange(object sender, RoutedEventArgs e)
        {      
            ListView lvsender = (ListView)sender;
            stackProperties[tabMain.SelectedIndex].IsEnabled = true;

            if (lvsender.SelectedIndex == -1)
            {
                stackProperties[tabMain.SelectedIndex].IsEnabled = false;
                return;
            }

            buildOSView(stackProperties[tabMain.SelectedIndex], (ObjectStructure)lvsender.Items[lvsender.SelectedIndex], 0);
        }

        private void Add_Click(object sender, RoutedEventArgs e, ListView lv)
        {
            int ind = tabMain.SelectedIndex;
            string n = ((TabItem)tabMain.Items[ind]).Header.ToString();


            database.data[ind].Add(loadManifest(n));
            lv.Items.Refresh();
            /*ListViewItem lvi = new ListViewItem();
            lvi.Content = "New entry";

            ContextMenu cm = new ContextMenu();
            MenuItem duplicate = new MenuItem();
            duplicate.Header = "Duplicate";
            duplicate.Click += (s, EventArgs) => { Entry_Copy(s, EventArgs, lv, lvi); };
            cm.Items.Add(duplicate);
            

            lvi.ContextMenu = cm;

            lv.Items.Add(lvi);*/

            //Console.WriteLine("CLICK!");
        }

        public void loadCategories()
        {
            string[] s = File.ReadAllLines("config/categories.txt");
            for (int i = 0; i < s.Count(); i++)
            {
                if (s[i][0] == '#')
                    continue;

                if (s[i][0] == '>')
                {
                    s[i] = s[i].Remove(0, 1);
                    if (!subclasses.ContainsKey(s[i - 1]))
                        Console.WriteLine("dict not found");
                    subclasses[s[i - 1]].Add(s[i]);

                    continue;
                }

                TabItem tI = new TabItem();
                tI.Header = s[i];
                database.addDB();

                Grid G = new Grid();
                tI.Content = G;

                Misc.SearchFilter SF = new Misc.SearchFilter();

                Card c = new Card();
                c.Padding = new Thickness(8);
                c.Margin = new Thickness(192, 0, 16, 16);

                ScrollViewer sv = new ScrollViewer();
                sv.Visibility = Visibility.Visible;
                StackPanel sp = new StackPanel();

                stackProperties.Add(sp);
                sp.IsEnabled = false;
                sv.Content = sp;
                c.Content = sv;



                Menu m = new Menu();
                m.HorizontalAlignment = HorizontalAlignment.Left;
                m.Height = 48;
                m.VerticalAlignment = VerticalAlignment.Top;



                ListView lv = new ListView();
                lv.HorizontalAlignment = HorizontalAlignment.Left;
                lv.VerticalAlignment = VerticalAlignment.Top;
                lv.Width = 180;
                lv.Margin = new Thickness(0, 96, 0, 0);


                //lv.ItemsSource = database.Last();

                int tempI = i;

                MultiBinding mb = new MultiBinding();

                Binding lhs = new Binding();
                lhs.Path = new PropertyPath("data[" + tempI + "]");
                lhs.Source = database;

                Binding rhs = new Binding();
                rhs.Source = SF;
                rhs.Path = new PropertyPath("FILTERS");

                mb.Bindings.Add(lhs);
                mb.Bindings.Add(rhs);

                mb.Converter = new Misc.HierarchyConverter();

                /*Binding hierarchyBinding = new Binding();
                hierarchyBinding.Path = new PropertyPath("data[" + tempI + "]");
                hierarchyBinding.Source = database;
                hierarchyBinding.Converter = mb;*/
                //new Misc.HierarchyConverter();

                lv.DataContext = database;
                lv.SetBinding(ListView.ItemsSourceProperty,
                    mb);

                /*Binding exclBind = new Binding();
                exclBind.Source = database;
                exclBind.Path = new PropertyPath("data[" + tempI + "].excludeExport");
                exclBind.Converter = new Misc.ExclusionConverter();
                lv.SetBinding(ListView.color, exclBind);*/

                lv.SelectionChanged += SelectionChange;

                ContextMenu cm = new ContextMenu();
                MenuItem duplicateMI = new MenuItem();
                duplicateMI.Header = "Duplicate";
                MenuItem removeMI = new MenuItem();
                removeMI.Header = "Remove";
                MenuItem excludeMI = new MenuItem();
                excludeMI.Header = "Exclude";

                duplicateMI.Click += (rs, EventArgs) => { Entry_Copy(rs, EventArgs, lv, tempI); };
                removeMI.Click += (rs, EventArgs) => { Entry_Delete(rs, EventArgs, lv, tempI); };
                excludeMI.Click += (rs, EventArgs) => { Entry_Exclude(rs, EventArgs, lv, tempI); };

                cm.Items.Add(duplicateMI);
                cm.Items.Add(removeMI);
                cm.Items.Add(excludeMI);
                lv.ContextMenu = cm;


                Button bA = new Button();
                bA.Content = "New";
                bA.Width = 164;

                ContextMenu additionalAddMenu = new ContextMenu();
                MenuItem miATemp = new MenuItem();
                miATemp.Header = "From Template";

                MenuItem miSub = new MenuItem();
                miSub.Header = "Subclass";

                //Console.WriteLine("Adding {0} to sub dict", s[i]);
                subclasses.Add(s[i], new ObservableCollection<string>());

                Binding subBinding = new Binding();
                subBinding.Converter = new Misc.SubclassMenuConverter(this);
                subBinding.Source = this;
                subBinding.Path = new PropertyPath("subclasses[" + s[i] + "]");

                miSub.SetBinding(MenuItem.ItemsSourceProperty, subBinding);

                //miSub.DataContext = subclasses[s[i]];
                //miSub.ItemsSource = subclasses[s[i]];

                additionalAddMenu.Items.Add(miATemp);
                additionalAddMenu.Items.Add(miSub);

                Button bB = new Button();
                PackIcon ic = new PackIcon();
                ic.Kind = PackIconKind.ChevronDown;
                bB.Content = ic;
                bB.Width = 46;
                bB.Height = 36;
                bB.HorizontalAlignment = HorizontalAlignment.Right;
                bB.ContextMenu = additionalAddMenu;

                Grid bGrid = new Grid();
                bGrid.Children.Add(bA);
                bGrid.Children.Add(bB);

                database.tabNames.Add(s[i]);
                
                bA.Click += (sender, EventArgs) => { database.addItem(tempI); };
                //{ Add_Click(sender, EventArgs, lv); };

                m.Width = 350;
                m.Height = 900;
                m.Items.Add(bGrid);
                m.Items.Add(SF);
                G.Children.Add(m);
                G.Children.Add(lv);
                G.Children.Add(c);
                tabMain.Items.Add(tI);
            }
        }

        public void buildOSView(Panel tar, ObjectStructure os, int depth)
        {
            if (depth == 0)
                tar.Children.Clear();
            int c = 0;
            foreach(Object o in os.FIELDS)
            {
                Console.WriteLine(o.GetType());

                //TODO: Switch statement
                if (o is bool)
                {
                    FieldSnippets.fsnipBool fsb = new FieldSnippets.fsnipBool();
                    fsb.labelTitle.Content = "USABLE";
                    fsb.toggleVal.Content = "U";

                    fsb.toggleVal.DataContext = os;
                    fsb.toggleVal.SetBinding(ToggleButton.IsCheckedProperty,
                        new Binding() { Path = new PropertyPath("FIELDS[" + c + "]"), Source = os });

                    fsb.container.Width = 400 - (depth * 100);

                    tar.Children.Add(fsb);
                    c++;
                    
                    continue;
                }

                if (o is Int64 || o is int || o is Int32)
                {
                    FieldSnippets.fsnipNumber fsn = new FieldSnippets.fsnipNumber(false);

                    Binding intBinding = new Binding();
                    intBinding.Path = new PropertyPath("FIELDS[" + c + "]");
                    intBinding.Source = os;
                    intBinding.Converter = new Misc.IntegerConverter();

                    fsn.numVal.SetBinding(TextBox.TextProperty,
                        intBinding);

                    fsn.container.Width = 400 - (depth * 200);

                    tar.Children.Add(fsn);

                    c++;
                    continue;
                }

                if (o is float || o is double)
                {
                    FieldSnippets.fsnipNumber fsn = new FieldSnippets.fsnipNumber(false);

                    Binding intBinding = new Binding();
                    intBinding.Path = new PropertyPath("FIELDS[" + c + "]");
                    intBinding.Source = os;
                    intBinding.Converter = new Misc.FloatConverter();

                    fsn.numVal.SetBinding(TextBox.TextProperty,
                        intBinding);

                    fsn.container.Width = 400 - (depth * 200);

                    tar.Children.Add(fsn);

                    c++;
                    continue;
                }

                if (o is string)
                {
                    FieldSnippets.fsnipString fss = new FieldSnippets.fsnipString(3);

                    fss.textValue.DataContext = os;

                    Binding textBind = new Binding();
                    textBind.Path = new PropertyPath("FIELDS[" + c + "]");
                    textBind.Source = os;
                    textBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    fss.textValue.SetBinding(TextBox.TextProperty,
                        textBind);

                    tar.Children.Add(fss);
                    c++;
                    continue;
                }

                if (o is EnumField)
                {
                    FieldSnippets.fsnipEnum es = new FieldSnippets.fsnipEnum();
                    EnumField ef = o as EnumField;

                    Binding options = new Binding();
                    options.Path = new PropertyPath("fields");
                    options.Source = em.getEnum(ef.reference);

                    Binding value = new Binding();
                    value.Path = new PropertyPath("selected");
                    value.Source = ef;

                    es.comboEnum.SetBinding(ComboBox.ItemsSourceProperty, options);
                    es.comboEnum.SetBinding(ComboBox.SelectedIndexProperty, value);

                    tar.Children.Add(es);
                    c++;
                    continue;
                }

                if (o is Misc.FilePathStructure)
                {
                    Misc.FilePathStructure fp = o as Misc.FilePathStructure;
                    FieldSnippets.fsnipSprite sp = new FieldSnippets.fsnipSprite(fp);
                    

                    Binding lhs = new Binding();
                    lhs.Path = new PropertyPath("PATH");
                    lhs.Source = o;
                    lhs.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    Binding rhs = new Binding();
                    rhs.Path = new PropertyPath("PATH");
                    rhs.Source = o;
                    rhs.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    sp.Path.SetBinding(TextBox.TextProperty, lhs);
                    sp.Render.SetBinding(Image.SourceProperty, rhs);

                    tar.Children.Add(sp);
                    c++;
                    continue;
                }

                if (o is TagField)
                {
                    TagField tf = o as TagField;
                    FieldSnippets.fsnipTags es = new FieldSnippets.fsnipTags(TagManager.tags["Global"], tf);
                    
                    es.tagList.DataContext = o;
                    Binding textBind = new Binding();
                    textBind.Path = new PropertyPath("value");
                    textBind.Source = o;
                    textBind.Converter = new Misc.TagChipConverter(tf.value);
                    textBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    es.tagList.SetBinding(ListView.ItemsSourceProperty,
                        textBind);

                    //es.tagList.SetBinding(ListView.ItemsSourceProperty, value);

                    tar.Children.Add(es);
                    c++;
                    continue;
                }



                if (o is ObjectHeader)
                {
                    FieldSnippets.fsnipHeader fsh = new FieldSnippets.fsnipHeader((o as ObjectHeader).Name);
                    tar.Children.Add(fsh);
                    c++;
                    continue;
                }

                if (o is intArray)
                {
                    FieldSnippets.fsnipIntArray fsh = new FieldSnippets.fsnipIntArray();

                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("FIELDS[" + c + "]");
                    binding.Source = os;

                    //fsh.SetBinding(FieldSnippets.fsnipIntArray.ValuesProperty,
                    //    binding);

                    tar.Children.Add(fsh);
                    c++;
                    continue;
                }

                if (o is OReferenceList)
                {
                    FieldSnippets.fsnipOReference fsor = new FieldSnippets.fsnipOReference("Spells", o as OReferenceList, 
                        database.data[(o as OReferenceList).getRA()]);

                    //fsor.lView.ItemsSource = ((OReferenceList)o).refnames;

                    MultiBinding mb = new MultiBinding();

                    Binding lhs = new Binding();
                    lhs.Path = new PropertyPath("REFS");
                    lhs.Source = o;

                    Binding rhs = new Binding();
                    rhs.Source = database.data[(o as OReferenceList).getRA()];

                    mb.Bindings.Add(lhs);
                    mb.Bindings.Add(rhs);

                    mb.Converter = new Misc.ORefConverter();
                    

                    fsor.lView.DataContext = o;
                    fsor.lView.SetBinding(ListView.ItemsSourceProperty,
                        mb);
                    
                        
                    tar.Children.Add(fsor);
                    c++;
                    continue;
                }

                if (o is Misc.TupleStructure)
                {
                    Grid g = new Grid();
                    g.Margin = new Thickness(0);
                    g.UseLayoutRounding = false;

                    Console.WriteLine((o as Misc.TupleStructure).fields.Count + " fc");
                    int gc = 0;
                    foreach (ObjectStructure tb in (o as Misc.TupleStructure).fields)
                    {
                        g.ColumnDefinitions.Add(new ColumnDefinition());
                        StackPanel sp = new StackPanel();
                        sp.Orientation = Orientation.Horizontal;

                        buildOSView(sp, tb, 1);
                        
                        Grid.SetColumn(sp, gc);
                        g.Children.Add(sp);
                        gc++;
                    }

                    tar.Children.Add(g);
                }
                
                if (o is ArrayStructure)
                {
                    FieldSnippets.fsnipArray fsor = new FieldSnippets.fsnipArray("TestArray", this, o as ArrayStructure);

                    fsor.lView.DataContext = os;
                    Binding itemBind = new Binding();
                    itemBind.Path = new PropertyPath("REFS");
                    itemBind.Source = o;
                    itemBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    itemBind.Converter = new Misc.AStructConverter(this);


                    
                    fsor.lView.SetBinding(ListView.ItemsSourceProperty,
                        itemBind);

                    tar.Children.Add(fsor);
                    c++;
                    continue;
                }

                if (o is Misc.WeightedGroup)
                {
                    FieldSnippets.fsnipSlider fsor = new FieldSnippets.fsnipSlider();

                    Binding lhs = new Binding();
                    lhs.Path = new PropertyPath("POINTS");
                    lhs.Source = o;
                    lhs.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    Binding rhs = new Binding();
                    rhs.Path = new PropertyPath("points");
                    rhs.Source = (o as Misc.WeightedGroup).tiedWP;
                    rhs.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    fsor.Slider.SetBinding(Slider.ValueProperty, lhs);
                    fsor.NumDisplay.SetBinding(Label.ContentProperty, lhs);
                    fsor.Slider.SetBinding(Slider.MaximumProperty, rhs);

                    tar.Children.Add(fsor);
                    c++;
                    continue;
                }
            }
        }

        public static ObjectStructure loadManifest(string catName, ObjectStructure parent = null, ObjectStructure ovrrd = null)
        {
            string[] s = File.ReadAllLines("config/"+catName+".txt");
            ObjectStructure nost = new ObjectStructure();
            if (ovrrd != null)
                nost = ovrrd;

            int c = 0;
            for (int i = 0; i < s.Count(); i++)
            {
                if (s[i][0] == '#')
                    continue;

                string clipped = s[i].Split(' ')[1];

                if (s[i][0] == 'I')
                {
                    loadManifest(clipped, parent, nost);
                    continue;
                }

                if (s[i][0] == '!')
                {
                    nost.weightpools.Add(clipped, new Misc.weightpool());
                    Console.WriteLine(clipped + " weightpool");
                    continue;
                }

                if (s[i][0] == 'b')
                {
                    nost.FIELDS.Add(false);
                }

                if (s[i][0] == 'h')
                {
                    nost.FIELDS.Add(new ObjectHeader(clipped));
                }

                if (s[i][0] == 'i')
                {
                    nost.FIELDS.Add(0);
                }

                if (s[i][0] == 'f')
                {
                    nost.FIELDS.Add(0.0f);
                }

                if (s[i][0] == 'S')
                {
                    nost.FIELDS.Add(new Misc.FilePathStructure());
                }

                if (s[i][0] == 's')
                {
                    nost.FIELDS.Add("New Entry");
                }

                if (s[i][0] == 'm')
                {
                    Console.WriteLine("M");
                }

                if (s[i][0] == 'W')
                {
                    if (parent == null)
                        nost.FIELDS.Add(new Misc.WeightedGroup(nost.weightpools[clipped]));
                    else
                        nost.FIELDS.Add(new Misc.WeightedGroup(parent.weightpools[clipped]));
                }

                if (s[i][0] == 'E')
                {
                    nost.FIELDS.Add(new EnumField(clipped));
                }

                if (s[i][0] == 'l')
                {
                    nost.FIELDS.Add(new TagField(clipped));
                }

                if (s[i][0] == 'r')
                {
                    OReferenceList orl = new OReferenceList();
                    clipped = s[i].Split(' ')[2];
                    orl.setRA(Int32.Parse(clipped));
                    nost.FIELDS.Add(orl);
                }

                if (s[i][0] == 'G')
                {
                    intArray iA = new intArray(100);
                    nost.FIELDS.Add(iA);
                }

                if (s[i][1] == 'N')
                {
                    nost.setOverride(c);
                }

                if (s[i][0] == 'A')
                {
                    string eVAL = s[i].Split(' ')[1];

                    ArrayStructure aStruct = new ArrayStructure(eVAL, nost);
                    nost.FIELDS.Add(aStruct);
                }

                if (s[i][0] == 'T')
                {
                    List<ObjectStructure> dummytuple = new List<ObjectStructure>();
                    bool skipped = false;
                    foreach (string TT in s[i].Split(' '))
                    {
                        if (!skipped)
                        {
                            skipped = true;
                            continue;
                        }
                        ObjectStructure os = loadManifest(TT, parent);
                        dummytuple.Add(os);

                    }

                    nost.FIELDS.Add(new Misc.TupleStructure(dummytuple));
                }

                nost.fieldexportnames.Add(clipped);
                c++;
            }

            return nost;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            em.parseAll();
            TagManager.parseAll();
            loadCategories();   
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                saveDatabase(saveFileDialog.FileName);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                loadDatabase(openFileDialog.FileName);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("export.db", database.export());
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            Assistant.SetupWizard sw = new Assistant.SetupWizard();
            sw.Show();
        }

        public void addFromTemplate(string template)
        {
            Console.WriteLine("adding by template " + template);
            int ind = tabMain.SelectedIndex;

            database.data[ind].Add(loadManifest(template));
        }
    }
}
