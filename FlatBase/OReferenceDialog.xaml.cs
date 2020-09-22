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
using System.Windows.Shapes;

namespace FlatBase
{
    /// <summary>
    /// Interaction logic for OReferenceDialog.xaml
    /// </summary>
    public partial class OReferenceDialog : Window
    {
        public int returnVal = -1;
        public OReferenceDialog(System.Collections.ObjectModel.ObservableCollection<ObjectStructure> options)
        {
            InitializeComponent();

            foreach (ObjectStructure s in options)
            {
                listOptions.Items.Add(s.Name);
            }
        }

        public void accept(object sender, RoutedEventArgs e)
        {
            if (listOptions.SelectedIndex != -1)
            {
                DialogResult = true;
                returnVal = listOptions.SelectedIndex;
                Close();
            }
        }
    }
}
