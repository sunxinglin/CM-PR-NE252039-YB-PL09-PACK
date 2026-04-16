using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using System.Net;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace GetMHRUser
{
    public class MesInvoke : MesInvokeBase
    {
        public string SectionNamePath;
        private string authToken;

        public MesInvoke(string configfile, string SectionName, string interfaceName, string paramsconigfile = "")
            : base(configfile, SectionName, interfaceName, paramsconigfile)
        {
            Model = new MesDataModel(configfile, SectionName);
            TxtFile.ExistsPath(SectionName);
            SectionNamePath = SectionName;
        }

        public override List<object> GetResult(List<object> objs)
        {
            StringBuilder Strb_Log = new StringBuilder();
            List<object> ReturnResult = new List<object>();
            MesDataModel model = (MesDataModel)Model;

            // 记录开始时间
            DateTime startTime = DateTime.Now;
            Strb_Log.AppendLine($"========== MES接口调用开始 ==========");
            Strb_Log.AppendLine($"开始时间: {startTime:yyyy-MM-dd HH:mm:ss.fff}");
            Strb_Log.AppendLine($"接口名称: {InterfaceName}");
            Strb_Log.AppendLine($"配置节点: {SectionNamePath}");

            try
            {
                // 记录获取令牌的请求参数
                Strb_Log.AppendLine($"\n--- 获取授权令牌 ---");
                Strb_Log.AppendLine($"请求URL: {model.GetTokenUrl}");
                Strb_Log.AppendLine($"用户名: {model.LoginName}");
                Strb_Log.AppendLine($"密码: [已隐藏]");

                // 1. 获取授权令牌
                var tokenResult = GetAuthToken(model.GetTokenUrl, model.LoginName, model.PassWord, Strb_Log);

                if (!tokenResult.Success)
                {
                    // 获取令牌失败，直接返回错误信息
                    Strb_Log.AppendLine($"获取授权令牌失败!");
                    Strb_Log.AppendLine($"错误码: {tokenResult.ErrorCode}");
                    Strb_Log.AppendLine($"错误信息: {tokenResult.ErrorMessage}");

                    ReturnResult.Add(tokenResult.ErrorCode);
                    ReturnResult.Add(tokenResult.ErrorMessage);
                    return ReturnResult;
                }

                authToken = tokenResult.Token;
                Strb_Log.AppendLine($"授权令牌获取成功");

                // 2. 获取权限接口的响应
                Strb_Log.AppendLine($"\n--- 获取用户权限 ---");
                Strb_Log.AppendLine($"请求URL: {model.GetPermissionsUrl}");
                Strb_Log.AppendLine($"设备资源ID: {model.Resource}");
                Strb_Log.AppendLine($"基地ID: {model.BaseId}");

                var apiResponse = GetPermissions(authToken, model.GetPermissionsUrl, model.Resource, model.BaseId, Strb_Log);

                // 记录接口返回信息
                Strb_Log.AppendLine($"\n--- 接口返回信息 ---");
                Strb_Log.AppendLine($"成功状态: {apiResponse.Success}");
                Strb_Log.AppendLine($"返回代码: {apiResponse.ErrorCode}");
                Strb_Log.AppendLine($"返回信息: {apiResponse.Msg}");

                // 直接返回接口的ErrorCode和Message
                ReturnResult.Add(apiResponse.ErrorCode);
                ReturnResult.Add(apiResponse.Msg);

                if (apiResponse.Success && apiResponse.Data != null)
                {
                    ReturnResult.Add(JsonConvert.SerializeObject(apiResponse.Data.list));
                }
            }
            catch (Exception ex)
            {
                Strb_Log.AppendLine($"\n--- 异常信息 ---");
                Strb_Log.AppendLine($"异常类型: {ex.GetType().Name}");
                Strb_Log.AppendLine($"异常信息: {ex.Message}");
                Strb_Log.AppendLine($"堆栈跟踪: {ex.StackTrace}");

                ReturnResult.Add(231);
                ReturnResult.Add(ex.Message);
                return ReturnResult;
            }
            finally
            {
                // 记录结束时间和总耗时
                DateTime endTime = DateTime.Now;
                TimeSpan duration = endTime - startTime;

                Strb_Log.AppendLine($"\n========== MES接口调用结束 ==========");
                Strb_Log.AppendLine($"结束时间: {endTime:yyyy-MM-dd HH:mm:ss.fff}");
                Strb_Log.AppendLine($"总耗时: {duration.TotalMilliseconds}ms");
                Strb_Log.AppendLine($"返回结果: [{ReturnResult[0]}] {ReturnResult[1]}");

                // 写入日志文件
                txtFile.WriteFile(SectionNamePath, Strb_Log.ToString());
            }

            return ReturnResult;
        }

        private TokenResult GetAuthToken(string url, string username, string password, StringBuilder logBuilder)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 设置合理的超时时间
                    client.Timeout = TimeSpan.FromSeconds(30);

                    var requestData = new
                    {
                        loginName = username,
                        password = password
                    };

                    string jsonContent = JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    logBuilder.AppendLine($"请求参数: {jsonContent}");
                    logBuilder.AppendLine($"请求时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

                    HttpResponseMessage response = client.PostAsync(url, content).Result;

                    // 记录响应信息
                    logBuilder.AppendLine($"响应时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                    logBuilder.AppendLine($"HTTP状态码: {(int)response.StatusCode} {response.StatusCode}");

                    string responseContent = response.Content.ReadAsStringAsync().Result;

                    // 检查HTTP状态码
                    if (!response.IsSuccessStatusCode)
                    {
                        return new TokenResult
                        {
                            Success = false,
                            ErrorCode = (int)response.StatusCode,
                            ErrorMessage = $"HTTP请求失败: {response.StatusCode} - {responseContent}"
                        };
                    }

                    // 尝试解析令牌响应
                    try
                    {
                        dynamic tokenResponse = JsonConvert.DeserializeObject(responseContent);
                        string token = responseContent;

                        // 如果响应是JSON对象，尝试提取令牌字段
                        if (tokenResponse != null && tokenResponse.Authorization != null)
                        {
                            token = tokenResponse.Authorization;
                        }

                        return new TokenResult
                        {
                            Success = true,
                            Token = token
                        };
                    }
                    catch (JsonException jsonEx)
                    {
                        logBuilder.AppendLine($"JSON解析异常: {jsonEx.Message}");
                        return new TokenResult
                        {
                            Success = false,
                            ErrorCode = 500,
                            ErrorMessage = $"令牌响应格式错误: {jsonEx.Message}"
                        };
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                logBuilder.AppendLine($"HTTP请求异常: {httpEx.Message}");
                return new TokenResult
                {
                    Success = false,
                    ErrorCode = 503,
                    ErrorMessage = $"网络连接失败: {httpEx.Message}"
                };
            }
            catch (TaskCanceledException)
            {
                logBuilder.AppendLine($"请求超时异常");
                return new TokenResult
                {
                    Success = false,
                    ErrorCode = 408,
                    ErrorMessage = "请求超时"
                };
            }
            catch (Exception ex)
            {
                logBuilder.AppendLine($"获取令牌异常: {ex.Message}");
                return new TokenResult
                {
                    Success = false,
                    ErrorCode = 500,
                    ErrorMessage = $"获取令牌失败: {ex.Message}"
                };
            }
        }

        private ApiResponse<UserListData> GetPermissions(string token, string url, string deviceResourceId, string baseId, StringBuilder logBuilder)
        {
            using (HttpClient client = new HttpClient())
            {
                var requestData = new
                {
                    device_resource_id = deviceResourceId,
                    base_id = baseId
                };

                string jsonContent = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                logBuilder.AppendLine($"请求参数: {jsonContent}");
                logBuilder.AppendLine($"请求时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

                try
                {
                    dynamic authObj = JsonConvert.DeserializeObject(token);
                    string authorizationHeader = authObj.Authorization;
                    client.DefaultRequestHeaders.Add("Authorization", $"{authorizationHeader}");
                    logBuilder.AppendLine($"授权头: {authorizationHeader}");
                }
                catch (JsonException)
                {
                    // 如果令牌不是JSON格式，直接使用原始令牌
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    logBuilder.AppendLine($"授权头: {token}");
                }

                HttpResponseMessage response = client.PostAsync(url, content).Result;

                // 记录响应信息
                logBuilder.AppendLine($"响应时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                logBuilder.AppendLine($"HTTP状态码: {(int)response.StatusCode} {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                string responseContent = response.Content.ReadAsStringAsync().Result;


                // 反序列化完整响应并直接返回
                return JsonConvert.DeserializeObject<ApiResponse<UserListData>>(responseContent);
            }
        }
    }

    // 令牌获取结果类
    public class TokenResult
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    // API响应包装类
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string Msg { get; set; }
        public T Data { get; set; }
    }

    // 用户列表数据包装类
    public class UserListData
    {
        public long timestamp { get; set; }
        public List<UserLogin> list { get; set; }
    }

    // 用户登录信息类
    public class UserLogin
    {
        public string work_id { get; set; }
        public string card_id { get; set; }
        public string access_level { get; set; }

        public int AccessLevelInt
        {
            get
            {
                if (int.TryParse(access_level, out int result))
                    return result;
                return 0; // 默认值
            }
        }
    }
}