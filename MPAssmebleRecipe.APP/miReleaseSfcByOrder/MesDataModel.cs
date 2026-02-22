using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CatlMesBase;
using miReleaseSfcByOrder.miReleaseSfcWithActivityByShoporder;

namespace miReleaseSfcByOrder
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
            //Type t = this.GetType();
            //PropertyInfo[] infos = t.GetProperties();
            //foreach (var item in infos)
            //{
            //    item.SetValue(IniFileHelper.ReadValue(SectionName, item.Name, FileName));
            //}
            UserName = IniFileHelper.ReadValue(SectionName, "UserName", FileName);
            PassWord = IniFileHelper.ReadValue(SectionName, "PassWord", FileName);
            SiteField = IniFileHelper.ReadValue(SectionName, "Site", FileName);

            ResourceField = IniFileHelper.ReadValue(SectionName, "Resource", FileName);

            OperationField = IniFileHelper.ReadValue(SectionName, "Operation", FileName);

            OperationRevisionField = IniFileHelper.ReadValue(SectionName, "OperationRevision", FileName);

            ActivityField = IniFileHelper.ReadValue(SectionName, "Activity", FileName);

            UserField = IniFileHelper.ReadValue(SectionName, "User", FileName);

            ProcesslotField = IniFileHelper.ReadValue(SectionName, "Processlot", FileName);
            //LocationField = IniFileHelper.ReadValue(SectionName, "Location", FileName);
            ShopOrderField = IniFileHelper.ReadValue(SectionName, "ShopOrder", FileName);
            string value = IniFileHelper.ReadValue(SectionName, "IsCarrierType", FileName);
            if (bool.TryParse(value, out isCarrierTypeField))
            {

            }
            else
            {
                isCarrierTypeField = false;
            }
            value = IniFileHelper.ReadValue(SectionName, "ReleaseSfcByOrderColumnOrRowFirstSpecified", FileName);
            if (Enum.TryParse<columnOrRow>(value, out columnOrRowFirstField))
            {

            }
            else
            {
                ReleaseSfcByOrderColumnOrRowFirstField = columnOrRow.COLUMN_FIRST;
            }

            value = IniFileHelper.ReadValue(SectionName, "ReleaseSfcByOrderModeProcessSFC", FileName);
            if (Enum.TryParse<modeProcessSFC>(value, out modeProcessSFCField))
            {

            }
            else
            {
                ReleaseSfcByOrderModeProcessSFCField = modeProcessSFC.MODE_NONE;
            }

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

        private string resourceField;

        private string operationField;

        private string operationRevisionField;

        private string activityField;

        private string userField;

        private string processlotField;
        private string[] locationField;
        private string shopOrderField;



        private bool isCarrierTypeField;
        private bool columnOrRowFirstFieldSpecified;

        private columnOrRow columnOrRowFirstField;

        private modeProcessSFC modeProcessSFCField;

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

        public string ActivityField
        {
            get
            {
                return activityField;
            }

            set
            {
                activityField = value;
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

        public string ProcesslotField
        {
            get
            {
                return processlotField;
            }

            set
            {
                processlotField = value;
            }
        }

        public string[] LocationField
        {
            get
            {
                return locationField;
            }

            set
            {
                locationField = value;
            }
        }

        public string ShopOrderField
        {
            get
            {
                return shopOrderField;
            }

            set
            {
                shopOrderField = value;
            }
        }

        public bool IsCarrierTypeField
        {
            get
            {
                return isCarrierTypeField;
            }

            set
            {
                isCarrierTypeField = value;
            }
        }

        public bool ColumnOrRowFirstFieldSpecified
        {
            get
            {
                return columnOrRowFirstFieldSpecified;
            }

            set
            {
                columnOrRowFirstFieldSpecified = value;
            }
        }

        public columnOrRow ReleaseSfcByOrderColumnOrRowFirstField
        {
            get
            {
                return columnOrRowFirstField;
            }

            set
            {
                columnOrRowFirstField = value;
            }
        }

        public modeProcessSFC ReleaseSfcByOrderModeProcessSFCField
        {
            get
            {
                return modeProcessSFCField;
            }

            set
            {
                modeProcessSFCField = value;
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
