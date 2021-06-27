using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrelloNet;

namespace FlatBase.Assistant
{
    public class TrelloAssistant
    {
        ITrello trello;
        public string key { get; set; }
        public string token { get; set; }

        public TrelloAssistant()
        {
            //var url = trello.GetAuthorizationUrl("_GJDE", Scope.ReadWrite);

            //Console.WriteLine(url);

            
            /*trello.Authorize("");

            Board tBoard = trello.Boards.WithId("");
            
            Console.WriteLine(tBoard.Name);

            List<Card> cards = trello.Cards.ForBoard(tBoard).ToList();

            foreach (Card c in cards)
            {
                if (c.Desc.Contains("IssueID: "))
                    Console.WriteLine(c.Name);
            }*/


        }

        public void setKey(string k)
        {
            key = k;
            trello = new Trello(key);
        }

        public void setToken(string t)
        {
            token = t;
        }

        public string getURL()
        {
            return trello.GetAuthorizationUrl("_GJDE", Scope.ReadWrite, Expiration.Never).ToString();
        }

        public string getBoards()
        {
            trello.Authorize(token);

            IBoards boards = trello.Boards;

            foreach(Board b in boards.ForMe(BoardFilter.All))
            {
                Console.WriteLine("BOARD {0}", b);
            }
            return "";
        }
    }
}
