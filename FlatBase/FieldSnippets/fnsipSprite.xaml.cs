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

namespace FlatBase.FieldSnippets
{
    /// <summary>
    /// Interaction logic for fsnipBool.xaml
    /// </summary>
    public partial class fsnipSprite : UserControl
    {
        public Misc.FilePathStructure fpsREF;
        public fsnipSprite(Misc.FilePathStructure fps)
        {
            InitializeComponent();
            fpsREF = fps;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                fpsREF.PATH = openFileDialog.FileName;
                //openFileDialog.FileName);
        }
    }
}
