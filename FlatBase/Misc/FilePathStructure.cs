using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace FlatBase.Misc
{
    public class FilePathStructure : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        string path { get; set; }
        public string PATH
        {
            get 
            {
                return path; 
            }
            set 
            {
                path = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("PATH"));
                
            }
        }


        public FilePathStructure()
        {
            path = "";
        }
    }
}
