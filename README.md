# WebSever
## 第一阶段：基本功能
### 1.HTTPServer.cs
    1.1 定义监听端口：HttpServer(int set_port,string set_addr)
    1.2 端口监听TCP连接：Listen()
### 2.HTTPProcessor.cs
    2.1 请求解析：ClientHandler(Client)
    2.2 接收请求：GetRequest(Stream input, Stream output)
    2.3 读取数据流：Readline(Stream stream)
### 3.FileHandler.cs
    3.1 Handler(HTTPRequest request)
## 第二阶段：功能充钱
## 第三阶段：taking off
