using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace WebServer.HttpServer
{
    public class FileHandler
    {
        public string base_path { set; get; }

        public byte[] Handler(HttpRequest request)
        {
            string resourceUri = request.Uri;
            
            resourceUri = this.base_path + resourceUri.Replace('/', '\\');
            
            byte[] buffer = File.ReadAllBytes(resourceUri);

            MemoryStream ms = new MemoryStream();
            GZipStream gzip = new GZipStream(ms, CompressionMode.Compress);
            gzip.Write(buffer, 0, buffer.Length);
            gzip.Close();

            return ms.ToArray();
            //return buffer;
        }
    }
}
