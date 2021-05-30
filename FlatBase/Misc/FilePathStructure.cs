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
    public class CodeStructure : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        string code { get; set; }
        public string CODE
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CODE"));

            }
        }


        public CodeStructure()
        {
            code = "--new comment";
        }
    }

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
