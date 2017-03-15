# WebSever
## 第一阶段：基本功能HttpServer
### 1.HTTPServer.cs
    1.1 设置务务器端监听端口：HttpServer(int set_port,IPAddress set_addr)
    1.2 配置服务器站点根目录: SITE_PATH [全局static变量]
    1.3 设置服务器协议版本: PROTOCOL_VERSION [全局static变量]
    1.3 启动服务器端口监听TCP连接：Start() [Tcp连接建立后创建ClientHandler的处理线程]
### 2.HTTPProcessor.cs
    2.1 全局HTTP请求处理器：ClientHandler(object Client) [使用object类型以创建线程入口函数]
    2.2 客户端HTTP请求解析：GetRequest(Stream input, Stream output) [返回HttpRequest类实例]
    2.3 服务器HTTP响应报文: GetResponse(HttpRequest request) [返回HttpResponse类实例]
    2.4 服务器HTTP响应发送: WriteResponse(Stream output, HttpResponse response)
    2.3 读取数据流：Readline(Stream stream)
### 3.HTTPMethodHandler.cs
    3.1 Get方法请求处理器: GetMethodHandler(HttpRequest request, HttpResponse response)
### 4.FileHandler.cs
    4.1 解析URI并将媒介内容载入缓冲区buffer: Handler(HTTPRequest request, string encoding) [返回buffer数组]
    4.2 获取文件MIME: GetMimeType(string extension)
    4.3 文件MIME字典：_mappings<string, string>
### 5.HttpException.cs
    异常处理
    基类:HttpException
    子类:
    1. WrongProtocolVersion
    2. NoContent
    3. NotFound
    4. MethodNotAllowed
    5. BadRequest
    6. HTTPVersionNotSupported
### 6.HttpResponse/HttpRequest.cs
    定义了Http请求与响应报文的基本结构
