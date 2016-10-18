using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            WebRequest wrq = WebRequest.Create("http://www.baidu.com");
            WebResponse wrs = wrq.GetResponse();
            Stream strm = wrs.GetResponseStream();
            StreamReader sr = new StreamReader(strm);
            string line;
            while((line = sr.ReadLine())!= null)
            {
                listBox1.Items.Add(line);
            }
            strm.Close();
        }
    }
}
