using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desoutter.ElectricScrewDriver
{
    public class DesoutterApi : IDesoutterApi
    {
        private readonly SendMessage _sendMessage;
        private readonly DesoutterMgr _desoutterMgr;
        public DesoutterApi(SendMessage sendMessage, DesoutterMgr desoutterMgr)
        {
            _desoutterMgr = desoutterMgr;
            _sendMessage = sendMessage;
        }

        public async Task SendMID0001(string key)
        {
            var message = _sendMessage.MID0001_CommunicationStart();
            await _desoutterMgr.DesoutterCtrls[key].Send("MID0001_CommunicationStart",message);
        }
        public async Task SendMID0014(string key)
        {
            var message = _sendMessage.MID0014_Parameter_Set_Selected_Subscribe();
            await _desoutterMgr.DesoutterCtrls[key].Send("MID0014_ParameterSetSelectedSubscribe", message);
        }
        public async Task SendMID0016(string key)
        {
            var message = _sendMessage.MID0016_Parameter_Set_Selected_Acknowledge();
            await _desoutterMgr.DesoutterCtrls[key].Send("MID0016_ParameterSetSelectedAcknowledge", message);
        }
        public async Task SendMID0018(string key,int pset=0)
        {
            var message = _sendMessage.MID0018_Select_Parameter_Set(pset);
            await _desoutterMgr.DesoutterCtrls[key].Send("MID0018_SelectParameterSet", message);
        }
        public async Task SendMID0042(string key)
        {
            var message = _sendMessage.MID0042_Disable_Tool();
            await _desoutterMgr.DesoutterCtrls[key].Send("MID0042_DisableTool", message);
        }
        public async Task SendMID0043(string key)
        {
            var message = _sendMessage.MID0043_Enable_Tool();
            await _desoutterMgr.DesoutterCtrls[key].Send("MID0043_EnableTool", message);
        }
        public async Task SendMID0060(string key)
        {
            var message = _sendMessage.MID0060_Last_Tightening_Result_Data_Subscribe();
            await _desoutterMgr.DesoutterCtrls[key].Send("MID0060_LastTighteningResultDataSubscribe", message);
        }
        public async Task SendMID0062(string key)
        {
            var message = _sendMessage.MID0062_Last_Tightening_Result_Data_Acknowledge();
            await _desoutterMgr.DesoutterCtrls[key].Send("MID0062_LastTighteningResultDataAcknowledge", message);
        }
        public async Task SendMID0070(string key)
        {
            var message = _sendMessage.MID0070_Alarm_Subscribe();
            await _desoutterMgr.DesoutterCtrls[key].Send("MID0070_AlarmSubscribe", message);
        }
        public async Task SendMID0082(string key)
        {
            var message = _sendMessage.MID0082_Set_Time();
            await _desoutterMgr.DesoutterCtrls[key].Send("MID0082_SetTime", message);
        }
        public async Task SendMID0701(string key)
        {
            var message = _sendMessage.MID0701_Tool_List_Upload_Reply();
            await _desoutterMgr.DesoutterCtrls[key].Send("MID0701_ToolListUploadReply", message);
        }
    }
}