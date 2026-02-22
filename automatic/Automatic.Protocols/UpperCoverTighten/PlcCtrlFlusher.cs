using Automatic.Protocols.UpperCoverTighten.Models;
using Microsoft.Extensions.Options;

using StdUnit.Sharp7.Options;

namespace Automatic.Protocols.UpperCoverTighten
{
    public class UpperCoverTightenFlusher : S7PlcFlusher<UpperCoverTightenScanner, DevMsg, MstMsg>
    {
        public UpperCoverTightenFlusher(IOptionsMonitor<S7ScanOpt> scanOptsMonitor, UpperCoverTightenScanner scanner) : base(scanOptsMonitor, scanner)
        {
        }

        protected override string PlcName => PLCNames.上盖拧紧;

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


