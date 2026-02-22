using FutureTech.OpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NModbus;
using System.Net.Sockets;

namespace HJZK.IOController
{
    public class IOBoxApi : IIOBoxApi
    {
        IOBoxConnect _ioBoxConnect = new IOBoxConnect();
        private readonly IOptionsMonitor<IOBoxConfig> _ioOptions;
        private readonly IOBoxConfig _ioBox_Common;
        private readonly IOBoxConfig _ioBox_Special;

        private bool[] DoStatus = new bool[16];
        /// <summary>
        /// 异步读输出线圈
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        /// 
        public IOBoxApi(IServiceProvider serviceProvider, IOptionsMonitor<IOBoxConfig> ioOptions)
        {
            _ioOptions = ioOptions;
            _ioBox_Common = _ioOptions.Get("IO1");
            _ioBox_Special = _ioOptions.Get("IO2");
        }
        public async Task<bool[]> GetDoAsync(IOBoxConfig box)
        {
            try
            {
                await _ioBoxConnect.EnsureConnect(box);
                var statues = await box.Master.ReadCoilsAsync(box.SlaveAddr, (ushort)box.DoItems[0].Adress, (ushort)box.DoItems.Count);
                return statues;
            }
            catch (Exception ex)
            {
                box.Client.Dispose();
                box.Master.Dispose();
                throw;
            }
        }


