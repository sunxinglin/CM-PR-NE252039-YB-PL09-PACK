using System.Globalization;
using System.IO.Ports;
using System.Text;

using AsZero.Core.Services.Messages;

using MediatR;

using Microsoft.Extensions.Logging;

using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.Protocols.AnyLoad_Wifi;

public class AnyLoadCtrl
{
    private readonly ILogger<AnyLoadCtrl> _logger;
    private readonly IMediator _mediator;

    public string? PortName { get; }
    public int Baud { get; }
    public SerialPort _serialPort { get; set; }
    private volatile bool _keepReading;
    private readonly AnyLoadSendMessage _sendMessage;

    private readonly string ResultCommandStartStr = "FF AA ";

    private readonly List<string> ResultCommandEndStr = new() { "30 30 F0 F0", "30 30 00 00", "30 30 F0 00", "30 30 00 F0" };

    private readonly string ResultCommandUnitStr = " 4B 47";
    private readonly string ResultUnitStr = "KG";


    public Base_ProResource _AnyLoadResource;

    public AnyLoadCtrl(Base_ProResource resource, ILogger<AnyLoadCtrl> logger, IMediator mediator,
        AnyLoadSendMessage sendMessage)
    {
        _AnyLoadResource = resource ?? throw new ArgumentException("AnyLoad Resource is invalid");
        _sendMessage = sendMessage;
        _logger = logger;
        _mediator = mediator;
        PortName = _AnyLoadResource.Port;
        Baud = _AnyLoadResource.Baud;
        _serialPort = new SerialPort(_AnyLoadResource.Port, _AnyLoadResource.Baud);
    }

    public async Task ConnectAsync()
    {
        if (!string.IsNullOrEmpty(_AnyLoadResource.Port) && !string.IsNullOrEmpty(_AnyLoadResource.Baud.ToString()))
        {
            Open();
            await _mediator.Publish(new UILogNotification(new LogMessage
                { Content = "连接电子秤[" + _AnyLoadResource.Name + ":" + _AnyLoadResource.Port + "]成功" }));
        }
    }


    public bool IsOpen => _serialPort.IsOpen;

