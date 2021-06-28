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

using MaterialDesignThemes.Wpf;

namespace FlatBase.Assistant
{
    /// <summary>
    /// Interaction logic for IssuePicker.xaml
    /// </summary>
    public partial class IssuePicker : Window
    {
        int _sguid;
        Assistant.TrelloAssistant _ta;
        Dictionary<Card, TrelloNet.Card> clist = new Dictionary<Card, TrelloNet.Card>();
        public IssuePicker(int guid, Assistant.TrelloAssistant ta)
        {
            InitializeComponent();
            _sguid = guid;
            _ta = ta;

            List<TrelloNet.Card> cardsList = Assistant.TrelloAssistant.cardsList;

            foreach (TrelloNet.Card s in cardsList)
            {
                Card c = new Card();
                c.Content = s.Name + "\n" + s.Desc;
                c.Cursor = Cursors.Hand;
                c.Width = 300;
                c.MouseDoubleClick += C_MouseDoubleClick;
                c.Margin = new Thickness(0, 10, 0, 10);
                issuesList.Children.Add(c);
                clist.Add(c, s);
            }
        }

        private void C_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _ta.appendIssue(_sguid, clist[sender as Card]);
            Close();
        }
    }
}
