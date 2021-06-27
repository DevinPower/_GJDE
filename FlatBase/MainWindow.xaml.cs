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

        public Misc.SubclassDict objectTemplates = new Misc.SubclassDict();
        public Misc.SubclassDict scd = new Misc.SubclassDict();
        List<StackPanel> stackProperties = new List<StackPanel>();

        public static List<Window> controlledWindows = new List<Window>();

        public ObjectStructure[] selected;

        public static List<Misc.PluginManager> plugins = new List<Misc.PluginManager>();

        public Assistant.TrelloAssistant trelloIntegration;
        
        public void newDB()
        {
            database = new GJDB();
        }

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
            loadPlugins();
            string[] args = Environment.GetCommandLineArgs();
            
            if (args.Count() == 1)
            {
                Misc.SplashScreen ss = new Misc.SplashScreen(this);
                ss.Show();

                controlledWindows.Add(ss);
            }
            else
            {
                database = new GJDB();
            }

            trelloIntegration = new Assistant.TrelloAssistant();
        }

        public void loadPlugins()
        {
            Misc.PluginManager pm = new Misc.PluginManager(@"C:\Users\devin\Documents\_GJDE\FlatBase\bin\Debug\Plugins\DefaultPlugins.dll");

            plugins.Add(pm);
        }

        private void Entry_Duplicate(object sender, RoutedEventArgs e, ListView lv, int index)
        {
            int ind = tabMain.SelectedIndex;
            string n = ((TabItem)tabMain.Items[ind]).Header.ToString();
            database.addItem(index, ((ObjectStructure)database.data[index][lv.SelectedIndex]).Copy(loadManifest(n)));
        }

        private void Entry_Copy(object sender, RoutedEventArgs e, ListView lv, int index)
        {
            int ind = tabMain.SelectedIndex;
            string n = ((TabItem)tabMain.Items[ind]).Header.ToString();

            Clipboard.SetText(((ObjectStructure)database.data[index][lv.SelectedIndex]).getData());
        }

        private void Entry_Template(object sender, RoutedEventArgs e, ListView lv, int index)
        {
            int ind = tabMain.SelectedIndex;
            string n = ((TabItem)tabMain.Items[ind]).Header.ToString();

            ObjectStructure os = ((ObjectStructure)database.data[index][lv.SelectedIndex]);

            string path = "config/Templates/" + n + "/";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            File.WriteAllText(path + os.Name + ".txt", os.getData());

            objectTemplates.subclasses[n].Add(new Misc.SubclassHelper(os.Name));
        }

        private void Entry_Delete(object sender, RoutedEventArgs e, ListView lv, int index)
        {
            int curIndex = lv.SelectedIndex;
            database.data[index].RemoveAt(lv.SelectedIndex);
            lv.SelectedIndex = curIndex;

            if (selected[tabMain.SelectedIndex] == null)
                stackProperties[tabMain.SelectedIndex].IsEnabled = false;
        }

        private void Entry_Exclude(object sender, RoutedEventArgs e, ListView lv, int index)
        {
            ((ObjectStructure)database.data[index][lv.SelectedIndex]).excludeExport ^= true;
        }

        private void SelectionChange(object sender, RoutedEventArgs e)
        {
            ListView lvsender = (ListView)sender;
            stackProperties[tabMain.SelectedIndex].IsEnabled = true;
            Console.WriteLine("SC {0}", lvsender.SelectedIndex);
            if (lvsender.SelectedIndex == -1)
            {
                selected[tabMain.SelectedIndex] = null;
                return;
            }

            selected[tabMain.SelectedIndex] = ((Misc.HierarchyItemViewer)lvsender.Items[lvsender.SelectedIndex]).refObj;
            buildOSView(stackProperties[tabMain.SelectedIndex], ((Misc.HierarchyItemViewer)lvsender.Items[lvsender.SelectedIndex]).refObj, 0);
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
            if (!File.Exists("config/categories.txt"))
                return;
            string[] s = File.ReadAllLines("config/categories.txt");

            database.data.Clear();
            tabMain.Items.Clear();
            scd.subclasses.Clear();

            selected = new ObjectStructure[s.Count()];

            for (int i = 0; i < s.Count(); i++)
            {
                if (s[i].Length <= 0)
                    continue;

                if (s[i][0] == '#')
                    continue;

                if (s[i][0] == '>')
                {
                    s[i] = s[i].Remove(0, 1);
                    if (!scd.subclasses.ContainsKey(s[i - 1]))
                        Console.WriteLine("dict not found");
                    scd.subclasses[s[i - 1]].Add(new Misc.SubclassHelper(s[i]));

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
                MenuItem templateMI = new MenuItem();
                templateMI.Header = "Template";
                MenuItem copyMI = new MenuItem();
                copyMI.Header = "Copy";

                duplicateMI.Click += (rs, EventArgs) => { Entry_Duplicate(rs, EventArgs, lv, tempI); };
                removeMI.Click += (rs, EventArgs) => { Entry_Delete(rs, EventArgs, lv, tempI); };
                excludeMI.Click += (rs, EventArgs) => { Entry_Exclude(rs, EventArgs, lv, tempI); };
                templateMI.Click += (rs, EventArgs) => { Entry_Template(rs, EventArgs, lv, tempI); };
                copyMI.Click += (rs, EventArgs) => { Entry_Copy(rs, EventArgs, lv, tempI); };

                cm.Items.Add(duplicateMI);
                cm.Items.Add(removeMI);
                cm.Items.Add(excludeMI);
                cm.Items.Add(templateMI);
                cm.Items.Add(copyMI);
                lv.ContextMenu = cm;


                Button bA = new Button();
                bA.Content = "New";
                bA.Width = 164;

                ContextMenu additionalAddMenu = new ContextMenu();
                MenuItem miATemp = new MenuItem();
                miATemp.Header = "From Template";

                objectTemplates.addTable(s[i]);

                Binding OTBinding = new Binding();
                OTBinding.Converter = new Misc.TemplateMenuConverter(this);
                OTBinding.Source = objectTemplates;

                OTBinding.Path = new PropertyPath("subclasses[" + s[i] + "]");

                miATemp.SetBinding(MenuItem.ItemsSourceProperty, OTBinding);




                MenuItem miSub = new MenuItem();
                miSub.Header = "Subclass";

                //Console.WriteLine("Adding {0} to sub dict", s[i]);
                scd.addTable(s[i]);

                Binding subBinding = new Binding();
                subBinding.Converter = new Misc.SubclassMenuConverter(this);
                subBinding.Source = scd;
                
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
                bB.Click += (rs, EventArgs) => { bB.ContextMenu.IsOpen = true; };

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

                if (o is Misc.CodeStructure)
                {
                    Misc.CodeStructure fp = o as Misc.CodeStructure;
                    FieldSnippets.fsnipCode cde = new FieldSnippets.fsnipCode();


                    Binding lhs = new Binding();
                    lhs.Path = new PropertyPath("CODE");
                    lhs.Source = o;
                    lhs.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    cde.codeBox.SetBinding(TextBox.TextProperty, lhs);

                    tar.Children.Add(cde);
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
                
                if (o is OReference)
                {
                    FieldSnippets.fsnipOReferenceSingleton fsor = new FieldSnippets.fsnipOReferenceSingleton("Spells", o as OReference,
                        database.data[(o as OReference).getRA()]);

                    //fsor.lView.ItemsSource = ((OReferenceList)o).refnames;

                    MultiBinding mb = new MultiBinding();

                    Binding lhs = new Binding();
                    lhs.Path = new PropertyPath("REF");
                    lhs.Source = o;

                    Binding rhs = new Binding();
                    rhs.Source = database.data[(o as OReference).getRA()];

                    mb.Bindings.Add(lhs);
                    mb.Bindings.Add(rhs);

                    mb.Converter = new Misc.ORefSingleConverter();


                    fsor.textDisplay.DataContext = o;
                    fsor.textDisplay.SetBinding(Label.ContentProperty,
                        mb);


                    tar.Children.Add(fsor);
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

        public static ObjectStructure loadManifest(string catName, ObjectStructure parent = null, ObjectStructure ovrrd = null, bool readOverride = false)
        {
            string[] s = new string[1];

            if (!readOverride)
                s = File.ReadAllLines("config/" + catName + ".txt");
            else
                s = catName.Split('\n');


            ObjectStructure nost = new ObjectStructure();
            if (ovrrd != null)
                nost = ovrrd;

            int c = 0;
            for (int i = 0; i < s.Count(); i++)
            {
                if (s[i].Count() <= 0)
                    continue;

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

                if (s[i][0] == 'C')
                {
                    nost.FIELDS.Add(new Misc.CodeStructure());
                }

                if (s[i][0] == 'S')
                {
                    nost.FIELDS.Add(new Misc.FilePathStructure());
                }

                if (s[i][0] == 's')
                {
                    nost.FIELDS.Add("New Entry");
                    if (s[i][1] == 'N')
                    {
                        nost.setOverride(c);
                    }
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

                if (s[i][0] == 'R')
                {
                    OReference or = new OReference();
                    clipped = s[i].Split(' ')[2];
                    or.setRA(Int32.Parse(clipped));
                    nost.FIELDS.Add(or);
                }

                if (s[i][0] == 'G')
                {
                    intArray iA = new intArray(100);
                    nost.FIELDS.Add(iA);
                }

                if (s[i][0] == 'A')
                {
                    string eVAL = s[i].Remove(0, 1);

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
            loadData();
        }

        public void loadData()
        {
            em.parseAll();
            TagManager.parseAll();
            loadCategories();
            getTemplates();
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
            File.WriteAllText("export.db", database.export(plugins[0]));
        }

        private void MenuItem_Classes(object sender, RoutedEventArgs e)
        {
            Assistant.SetupWizard sw = new Assistant.SetupWizard(this);
            sw.Show();
        }

        public void getTemplates()
        {
            if (!Directory.Exists("config/Templates"))
                return;
            foreach (string s in Directory.GetDirectories("config/Templates/"))
            {
                foreach (string f in Directory.GetFiles(s))
                {
                    string entry = f.Split('\\')[1].Replace(".txt", "");

                    string table = s.Split('/').Last();

                    objectTemplates.subclasses[table].Add(new Misc.SubclassHelper(entry));
                }
            }
        }

        public void addFromData(string path)
        {
            //File.WriteAllText(path + os.Name + ".txt", os.getData());

            int ind = tabMain.SelectedIndex;
            string n = ((TabItem)tabMain.Items[ind]).Header.ToString();
            string compPath = "config/Templates/" + n + "/" + path + ".txt";

            string s = File.ReadAllText(compPath);
            var deserializeSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            ObjectStructure os = JsonConvert.DeserializeObject<ObjectStructure>(s);

            database.addItem(ind, ObjectStructure.fromString(s, loadManifest(n)));
        }

        public void addFromTemplate(string template)
        {
            Console.WriteLine("adding by template " + template);
            int ind = tabMain.SelectedIndex;

            database.data[ind].Add(loadManifest(template));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            foreach(Window w in controlledWindows)
            {
                if (w != null)
                    w.Close();
            }
        }

        private void MenuItem_Labels(object sender, RoutedEventArgs e)
        {
            Assistant.TagWizard tw = new Assistant.TagWizard(TagManager.tags);
            
            controlledWindows.Add(tw);
            tw.Show();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", Environment.CurrentDirectory);
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            Assistant.Settings settings = new Assistant.Settings(trelloIntegration);
            settings.Show();
        }
    }
}
