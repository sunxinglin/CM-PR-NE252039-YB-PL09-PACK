using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;

namespace miCheckSfcStatusEx
{
    public class MesDataModel:MesModel
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

            IsGetSFCFromCustomerBarcodeField = IniFileHelper.ReadValue(SectionName, "IsGetSFCFromCustomerBarcode", FileName);
            Url = IniFileHelper.ReadValue(SectionName, "Url", FileName);
            //TimeOut = int.Parse(IniFileHelper.ReadValue(SectionName, "timeOut", FileName));
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

        private string operationField;

        private string operationRevisionField;

        private string isGetSFCFromCustomerBarcodeField;

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

        public string IsGetSFCFromCustomerBarcodeField
        {
            get
            {
                return isGetSFCFromCustomerBarcodeField;
            }

            set
            {
                isGetSFCFromCustomerBarcodeField = value;
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
