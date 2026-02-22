using Automatic.Protocols.ModuleTighten.Models;
using Microsoft.Extensions.Options;
using StdUnit.Sharp7.Options;

namespace Automatic.Protocols.ModuleTighten
{
    public class ModuleTightenFlusher : S7PlcFlusher<ModuleTightenScanner, DevMsg, MstMsg>
    {
        public ModuleTightenFlusher(IOptionsMonitor<S7ScanOpt> scanOptsMonitor, ModuleTightenScanner scanner) : base(scanOptsMonitor, scanner)
        {
        }

        protected override string PlcName => PLCNames.模组拧紧;

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


