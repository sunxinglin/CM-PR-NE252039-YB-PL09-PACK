using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Ctp0600P.Client.PLC.Common
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class BigFloat
    {
        [Endian(Endianness.BigEndian)]

        public float Value;
    }
}
