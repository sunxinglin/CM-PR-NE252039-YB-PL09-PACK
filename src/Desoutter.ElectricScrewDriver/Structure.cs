using AsZero.Core.Services.Messages;
using System.Text;

namespace Desoutter.ElectricScrewDriver
{

    public class Header
    {
        public Header()
        {
            Length = new byte[4] {32, 32, 32, 32};
            MID = new byte[4] { 32, 32, 32, 32 };
            Revision = new byte[3] { 32, 32, 32 };
            NoAckFlag = new byte[1] { 32 };
            StationId = new byte[2] { 32, 32 };
            SpindleID = new byte[2] { 32, 32 };
            SequenceNumber = new byte[2] { 32, 32 };
            NumberOfMessageParts = new byte[1] { 32 };
            MessagePartNumber = new byte[1] { 32 };

        }


        /// <summary>
        /// Length Byte:1-4
        /// </summary>
        public byte[] Length { get; set; }

        /// <summary>
        /// MID Byte:5-8
        /// </summary>
        public byte[] MID { get; set; }

        /// <summary>
        /// Revision Byte:9-11
        /// </summary>
        public byte[] Revision { get; set; }   

        /// <summary>
        /// NoAckFlag Byte:12
        /// </summary>
        public byte[] NoAckFlag { get; set; } 

        /// <summary>
        /// StationId Byte:13-14
        /// </summary>
        public byte[] StationId { get; set; } 

        /// <summary>
        /// SpindleID Byte:15-16
        /// </summary>
        public byte[] SpindleID { get; set; }

        /// <summary>
        /// SpindleID Byte:17-18
        /// </summary>
        public byte[] SequenceNumber { get; set; }

        /// <summary>
        /// SpindleID Byte:19
        /// </summary>
        public byte[] NumberOfMessageParts { get; set; }


        /// <summary>
        /// SpindleID Byte:20
        /// </summary>
        public byte[] MessagePartNumber { get; set; }
    }

    public class DesoutterResult
    {
        public DesoutterResult()
        {
            IsSucc = false;
            Message = string.Empty;
            Data = new byte[1024];
        }

        public bool IsSucc { get; set; }

        public string Message { get; set; }

        public byte[] Data { get; set; }
    }

    public class DataField
    {
        public DataField()
        {
            Parameter = new List<string>();
        }
        public List<string> Parameter { get; set; }
    }

    public class End
    {
        public End()
        {
            MessageEnd = new byte[1];
            MessageEnd[0] = 0;
        }
        public byte[] MessageEnd { get; set; }
    }

    public class MidLogMessage : LogMessage
    {
        public MidLogMessage()
        {
            this.Timestamp = DateTime.Now;
            this.Level = Microsoft.Extensions.Logging.LogLevel.Information;
        }

    }

}

