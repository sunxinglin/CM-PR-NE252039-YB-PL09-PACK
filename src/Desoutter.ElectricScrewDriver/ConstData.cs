using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desoutter.ElectricScrewDriver
{
    public static class ConstData
    {
        public const int HEADER_LENGTH_LENGTH = 4;

        public const int HEADER_MID_LENGTH = 4;

        public const int HEADER_REVISION_LENGTH = 3;

        public const int HEADER_NOACKFLAG_LENGTH = 1;

        public const int HEADER_STATIONID_LENGTH = 2;

        public const int HEADER_SPINDLEID_LENGTH = 2;

        public const int HEADER_SPARE_LENGTH = 4;

        public const string END = "0";
    }


    public static class DesoutterMessage
    {
        public const string CommunicationStart = "0001";
        public const string CommunicationStartAcknowledge = "0002";
        public const string CommunicationEnd = "0003";
        public const string ApplicationCommunicationNegativeAcknowledge = "0004";
        public const string CommandAccepted = "0005";
        public const string ParameterSetSelected = "0015";
        public const string LastTighteningResultData = "0061";
        public const string Alarm = "0071";
        public const string AlarmStatus = "0076";
        public const string KeepAliveMessage = "9999";

    }

    public enum EnumDesoutterMessage
    {
        CommunicationStart = 1,
        CommunicationStartAcknowledge = 2,
        CommunicationEnd = 3,
    }
}
