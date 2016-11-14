using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WebServer.HttpServer
{
    public class CgiHandler
    {
        public static byte[] PhpHandler(HttpResponse response, HttpRequest request, string php_uri, string form)
        {
            byte[] buffer;
            switch (request.Method)
            {
                #region Process GET requests
                case "GET":
                    {
                        Environment.SetEnvironmentVariable("REDIRECT_STATUS", "true");
                        Environment.SetEnvironmentVariable("GATEWAY_INTERFACE", "CGI/1.1");
                        Environment.SetEnvironmentVariable("SCRIPT_FILENAME", php_uri);
                        Environment.SetEnvironmentVariable("QUERY_STRING", form);
                        Environment.SetEnvironmentVariable("REQUEST_METHOD", "GET");

                        ProcessStartInfo pri = new ProcessStartInfo(HttpServer.SITE_PATH + @"\config.bat");
                        pri.UseShellExecute = false;
                        pri.RedirectStandardInput = true;
                        pri.RedirectStandardOutput = true;

                        //pri.Arguments = @"true CGI\1.1 " + php_uri + @" """ + form +  @""" GET";
                        Process handle = Process.Start(pri);
                        System.IO.StreamReader myOutput = handle.StandardOutput;
                        System.IO.StreamWriter myInput = handle.StandardInput;

                        response.Version = request.Version;
                        response.StatusCode = Convert.ToString((int)HttpStatusCode.Ok);
                        response.ReasonPhrase = Convert.ToString(HttpStatusCode.Ok.ToString());
                        response.Header.Add("Server", "Niushen/6.6.66(Niuix) DAV/2 mod_jk/1.2.23");
                        buffer = Encoding.UTF8.GetBytes((response.GetResponse() + myOutput.ReadToEnd()));
                        myInput.Write(-1);

                        if (handle.HasExited == false)
                        {
                            //当php解释进程未正常退出时
                            //强制杀死该进程
                            handle.Kill();
                            handle.Close();
                        }

                        break;
                    }
                #endregion
                #region POST requests
                case "POST":
                    {

                        Environment.SetEnvironmentVariable("REDIRECT_STATUS", "true");
                        Environment.SetEnvironmentVariable("GATEWAY_INTERFACE", "CGI/1.1");
                        Environment.SetEnvironmentVariable("SCRIPT_FILENAME", php_uri);
                        Environment.SetEnvironmentVariable("REQUEST_METHOD", "POST");
                        Environment.SetEnvironmentVariable("CONTENT_LENGTH", request.Header["Content-Length"]);
                        Environment.SetEnvironmentVariable("CONTENT_TYPE", request.Header["Content-Type"]);

                        ProcessStartInfo pri = new ProcessStartInfo(HttpServer.SITE_PATH + @"\config.bat");
                        pri.UseShellExecute = false;
                        pri.RedirectStandardInput = true;
                        pri.RedirectStandardOutput = true;

                        Process handle = Process.Start(pri);
                        System.IO.StreamWriter myInput = handle.StandardInput;
                        System.IO.StreamReader myOutput = handle.StandardOutput;

                        myInput.Write(request.Content + '\n');
                        myInput.Flush();
                        myInput.Close();
                        
                        response.Version = request.Version;
                        response.StatusCode = Convert.ToString((int)HttpStatusCode.Ok);
                        response.ReasonPhrase = Convert.ToString(HttpStatusCode.Ok.ToString());
                        response.Header.Add("Server", "Niushen/6.6.66(Niuix) DAV/2 mod_jk/1.2.23");
                        buffer = Encoding.UTF8.GetBytes((response.GetResponse() + myOutput.ReadToEnd()));

                        if (handle.HasExited == false)
                        {
                            handle.Kill();
                            handle.Close();
                        }

                        break;
                    }
                #endregion
                default:
                    {
                        response = new HttpResponse();
                        response.Version = request.Version;
                        response.StatusCode = Convert.ToString((int)HttpStatusCode.Ok);
                        response.ReasonPhrase = Convert.ToString(HttpStatusCode.Ok.ToString());
                        response.Header.Add("Server", "Niushen/6.6.66(Niuix) DAV/2 mod_jk/1.2.23");
                        buffer = Encoding.UTF8.GetBytes(response.GetResponse());

                        break;
                    }
            }
            return buffer;
        }
    }
}
