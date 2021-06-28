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

namespace FlatBase.Assistant
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        TrelloAssistant _ta;

        public Settings(TrelloAssistant ta)
        {
            InitializeComponent();
            _ta = ta;
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            _ta.setToken(textToken.Text);
            _ta.getBoards();

            foreach(TrelloNet.Board b in Assistant.TrelloAssistant.boardsList)
                comboBoard.Items.Add(b.Name);
        }

        private void buttonGetAuth_Click(object sender, RoutedEventArgs e)
        {
            textURL.Text = _ta.getURL();
            textURL.IsEnabled = true;
            textToken.IsEnabled = true;
            buttonApply.IsEnabled = true;
        }

        private void buttonKey_Click(object sender, RoutedEventArgs e)
        {
            _ta.setKey(textKey.Text);
            buttonGetAuth.IsEnabled = true;
            
        }

        private void comboBoard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoard.Text != "")
            {
                _ta.setBoard(comboBoard.Text);
                _ta.refreshData();
            }
        }
    }
}
