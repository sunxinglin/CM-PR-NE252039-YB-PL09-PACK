using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using MiAssembleComponentsToSfcs.MiAssembleComponentsToSfcs;

namespace MiAssembleComponentsToSfcs
{
    class MesDataModel : MesModel
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
            OperationField = IniFileHelper.ReadValue(SectionName, "Operation", FileName);
            OperationRevisionField = IniFileHelper.ReadValue(SectionName, "OperationRevision", FileName);
            ResourceField = IniFileHelper.ReadValue(SectionName, "Resource", FileName);
            UserField = IniFileHelper.ReadValue(SectionName, "User", FileName);
            ActivityIdField = IniFileHelper.ReadValue(SectionName, "ActivityId", FileName);
            AmountField= IniFileHelper.ReadValue(SectionName, "Amount", FileName);
            String value = string.Empty;
            Url = IniFileHelper.ReadValue(SectionName, "Url", FileName);
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
        private string operationField;
        private string operationRevisionField;
        private string resourceField;
        private string userField;
        private string activityIdField;
        private string amountField;

        public string AmountField
        {
            get
            {
                return amountField;
            }
            set
            {
                amountField = value;
            }
        }

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
    }
}
