using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using MiAssembleAndCollectDataForSfc.miAssmebleAndCollectDataForSfc;

namespace MiAssembleAndCollectDataForSfc
{
    class MesDataModel:MesModel
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
            String value = string.Empty;
            value = IniFileHelper.ReadValue(SectionName, "AssembleAndCollectDataForSfcModeProcessSfcField", FileName);
            if(Enum.TryParse(value, out dataCollectForSfcModeProcessSfcField))
            {

            }
            else
            {
                AssembleAndCollectDataForSfcModeProcessSfcField = dataCollectForSfcModeProcessSfc.MODE_NONE;
            }
            value = IniFileHelper.ReadValue(SectionName, "PartialAssembly", FileName);
            if (bool.TryParse(value,out partialAssemblyField))
            {

            }
            else
            {
                PartialAssemblyField = false;
            }
            RemarkField = IniFileHelper.ReadValue(SectionName, "Remark", FileName);
            Url = IniFileHelper.ReadValue(SectionName, "Url", FileName);
            //TimeOut = int.Parse(IniFileHelper.ReadValue(SectionName, "timeOut", FileName));
            value = IniFileHelper.ReadValue(SectionName, "timeOut", FileName);
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

        private string sfcField;

        private string dcGroupField;

        private string dcGroupRevisionField;

        private string operationField;

        private string operationRevisionField;

        private string resourceField;

        private string userField;

        private string activityIdField;

        private dataCollectForSfcModeProcessSfc dataCollectForSfcModeProcessSfcField;

        private bool partialAssemblyField;

        private string remarkField;

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

        public string SfcField
        {
            get
            {
                return sfcField;
            }

            set
            {
                sfcField = value;
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

        public dataCollectForSfcModeProcessSfc AssembleAndCollectDataForSfcModeProcessSfcField
        {
            get
            {
                return dataCollectForSfcModeProcessSfcField;
            }

            set
            {
                dataCollectForSfcModeProcessSfcField = value;
            }
        }

        public bool PartialAssemblyField
        {
            get
            {
                return partialAssemblyField;
            }

            set
            {
                partialAssemblyField = value;
            }
        }

        public string RemarkField
        {
            get
            {
                return remarkField;
            }

            set
            {
                remarkField = value;
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
    }
}
