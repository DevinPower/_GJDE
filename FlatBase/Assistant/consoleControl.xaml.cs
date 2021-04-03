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
    public partial class consoleControl : UserControl
    {
        FlatBase.Misc.consoleInstance CI = new Misc.consoleInstance();
        public consoleControl()
        {
            InitializeComponent();

            consoleOutput.DataContext = CI;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            Console.WriteLine("BI");
            CI.input(consoleInput.Text);
            consoleInput.Text = "";

        }
    }
}
