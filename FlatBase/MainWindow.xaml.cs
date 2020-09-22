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

        public List<ObjectStructure> objectTemplates = new List<ObjectStructure>();
        List<StackPanel> stackProperties = new List<StackPanel>();
        
        public void loadDatabase()
        {
            string file = File.ReadAllText("autosave.db");
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

        public void saveDatabase()
        {
            string json = JsonConvert.SerializeObject(database, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            File.WriteAllText("autosave.db", json);
        }

        public MainWindow()
        {
            database = new GJDB();
            
        }

        private void Entry_Copy(object sender, RoutedEventArgs e, ListView lv, int index)
        {
            database.data[index].Add(((ObjectStructure)database.data[index][lv.SelectedIndex]).Copy());
        }

        private void Entry_Delete(object sender, RoutedEventArgs e, ListView lv, int index)
        {
            database.data[index].RemoveAt(lv.SelectedIndex);
            lv.SelectedIndex = -1;
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

                TabItem tI = new TabItem();
                tI.Header = s[i];
                database.addDB();

                Grid G = new Grid();
                tI.Content = G;

                
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
                lv.Margin = new Thickness(0, 58, 0, 0);
                //lv.ItemsSource = database.Last();

                int tempI = i;

                Binding hierarchyBinding = new Binding();
                hierarchyBinding.Path = new PropertyPath("data[" + tempI + "]");
                hierarchyBinding.Source = database;
                hierarchyBinding.Converter = new Misc.HierarchyConverter();

                lv.DataContext = database;
                lv.SetBinding(ListView.ItemsSourceProperty,
                    hierarchyBinding);

                lv.SelectionChanged += SelectionChange;

                ContextMenu cm = new ContextMenu();
                MenuItem duplicateMI = new MenuItem();
                duplicateMI.Header = "Duplicate";
                MenuItem removeMI = new MenuItem();
                removeMI.Header = "Remove";

                duplicateMI.Click += (rs, EventArgs) => { Entry_Copy(rs, EventArgs, lv, tempI); };
                removeMI.Click += (rs, EventArgs) => { Entry_Delete(rs, EventArgs, lv, tempI); };

                cm.Items.Add(duplicateMI);
                cm.Items.Add(removeMI);
                lv.ContextMenu = cm;


                Button bA = new Button();
                bA.Content = "+ Compose";
                bA.Width = 164;

                database.tabNames.Add(s[i]);
                
                bA.Click += (sender, EventArgs) => { database.addItem(tempI); };
                //{ Add_Click(sender, EventArgs, lv); };

                m.Items.Add(bA);

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
                //TODO: Switch statement
                if (o is bool)
                {
                    FieldSnippets.fsnipBool fsb = new FieldSnippets.fsnipBool();
                    fsb.labelTitle.Content = "USABLE";
                    fsb.toggleVal.Content = "U";

                    fsb.toggleVal.DataContext = os;
                    fsb.toggleVal.SetBinding(ToggleButton.IsCheckedProperty,
                        new Binding() { Path = new PropertyPath("FIELDS["+c+"]"), Source = os });

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

                if (o is float)
                {
                    FieldSnippets.fsnipNumber fsn = new FieldSnippets.fsnipNumber(true);
                    fsn.numVal.SetBinding(TextBox.TextProperty,
                        new Binding() { Path = new PropertyPath("FIELDS[" + c + "]"), Source = os });

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
                    FieldSnippets.fsnipArray fsor = new FieldSnippets.fsnipArray("TestArray", this);

                    tar.Children.Add(fsor);
                    c++;
                    continue;
                }
            }
        }

        public static ObjectStructure loadManifest(string catName)
        {
            string[] s = File.ReadAllLines("config/"+catName+".txt");
            ObjectStructure nost = new ObjectStructure();
            int c = 0;
            for (int i = 0; i < s.Count(); i++)
            {
                if (s[i][0] == '#')
                    continue;

                string clipped = s[i].Split(' ')[1];

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

                if (s[i][0] == 's')
                {
                    nost.FIELDS.Add("New Entry");
                }

                if (s[i][0] == 'E')
                {
                    nost.FIELDS.Add(new EnumField(clipped));
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
                    nost.FIELDS.Add(new ArrayStructure());
                }

                if (s[i][0] == 'T')
                {
                    List<ObjectStructure> dummytuple = new List<ObjectStructure>();
                    ObjectStructure os1 = new ObjectStructure();
                    os1.FIELDS.Add(12);

                    ObjectStructure os2 = new ObjectStructure();
                    os2.FIELDS.Add(1);

                    ObjectStructure os3 = new ObjectStructure();
                    os3.FIELDS.Add(3);


                    dummytuple.Add(os1);
                    dummytuple.Add(os2);
                    dummytuple.Add(os3);

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
            loadCategories();   
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            saveDatabase();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            loadDatabase();
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
    }
}
