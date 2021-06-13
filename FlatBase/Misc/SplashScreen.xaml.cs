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

using System.Windows.Forms;
using System.IO;

using Newtonsoft.Json;

namespace FlatBase.Misc
{
    [Serializable]
    public class projectFile
    {
        public string name { get; set; }
        public DateTime accessed { get; set; }
        public string configName { get; set; }

        public string file;

        public projectFile(string n, DateTime t, string c)
        {
            name = n;
            accessed = t;
            configName = c;
        }
    }


    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        MainWindow _mw;
        public SplashScreen(MainWindow mw)
        {
            InitializeComponent();
            _mw = mw;

            parseAccess();
        }

        private void Button_New(object sender, RoutedEventArgs e)
        {
            createProjectDirectory();
        }

        private void Button_Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Load Project";
            openFileDialog.Filter = "database files (*.gjde)|*.gjde|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Console.WriteLine("Loading project");
                load(openFileDialog.FileName);
            }
        }

        public void load(string fileName)
        {
            string data = File.ReadAllText(fileName);
            projectFile project = JsonConvert.DeserializeObject<projectFile>(data);

            project.accessed = DateTime.Now;

            var deserializeSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string raw = JsonConvert.SerializeObject(project, deserializeSettings);

            File.WriteAllText(fileName, raw);

            string parentDir = System.IO.Path.GetDirectoryName(fileName);
            Environment.CurrentDirectory = parentDir;
            _mw.newDB();
            _mw.loadData();
            this.Close();
        }

        void createProjectDirectory()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Title = "New Project";
            saveFileDialog.Filter = "database files (*.gjde)|*.gjde|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string configName = "config";
                string name = System.IO.Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                string dir = System.IO.Path.GetDirectoryName(saveFileDialog.FileName) + "/" + configName + "/";
                string parentDir = System.IO.Path.GetDirectoryName(saveFileDialog.FileName) + "/";

                Directory.CreateDirectory(parentDir + "/Tables");

                Directory.CreateDirectory(dir + "/Templates");
                Directory.CreateDirectory(dir + "/Enums");
                Directory.CreateDirectory(dir + "/Syntax");
                

                ///---------------------------------------------------------
                Directory.CreateDirectory(dir + "/System");
                foreach(string f in Directory.GetFiles("defaultconfig/System"))
                {
                    string rawFName = System.IO.Path.GetFileNameWithoutExtension(f);
                    File.Copy(f, dir + "/System/" + rawFName + ".txt");
                }
                ///---------------------------------------------------------
                Directory.CreateDirectory(dir + "/Tags");
                File.WriteAllText(dir + "/Tags/Global.txt", "155 155 155 Default");
                ///---------------------------------------------------------

                File.WriteAllText(dir + "/tagsManifest.txt", "");
                File.WriteAllText(dir + "/enumsManifest.txt", "");

                projectFile pf = new projectFile(name, DateTime.Now, configName);
                var deserializeSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

                string raw = JsonConvert.SerializeObject(pf, deserializeSettings);

                File.WriteAllText(saveFileDialog.FileName, raw);

                string settingsPath = "Settings/accessList.txt";
                string sData = File.ReadAllText(settingsPath);
                sData += "\n" + saveFileDialog.FileName;

                File.WriteAllText(settingsPath, sData);

                Environment.CurrentDirectory = parentDir;
                _mw.newDB();
                this.Close();
            }
        }

        public void parseAccess()
        {
            string path = "Settings/accessList.txt";
            if (File.Exists(path))
            {
                List<projectFile> projects = new List<projectFile>();
                string[] files = File.ReadAllLines(path);

                foreach(string file in files)
                {
                    if (!File.Exists(file))
                        continue;
                    string data = File.ReadAllText(file);
                    projectFile project = JsonConvert.DeserializeObject<projectFile>(data);
                    project.file = file;

                    projects.Add(project);
                }

                projects = projects.OrderBy(o => o.accessed).ToList();
                projects.Reverse();

                foreach(projectFile p in projects)
                {
                    Misc.SplashScreenMisc.ProjectDisplay pd = new SplashScreenMisc.ProjectDisplay(p, p.file, this);
                    projectList.Children.Add(pd);
                }
            }
        }
    }
}
