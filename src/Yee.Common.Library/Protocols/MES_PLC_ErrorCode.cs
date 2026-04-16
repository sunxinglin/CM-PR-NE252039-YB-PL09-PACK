namespace Yee.Common.Library.CommonEnum
{
    public class MES_PLC_ErrorCode
    {
        public Dictionary<int, string> DIC_ErrorCodes = new Dictionary<int, string>();
        public MES_PLC_ErrorCode()
        {
            DIC_ErrorCodes.Add(500, "系统异常，需查询Log文件分析");
            DIC_ErrorCodes.Add(10001,"找不到对应的AGV小车");
            DIC_ErrorCodes.Add(10002,"AGV小车绑定的条码跟PLC请求条码不匹配");
            DIC_ErrorCodes.Add(10003,"MES系统中找不到PLC请求条码对应的产品数据");
            DIC_ErrorCodes.Add(10004,"MES系统中找不到PLC请求条码对应的工艺流程");
            DIC_ErrorCodes.Add(10005,"MES系统中找不到PLC请求条码对应的工序");
            DIC_ErrorCodes.Add(10006,"PLC请求条码的工位任务逻辑顺序有误");
            DIC_ErrorCodes.Add(10007, "重复发起涂胶请求");
            DIC_ErrorCodes.Add(10008, "与前三个Pack比较，涂胶数据不满同值足防呆要求");
            DIC_ErrorCodes.Add(10009, "未找到涂胶开始记录");
            DIC_ErrorCodes.Add(10010, "检测到涂胶数据不合法");
            DIC_ErrorCodes.Add(10011, "涂胶开始时间不合法");
            DIC_ErrorCodes.Add(10012, "涂胶数据不允许0值");
            DIC_ErrorCodes.Add(10013, "未找到对应的涂胶开始记录,请检查AGV车号、下箱体条码是否正确");
            DIC_ErrorCodes.Add(10014, "对应的涂胶已完成,请检查AGV车号、下箱体条码是否正确");
            DIC_ErrorCodes.Add(10015, "MES在当前工位没有收到此AGV的到站信号");

            DIC_ErrorCodes.Add(20001, "程序号或产品类型不正确");
            DIC_ErrorCodes.Add(20002, "拧紧数据为空，无法完成");
            DIC_ErrorCodes.Add(20003, "与前三个Pack比较，拧紧数据不满足同值防呆要求");
            DIC_ErrorCodes.Add(20004, "拧紧结果中的程序号不对");
            DIC_ErrorCodes.Add(20005, "PLC给出的拧紧数量与配置的螺丝数量不一致");

            DIC_ErrorCodes.Add(30001, "未找到状态为写入中的RFID");
            DIC_ErrorCodes.Add(30002, "未找到状态为写入完成的RFID");
            DIC_ErrorCodes.Add(30003, "写入和完成不能同时为True");
            DIC_ErrorCodes.Add(30004, "C接口校验模组码失败");
            DIC_ErrorCodes.Add(30005, "来料不符合当前缓存需求");
            DIC_ErrorCodes.Add(30006, "找不到对应的生产类型信息，请检查PLC的生产类型是否有误");
            DIC_ErrorCodes.Add(30007, "校验的RFID模组信息不是最新的托盘信息");

            DIC_ErrorCodes.Add(40001, "模组编号不合法");
            DIC_ErrorCodes.Add(40002, "未找到模组编号对应的RFID");
            DIC_ErrorCodes.Add(40003, "模组编号已存在");
            DIC_ErrorCodes.Add(40004, "库位已满");
            DIC_ErrorCodes.Add(40005, "未找到对应的入缓存位请求");
            DIC_ErrorCodes.Add(40006, "库位号错误");
            DIC_ErrorCodes.Add(40007, "入缓存位完成和RFID校验不能同为True");

            DIC_ErrorCodes.Add(50001, "小车号和Pack码不能为空");
            DIC_ErrorCodes.Add(50002, "未找到此AGV和Pack对应的进光栅任务记录，或该任务未完成");

            DIC_ErrorCodes.Add(60001, "抓取完成和放入完成不可同时为True");
            DIC_ErrorCodes.Add(60002, "库位或模组号校验失败");
            DIC_ErrorCodes.Add(60003, "不存在此抓取任务");
            DIC_ErrorCodes.Add(60004, "不存在此缓存库位");
            DIC_ErrorCodes.Add(60005, "入箱完成时OK和NG不能同值");
            DIC_ErrorCodes.Add(60006, "取消入箱时 找不到对应的入箱记录，请检查取消入箱的AGV车号，下箱体条码是否正确");
            DIC_ErrorCodes.Add(60007, "取消入箱时 找不到对应的AGV进光栅记录，请检查取消入箱的AGV车号，下箱体条码是否正确");
            DIC_ErrorCodes.Add(60008, "取消入箱时 找不到对应的AGV到位记录，请检查取消入箱的AGV车号，下箱体条码是否正确");
            DIC_ErrorCodes.Add(60009, "在MES没有下发入箱指令时，PLC向MES发起了入箱完成请求，请联系ME或电气工程师处理");
            DIC_ErrorCodes.Add(90000,"C接口调用失败");
            
        }
    }
}
