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
    public partial class tagDisplay : UserControl
    {
        UserTag tagRef;

        public tagDisplay(UserTag t)
        {
            InitializeComponent();
            tagRef = t;
    
            tagDisplayView.DataContext = t;
            tagDisplayView.SetBinding(MaterialDesignThemes.Wpf.Chip.IconBackgroundProperty, new Binding() { Path = new PropertyPath("icon"), Source = t });

            nameDisplay.DataContext = t;
            nameDisplay.SetBinding(TextBox.TextProperty, new Binding() { Path = new PropertyPath("name"), Source = t });

            cardDisplay.DataContext = t;
            cardDisplay.SetBinding(MaterialDesignThemes.Wpf.Card.BackgroundProperty, new Binding() { Path = new PropertyPath("displayColor"), Source = t });
        }

        public UserTag getTag()
        {
            return tagRef;
        }
    }
}
