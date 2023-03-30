using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UAParser;

namespace LgyUtil
{
    /// <summary>
    /// 网络请求帮助类
    /// </summary>
    public sealed class NetUtil
    {
        #region Post请求
        private static HttpRequestMessage Post_BuildRequest(string url, string postData = "", Dictionary<string, string> dicHeader = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(url))
            {
                Content = new StringContent(postData)
            };
            if (dicHeader != null)
            {
                foreach (var kvHeader in dicHeader)
                {
                    request.Content.Headers.Add(kvHeader.Key, kvHeader.Value);
                }
            }
            if (dicHeader == null || !dicHeader.ContainsKey("ContentType"))
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return request;
        }
        #region 同步Post
        /// <summary>
        /// post请求(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求路径</param>
        /// <param name="postData">请求体body,json字符串</param>
        /// <param name="dicHeader">请求头</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static HttpResponseMessage Post(string url, string postData = "", Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
        {
            HttpResponseMessage ret = null;
            using (HttpClient client = new HttpClient())
            {
                if (timeout != null)
                {
                    client.Timeout = timeout.Value;
                }
                var request = Post_BuildRequest(url, postData, dicHeader);
                ret = client.SendAsync(request).GetAwaiter().GetResult();
            }
            return ret;
        }
        /// <summary>
        /// post请求，返回请求结果字符串(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求路径</param>
        /// <param name="postData">请求体body,json字符串</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static string Post_ReturnString(string url, string postData = "", Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
        {
            var response = Post(url, postData, dicHeader, timeout);
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
        /// <summary>
        /// post请求，返回模型(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求路径</param>
        /// <param name="postData">请求体body,json字符串</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static T Post_ReturnModel<T>(string url, string postData = "", Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null) where T : class, new()
        {
            return (Post_ReturnString(url, postData, dicHeader, timeout)).DeserializeNewtonJson<T>();
        }
        /// <summary>
        /// post请求，返回请求结果流(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求路径</param>
        /// <param name="postData">请求体body,json字符串</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static Stream Post_ReturnStream(string url, string postData = "", Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
        {
            var response = Post(url, postData, dicHeader, timeout);
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
        }
        #endregion
        #region 异步Post
        /// <summary>
        /// post请求(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求路径</param>
        /// <param name="postData">请求体body,json字符串</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostAsync(string url, string postData = "", Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
        {
            HttpResponseMessage ret = null;
            using (HttpClient client = new HttpClient())
            {
                if (timeout != null)
                {
                    client.Timeout = timeout.Value;
                }
                var request = Post_BuildRequest(url, postData, dicHeader);
                ret = await client.SendAsync(request);
            }
            return ret;
        }
        /// <summary>
        /// post请求，返回请求结果字符串(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求路径</param>
        /// <param name="postData">请求体body,json字符串</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static async Task<string> PostAsync_ReturnString(string url, string postData = "", Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
        {
            var response = await PostAsync(url, postData, dicHeader, timeout);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// post请求，返回模型(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求路径</param>
        /// <param name="postData">请求体body,json字符串</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static async Task<T> PostAsync_ReturnModel<T>(string url, string postData = "", Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null) where T : class, new()
        {
            return (await PostAsync_ReturnString(url, postData, dicHeader, timeout)).DeserializeNewtonJson<T>();
        }
        /// <summary>
        /// post请求，返回请求结果流(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求路径</param>
        /// <param name="postData">请求体body,json字符串</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static async Task<Stream> PostAsync_ReturnStream(string url, string postData = "", Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
        {
            var response = await PostAsync(url, postData, dicHeader, timeout);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }
        #endregion
        #endregion

        #region Get请求
        #region 同步Get
        /// <summary>
        /// Get请求(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求地址，参数加在这里</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static HttpResponseMessage Get(string url, Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
        {
            HttpResponseMessage ret = null;
            using (HttpClient client = new HttpClient())
            {
                if (dicHeader != null)
                {
                    foreach (var kvHeader in dicHeader)
                    {
                        client.DefaultRequestHeaders.Add(kvHeader.Key, kvHeader.Value);
                    }
                }
                if (timeout != null)
                {
                    client.Timeout = timeout.Value;
                }
                ret = client.GetAsync(new Uri(url)).GetAwaiter().GetResult();
            }
            return ret;
        }
        /// <summary>
        /// Get请求，返回结果字符串(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求地址，参数加在这里</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static string Get_ReturnString(string url, Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
        {
            var response = Get(url, dicHeader, timeout);
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
        /// <summary>
        /// Get请求，返回模型(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求路径</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static T Get_ReturnModel<T>(string url, Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null) where T : class, new()
        {
            return (Get_ReturnString(url, dicHeader, timeout)).DeserializeNewtonJson<T>();
        }
        /// <summary>
        /// Get请求，返回结果流(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求地址，参数加在这里</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static Stream Get_ReturnStream(string url, Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
        {
            var response = Get(url, dicHeader, timeout);
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
        }
        #endregion
        #region 异步Get
        /// <summary>
        /// Get请求(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求地址，参数加在这里</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
        {
            HttpResponseMessage ret = null;
            using (HttpClient client = new HttpClient())
            {
                if (dicHeader != null)
                {
                    foreach (var kvHeader in dicHeader)
                    {
                        client.DefaultRequestHeaders.Add(kvHeader.Key, kvHeader.Value);
                    }
                }
                if (timeout != null)
                {
                    client.Timeout = timeout.Value;
                }
                ret = await client.GetAsync(new Uri(url));
            }
            return ret;
        }
        /// <summary>
        /// Get请求，返回结果字符串(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求地址，参数加在这里</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static async Task<string> GetAsync_ReturnString(string url, Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
        {
            var response = await GetAsync(url, dicHeader, timeout);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// Get请求，返回模型(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求路径</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static async Task<T> GetAsync_ReturnModel<T>(string url, Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null) where T : class, new()
        {
            return (await GetAsync_ReturnString(url, dicHeader, timeout)).DeserializeNewtonJson<T>();
        }
        /// <summary>
        /// Get请求，返回结果流(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求地址，参数加在这里</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static async Task<Stream> GetAsync_ReturnStream(string url, Dictionary<string, string> dicHeader = null, TimeSpan? timeout = null)
        {
            var response = await GetAsync(url, dicHeader, timeout);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }
        #endregion
        #endregion

        /// <summary>
        /// 发送Server-Send-Events，前端使用EventSource接收
        /// </summary>
        /// <param name="response">当前请求的Response对象</param>
        /// <param name="id">当前事件id，用于前端断开重连时使用</param>
        /// <param name="data">发送的数据</param>
        /// <param name="eventName">前端接收的事件名称</param>
        /// <param name="retry">多长时间前端再次发送消息</param>
        public static void SendSseMessage(HttpResponse response, string id, string data, string eventName, TimeSpan retry)
        {
            //浏览器规定返回类型
            response.ContentType = "text/event-stream";
            string writeString = $"retry:{retry.Milliseconds}\nevent:{eventName}\nid:{id}\ndata:{data}\n\n";
            var writeBytes = writeString.ToByteArr(Encoding.UTF8);//必须用utf8格式的内容
            response.Body.Write(writeBytes, 0, writeBytes.Length);
            response.Body.Flush();
        }

        /// <summary>
        /// 发送Server-Send-Events，前端使用EventSource接收
        /// </summary>
        /// <param name="response">当前请求的Response对象</param>
        /// <param name="id">当前事件id，用于前端断开重连时使用</param>
        /// <param name="data">发送的数据</param>
        /// <param name="eventName">前端接收的事件名称</param>
        /// <param name="retry">多长时间前端再次发送消息</param>
        /// <returns></returns>
        public static async Task SendSseMessageAsync(HttpResponse response, string id, string data, string eventName, TimeSpan retry)
        {
            //浏览器规定返回类型
            response.ContentType = "text/event-stream";
            string writeString = $"retry:{retry.Milliseconds}\nevent:{eventName}\nid:{id}\ndata:{data}\n\n";
            var writeBytes = writeString.ToByteArr(Encoding.UTF8);//必须用utf8格式的内容
            await response.Body.WriteAsync(writeBytes, 0, writeBytes.Length);
            await response.Body.FlushAsync();
        }

        /// <summary>
        /// 检查服务器端口是否占用
        /// </summary>
        /// <param name="iPort">端口号</param>
        /// <returns></returns>
        public static bool CheckPortInUse(int iPort)
        {
            IPEndPoint[] activeTcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            return activeTcpListeners.Any(ipe => ipe.Port == iPort);
        }
        /// <summary>
        /// 获取ip
        /// 若没有匹配到headerName，返回RemoteIpAddress
        /// </summary>
        /// <param name="context"></param>
        /// <param name="headerName"></param>
        /// <returns></returns>
        public static string GetIp(HttpContext context, string headerName = null)
        {
            if (context is null)
                return "";
            if (!string.IsNullOrEmpty(headerName) && context.Request.Headers.ContainsKey(headerName))
                return context.Request.Headers[headerName];
            if (context.Connection.RemoteIpAddress.IsIPv4MappedToIPv6)
                return context.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//改成ipv4格式的ip
            else
                return context.Connection.RemoteIpAddress.ToString();
        }
        /// <summary>
        /// 通过nginx获取ip，nginx需要配置X-Real-IP节点
        /// 没有配置的话，返回RemoteIpAddress
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetIp_Nginx(HttpContext context)
        {
            return GetIp(context, "X-Real-IP");
        }

        private static Parser parser;
        private static Parser ParserObj
        {
            get
            {
                if (parser is null)
                    parser = Parser.GetDefault(new ParserOptions
                    {
                        //1秒超时
                        MatchTimeOut = TimeSpan.FromSeconds(1),
                        //使用编译的正则
                        UseCompiledRegex = true
                    });
                return parser;
            }
        }

        /// <summary>
        /// 解析UserAgent(引用了UAParser)，OS:操作系统信息  Device:设备信息   UA:浏览器信息
        /// </summary>
        /// <param name="userAgent">请求的UserAgent</param>
        /// <returns></returns>
        public static ClientInfo GetUserAgentDetail(string userAgent)
        {
            return ParserObj.Parse(userAgent);
        }
    }
}