using Ctp0600P.Client.PLC.PLC01.Models;
using Microsoft.Extensions.Options;
using StdUnit.Sharp7.Options;

namespace Ctp0600P.Client.PLC.PLC01
{
    public class PlcCtrlFlusher : S7PlcFlusher<PlcScanner, DevMsg, MstMsg>
    {
        public PlcCtrlFlusher(IOptionsMonitor<S7ScanOpt> scanOptsMonitor, PlcScanner scanner) : base(scanOptsMonitor, scanner)
        {
        }

        protected override string PlcName => PLCNames.PLC01;

        public override async Task FlushAsync(MstMsg mstmsg)
        {
            var s7ScanOpt = _scanOptsMonitor.Get(_scanner.ScanName);
            var write = await _scanner.PlcCtrl.WriteDBAsync(s7ScanOpt.MstMsg_DB_INDEX, s7ScanOpt.MstMsg_DB_OFFSET, mstmsg);
            if (write.IsError)
            {
                throw new Exception($"向【{PlcName}】写数据错误：{write.ErrorValue}");
            }
        }
    }

}


