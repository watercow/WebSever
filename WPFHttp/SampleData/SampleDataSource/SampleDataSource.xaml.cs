//   *********  请勿修改此文件   *********
//   此文件由设计工具再生成。更改
//   此文件可能会导致错误。
namespace Expression.Blend.SampleData.SampleDataSource
{
    using System; 
    using System.ComponentModel;

// 若要在生产应用程序中显著减小示例数据涉及面，则可以设置
// DISABLE_SAMPLE_DATA 条件编译常量并在运行时禁用示例数据。
#if DISABLE_SAMPLE_DATA
    internal class SampleDataSource { }
#else

    public class SampleDataSource : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public SampleDataSource()
        {
            try
            {
                Uri resourceUri = new Uri("/WpfApplication3;component/SampleData/SampleDataSource/SampleDataSource.xaml", UriKind.RelativeOrAbsolute);
                System.Windows.Application.LoadComponent(this, resourceUri);
            }
            catch
            {
            }
        }

        private ItemCollection _Collection = new ItemCollection();

        public ItemCollection Collection
        {
            get
            {
                return this._Collection;
            }
        }
    }

    public class Item : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _IP = string.Empty;

        public string IP
        {
            get
            {
                return this._IP;
            }

            set
            {
                if (this._IP != value)
                {
                    this._IP = value;
                    this.OnPropertyChanged("IP");
                }
            }
        }

        private string _Port = string.Empty;

        public string Port
        {
            get
            {
                return this._Port;
            }

            set
            {
                if (this._Port != value)
                {
                    this._Port = value;
                    this.OnPropertyChanged("Port");
                }
            }
        }

        private string _Method = string.Empty;

        public string Method
        {
            get
            {
                return this._Method;
            }

            set
            {
                if (this._Method != value)
                {
                    this._Method = value;
                    this.OnPropertyChanged("Method");
                }
            }
        }

        private string _URI = string.Empty;

        public string URI
        {
            get
            {
                return this._URI;
            }

            set
            {
                if (this._URI != value)
                {
                    this._URI = value;
                    this.OnPropertyChanged("URI");
                }
            }
        }

        private string _Version = string.Empty;

        public string Version
        {
            get
            {
                return this._Version;
            }

            set
            {
                if (this._Version != value)
                {
                    this._Version = value;
                    this.OnPropertyChanged("Version");
                }
            }
        }

        private string _Status = string.Empty;

        public string Status
        {
            get
            {
                return this._Status;
            }

            set
            {
                if (this._Status != value)
                {
                    this._Status = value;
                    this.OnPropertyChanged("Status");
                }
            }
        }

        private string _Seq = string.Empty;

        public string Seq
        {
            get
            {
                return this._Seq;
            }

            set
            {
                if (this._Seq != value)
                {
                    this._Seq = value;
                    this.OnPropertyChanged("Seq");
                }
            }
        }
    }

    public class ItemCollection : System.Collections.ObjectModel.ObservableCollection<Item>
    { 
    }
#endif
}
