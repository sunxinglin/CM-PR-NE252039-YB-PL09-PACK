using FutureTech.Protocols;
using MediatR;
using System.Runtime.InteropServices;

namespace Ctp0600P.Client.PLC.PLC01.Models.Datas
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class GlueData : INotification
    {
        [Endian(Endianness.BigEndian)]
        public float _GlueValueA;

        [Endian(Endianness.BigEndian)]
        public float _GlueValueB;

        [Endian(Endianness.BigEndian)]
        public float _GlueProportion;

        [Endian(Endianness.BigEndian)]
        public float _GlueValueTotal;

        public float GlueValueA => _GlueValueA;
        public float GlueValueB => _GlueValueB;
        public float GlueProportion => _GlueProportion;
        public float GlueValueTotal => _GlueValueTotal;

    }
}
