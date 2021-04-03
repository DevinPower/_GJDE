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

namespace FlatBase.Misc
{
    /// <summary>
    /// Interaction logic for SearchFIlter.xaml
    /// </summary>
    public partial class SearchFilter : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> filters = new ObservableCollection<string>();
        public ObservableCollection<string> FILTERS
        {
            get
            {
                return filters;
            }
            set
            {
                filters = value;
            }
        }

        public SearchFilter()
        {
            InitializeComponent();
            filters.CollectionChanged += ContentCollectionChanged;
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("FILTERS"));
            }
        }

        private void Chip_OnDeleteClick(object sender, RoutedEventArgs e)
        {
            filters.Remove((sender as MaterialDesignThemes.Wpf.Chip).Content.ToString());
            searchStack.Children.Remove(sender as UIElement);
        }

        void autoComplete(string partial)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            autoComplete(searchBox.Text);
            if (searchBox.Text.Length > 0 && searchBox.Text.Last() == ' ')
            {
                string spcRem = searchBox.Text;
                spcRem = spcRem.Replace(" ", "");
                MaterialDesignThemes.Wpf.Chip chip = new MaterialDesignThemes.Wpf.Chip();
                chip.Height = 24;
                chip.IsDeletable = true;
                chip.Content = spcRem;

                Brush Brush = chip.Background;
                if (TagManager.isTag("Global", spcRem))
                    Brush = new SolidColorBrush(TagManager.tagColor("Global", spcRem));
                chip.Background = Brush;
                filters.Add(spcRem);


                chip.DeleteClick += Chip_OnDeleteClick;

                searchStack.Children.Add(chip);
                searchBox.Text = "";

                searchStack.Children.Remove(searchBox);
                searchStack.Children.Add(searchBox);
            }
        }
    }
}
