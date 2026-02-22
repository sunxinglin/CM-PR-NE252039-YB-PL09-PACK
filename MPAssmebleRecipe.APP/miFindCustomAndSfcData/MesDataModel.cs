using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using miFindCustomAndSfcData.miFindCustomAndSfcData;

namespace miFindCustomAndSfcData
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

            OperationField = IniFileHelper.ReadValue(SectionName, " Operation", FileName);

            OperationRevisionField = IniFileHelper.ReadValue(SectionName, "OperationRevision", FileName);

            ResourceField = IniFileHelper.ReadValue(SectionName, " Resource", FileName);

            UserField = IniFileHelper.ReadValue(SectionName, "User", FileName);

            ActivityField = IniFileHelper.ReadValue(SectionName, "Activity", FileName);

            InventoryField = IniFileHelper.ReadValue(SectionName, "Inventory", FileName);

            SfcOrderField = IniFileHelper.ReadValue(SectionName, "SfcOrder", FileName);

            TargetOrderField = IniFileHelper.ReadValue(SectionName, "TargetOrder", FileName);

            CheckInventoryABField = IniFileHelper.ReadValue(SectionName, "CheckInventoryAB", FileName);

            IsGetXYField = IniFileHelper.ReadValue(SectionName, "IsGetXY", FileName) == "true" ? true : false;

            FindSfcByInventoryField = IniFileHelper.ReadValue(SectionName, "FindSfcByInventory", FileName) == "true" ? true : false;

            //ShowMarkingField = IniFileHelper.ReadValue(SectionName, "ShowMarking", FileName); ;

            //ShowMarkingFieldSpecified = IniFileHelper.ReadValue(SectionName, "ShowMarkingSpecified", FileName); 

            string value = IniFileHelper.ReadValue(SectionName, "FindCustomAndSfcDataModeProcessSFC", FileName);
            if (Enum.TryParse<modeProcessSFC>(value, out modeProcessSFCField))
            {

            }
            else
            {
                FindCustomAndSfcDataModeProcessSFCField = modeProcessSFC.MODE_NONE;
            }
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
            value = IniFileHelper.ReadValue(SectionName, "FindCustomAndSfcDataCategory", FileName);
            if (Enum.TryParse<ObjectAliasEnum>(value, out category))
            {

            }
            else
            {
                FindCustomAndSfcDataCategory = ObjectAliasEnum.ITEM;
            }
            DataField = IniFileHelper.ReadValue(SectionName, "DataField", FileName);
        }
    
        private string dataField;
        private string url;
        private int timeOut;
        private string userName;
        private string passWord;

        private string siteField;

        private string operationField;

        private string operationRevisionField;

        private string resourceField;

        

        private string userField;

        private string activityField;

        private string inventoryField;

        private string sfcOrderField;

        private string targetOrderField;

        private string checkInventoryABField;

        private bool findSfcByInventoryField;

        private bool isGetXYField;

        private bool showMarkingField;

        private bool showMarkingFieldSpecified;

        private modeProcessSFC modeProcessSFCField;
        private ObjectAliasEnum category;
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

        public string InventoryField
        {
            get
            {
                return inventoryField;
            }

            set
            {
                inventoryField = value;
            }
        }

        public string SfcOrderField
        {
            get
            {
                return sfcOrderField;
            }

            set
            {
                sfcOrderField = value;
            }
        }

        public string TargetOrderField
        {
            get
            {
                return targetOrderField;
            }

            set
            {
                targetOrderField = value;
            }
        }

        public string CheckInventoryABField
        {
            get
            {
                return checkInventoryABField;
            }

            set
            {
                checkInventoryABField = value;
            }
        }

        public bool FindSfcByInventoryField
        {
            get
            {
                return findSfcByInventoryField;
            }

            set
            {
                findSfcByInventoryField = value;
            }
        }

        public bool IsGetXYField
        {
            get
            {
                return isGetXYField;
            }

            set
            {
                isGetXYField = value;
            }
        }

        public bool ShowMarkingField
        {
            get
            {
                return showMarkingField;
            }

            set
            {
                showMarkingField = value;
            }
        }

        public bool ShowMarkingFieldSpecified
        {
            get
            {
                return showMarkingFieldSpecified;
            }

            set
            {
                showMarkingFieldSpecified = value;
            }
        }

        public modeProcessSFC FindCustomAndSfcDataModeProcessSFCField
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

        public ObjectAliasEnum FindCustomAndSfcDataCategory
        {
            get
            {
                return category;
            }

            set
            {
                category = value;
            }
        }
    }
}
