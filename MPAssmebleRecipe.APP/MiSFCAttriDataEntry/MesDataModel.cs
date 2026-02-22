using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using MiSFCAttriDataEntry.MiSFCAttriDataEntry;

namespace MiSFCAttriDataEntry
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
            UserField = IniFileHelper.ReadValue(SectionName, "User", FileName);
         
            SfcModeField = IniFileHelper.ReadValue(SectionName, "SfcMode", FileName);
            ItemGroupField = IniFileHelper.ReadValue(SectionName, "ItemGroup", FileName);
            isCheckSequence = IniFileHelper.ReadValue(SectionName, "IsCheckSequence", FileName);

         
            String value = string.Empty;
           
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
        private string userField;

        private string sfcModeField;
        private string itemGroupField;
        private string isCheckSequence;



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
        public string SfcModeField
        {
            get { return sfcModeField; }
            set { sfcModeField = value; }
        }

        public string ItemGroupField
        {
            get { return itemGroupField; }
            set { itemGroupField = value; }
        }
        public string IsCheckSequence
        {
            get { return isCheckSequence; }
            set { isCheckSequence = value; }
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
