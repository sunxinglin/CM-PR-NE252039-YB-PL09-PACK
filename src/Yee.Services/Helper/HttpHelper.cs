using System.Net;
using System.Text;

using Newtonsoft.Json;

namespace Yee.Services.Helper;

public static class HttpHelper
{
    /// <summary>
    /// url为请求的网址，param参数为需要查询的条件（服务端接收的参数，没有则为null）
    /// </summary>
    /// <param name="url"></param>
    /// <param name="param"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    public async static Task<string> GetAsync(string url, Dictionary<string, string> param = null, Dictionary<string, string> headers = null)
    {
        //ConfigurationManager.RefreshSection("appSettings");

        try
        {

            if (param != null) //有参数的情况下，拼接url
            {
                url = url + "?";
                url = param.Aggregate(url, (current, item) => current + item.Key + "=" + item.Value + "&");
                url = url.Substring(0, url.Length - 1);
            }

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;//创建请求
            request.Timeout = 10000;
            request.ReadWriteTimeout = 10000;
            request.ContinueTimeout = 1;
            request.Method = "GET"; //请求方法为GET
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    request.Headers[item.Key] = item.Value;
                }
            }
            HttpWebResponse res; //定义返回的response

            res = (HttpWebResponse)request.GetResponse(); //此处发送了请求并获得响应
            ///响应转化为String字符串
            StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
            string content = sr.ReadToEnd();
            return content;
        }
        catch (WebException ex)
        {
            ///添加重连机制 用管理账户获取token
            if (ex.Message == "The remote server returned an error: (401) Unauthorized.")
            {
                return await GetAsync(url, param, headers);
            }
            // TimeOut TODO 
            return null;
        }



    }
    /// <summary>
    /// url为请求的网址，param参数为需要查询的条件（服务端接收的参数，没有则为null）
    /// </summary>
    /// <param name="url"></param>
    /// <param name="param"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    public static string Get(string url, Dictionary<string, string> param = null, Dictionary<string, string> headers = null)
    {
        //ConfigurationManager.RefreshSection("appSettings");
        try
        {

            if (param != null) //有参数的情况下，拼接url
            {
                url = url + "?";
                url = param.Aggregate(url, (current, item) => current + item.Key + "=" + item.Value + "&");
                url = url.Substring(0, url.Length - 1);
            }

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;//创建请求
            request.Timeout = 10000;
            request.ReadWriteTimeout = 10000;
            request.ContinueTimeout = 1;
            request.Method = "GET"; //请求方法为GET
            HttpWebResponse res; //定义返回的response
            res = (HttpWebResponse)request.GetResponse(); //此处发送了请求并获得响应
            ///响应转化为String字符串
            StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
            string content = sr.ReadToEnd();
            return content;
        }
        catch (WebException ex)
        {
            ///添加重连机制 用管理账户获取token
            if (ex.Message == "The remote server returned an error: (401) Unauthorized.")
            {
                return Get(url, param, headers);
            }

            // TimeOut TODO 

            return null;
        }
    }
    /// <summary>
    /// url为请求的网址，param为需要传递的参数
    /// </summary>
    /// <param name="url"></param>
    /// <param name="param"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    public static string Post(string url, Dictionary<String, object> param, Dictionary<string, string> headers = null)
    {

        try
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest; //创建请求
            CookieContainer cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = true;
            request.MaximumResponseHeadersLength = 1024;
            request.Timeout = 30000;
            request.Method = "POST"; //请求方式为post
            request.ContentType = "application/json";
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    request.Headers[item.Key] = item.Value;
                }
            }

            string jsonstring = string.Empty;//获得参数的json字符串
            if (param.Count != 0) //将参数添加到json对象中
            {
                jsonstring = JsonConvert.SerializeObject(param);
            }

            byte[] jsonbyte = Encoding.UTF8.GetBytes(jsonstring);
            Stream postStream = request.GetRequestStream();
            postStream.Write(jsonbyte, 0, jsonbyte.Length);
            postStream.Close();
            //发送请求并获取相应回应数据       
            HttpWebResponse res = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
            string content = sr.ReadToEnd(); //获得响应字符串
            return content;
        }
        catch (Exception ex)
        {
            // 添加重连机制 用管理账户获取token
            if (ex.Message == "The remote server returned an error: (401) Unauthorized.")
            {
            }

            return null;
        }
    }


    public static string PostJson(string _url, string jsonParam, Dictionary<string, string> headers = null)
    {
        //request
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(_url);
            request.Timeout = 10000;
            request.ReadWriteTimeout = 10000;
            request.ContinueTimeout = 1;
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            byte[] byteData = Encoding.UTF8.GetBytes(jsonParam);
            int length = byteData.Length;
            request.ContentLength = length;
            Stream writer = request.GetRequestStream();
            writer.Write(byteData, 0, length);
            writer.Close();
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
            return responseString;
        }
        catch (Exception ex)
        {

            if (ex.Message == "The remote server returned an error: (401) Unauthorized.")
            {
                return PostJson(_url, jsonParam, headers);
            }

            return null;
        }

    }

    /// <summary>
    /// url为请求的网址，param为需要传递的参数 返回文件类型数据
    /// </summary>
    /// <param name="url"></param>
    /// <param name="param"></param>
    /// <param name="paramStr"></param>
    /// <returns></returns>
    public static MemoryStream PostFile(string url, Dictionary<String, object> param, string paramStr = null)
    {

        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest; //创建请求
        CookieContainer cookieContainer = new CookieContainer();
        request.CookieContainer = cookieContainer;
        request.AllowAutoRedirect = true;
        //request.AllowReadStreamBuffering = true;
        request.MaximumResponseHeadersLength = 1024;
        request.Method = "POST"; //请求方式为post
        request.AllowAutoRedirect = true;
        request.MaximumResponseHeadersLength = 1024;
        request.ContentType = "application/json";

        string jsonstring = string.Empty;
        if (string.IsNullOrEmpty(paramStr))
        {
            if (param != null)
            {
                jsonstring = JsonConvert.SerializeObject(param);
            }
        }
        else
        {
            jsonstring = paramStr;
        }

        byte[] jsonbyte = Encoding.UTF8.GetBytes(jsonstring);
        Stream postStream = request.GetRequestStream();
        postStream.Write(jsonbyte, 0, jsonbyte.Length);
        postStream.Close();
        //发送请求并获取相应回应数据       
        HttpWebResponse res;
        try
        {
            res = (HttpWebResponse)request.GetResponse();
        }
        catch (WebException)
        {
            throw new WebException("API连接失败");
        }
        Stream resultStream = res.GetResponseStream();
        MemoryStream ms = new MemoryStream();
        byte[] buffer = new byte[1024];
        while (true)
        {
            int sz = resultStream.Read(buffer, 0, 1024);
            if (sz == 0) break;
            ms.Write(buffer, 0, sz);
        }
        //string content = Encoding.UTF8.GetString(ms.ToArray()); 
        return ms;
    }
}