using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;
using GetParametricValue.GetParametricValue;

namespace GetParametricValue
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
            UserField = IniFileHelper.ReadValue(SectionName, "User", FileName);
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
            UploadCode = IniFileHelper.ReadValue(SectionName, "UploadCode", FileName);

        }

        private string url;
        private int timeOut;
        private string userName;
        private string passWord;
        private string siteField;
        private string userField;
        private string uploadCode;


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

        public string UploadCode
        {
            get
            {
                return uploadCode;
            }

            set
            {
                uploadCode = value;
            }
        }

    }
}
