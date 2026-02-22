using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using miCheckInventoryAttributes.MesService;

namespace miCheckInventoryAttributes
{
    class MesDataModel : MesModel
    {
        public MesDataModel(string FileName, string SectionName):base(FileName,SectionName)
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
         //   RequiredQuantity=int.Parse( IniFileHelper.ReadValue(SectionName, "RequiredQuantity", FileName));
        //    ModeCheckInventoryField = (modeCheckInventory)Enum.Parse(typeof(modeCheckInventory),IniFileHelper.ReadValue(SectionName, "ModeCheckInventory", FileName));
            Url = IniFileHelper.ReadValue(SectionName, "Url", FileName);          
            string value = IniFileHelper.ReadValue(SectionName, "timeOut", FileName);
     

            if (Int32.TryParse(value, out timeOut))
            {

            }
            else
            {
                timeOut = 30000;
            }
            value = IniFileHelper.ReadValue(SectionName, "RequiredQuantity", FileName);
            if (Int32.TryParse(value, out requiredQuantity))
            {

            }
            else
            {
                requiredQuantity = 1;
            }
            value = IniFileHelper.ReadValue(SectionName, "ModeCheckInventory", FileName);
            if (Enum.TryParse<modeCheckInventory>(value, out modeCheckInventoryField))
            {

            }
            else
            {
                modeCheckInventoryField = modeCheckInventory.MODE_MARK_ONLY;
            }
        }

        private string activityId;
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

        private modeCheckInventory modeCheckInventoryField;

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

        public modeCheckInventory ModeCheckInventoryField
        {
            get
            {
                return modeCheckInventoryField;
            }

            set
            {
                modeCheckInventoryField = value;
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
