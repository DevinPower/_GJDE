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
        public string board { get; set; }

        public static List<Board> boardsList = new List<Board>();

        public static List<Card> cardsList = new List<Card>();

        public bool enabled { get; set; }

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

            refreshData();
        }

        public void appendIssue(int guid, Card c)
        {
            string csid = c.GetCardId();
            CardId cid = new CardId(csid);
            trello.Cards.ChangeDescription(cid, "IssueID: " + guid + "\n" + c.Desc);
            refreshData();
        }

        public void refreshData()
        {
            if (token != null && key != null)
            {
                if (trello == null)
                {
                    trello = new Trello(key);
                    trello.Authorize(token);
                    enabled = true;
                }
                if (board == "")
                {
                    getBoards();
                }
                else
                {
                    getCards();
                }
            }
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

        public void setBoard(string name)
        {
            Console.WriteLine(name);
            foreach(Board b in boardsList)
            {
                if (b.Name == name)
                {
                    board = b.GetBoardId();
                    Console.WriteLine(board);
                    return;
                }
            }
        }

        public string getURL()
        {
            return trello.GetAuthorizationUrl("_GJDE", Scope.ReadWrite, Expiration.Never).ToString();
        }

        public void getCards()
        {
            if (board != "")
            {
                BoardId b = new BoardId(board);
                cardsList = trello.Cards.ForBoard(b).ToList();

                foreach (Card c in cardsList)
                    Console.WriteLine(c.Name);
            }
        }

        public string getBoards()
        {
            IBoards boards = trello.Boards;
            
            boardsList = boards.ForMe(BoardFilter.All).ToList();

            return "";
        }
    }
}
