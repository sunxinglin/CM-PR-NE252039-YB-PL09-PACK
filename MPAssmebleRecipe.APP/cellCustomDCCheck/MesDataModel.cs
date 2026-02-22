using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;

namespace cellCustomDCCheck
{
    public class MesDataModel:MesModel
    {

        public MesDataModel(string FileName, string SectionName):base(FileName,SectionName)
        {
            //初始化值

            Initialize();
            ReadConfig();
        }
        public override void ReadConfig()
        {
            //string[] value = IniFileHelper.ReadSections(FileName);
            //string[] keys = IniFileHelper.ReadKeys(SectionName, FileName);
            userName = IniFileHelper.ReadValue(SectionName, "UserName", FileName);
            passWord = IniFileHelper.ReadValue(SectionName, "PassWord", FileName);
            siteField = IniFileHelper.ReadValue(SectionName, "Site", FileName);
            dcSequenceField = IniFileHelper.ReadValue(SectionName, "DcSequence", FileName);
            userField = IniFileHelper.ReadValue(SectionName, "User", FileName);
            multispecField = IniFileHelper.ReadValue(SectionName, "Multispec", FileName);
            operationField = IniFileHelper.ReadValue(SectionName, "Operation", FileName);
            resourceField = IniFileHelper.ReadValue(SectionName, "Resource", FileName);
            Url = IniFileHelper.ReadValue(SectionName,"Url",FileName);
            string value = IniFileHelper.ReadValue(SectionName, "timeOut", FileName);
            if(Int32.TryParse(value,out timeOut))
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

        private string dcSequenceField;

        private string userField;

        private string multispecField;

        private string operationField;

        private string resourceField;

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

        public string DcSequenceField
        {
            get
            {
                return dcSequenceField;
            }

            set
            {
                dcSequenceField = value;
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

        public string MultispecField
        {
            get
            {
                return multispecField;
            }

            set
            {
                multispecField = value;
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
