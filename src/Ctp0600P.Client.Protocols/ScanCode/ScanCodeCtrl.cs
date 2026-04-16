using System.IO.Ports;
using System.Text;

using AsZero.Core.Services.Messages;

using Ctp0600P.Client.Protocols.ScanCode.Models;

using MediatR;

using Microsoft.Extensions.Logging;

using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.Protocols.ScanCode;

public class ScanCodeCtrl
{
    private readonly ILogger<ScanCodeCtrl> _logger;
    private readonly IMediator _mediator;

    public string? PortName { get; }
    public int Baud { get; }
    public SerialPort _serialPort { get; set; }
    private volatile bool _keepReading;
    private int ReadTimeSpan;

    public Base_ProResource _scanCodeResource;

    public ScanCodeCtrl(Base_ProResource resource, ILogger<ScanCodeCtrl> logger, IMediator mediator)
    {
        ReadTimeSpan = 500;

        _scanCodeResource = resource ?? throw new ArgumentException("ScanCode Resource is invalid");
        _logger = logger;
        _mediator = mediator;
        PortName = _scanCodeResource.Port;
        Baud = _scanCodeResource.Baud;
        _serialPort = new SerialPort(_scanCodeResource.Port, _scanCodeResource.Baud);
    }

    public async Task ConnectAsync()
    {
        if (!string.IsNullOrEmpty(_scanCodeResource.Port) &&
            !string.IsNullOrEmpty(_scanCodeResource.Baud.ToString()))
        {
            Open();
            await _mediator.Publish(new UILogNotification(new LogMessage
            {
                Timestamp = DateTime.Now,
                Content = "连接扫码枪[" + _scanCodeResource.Name + ":" + _scanCodeResource.Port + "]成功"
            }));
        }
    }

    public bool IsOpen => _serialPort.IsOpen;

    public async void ReadPort()
    {
        var resultBuffer = new ByteBuffer();
        while (_keepReading)
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    if (_serialPort.BytesToRead == 0) continue;

                    int result = 0;
                    // 定义缓冲区大小
                    byte[] m_recvBytes = new byte[_serialPort.BytesToRead];
                    // 从串口读取数据
                    var responseTask =
                        // ToOptimized:CA1835:请将“ReadAsync”方法调用更改为使用“Stream.ReadAsync(Memory<byte>,CancellationToken)”重载
                        await _serialPort.BaseStream.ReadAsync(m_recvBytes, 0, m_recvBytes.Length);

                    if (responseTask > 0)
                    {
                        resultBuffer.PushByteArray(m_recvBytes);
                    }

                    m_recvBytes = null;

                    var resultBytes = resultBuffer.ToByteArray();
                    if (resultBytes.Length > 1)
                    {
                        var strResult = Encoding.ASCII.GetString(resultBytes, 0, resultBytes.Length);
                        if (!string.IsNullOrEmpty(strResult) && (strResult.EndsWith("\r") || strResult.EndsWith("\n") ||
                                                                 strResult.EndsWith("\t")))
                        {
                            resultBuffer.Initialize();

                            var content = strResult.TrimEnd('\r', '\n', '\t');
                            if (!string.IsNullOrEmpty(content))
                            {
                                var request = new ScanCodeGunRequest
                                {
                                    ScanCodeContext = content,
                                    ScanCodePortName = PortName
                                };
                                await _mediator.Publish(request);
                            }
                        }
                    }

                    resultBytes = null;
                }
            }
            catch (Exception ex)
            {
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.扫码枪错误, Name = nameof(AlarmCode.扫码枪错误), Module = AlarmModule.SCAN_CODE_GUN_MODULE,
                    Description = $"{PortName},{ex.Message}"
                });
            }

            await Task.Delay(ReadTimeSpan);
            await Task.CompletedTask;
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
                    Code = AlarmCode.扫码枪错误, Name = nameof(AlarmCode.扫码枪错误),
                    Module = AlarmModule.SCAN_CODE_GUN_MODULE, Description = $"串口{PortName}打开失败！"
                });
        }
        catch (Exception ex)
        {
            _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.扫码枪错误, Name = nameof(AlarmCode.扫码枪错误), Module = AlarmModule.SCAN_CODE_GUN_MODULE,
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

    /// <summary>
    /// 向扫码枪所在的串口写入数据
    /// </summary>
    /// <param name="send"></param>
    /// <param name="offSet"></param>
    /// <param name="count"></param>
    public void WritePort(byte[] send, int offSet, int count)
    {
        if (IsOpen)
        {
            _serialPort.Write(send, offSet, count);
        }
    }
}