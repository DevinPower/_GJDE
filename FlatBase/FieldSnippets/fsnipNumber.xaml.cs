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
using System.Text.RegularExpressions;

namespace FlatBase.FieldSnippets
{
    /// <summary>
    /// Interaction logic for fsnipBool.xaml
    /// </summary>
    public partial class fsnipNumber : UserControl
    {
        
        public fsnipNumber(bool isFloat)
        {
            InitializeComponent();

            if (isFloat)
            {
                numVal.PreviewTextInput += floatValidation;
            }
            else
            {
                numVal.PreviewTextInput += intValidation;
            }
        }

        private void intValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[1-9][0-9]+\.?[0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void floatValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[1-9][0-9]+\.?[0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
