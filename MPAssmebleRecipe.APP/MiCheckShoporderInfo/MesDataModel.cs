using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using MiCheckShoporderInfo.MiCheckShoporderInfo;

namespace MiCheckShoporderInfo
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
            ResourceField = IniFileHelper.ReadValue(SectionName, " Resource", FileName);
            Url = IniFileHelper.ReadValue(SectionName, "Url", FileName);
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
        private string resourceField;
  

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
