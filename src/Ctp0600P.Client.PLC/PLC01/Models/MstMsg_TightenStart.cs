using System.Runtime.InteropServices;

using Ctp0600P.Client.PLC.Common;

using FutureTech.Protocols;

namespace Ctp0600P.Client.PLC.PLC01.Models;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class MstMsg_TightenStart
{
    public RequestFlag Flag;

    [Endian(Endianness.BigEndian)]
    public ushort DeviceNo;

    [Endian(Endianness.BigEndian)]
    public ushort DeviceBrand;

    [Endian(Endianness.BigEndian)]
    public ushort ProgramNo;

    [Endian(Endianness.BigEndian)]
    public ushort SortNo;

}