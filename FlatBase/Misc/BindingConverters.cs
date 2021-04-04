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

            List<ObjectStructure> toret = new List<ObjectStructure>();

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
                    toret.Add(os);
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
            ObservableCollection<string> idx = value as ObservableCollection<string>;
            Console.WriteLine(idx.Count + "count");
            List<object> toret = new List<object>();
            foreach(string s in idx)
            {
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
                    brush = new SolidColorBrush(TagManager.tagColor("Global", collection[i]));
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
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return null;

            ObservableCollection<Assistant.fieldHelper> collection = (values as Assistant.ConvertedClass).fields;

            if (collection == null)
                return null;

            List<object> toret = new List<object>();
            
            foreach(string cc in (values as Assistant.ConvertedClass).dependencies)
            {
                Console.WriteLine("looking for " + cc);
                foreach(Assistant.ConvertedClass vw in Assistant.SetupWizard.inspectedClasses)
                {
                    if (vw.name == cc)
                    {
                        foreach(Assistant.fieldHelper fh in vw.fields)
                        {
                            Assistant.fieldDisplay fd = new Assistant.fieldDisplay(fh);
                            fd.IsEnabled = false;
                            toret.Add(fd);
                        }
                        continue;
                    }
                }
            }

            foreach (Assistant.fieldHelper os in collection)
            {
                //toret.Add(os);
                toret.Add(new Assistant.fieldDisplay(os));
            }

            return toret;//collection[idx.Value].Name;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
