using Automatic.Protocols.ShoulderGlue.Models;
using Microsoft.Extensions.Options;
using StdUnit.Sharp7.Options;

namespace Automatic.Protocols.ShoulderGlue
{
    public class ShoulderGlueFlusher : S7PlcFlusher<ShoulderGlueScanner, DevMsg, MstMsg>
    {
        public ShoulderGlueFlusher(IOptionsMonitor<S7ScanOpt> scanOptsMonitor, ShoulderGlueScanner scanner) : base(scanOptsMonitor, scanner)
        {
        }

        protected override string PlcName => PLCNames.肩部涂胶;

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


