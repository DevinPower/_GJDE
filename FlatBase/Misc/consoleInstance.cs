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
            string[] args = v.Split(' ');
            readData rd = new readData(this);

            int i = 0;
            while (i < args.Length)
            {
                if (args[i][0] == '!')
                {
                    //TODO: close out previous control
                    switch (args[i])
                    {
                        case "!textBox":
                            //TODO: setup new textbox
                            goto default;
                        default:
                            rd = new readData(this);
                            i++;
                            break;
                    }
                    continue;
                }


                if (args[i][0] == '-')
                {
                    switch (args[i])
                    {
                        case "-clear":
                            history = "";
                            i++;
                            break;
                        case "-o":
                            output(args[i + 1]);
                            i += 2;
                            break;
                        case "-n":
                            rd.name = args[i + 1];
                            i += 2;
                            break;
                        case "-order":
                            rd.order = int.Parse(args[i + 1]);
                            i += 2;
                            break;
                        case "-f":
                            MethodInfo theMethod = typeof(readData).GetMethod(args[i + 1]);
                            int c = theMethod.GetParameters().Length;

                            object[] arguments = new object[c];
                            for (int a = 0; a < c; a++)
                            {
                                arguments[a] = args[i + a + 2];
                            }
                            theMethod.Invoke(rd, arguments);
                            i += 2 + c;
                            break;
                        default:
                            output(args[i] + " not recognized");
                            break;
                    }
                }
            }
        }

        public consoleInstance()
        {
            history = "Console begin.\n";
        }
    }
}