        public async Task PutDoAsync(IOBoxConfig box)
        {
            try
            {
                await _ioBoxConnect.EnsureConnect(box);
                await box.Master.WriteMultipleCoilsAsync(box.SlaveAddr, (ushort)box.DoItems[0].Adress, DoStatus);
            }
            catch (Exception ex)
            {
                box.Client.Dispose();
                box.Master.Dispose();
                throw;
            }
        }
        /// <summary>
        /// 异步读输入线圈
        /// </summary>
        /// <param name="box">8位IO盒</param>
        /// <returns></returns>
        public async Task<bool[]> GetDIAsync(IOBoxConfig box)
        {
            try
            {
                await _ioBoxConnect.EnsureConnect(box);

                var statues = await box.Master.ReadInputsAsync(box.SlaveAddr, (ushort)box.DiItems[0].Adress, (ushort)box.DiItems.Count);
                return statues;
            }
            catch (Exception ex)
            {
                box.Client.Dispose();

                box.Master.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 异步写多输出线圈
        /// </summary>
        /// <param name="box"></param>
        /// <param name="statues"></param>
        /// <returns></returns>
        public async Task PutMultipleDoAsync(IOBoxConfig box, bool[] statues)
        {
            try
            {
                await _ioBoxConnect.EnsureConnect(box);

                await box.Master.WriteMultipleCoilsAsync(box.SlaveAddr, (ushort)box.DoItems[0].Adress, statues);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 异步写单输出线圈
        /// </summary>
        /// <param name="box"></param>
        /// <param name="adress"></param>
        /// <param name="statue"></param>
        /// <returns></returns>
        public async Task PutSingleDoAsync(IOBoxConfig box, ushort adress, bool statue)
        {
            try
            {
                await _ioBoxConnect.EnsureConnect(box);

                await box.Master.WriteSingleCoilAsync(box.SlaveAddr, adress, statue);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async void PutMultipleCoil(IOBoxConfig box, bool[] statues)
        {
            try
            {
                await _ioBoxConnect.EnsureConnect(box);
                await box.Master.WriteMultipleCoilsAsync(box.SlaveAddr, (ushort)box.DoItems[0].Adress, statues);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async void PutMultipleCoil(IOBoxConfig box, ushort adress, bool[] statues)
        {
            try
            {
                await _ioBoxConnect.EnsureConnect(box);
                await box.Master.WriteMultipleCoilsAsync(box.SlaveAddr, adress, statues);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async void PutsingleCoil(IOBoxConfig box, ushort adress, bool statue)
        {
            await _ioBoxConnect.EnsureConnect(box);
            await box.Master.WriteSingleCoilAsync(box.SlaveAddr, adress, statue);
        }

        public async Task<OpResult<object>> OnRedLed(DoStatusEnum status)
        {
            OpResult<object> result = new OpResult<object>();
            try
            {
                if (_ioBox_Common != null && _ioBox_Common.Enable)
                {
                    if (status == DoStatusEnum.ON)
                        //await PutSingleDoAsync(_ioBox_Common, (int)IOBoxStatusEnum.RedLed, true);
                        DoStatus[(int)IOBoxStatusEnum.RedLed] = true;
                    else if (status == DoStatusEnum.OFF)
                            DoStatus[(int)IOBoxStatusEnum.RedLed] = false;
                        //await PutSingleDoAsync(_ioBox_Common, (int)IOBoxStatusEnum.RedLed, false);

                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "IO box configuration error";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"{ex.Message}";
            }

            return result;
        }

        public async Task<OpResult<object>> OnGreenLed(DoStatusEnum status)
        {
            OpResult<object> result = new OpResult<object>();
            try
            {
                if (_ioBox_Common != null && _ioBox_Common.Enable)
                {
                    if (status == DoStatusEnum.ON)
                        DoStatus[(int)IOBoxStatusEnum.GreenLed] = true;
                    //await PutSingleDoAsync(_ioBox_Common, (int)IOBoxStatusEnum.GreenLed, true);
                    else if (status == DoStatusEnum.OFF)
                        DoStatus[(int)IOBoxStatusEnum.GreenLed] = false;
                    //await PutSingleDoAsync(_ioBox_Common, (int)IOBoxStatusEnum.GreenLed, false);

                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "IO box configuration error";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"{ex.Message}";
            }

            return result;
        }

        public async Task<OpResult<object>> OnYellowLed(DoStatusEnum status)
        {
            OpResult<object> result = new OpResult<object>();
            try
            {
                if (_ioBox_Common != null && _ioBox_Common.Enable)
                {
                    if (status == DoStatusEnum.ON)
                        DoStatus[(int)IOBoxStatusEnum.YellowLed] = true;

                    //await PutSingleDoAsync(_ioBox_Common, (int)IOBoxStatusEnum.YellowLed, true);
                    else if (status == DoStatusEnum.OFF)
                        DoStatus[(int)IOBoxStatusEnum.YellowLed] = false;
                    await PutSingleDoAsync(_ioBox_Common, (int)IOBoxStatusEnum.YellowLed, false);

                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "IO box configuration error";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"{ex.Message}";
            }
            return result;
        }

        public async Task<OpResult<object>> StartAction(DoStatusEnum status)
        {
            OpResult<object> result = new OpResult<object>();
            try
            {
                if (_ioBox_Common != null && _ioBox_Common.Enable)
                {
                    if (status == DoStatusEnum.ON)
                        DoStatus[(int)IOBoxStatusEnum.StartSignl] = true;

                    //await PutSingleDoAsync(_ioBox_Common, (int)IOBoxStatusEnum.StartSignl, true);
                    else if (status == DoStatusEnum.OFF)
                        DoStatus[(int)IOBoxStatusEnum.StartSignl] = false;

                    //await PutSingleDoAsync(_ioBox_Common, (int)IOBoxStatusEnum.StartSignl, false);

                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "IO box configuration error";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"{ex.Message}";
            }

            return result;
        }

        public async Task<OpResult<object>> ResetAction(DoStatusEnum status)
        {
            OpResult<object> result = new OpResult<object>();
            try
            {
                if (_ioBox_Common != null && _ioBox_Common.Enable)
                {
                    if (status == DoStatusEnum.ON)
                        DoStatus[(int)IOBoxStatusEnum.ResetSignl] = true;

                    //await PutSingleDoAsync(_ioBox_Common, (int)IOBoxStatusEnum.ResetSignl, true);
                    else if (status == DoStatusEnum.OFF)
                        DoStatus[(int)IOBoxStatusEnum.ResetSignl] = false;

                    //await PutSingleDoAsync(_ioBox_Common, (int)IOBoxStatusEnum.ResetSignl, false);

                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "IO box configuration error";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"{ex.Message}";
            }

            return result;
        }
        public async Task<OpResult<object>> OnBuzzer(DoStatusEnum status)
        {
            OpResult<object> result = new OpResult<object>();
            try
            {
                if (_ioBox_Common != null && _ioBox_Common.Enable)
                {
                    if (status == DoStatusEnum.ON)
                        DoStatus[(int)IOBoxStatusEnum.Beep] = true;

                    //await PutSingleDoAsync(_ioBox_Common, (int)IOBoxStatusEnum.Beep, true);
                    else if (status == DoStatusEnum.OFF)
                        DoStatus[(int)IOBoxStatusEnum.Beep] = false;

                    //await PutSingleDoAsync(_ioBox_Common, (int)IOBoxStatusEnum.Beep, false);

                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "IO box configuration error";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"{ex.Message}";
            }

            return result;
        }


        public async Task<OpResult<object>> StartSpecialAction(DoStatusEnum status)
        {
            OpResult<object> result = new OpResult<object>();
            try
            {
                if (_ioBox_Special != null && _ioBox_Special.Enable)
                {
                    if (status == DoStatusEnum.ON)
                        DoStatus[(int)IOBoxStatusEnum.StartSpecialSignl] = true;

                    //await PutSingleDoAsync(_ioBox_Special, (int)IOBoxStatusEnum.StartSpecialSignl, true);
                    else if (status == DoStatusEnum.OFF)
                        DoStatus[(int)IOBoxStatusEnum.StartSpecialSignl] = false;

                    //await PutSingleDoAsync(_ioBox_Special, (int)IOBoxStatusEnum.StartSpecialSignl, false);
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "IO box configuration error";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"{ex.Message}";
            }

            return result;
        }

        public async Task<OpResult<object>> OnRedLedSpecial(DoStatusEnum status)
        {
            OpResult<object> result = new OpResult<object>();
            try
            {
                if (_ioBox_Special != null && _ioBox_Special.Enable)
                {
                    if (status == DoStatusEnum.ON)
                        DoStatus[(int)IOBoxStatusEnum.RedLedSpecial] = true;
                    //await PutSingleDoAsync(_ioBox_Special, (int)IOBoxStatusEnum.RedLedSpecial, true);
                    else if (status == DoStatusEnum.OFF)
                        DoStatus[(int)IOBoxStatusEnum.RedLedSpecial] = false;

                    //await PutSingleDoAsync(_ioBox_Special, (int)IOBoxStatusEnum.RedLedSpecial, false);

                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "IO box configuration error";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"{ex.Message}";
            }

            return result;
        }


        public async Task<OpResult<object>> OnTaoTongLed(int taoTongNo, DoStatusEnum status)
        {
            OpResult<object> result = new OpResult<object>();
            try
            {
                if (_ioBox_Common != null && _ioBox_Common.Enable)
                {
                    var list = Convert.ToString(taoTongNo, 2)
                        .PadLeft(3, '0')  //套筒号转二进制字符串
                        .ToList()  //拆成集合
                        .Select(e => e == '1')  //集合转bool
                        .ToList();
                    list.Reverse();//反转集合

                    await PutSingleDoAsync(_ioBox_Common, (int)IOBoxStatusEnum.套筒1, list[0]);
                    await PutSingleDoAsync(_ioBox_Common, (int)IOBoxStatusEnum.套筒2, list[1]);
                    await PutSingleDoAsync(_ioBox_Common, (int)IOBoxStatusEnum.套筒3, list[2]);

                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "IO box configuration error";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"{ex.Message}";
            }

            return result;
        }
    }
}
