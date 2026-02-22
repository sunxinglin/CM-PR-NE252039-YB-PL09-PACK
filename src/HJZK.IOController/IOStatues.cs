namespace HJZK.IOController
{
    public class IOStatues
    {
        //  属性变化需发布出去
        private bool manual = false;//手动
        private bool reset = false;
        private bool auto = false;
        private bool start = false;
        private bool powerAll = false;
        private bool power_24V = false;
        private bool power_24V_Out = false;
        private bool powerFan = false;
        private bool resetLight = false;
        private bool startLight = false;
        private bool redLight = false;
        private bool greenLight = false;
        private bool yellowLight = false;
        private bool beep = false;
        private bool spareFri = false;
        private bool spareSec = false;
        //public bool Error { get; set; }

        public bool Manual
        {
            get => manual;
            set
            {
                manual = value;
            }
        }
        public bool SpareFri
        {
            get => spareFri;
            set
            {
                spareFri = value;
            }
        }

        public bool SpareSec
        {
            get => spareSec;
            set
            {
                spareSec = value;
            }
        }

        public bool Reset
        {
            get => reset;
            set
            {
                reset = value;
            }
        }
        public bool Auto
        {
            get => auto;
            set
            {
                auto = value;
            }
        }
        public bool Start
        {
            get => start;
            set
            {
                start = value;
            }
        }

        public bool PowerAll
        {
            get => powerAll;
            set
            {
                powerAll = value;
            }
        }

        public bool Power_24V
        {
            get => power_24V;
            set
            {
                power_24V = value;
            }
        }
        public bool Power_24V_Out
        {
            get => power_24V_Out;
            set
            {
                power_24V_Out = value;
            }
        }
        public bool RedLight
        {
            get => redLight;
            set
            {
                redLight = value;
            }
        }
        public bool GreenLight
        {
            get => greenLight;
            set
            {
                greenLight = value;
            }
        }
        public bool YellowLight
        {
            get => yellowLight;
            set
            {
                yellowLight = value;
            }
        }
        public bool PowerFan
        {
            get => powerFan;
            set
            {
                powerFan = value;
            }
        }
        public bool ResetLight
        {
            get => resetLight;
            set
            {
                resetLight = value;
            }
        }
        public bool StartLight
        {
            get => startLight;
            set
            {
                startLight = value;
            }
        }
        public bool Beep
        {
            get => beep;
            set
            {
                beep = value;
            }
        }
    }
}
