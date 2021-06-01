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

namespace FlatBase.Misc.SplashScreenMisc
{
    /// <summary>
    /// Interaction logic for ProjectDisplay.xaml
    /// </summary>
    public partial class ProjectDisplay : UserControl
    {
        SplashScreen splashRef;
        string storedPath;
        public ProjectDisplay(projectFile pf, string path, SplashScreen ss)
        {
            InitializeComponent();

            projectName.Content= pf.name;
            accessDate.Content = pf.accessed;
            storedPath = path;
            filePath.Content = path;

            splashRef = ss;

            ContextMenu cm = new ContextMenu();
            MenuItem cPath = new MenuItem();
            cPath.Header = "Copy Path";
            cPath.Click += copyPath;

            cm.Items.Add(cPath);

            projectButton.ContextMenu = cm;
        }

        public void copyPath(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(storedPath);
        }

        private void Button_Load(object sender, RoutedEventArgs e)
        {
            splashRef.load(storedPath);
        }
    }
}
