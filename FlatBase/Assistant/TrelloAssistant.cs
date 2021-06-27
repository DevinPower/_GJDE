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

        public TrelloAssistant(string authToken)
        {
            trello = new Trello(authToken);
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

        public string getURL()
        {
            getBoards();
            return trello.GetAuthorizationUrl("_GJDE", Scope.ReadWrite, Expiration.Never).ToString();

            
        }

        public string getBoards()
        {
            trello.Authorize("d56049e96c2ec41816d9818fea14dd4787f67614f23e5fa849da95434d91e4fe");

            IBoards boards = trello.Boards;

            foreach(Board b in boards.ForMe(BoardFilter.All))
            {
                Console.WriteLine("BOARD {0}", b);
            }
            return "";
        }
    }
}
