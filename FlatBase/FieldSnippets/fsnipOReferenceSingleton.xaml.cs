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
using System.Collections.ObjectModel;

namespace FlatBase.FieldSnippets
{
    /// <summary>
    /// Interaction logic for fsnipBool.xaml
    /// </summary>
    public partial class fsnipOReferenceSingleton : UserControl
    {
        OReference or;
        ObservableCollection<ObjectStructure> pullList;

        public fsnipOReferenceSingleton(string title, OReference refferal, ObservableCollection<ObjectStructure> pl)
        {
            InitializeComponent();

            or = refferal;
            pullList = pl;
        }

        public void addItem(object sender, RoutedEventArgs e)
        {
            OReferenceDialog ord = new OReferenceDialog(pullList);
            if (ord.ShowDialog() == true)
            {
                OReferenceDialog content = ord.Content as OReferenceDialog;
                or.REF = ord.returnVal;

            }
        }
    }
}
