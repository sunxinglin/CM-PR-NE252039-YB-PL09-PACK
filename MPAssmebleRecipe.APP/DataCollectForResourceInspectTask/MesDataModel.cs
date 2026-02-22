using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using DataCollectForResourceInspectTask.DataCollectForResourceInspectTask;

namespace DataCollectForResourceInspectTask
{
    public class MesDataModel : MesModel
    {
        public MesDataModel(string FileName, string SectionName) : base(FileName, SectionName)
        {
            Initialize();
            ReadConfig();
        }
        public override void ReadConfig()
        {


            // < site > 2001 </ site >
            //< user > 1 </ user >
            //< dcMode > 1 </ dcMode >         
            //< executeMode >?</ executeMode >
            //< resource >?</ resource >
            //< operation >?</ operation >
            //< operationRevision >?</ operationRevision >         
            //< dcGroup >?</ dcGroup >         
            //< dcGroupRevision >?</ dcGroupRevision >          
            //< dcSequence >?</ dcSequence >

            UserName = IniFileHelper.ReadValue(SectionName, "UserName", FileName);
            PassWord = IniFileHelper.ReadValue(SectionName, "PassWord", FileName);
            SiteField = IniFileHelper.ReadValue(SectionName, "Site", FileName);
            DcGroupField = IniFileHelper.ReadValue(SectionName, "DcGroup", FileName);
            DcGroupRevisionField = IniFileHelper.ReadValue(SectionName, "DcGroupRevision", FileName);
            UserField = IniFileHelper.ReadValue(SectionName, "User", FileName);
            ExecuteModeField = IniFileHelper.ReadValue(SectionName, "ExecuteMode", FileName);
            DcModeField = IniFileHelper.ReadValue(SectionName, "DcMode", FileName);
            DcSequenceField = IniFileHelper.ReadValue(SectionName, "DcSequence", FileName);
            OperationField = IniFileHelper.ReadValue(SectionName, "Operation", FileName);
            OperationRevisionField = IniFileHelper.ReadValue(SectionName, "OperationRevision", FileName);
            ResourceField = IniFileHelper.ReadValue(SectionName, "Resource", FileName);


            Url = IniFileHelper.ReadValue(SectionName, "Url", FileName);
            // TimeOut = int.Parse(IniFileHelper.ReadValue(SectionName, "timeOut", FileName));
            string value = IniFileHelper.ReadValue(SectionName, "timeOut", FileName);
            if (Int32.TryParse(value, out timeOut))
            {

            }
            else
            {
                timeOut = 30000;
            }

        }

        private string url;
        private int timeOut;
        private string userName;
        private string passWord;
        private string siteField;
        private string dcGroupField;
        private string dcGroupRevisionField;
        private string dcSequenceField;
        private string dcModeField;
        private string executeModeField;
        private string operationField;
        private string operationRevisionField;
        private string resourceField;
        private string userField;

        public string UserName
        {
            get
            {
                return userName;
            }

            set
            {
                userName = value;
            }
        }

        public string PassWord
        {
            get
            {
                return passWord;
            }

            set
            {
                passWord = value;
            }
        }

        public string SiteField
        {
            get
            {
                return siteField;
            }

            set
            {
                siteField = value;
            }
        }

        public string DcGroupField
        {
            get
            {
                return dcGroupField;
            }

            set
            {
                dcGroupField = value;
            }
        }

        public string DcGroupRevisionField
        {
            get
            {
                return dcGroupRevisionField;
            }

            set
            {
                dcGroupRevisionField = value;
            }
        }

        public string OperationField
        {
            get
            {
                return operationField;
            }

            set
            {
                operationField = value;
            }
        }

        public string OperationRevisionField
        {
            get
            {
                return operationRevisionField;
            }

            set
            {
                operationRevisionField = value;
            }
        }

        public string ResourceField
        {
            get
            {
                return resourceField;
            }

            set
            {
                resourceField = value;
            }
        }

        public string UserField
        {
            get
            {
                return userField;
            }

            set
            {
                userField = value;
            }
        }

        public string Url
        {
            get
            {
                return url;
            }

            set
            {
                url = value;
            }
        }

        public int TimeOut
        {
            get
            {
                return timeOut;
            }

            set
            {
                timeOut = value;
            }
        }
      
        public string ExecuteModeField
        {
            get
            {
                return executeModeField;
            }

            set
            {
                executeModeField = value;
            }
        }

        public string DcModeField
        {
            get
            {
                return dcModeField;
            }

            set
            {
                dcModeField = value;
            }
        }

        public string DcSequenceField
        {
            get
            {
                return dcSequenceField;
            }

            set
            {
                dcSequenceField = value;
            }

        }

    }
}
