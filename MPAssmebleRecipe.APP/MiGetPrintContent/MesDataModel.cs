using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;

namespace MiGetPrintContent
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
            itemField = IniFileHelper.ReadValue(SectionName, "ItemField", FileName);
            templateField = IniFileHelper.ReadValue(SectionName, "TemplateField", FileName);
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


        private string itemField;

        private string templateField;



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

        public string ItemField
        {
            get
            {
                return itemField;
            }

            set
            {
                itemField = value;
            }
        }

        public string TemplateField
        {
            get
            {
                return templateField;
            }

            set
            {
                templateField = value;
            }
        }

    }
}
