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

namespace FlatBase.Assistant
{
    /// <summary>
    /// Interaction logic for fsnipBool.xaml
    /// </summary>
    public partial class fieldDisplay : UserControl
    {
        public fieldDisplay(fieldHelper fh, List<string> v)
        {
            InitializeComponent();

            fhLabel.DataContext = fh;
            fhLabel.SetBinding(TextBox.TextProperty, new Binding() { Path = new PropertyPath("Name"), Source = fh });

            fhType.DataContext = fh;
            fhType.SetBinding(Label.ContentProperty, new Binding() { Path = new PropertyPath("Type"), Source = fh });

            fhStatus.DataContext = fh;
            fhStatus.SetBinding(Label.ContentProperty, new Binding() { Path = new PropertyPath("Status"), Source = fh });

            variantCombo.ItemsSource = v;

            variantCombo.DataContext = fh;
            variantCombo.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Path = new PropertyPath("Variant"), Source = fh });
        }
    }
}
