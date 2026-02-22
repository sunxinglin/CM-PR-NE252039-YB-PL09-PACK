using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using dataCollectForSfcEx.dataCollectForSfcEx;

namespace dataCollectForSfcEx
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
            UserName = IniFileHelper.ReadValue(SectionName, "UserName", FileName);
            PassWord = IniFileHelper.ReadValue(SectionName, "PassWord", FileName);
            SiteField = IniFileHelper.ReadValue(SectionName, "Site", FileName);
            DcGroupField = IniFileHelper.ReadValue(SectionName, "DcGroup", FileName);
            DcGroupRevisionField = IniFileHelper.ReadValue(SectionName, "DcGroupRevision", FileName);
            OperationField = IniFileHelper.ReadValue(SectionName, "Operation", FileName);
            OperationRevisionField = IniFileHelper.ReadValue(SectionName, "OperationRevision", FileName);
            ResourceField = IniFileHelper.ReadValue(SectionName, "Resource", FileName);
            UserField = IniFileHelper.ReadValue(SectionName, "User", FileName);
            ActivityIdField = IniFileHelper.ReadValue(SectionName, "ActivityId", FileName);
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

            value = IniFileHelper.ReadValue(SectionName, "DataCollectForSfcExModeProcessSfc", FileName);
            if (Enum.TryParse<ModeProcessSfc>(value, out modeProcessSfc))
            {

            }
            else
            {
                DataCollectForSfcExModeProcessSfc = ModeProcessSfc.MODE_NONE;
            }
        }
        private ModeProcessSfc modeProcessSfc;
        private string url;
        private int timeOut;
        private string userName;
        private string passWord;
        private string siteField;
        private string dcGroupField;
        private string dcGroupRevisionField;
        private string operationField;
        private string operationRevisionField;
        private string resourceField;
        private string userField;
        private string activityIdField;

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

        public string ActivityIdField
        {
            get
            {
                return activityIdField;
            }

            set
            {
                activityIdField = value;
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

        public ModeProcessSfc DataCollectForSfcExModeProcessSfc
        {
            get
            {
                return modeProcessSfc;
            }

            set
            {
                modeProcessSfc = value;
            }
        }
    }
}
