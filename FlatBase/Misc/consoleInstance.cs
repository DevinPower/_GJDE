using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Reflection;

using System.ComponentModel;
using System.Collections.Specialized;


namespace FlatBase.Misc
{
    public class parseNode
    {
        public string value = "";
        public List<parseNode> parseNodes = new List<parseNode>();
    }

    public class readData
    {
        consoleInstance _ci;
        public string name { get; set; }
        public string category { get; set; }
        public string color { get; set; }
        public int order { get; set; }

        public readData(consoleInstance CI)
        {
            _ci = CI;
        }

        public void ping()
        {
            _ci.output("pong");
        }

        public void pong()
        {
            _ci.output("ping");
        }

        public void echo(string s)
        {
            _ci.output("echoed: " + s);
        }

        public int add(int a, int b)
        {
            return a + b;
        }
    }

    public class consoleInstance : INotifyPropertyChanged
    {
        private string _history = "";
        public string history
        {
            get { return _history; }
            set { _history = value;
                if (PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("history"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void output(string v)
        {
            history += v + "\n";
        }

        public void input(string v)
        {
            history += v + "\n";
            
            readData rd = new readData(this);

            /*MethodInfo theMethod = typeof(readData).GetMethod(args[i + 1]);
            int c = theMethod.GetParameters().Length;

            object[] arguments = new object[c];
            for (int a = 0; a < c; a++)
            {
                arguments[a] = args[i + a + 2];
            }
            theMethod.Invoke(rd, arguments);
            i += 2 + c;*/

            string totalExpanded = "";

            parseInput(v);
        }

        public string parseInput(string input)
        {
            string[] args = input.Split(' ');

            int i = 0;
            List<int> closeparams = new List<int>();
            parseNode pn = new parseNode();
            
            foreach(string s in args)
            {
                int level = closeparams.Count - 1;
                if (s[0] == '-')
                {
                    string handleString = s;
                    handleString = handleString.Remove(0, 1);
                    MethodInfo theMethod = typeof(readData).GetMethod(handleString);
                    int c = theMethod.GetParameters().Length;
                    closeparams.Add(c);
                    args[i] = "COMMAND: " + handleString + " " + c.ToString();
                    

                }
                else
                {
                    closeparams[closeparams.Count - 1]--;
                    args[i] = args[i] + ", " + closeparams[closeparams.Count - 1];
                    
                    if (closeparams[closeparams.Count - 1] == 0)
                        closeparams.Remove(closeparams.Last());
                }
                i++;
            }

            foreach(string s in args)
                output(s);
            

            return "";
        }

        public consoleInstance()
        {
            history = "Console begin.\n";
        }
    }
}