﻿using System;
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

namespace FlatBase.Misc
{
    /// <summary>
    /// Interaction logic for HierarchyItemViewer.xaml
    /// </summary>
    public partial class HierarchyItemViewer : UserControl
    {
        public ObjectStructure refObj;
        public HierarchyItemViewer(string n, Brush c, ObjectStructure sr)
        {
            InitializeComponent();
            labelDisplay.Content = n;
            labelDisplay.Foreground = c;
            refObj = sr;
        }
    }
}
