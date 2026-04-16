using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatlMesBase;

namespace GetMHRUser
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
            LoginName = IniFileHelper.ReadValue(SectionName, "LoginName", FileName);
            PassWord = IniFileHelper.ReadValue(SectionName, "PassWord", FileName);
            GetTokenUrl = IniFileHelper.ReadValue(SectionName, "GetTokenUrl", FileName);
            GetPermissionsUrl = IniFileHelper.ReadValue(SectionName, "GetPermissionsUrl", FileName);
            resource = IniFileHelper.ReadValue(SectionName, "Resource", FileName);
            BaseId = IniFileHelper.ReadValue(SectionName, "BaseId", FileName);
            // TimeOut = int.Parse(IniFileHelper.ReadValue(SectionName, "timeOut", FileName));
            string value = IniFileHelper.ReadValue(SectionName, "timeOut", FileName);
            if (Int32.TryParse(value, out timeOut))
            {

            }
            else
            {
                timeOut = 30000;
            }

        }
        //        device_resource_id:"P555666"
        //base_id:"HD"
        private string baseId;
        private string resource;
        private string getPermissionsUrl;
        private string getTokenUrl;
        private int timeOut;
        private string loginName;
        private string password;

        public string BaseId
        {
            get
            {
                return baseId;
            }

            set
            {
                baseId = value;
            }
        }
        public string Resource
        {
            get
            {
                return resource;
            }

            set
            {
                resource = value;
            }
        }

        public string LoginName
        {
            get
            {
                return loginName;
            }

            set
            {
                loginName = value;
            }
        }

        public string PassWord
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
            }
        }


        public string GetTokenUrl
        {
            get
            {
                return getTokenUrl;
            }

            set
            {
                getTokenUrl = value;
            }
        }
        public string GetPermissionsUrl
        {
            get
            {
                return getPermissionsUrl;
            }

            set
            {
                getPermissionsUrl = value;
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
