using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Yee.Entitys.DTOS;

namespace Ctp0600P.Client.DTOS
{
    public class Proc_AutoBoltInfo_Detail_DTO_Ext: Proc_AutoBoltInfo_Detail_DTO
    {
        public new event PropertyChangedEventHandler PropertyChanged;

        private bool _Show = false;
        public bool Show
        {
            get => _Show;
            set
            {
                _Show = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Show"));
                }
            }
        }

        private Thickness _Margin  = new Thickness(0,0,0,0);
        public Thickness Margin
        {
            get => _Margin;
            set
            {
                _Margin = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Margin"));
                }
            }
        }
    }
}