    public async void ReadPort()
    {
        // 串口读到的数据可能是“半包/粘包”，因此使用缓冲区累计数据，再从中按协议帧头/帧尾解析完整帧
        var resultBuffer = new ByteBuffer();

        // _keepReading 在 Open() 中置为 true，在 StopReading() 中置为 false，用于控制读循环退出
        while (_keepReading)
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    // 当前没有可读数据时直接进入下一轮（避免空读）
                    if (_serialPort.BytesToRead == 0)
                    {
                        continue;
                    }

                    // 一次性读取当前串口缓冲区中的所有可读字节
                    byte[] m_recvBytes = new byte[_serialPort.BytesToRead];
                    var responseTask = await _serialPort.BaseStream.ReadAsync(m_recvBytes);

                    if (responseTask > 0)
                    {
                        // 将本次读到的数据追加到累计缓冲
                        resultBuffer.PushByteArray(m_recvBytes);
                    }

                    // 获取当前累计缓冲数据的快照，用于解析（避免边读边改造成混乱）
                    var resultBytes = resultBuffer.ToByteArray();
                    if (resultBytes.Length >= 14)
                    {
                        // 将字节流转成十六进制字符串，便于用字符串查找帧头/帧尾（协议关键字均以 hex 字符串定义）
                        var resultStr = _sendMessage.byteToHexStr(resultBytes);

                        // 查找最后一个帧头，尽量丢弃前面残缺/噪声数据
                        var startStrIndex = resultStr.LastIndexOf(ResultCommandStartStr);
                        if (startStrIndex >= 0)
                        {
                            // 截取从帧头开始的内容
                            resultStr = resultStr.Substring(startStrIndex);

                            // 帧头存在且长度足够时才进入进一步解析（避免后续 Substring 越界）
                            if (resultStr.StartsWith(ResultCommandStartStr) && resultStr.Length >= 35)
                            {
                                int endStrIndex = -1;

                                // 帧尾存在多种可能，找到任意一种即可认为“收到完整帧”
                                foreach (var endStr in ResultCommandEndStr)
                                {
                                    endStrIndex = resultStr.IndexOf(endStr);
                                    if (endStrIndex >= 0) break;
                                }

                                if (endStrIndex >= 0)
                                {
                                    // 丢弃帧尾及其后面的内容，仅保留“帧头 + 数据区”
                                    resultStr = resultStr.Substring(0, endStrIndex);

                                    // 去掉帧头（FF AA），只保留数据区
                                    resultStr = resultStr.Substring(ResultCommandStartStr.Length);

                                    // 协议约定：数据区长度必须为 18（包含：数值字段 + 符号/小数点等控制位）
                                    if (resultStr.Length == 18)
                                    {
                                        // 前 14 个字符为数值字段（hex 表示的 ASCII），先转换为 byte[] 再按 ASCII 解码
                                        string valueStr = resultStr.Substring(0, 14);
                                        resultBytes = _sendMessage.strToHexByte(valueStr);
                                        string strResult =
                                            Encoding.ASCII.GetString(resultBytes, 0, resultBytes.Length);

                                        // 位置 15 的控制位为 F 时，表示负数/异常值；当前实现选择直接置 0（避免上传负数）
                                        if (resultStr.Substring(15, 1) == "F")
                                        {
                                            //strResult = "-" + strResult;
                                            strResult = "0";
                                        }

                                        // 位置 16 表示小数点位数，根据位数在字符串中插入小数点
                                        var pointCount = int.Parse(resultStr.Substring(16, 1));
                                        if (pointCount > 0)
                                        {
                                            strResult = strResult.Substring(0, strResult.Length - pointCount) + "." +
                                                        strResult.Substring(strResult.Length - pointCount);
                                        }
                                        
                                        // 成功解析出一帧后，清空缓冲，避免重复解析旧数据
                                        resultBuffer = new ByteBuffer();

                                        // 将称重结果通过 MediatR 广播给 UI/业务层（UI 侧会订阅 AnyLoadRequest 并更新界面）
                                        AnyLoadRequest request = new AnyLoadRequest
                                        {
                                            AnyLoadContext =
                                                double.Parse(strResult).ToString(CultureInfo.InvariantCulture) +
                                                ResultUnitStr,
                                            AnyLoadPortName = PortName
                                        };
                                        await _mediator.Publish(request);
                                    }
                                }
                            }
                        }
                    }

                    // 长时间收不到完整帧时，缓冲可能持续增长；超过阈值则清空，避免内存/解析成本上升
                    if (resultBuffer.Length > 90)
                    {
                        resultBuffer = new ByteBuffer();
                    }
                }
            }
            catch (Exception ex)
            {
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.称重错误, Name = nameof(AlarmCode.称重错误), Module = AlarmModule.ELECTRONIC_SCALE,
                    Description = $"{PortName},{ex.Message}"
                });
            }

            // 限制读循环频率，避免 CPU 空转；同时给串口设备/驱动留出时间产生数据
            await Task.Delay(500);
        }
    }

    private void Open()
    {
        try
        {
            Close();
            _keepReading = true;
            _serialPort.Open();
            if (!_serialPort.IsOpen)
                _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.称重错误, Name = nameof(AlarmCode.称重错误), Module = AlarmModule.ELECTRONIC_SCALE,
                    Description = $"串口{PortName}打开失败！"
                });
        }
        catch (Exception ex)
        {
            _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.称重错误, Name = nameof(AlarmCode.称重错误), Module = AlarmModule.ELECTRONIC_SCALE,
                Description = $"{PortName},{ex.Message}"
            });
        }
    }

    private void Close()
    {
        StopReading();
        _serialPort.Close();
    }

    private void StopReading()
    {
        if (_keepReading)
        {
            _keepReading = false;
        }
    }

    public void WritePort(byte[] send)
    {
        if (IsOpen)
        {
            _serialPort.Write(send, 0, send.Length);
        }
    }

    public Task ReadWeightData(byte[] message)
    {
        WritePort(message);
        return Task.CompletedTask;
    }
}
