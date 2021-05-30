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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Xml;
using System.IO;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace FlatBase.FieldSnippets
{
    /// <summary>
    /// Interaction logic for fsnipBool.xaml
    /// </summary>
    public partial class fsnipCode : UserControl
    {
        public fsnipCode()
        {
            InitializeComponent();
            loadLang();
        }

        /*Assembly assembly = Assembly.GetExecutingAssembly();
using (Stream s = assembly.GetManifestResourceStream("Your.xshd"))
{
    using (XmlTextReader reader = new XmlTextReader(s))
    {
        //Load default Syntax Highlighting
        InternalEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

        // Dynamic syntax highlighting for your own purpose
        var rules = InternalEditor.SyntaxHighlighting.MainRuleSet.Rules;

        _HighlightingRule = new HighlightingRule();
        _HighlightingRule.Color = new HighlightingColor()
        {
                Foreground = new CustomizedBrush(SomeColor)
        };

        String[] wordList = PseudoGetKeywords(); // Your own logic
        String regex = String.Format(@"\b({0})\w*\b", String.Join("|", wordList));
        _HighlightingRule.Regex = new Regex(regex);

        rules.Add(_HighlightingRule);
    }
}*/

        public void loadLang()
        {
            codeBox.SyntaxHighlighting = LoadHighlightingDefinition("");
        }

        public static IHighlightingDefinition LoadHighlightingDefinition(
    string resourceName)
        {
            using (Stream s = File.OpenRead(@"config/Syntax/LUA.xshd"))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    return HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }
    }
}
