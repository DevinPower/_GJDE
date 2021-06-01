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
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using Microsoft.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

using Microsoft.Win32;

namespace FlatBase.Assistant
{
    /// <summary>
    /// Interaction logic for SetupWizard.xaml
    /// </summary>
    public partial class TagWizard : Window
    {
        public Dictionary<string, ObservableCollection<UserTag>> tagsRef { get; set; }
        UserTag curTag;

        public TagWizard(Dictionary<string, ObservableCollection<UserTag>> tagdb)
        {
            InitializeComponent();
            /*                    Misc.FilePathStructure fp = o as Misc.FilePathStructure;
                    FieldSnippets.fsnipSprite sp = new FieldSnippets.fsnipSprite(fp);


                    Binding lhs = new Binding();
                    lhs.Path = new PropertyPath("PATH");
                    lhs.Source = o;
                    lhs.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    Binding rhs = new Binding();
                    rhs.Path = new PropertyPath("PATH");
                    rhs.Source = o;
                    rhs.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    sp.Path.SetBinding(TextBox.TextProperty, lhs);
                    sp.Render.SetBinding(Image.SourceProperty, rhs);

                    tar.Children.Add(sp);
                    c++;
                    continue;*/

            tagsRef = tagdb;

            Binding lhs = new Binding();
            lhs.Path = new PropertyPath("tagsRef");
            lhs.Source = this;
            lhs.Converter = new Misc.TagManagerConverter();
            lhs.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            loadedTags.SetBinding(ListView.ItemsSourceProperty, lhs);
        }

        private void loadedTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserTag t = (loadedTags.SelectedItem as tagDisplay).getTag();
            tagColorPicker.DataContext = t;
            tagColorPicker.SetBinding(MaterialDesignThemes.Wpf.ColorPicker.ColorProperty, new Binding() { Path = new PropertyPath("color"), Source = t });

            curTag = t;

            Path.DataContext = t;
            Path.SetBinding(TextBox.TextProperty, new Binding() { Path = new PropertyPath("iconPath"), Source = t });
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            TagManager.updateTags();
            this.Close();
        }

        private void Button_Open(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                curTag.iconPath = openFileDialog.FileName;
        }
    }
}