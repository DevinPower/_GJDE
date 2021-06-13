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
using System.Collections.ObjectModel;

namespace FlatBase.Misc
{
    class BoolColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool input = (bool)value;

            if (input)
                return Color.FromRgb(155, 155, 155);

            return Color.FromRgb(0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class TagManagerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObservableCollection<UserTag> tagslist = ((Dictionary<string, ObservableCollection<UserTag>>)value)["Global"];

            if (tagslist == null)
                return null;

            List<object> toret = new List<object>();

            foreach(UserTag t in tagslist)
            {
                Assistant.tagDisplay disp = new Assistant.tagDisplay(t);

                toret.Add(disp);
            }

            Console.WriteLine("updating tags count= " + toret.Count);

            return toret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class AStructConverter : IValueConverter
    {
        MainWindow refmw;

        public AStructConverter(MainWindow mw)
        {
            refmw = mw;
            
        }

        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return null;

            ObservableCollection<ObjectStructure> collection = values as ObservableCollection<ObjectStructure>;

            if (collection == null)
                return null;

            List<object> toret = new List<object>();

            /*foreach (ObjectStructure os in collection.)
            {
                toret.Add(os);
            }*/

            for (int i = 0; i < collection.Count; i++)
            {
                Grid g = new Grid();
                refmw.buildOSView(g, collection[i], 0);

                toret.Add(g);
            }

            return toret;//collection[idx.Value].Name;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class HierarchyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return null;

            ObservableCollection<ObjectStructure> collection = values[0] as ObservableCollection<ObjectStructure>;
            ObservableCollection<string> filters = values[1] as ObservableCollection<string>;

            if (collection == null)
                return null;

            List<HierarchyItemViewer> toret = new List<HierarchyItemViewer>();

            int i = 0;
            foreach (ObjectStructure os in collection)
            {
                bool validated = false;
                
                /*foreach (string s in filters)
                {
                    if (os.Name.ToLower().Contains(s.ToLower()))
                    {
                        validated = true;
                        break; 
                    }
                }*/



                if (filters.Count == 0 || os.meetsFilter(filters))
                    validated = true;

                if (validated)
                {
                    SolidColorBrush c = new SolidColorBrush(Color.FromRgb(0, 0, 0));

                    if (os.excludeExport)
                        c = new SolidColorBrush(Color.FromRgb(155, 155, 155));

                    HierarchyItemViewer hIV = new HierarchyItemViewer(c, os);

                    hIV.labelDisplay.DataContext = os;
                    Binding textBind = new Binding();
                    textBind.Path = new PropertyPath("Name");
                    textBind.Source = os;
                    textBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    hIV.labelDisplay.SetBinding(Label.ContentProperty, textBind);

                    BoolColorConverter bcc = new BoolColorConverter();
                    Binding b = new Binding();

                    b.Converter = bcc;
                    b.Path = new PropertyPath("excludeExport");
                    b.Source = os;
                    b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    BindingOperations.SetBinding(hIV.colorDisplay, SolidColorBrush.ColorProperty, b);

                    toret.Add(hIV);
                }
                i++;
            }

            return toret;//collection[idx.Value].Name;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    class ExclusionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObservableCollection<ObjectStructure> collection = value as ObservableCollection<ObjectStructure>;

            if (collection == null)
                return null;
            Console.WriteLine("newp");
            List<Color> toret = new List<Color>();

            /*foreach (ObjectStructure os in collection.)
            {
                toret.Add(os);
            }*/

                    for (int i = 0; i < collection.Count; i++)
            {
                toret.Add(Color.FromRgb(255, 0, 0));
            }

            return toret;//collection[idx.Value].Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    class ORefSingleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length != 2)
                return null;

            int idx = int.Parse(values[0].ToString());
            ObservableCollection<ObjectStructure> collection = values[1] as ObservableCollection<ObjectStructure>;

            if (collection == null)
                return null;

            if (idx > collection.Count - 1 || collection[idx] == null)
                return "Empty Array";
            return idx.ToString("000") + ": " + collection[idx].Name;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ORefConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length != 2)
                return null;

            ObservableCollection<int> idx = values[0] as ObservableCollection<int>;
            ObservableCollection<ObjectStructure> collection = values[1] as ObservableCollection<ObjectStructure>;

            if (idx == null || collection == null)
                return null;

            List<string> toret = new List<string>();

            foreach (int iR in idx)
            {
                toret.Add(iR.ToString("000") + ": " + collection[iR].Name);
                Console.WriteLine("added 1");
            }

            return toret;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class SubclassMenuConverter: IValueConverter
    {
        MainWindow mwref;

        public SubclassMenuConverter(MainWindow mw)
        {
            mwref = mw;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Console.WriteLine("starting to look");
            if (value == null)
                return null;
            Console.WriteLine("NOT NULL");
            ObservableCollection<Misc.SubclassHelper> idx = value as ObservableCollection<Misc.SubclassHelper>;
            Console.WriteLine(idx.Count + "count");
            List<object> toret = new List<object>();
            foreach(Misc.SubclassHelper h in idx)
            {
                string s = h.name;
                MenuItem mi = new MenuItem();
                mi.Header = s;
                mi.Click += (rs, EventArgs) => { mwref.addFromTemplate(s); };

                toret.Add(mi);
                Console.WriteLine("FOUND ONE");

            }

            return toret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    class TemplateMenuConverter : IValueConverter
    {
        MainWindow mwref;

        public TemplateMenuConverter(MainWindow mw)
        {
            mwref = mw;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Console.WriteLine("starting to look");
            if (value == null)
                return null;
            Console.WriteLine("NOT NULL");
            ObservableCollection<Misc.SubclassHelper> idx = value as ObservableCollection<Misc.SubclassHelper>;
            Console.WriteLine(idx.Count + "count");
            List<object> toret = new List<object>();
            foreach (Misc.SubclassHelper h in idx)
            {
                string s = h.name;
                MenuItem mi = new MenuItem();
                mi.Header = s;
                mi.Click += (rs, EventArgs) => { mwref.addFromData(s); };

                toret.Add(mi);
                Console.WriteLine("FOUND ONE");

            }

            return toret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class IntegerConverter : IValueConverter
    {
        public int EmptyStringValue = 0;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            else if (value is string)
                return value;
            else if (value is int && (int)value == EmptyStringValue)
                return string.Empty;
            else
                return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                string s = (string)value;
                int outval;
                if (Int32.TryParse(s, out outval))
                    return outval;
                else
                    return EmptyStringValue;
            }
            return value;
        }
    }

    class TagChipConverter : IValueConverter
    {
        ObservableCollection<string> reflist;

        public TagChipConverter(ObservableCollection<string> rl)
        {
            reflist = rl;
        }

        void delete(object sender, RoutedEventArgs e)
        {
            reflist.Remove((sender as MaterialDesignThemes.Wpf.Chip).Content.ToString());
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            ObservableCollection<String> collection = value as ObservableCollection<String>;

            if (collection == null)
                return null;

            List<object> toret = new List<object>();

            /*foreach (ObjectStructure os in collection.)
            {
                toret.Add(os);
            }*/

            for (int i = 0; i < collection.Count; i++)
            {
                MaterialDesignThemes.Wpf.Chip chip = new MaterialDesignThemes.Wpf.Chip();
                chip.Height = 24;
                chip.IsDeletable = true;
                chip.Content = collection[i];                

                Brush brush = chip.Background;
                if (TagManager.isTag("Global", collection[i]))
                {
                    brush = new SolidColorBrush(TagManager.tagColor("Global", collection[i]));
                }

                chip.Background = brush;
                chip.DeleteClick += delete;

                toret.Add(chip);
            }

            return toret;//collection[idx.Value].Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class FloatConverter : IValueConverter
    {
        public float EmptyStringValue = 0;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            else if (value is string)
                return value;
            else if (value is float && (float)value == EmptyStringValue)
                return string.Empty;
            else
                return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                string s = (string)value;
                float outval;
                if (float.TryParse(s, out outval))
                    return outval;
                else
                    return EmptyStringValue;
            }
            return value;
        }
    }

    class WizardHierarchyConverter : IValueConverter
    {
        Assistant.ConvertedClass lcc;
        public void setCC(Assistant.ConvertedClass rf)
        {
            lcc = rf;
        }

        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return null;

            ObservableCollection<Assistant.fieldHelper> collection = (values as ObservableCollection<Assistant.fieldHelper>);

            if (collection == null)
                return null;

            List<object> toret = new List<object>();
            
            foreach(string cc in lcc.dependencies)
            {
                foreach(Assistant.ConvertedClass vw in Assistant.SetupWizard.inspectedClasses)
                {
                    if (vw.name == cc)
                    {
                        foreach(Assistant.fieldHelper fh in vw.fields)
                        {
                            List<string> sList = new List<string>();
                            if (Assistant.SetupWizard.variants.ContainsKey(fh.Type))
                            {
                                List<Assistant.variantWrapper> vList = Assistant.SetupWizard.variants[fh.Type];

                                foreach (Assistant.variantWrapper vwd in vList)
                                    sList.Add(vwd.display);
                            }

                            Assistant.fieldDisplay fd = new Assistant.fieldDisplay(fh, sList);
                            fd.IsEnabled = false;
                            toret.Add(fd);
                        }
                        continue;
                    }
                }
            }

            foreach (Assistant.fieldHelper os in collection)
            { 
                List<string> sList = new List<string>();
                //toret.Add(os);
                if (Assistant.SetupWizard.variants.ContainsKey(os.Type))
                { 

                    List<Assistant.variantWrapper> vList = Assistant.SetupWizard.variants[os.Type];

                    
                    foreach (Assistant.variantWrapper vwd in vList)
                        sList.Add(vwd.display);
                }

                Assistant.fieldDisplay fd = new Assistant.fieldDisplay(os, sList);
                fd.MouseEnter += new MouseEventHandler((object sender, MouseEventArgs e) => { Console.WriteLine("mouse enter " + fd.Name); });
                toret.Add(fd);
                
            }

            return toret;//collection[idx.Value].Name;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
