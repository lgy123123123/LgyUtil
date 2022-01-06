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
using LgyUtil;

namespace LgyUtil.Net
{
    /// <summary>
    /// 网络请求帮助类
    /// </summary>
    public class NetUtil
    {
        #region Post请求
        /// <summary>
        /// post请求(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求路径</param>
        /// <param name="postData">请求体body,json字符串</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> Post(string url, string postData = "", Dictionary<string, string> dicHeader = null)
        {
            HttpResponseMessage ret = null;
            using (HttpClient client = new HttpClient())
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
        public static async Task<string> Post_ReturnString(string url, string postData = "", Dictionary<string, string> dicHeader = null)
        {
            var response = await Post(url, postData, dicHeader);
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
        public static async Task<T> Post_ReturnModel<T>(string url, string postData = "", Dictionary<string, string> dicHeader = null) where T : class, new()
        {
            return (await Post_ReturnString(url, postData, dicHeader)).DeserializeNewtonJson<T>();
        }
        /// <summary>
        /// post请求，返回请求结果流(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求路径</param>
        /// <param name="postData">请求体body,json字符串</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static async Task<Stream> Post_ReturnStream(string url, string postData = "", Dictionary<string, string> dicHeader = null)
        {
            var response = await Post(url, postData, dicHeader);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }
        #endregion

        #region Get请求
        /// <summary>
        /// Get请求(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求地址，参数加在这里</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> Get(string url, Dictionary<string, string> dicHeader = null)
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
        public static async Task<string> Get_ReturnString(string url, Dictionary<string, string> dicHeader = null)
        {
            var response = await Get(url, dicHeader);
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
        public static async Task<T> Get_ReturnModel<T>(string url, Dictionary<string, string> dicHeader = null) where T : class, new()
        {
            return (await Get_ReturnString(url, dicHeader)).DeserializeNewtonJson<T>();
        }
        /// <summary>
        /// Get请求，返回结果流(每次都会新建一个tcp，linux下可以放心使用，windows下会不会立即释放tcp连接，谨慎使用)
        /// </summary>
        /// <param name="url">请求地址，参数加在这里</param>
        /// <param name="dicHeader">请求头</param>
        /// <returns></returns>
        public static async Task<Stream> Get_ReturnStream(string url, Dictionary<string, string> dicHeader = null)
        {
            var response = await Get(url, dicHeader);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }
        #endregion

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
    }
}