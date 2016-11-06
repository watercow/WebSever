using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace WebServer.
HttpServer
{
    public class HttpRequest : INotifyPropertyChanged
    {
        //public string Method { get; set}
        private string method;
        public string Method
        {
            get { return method; }
            set
            {
                method = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Method"));
                }
            }
        }
        public string Uri { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }

        public Dictionary<string, string> Header { get; set; }

        public string Content { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
