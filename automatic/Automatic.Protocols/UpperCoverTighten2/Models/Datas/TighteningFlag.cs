namespace Automatic.Protocols.UpperCoverTighten2.Models.Datas
{
    [Flags]
    public enum TighteningFlag : ushort
    {
        None = 0,
        Tightening_OK = 1 << 0,
        Tightening_NOK = 1 << 1,
        res1 = 1 << 2,
        Ready = 1 << 3,
        TighteningRunning = 1 << 4,
    }
}
