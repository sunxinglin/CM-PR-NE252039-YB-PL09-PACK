using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RogerTech.Common;
using RogerTech.Common.Models;
using RogerTech.Tool;
using SqlSugar;

namespace RogerTech.BussnessCore.Bussness
{
    public class PlcProcessGetMHRUsers : PlcInProgressBase
    {
        private readonly MesInterface _mesInterface;
        public PlcProcessGetMHRUsers(string groupName, MesInterface mesInterface) : base(groupName, mesInterface)
        {
            _mesInterface = mesInterface;
        }

        public static List<MHRUser> ParseMesResponse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<MHRUser>();
            }
            try
            {
                //// 直接反序列化到 MHRUser 列表（忽略额外字段如 AccessLevelInt）
                //var list = JsonConvert.DeserializeObject<List<MHRUser>>(json);
                //return list ?? new List<MHRUser>();
                var list = JsonConvert.DeserializeObject<List<MHRUser>>(json);

                // 高性能设置更新时间（避免foreach）
                if (list?.Count > 0)
                {
                    DateTime updateTime = DateTime.Now;
                    list.ForEach(user => user.updatetime = updateTime);
                }
                return list ?? new List<MHRUser>();
            }
            catch (JsonException)
            {
            }
            return new List<MHRUser>();
        }

        // 泛型安全取值方法
        private T GetValue<T>(Dictionary<string, JToken> record, string key, T defaultValue = default)
        {
            if (record.TryGetValue(key, out JToken token))
            {
                try
                {
                    return token.ToObject<T>();
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        public override void Execute(Group group)
        {
            base.Execute(group);
            Task.Run(() => { DbContext.Info("", $"收到plc登录请求", 0, PlcGroup.GroupName); });
            WriteFinishSignal(true);
            StringBuilder message = new StringBuilder();
            BussnessUtility business = BussnessUtility.GetInstance();

            try
            {
                #region 查询本地数据库下发用户等级
                Tag userCardId = group.GetTag("UserCardId");
                Tag workId = group.GetTag("WorkId");
                Tag accessLevel = group.GetTag("AccessLevel");
                if (userCardId == null)
                    OnTagNullError(userCardId.TagName, group.GroupName);
                string cardID = userCardId.Result.Value?.ToString();

                if (string.IsNullOrEmpty(cardID))
                {
                    message.Append($"获取信息失败:传输Card ID为空");
                    return;
                }
                MHRUser user = DbContext.GetInstance().Queryable<MHRUser>().Where(r => r.card_id == cardID).First();
                //查询user，需要考虑首次数据库为空的情况。根据查询到的updatetime判断是否调用接口更新数据库。更新数据库后再次查询更新user
                if (user == null)
                {
                    DateTime? lastUpdateTime = DbContext.GetInstance().Queryable<MHRUser>()
                                                        .OrderBy(r => r.updatetime, OrderByType.Desc)
                                                        .Select(r => r.updatetime)
                                                        .First();

                    DateTime timetmp;
                    if (lastUpdateTime.HasValue)
                    {
                        timetmp = GetLatestUpdateTime(lastUpdateTime.Value);
                    }
                    else
                    {
                        timetmp = DateTime.MinValue;
                    }
                    if (DateTime.Now - timetmp > TimeSpan.FromHours(12))
                    {
                        message.Append("距离上次更新已超过12小时，开始更新用户数据库。");

                        //空循环模式
                        if (business.bMesSimulation)
                        {
                            return;
                        }

                        // 将卡号作为参数传入
                        List<object> inputs = new List<object> { cardID };
                        List<object> output = business.MesInvoke(inputs, _mesInterface);
                        if ((int)output[0] == 0)
                        {
                            message.Append("调用mes接口[GetMHRUser]更新用户成功");
                            // 接口成功：解析返回数据并更新数据库
                            List<MHRUser> newUsers = ParseMesResponse(output[2].ToString());
                            if (newUsers == null)
                            {
                                throw new Exception("调用mes接口[GetMHRUser]用户信息转换错误");
                            }
                            DbContext.GetInstance().Insertable(newUsers).ExecuteCommand();
                            // 更新后再次查询
                            user = DbContext.GetInstance().Queryable<MHRUser>()
                                            .Where(r => r.card_id == cardID)
                                            .First();

                            if (user == null)
                            {
                                // 仍不存在：返回错误
                                message.Append("更新用户信息后，本地数据库仍未找到该用户");
                                accessLevel.WriteValue(7);
                                WriteFinishSignal(false);
                            }
                            else
                            {
                                workId.WriteValue(user.work_id);
                                accessLevel.WriteValue(user.access_level);
                                WriteFinishSignal(false);
                                message.Append($"下发用户信息成功");
                            }
                        }
                        else
                        {
                            accessLevel.WriteValue(7);
                            WriteFinishSignal(false);
                            message.Append($"调用mes接口[GetMHRUser]更新用户失败，MES代码[{output[0]}] MES信息[{output[1]}]");
                        }
                    }
                    else
                    {
                        message.Append("本地数据库未找到该用户");
                        accessLevel.WriteValue(7);
                        WriteFinishSignal(false);
                    }
                }
                else
                {
                    workId.WriteValue(user.work_id);
                    accessLevel.WriteValue(user.access_level);
                    WriteFinishSignal(false);
                    message.Append($"下发用户信息成功");
                }
                #endregion
            }

            catch (Exception ex)
            {
                message.Append(ex.Message);
            }
            finally
            {
                WriteFinishSignal(false);
                Task.Run(() => { DbContext.Info("获取用户", message.ToString(), 0, PlcGroup.GroupName); });
            }
        }


        private DateTime GetLatestUpdateTime(DateTime referenceTime)
        {
            DateTime datePart = referenceTime.Date;
            DateTime eightAM = datePart.AddHours(8);
            DateTime eightPM = datePart.AddHours(20);

            if (referenceTime < eightAM)
            {
                return datePart.AddDays(-1).AddHours(20);
            }
            if (referenceTime < eightPM)
            {
                return eightAM;
            }
            return eightPM;
        }

    }
}
