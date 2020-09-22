using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace FlatBase.Misc
{
    class HierarchyConverter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return null;

            ObservableCollection<ObjectStructure> collection = values as ObservableCollection<ObjectStructure>;

            if (collection == null)
                return null;

            List<ObjectStructure> toret = new List<ObjectStructure>();

            foreach (ObjectStructure os in collection)
            {
                toret.Add(os);
            }

            return toret;//collection[idx.Value].Name;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
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
            }

            return toret;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
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

    class WizardHierarchyConverter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return null;

            ObservableCollection<Assistant.fieldHelper> collection = values as ObservableCollection<Assistant.fieldHelper>;

            if (collection == null)
                return null;

            List<Assistant.fieldHelper> toret = new List<Assistant.fieldHelper>();

            foreach (Assistant.fieldHelper os in collection)
            {
                toret.Add(os);
            }

            return toret;//collection[idx.Value].Name;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
