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

        private string uri;
        public string Uri
        {
            get { return uri; }
            set
            {
                uri = value;
                if(this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Uri"));
                }
            }
        }

        public string Version { get; set; }

        private string path;
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                if(this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Path"));
                }
            }
        }

        public Dictionary<string, string> Header { get; set; }

        public string Content { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
