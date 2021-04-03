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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;

namespace FlatBase.FieldSnippets
{
    /// <summary>
    /// Interaction logic for SearchFIlter.xaml
    /// </summary>
    public partial class fsnipTags : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public TagField refTags;

        public fsnipTags(List<UserTag> options, TagField tf)
        {
            InitializeComponent();
            refTags = tf;

            foreach(UserTag t in options)
            {
                searchBox.Items.Add(t.name);
            }

            //refTags.value.CollectionChanged += ContentCollectionChanged;
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("attempted change");
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("FILTERS"));
                Console.WriteLine("changed");
            }
        }

        private void Chip_OnDeleteClick(object sender, RoutedEventArgs e)
        {
            //filters.Remove((sender as MaterialDesignThemes.Wpf.Chip).Content.ToString());
            tagList.Items.Remove(sender);
            //searchStack.Children.Remove(sender as UIElement);
        }

        void autoComplete(string partial)
        {

        }

        private void searchBox_SelectionChanged(object sender, EventArgs e)
        {
            autoComplete(searchBox.Text);
            if (searchBox.Text.Length > 0)
            {
                string spcRem = searchBox.Text;
                /*MaterialDesignThemes.Wpf.Chip chip = new MaterialDesignThemes.Wpf.Chip();
                chip.Height = 24;
                chip.IsDeletable = true;
                chip.Content = spcRem;

                Brush brush = chip.Background;
                if (TagManager.isTag("Global", spcRem))
                    brush = new SolidColorBrush(TagManager.tagColor("Global", spcRem));
                chip.Background = brush;*/


                //chip.DeleteClick += Chip_OnDeleteClick;

                //searchStack.Children.Add(chip);
                //tagList.Items.Add(chip);

                refTags.value.Add(spcRem);
                Console.WriteLine("added tag");
                searchStack.Children.Remove(searchBox);
                searchStack.Children.Add(searchBox);
            }
        }
    }
}
