using FutureTech.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Automatic.Protocols.Common
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class BigFloat
    {
        [Endian(Endianness.BigEndian)]

        public float Value;
    }
}
