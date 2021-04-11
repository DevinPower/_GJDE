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
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("PRESS");
            pHeight = textValue.Height;
            sPos = Mouse.GetPosition(textValue);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            dispatcherTimer.Start();
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("RELEASE");
            dispatcherTimer.Stop();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            textValue.Height = Math.Max(0, pHeight + Mouse.GetPosition(textValue).Y - sPos.Y);
        }
    }
}
