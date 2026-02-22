using Automatic.Protocols.LowerBoxGlue.Models;
using Microsoft.Extensions.Options;

using StdUnit.Sharp7.Options;

namespace Automatic.Protocols.LowerBoxGlue
{
    public class LowerBoxGlueFlusher : S7PlcFlusher<LowerBoxGlueScanner, DevMsg, MstMsg>
    {
        public LowerBoxGlueFlusher(IOptionsMonitor<S7ScanOpt> scanOptsMonitor, LowerBoxGlueScanner scanner) : base(scanOptsMonitor, scanner)
        {
        }

        protected override string PlcName => PLCNames.下箱体涂胶;

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


