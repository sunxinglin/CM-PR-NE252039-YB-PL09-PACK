using System;

namespace RogerTech.Tool
{
    public interface IComProtocol
    {
        bool Connected { get; }
        void Connect();
        byte[] ReadBytes(int dbNr, int startAddr, int Length, out bool avilid);

   
        void WriteBytes(int dbNr, int startAddr, byte[] content, out bool avilid);
        void WriteBit(int dbNr, int startAddr, byte[] content, byte bit, out bool avilid);


        void WriteTag(Tag tag, byte[] bytes);
        void AddTag(Tag tag);
    }
}