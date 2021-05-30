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

using System.ComponentModel;
using System.Collections.Specialized;


namespace FlatBase.Misc
{
    /// <summary>
    /// Interaction logic for HierarchyItemViewer.xaml
    /// </summary>
    public partial class HierarchyItemViewer : UserControl, INotifyPropertyChanged
    {
        ObjectStructure os;
        public ObjectStructure refObj
        {
            get
            {
                return os;
            }

            set
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("refObj"));
                os = value;
            }
        }

        SolidColorBrush dBrush;

        public SolidColorBrush colorDisplay
        {
            get
            {
                return dBrush;
            }

            set
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("colorDisplay"));
                dBrush = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public HierarchyItemViewer(SolidColorBrush c, ObjectStructure sr)
        {
            InitializeComponent();
            colorDisplay = c;
            labelDisplay.Foreground = dBrush;
            refObj = sr;
        }
    }
}
