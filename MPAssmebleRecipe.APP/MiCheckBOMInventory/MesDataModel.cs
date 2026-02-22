using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using MiCheckBOMInventory.MiCheckBOMInventory;

namespace MiCheckBOMInventory
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
            userName = IniFileHelper.ReadValue(SectionName, "UserName", FileName);
            PassWord = IniFileHelper.ReadValue(SectionName, "PassWord", FileName);
            SiteField = IniFileHelper.ReadValue(SectionName, "Site", FileName);
            OperationField = IniFileHelper.ReadValue(SectionName, " Operation", FileName);
            OperationRevisionField = IniFileHelper.ReadValue(SectionName, "OperationRevision", FileName);
            ResourceField = IniFileHelper.ReadValue(SectionName, "Resource", FileName);
            UserField = IniFileHelper.ReadValue(SectionName, "User", FileName);
            activityIdField = IniFileHelper.ReadValue(SectionName, "ActivityId", FileName);
            UsageField= IniFileHelper.ReadValue(SectionName, "Usage", FileName);
            dataField = IniFileHelper.ReadValue(SectionName, "Data", FileName);
            modeCheckOperationField =IniFileHelper.ReadValue(SectionName, "FindSfcByInventory", FileName) == "true" ? true : false;

            Url = IniFileHelper.ReadValue(SectionName, "Url", FileName);
            string value = IniFileHelper.ReadValue(SectionName, "timeOut", FileName);
            if (Int32.TryParse(value, out timeOut))
            {

            }
            else
            {
                timeOut = 30000;
            }
       
            value = IniFileHelper.ReadValue(SectionName, "ModeProcessSFC", FileName);
            if (Enum.TryParse<modeProcessSFC>(value, out modeCheckBomField))
            {

            }
            else
            {
                modeCheckBomField = modeProcessSFC.MODE_NONE;
            }
            value = IniFileHelper.ReadValue(SectionName, "CheckBomCategoryField", FileName);
            if (Enum.TryParse<ObjectAliasEnum>(value, out checkBomCategoryField))
            {

            }
            else
            {
                checkBomCategoryField = ObjectAliasEnum.RESOURCE;
            }
        }


        private int requiredQuantity;
        private string url;
        private int timeOut;
        private string userName;
        private string passWord;
        private string siteField;

        private string operationField;

        private string operationRevisionField;

        private string resourceField;

        private string userField;

        private string activityIdField;

        private bool modeCheckOperationField;
       private string usageField;
        private ObjectAliasEnum checkBomCategoryField;
        private string dataField;
        private modeProcessSFC modeCheckBomField;

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
        public bool ModeCheckOperationField
        {
            get
            {
                return modeCheckOperationField;
            }

            set
            {
                modeCheckOperationField = value;
            }
        }
        public string UsageField
        {
            get
            {
                return usageField;
            }

            set
            {
                usageField = value;
            }
        }
        public ObjectAliasEnum CheckBomCategoryField
        {
            get
            {
                return checkBomCategoryField;
            }

            set
            {
                checkBomCategoryField = value;
            }
        }
        public string DataField
        {
            get
            {
                return dataField;
            }

            set
            {
                dataField = value;
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
     
        public modeProcessSFC ModeCheckBomField
        {
            get
            {
                return modeCheckBomField;
            }

            set
            {
                modeCheckBomField = value;
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
        public int RequiredQuantity
        {
            get
            {
                return requiredQuantity;
            }
            set
            {
                requiredQuantity = value;
            }
        }
    }
}
