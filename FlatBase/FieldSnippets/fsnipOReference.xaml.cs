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
    public partial class fsnipOReference : UserControl
    {
        OReferenceList orl;
        ObservableCollection<ObjectStructure> pullList;

        public fsnipOReference(string title, OReferenceList refferal, ObservableCollection<ObjectStructure> pl)
        {
            InitializeComponent();

            groupBox.Header = title;
            orl = refferal;
            pullList = pl;
        }

        public void addItem(object sender, RoutedEventArgs e)
        {
            OReferenceDialog ord = new OReferenceDialog(pullList);
            if (ord.ShowDialog() == true)
            {
                OReferenceDialog content = ord.Content as OReferenceDialog;
                orl.REFS.Add(ord.returnVal);
                
            }
        }

        private void LView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lView.SelectedIndex != -1)
            {
                Remove.IsEnabled = true;
            }
            else
            {
                Remove.IsEnabled = true;
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            orl.REFS.RemoveAt(lView.SelectedIndex);
            Remove.IsEnabled = false;
        }
    }
}
