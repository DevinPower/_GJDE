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
    public partial class fsnipArray : UserControl
    {
        MainWindow refmw;

        public fsnipArray(string title, MainWindow mw)
        {
            InitializeComponent();

            groupBox.Header = title;
            refmw = mw;
        }

        public void addItem(object sender, RoutedEventArgs e)
        {
            Grid g = new Grid();
            refmw.buildOSView(g, MainWindow.loadManifest("Tuple"), 0);
            lView.Items.Add(g);
        }
    }
}
