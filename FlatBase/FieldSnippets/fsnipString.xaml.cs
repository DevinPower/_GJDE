using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
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
    public partial class fsnipString : UserControl
    {
        double pHeight;
        Point sPos;
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public fsnipString(int height)
        {
            InitializeComponent();

            textValue.Height = 32;//height * 48;
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 5);
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("PRESS");
            pHeight = textValue.Height;
            sPos = Mouse.GetPosition(textValue); 
            if (!dispatcherTimer.IsEnabled)
                dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Released && dispatcherTimer.IsEnabled)
                dispatcherTimer.Stop();
            textValue.Height = Math.Max(0, pHeight + Mouse.GetPosition(textValue).Y - sPos.Y);
        }

        private void Button_Italic(object sender, RoutedEventArgs e)
        {
            tag("i");
        }

        private void Button_Bold(object sender, RoutedEventArgs e)
        {
            tag("b");
        }

        private void Button_Underline(object sender, RoutedEventArgs e)
        {
            tag("u");
        }

        public void tag(string notation)
        {
            string open = "<" + notation + ">";
            string close = "<" + notation + "/>";


            if (textValue.SelectionLength == 0)
            {
                textValue.Text += open;
            }
            else
            {
                string sVal = textValue.Text;
                sVal = sVal.Insert(textValue.SelectionStart + textValue.SelectionLength, close);
                sVal = sVal.Insert(textValue.SelectionStart, open);


                textValue.Text = sVal;
            }
        }
    }
}
