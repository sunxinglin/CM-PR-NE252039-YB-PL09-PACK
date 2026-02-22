using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Yee.Common.Library;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.DTOS
{
    public class Base_StationTaskBom_DTO : Base_StationTaskBom, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;


        private Visibility _FinishHidden = Visibility.Visible;

        /// <summary>
        /// 是否显示
        /// </summary>
        public Visibility FinishHidden
        {
            get => _FinishHidden;
            set
            {


                _FinishHidden = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("FinishHidden"));
                }

            }
        }

        private bool _HasPassed;
        /// <summary>
        /// 物料校验、测试等全部通过完成
        /// </summary>
        public bool HasPassed
        {
            get => _HasPassed;
            set
            {
                _HasPassed = value;
                if (_HasPassed)
                {
                    FinishHidden = Visibility.Hidden;
                }
                else
                    FinishHidden = Visibility.Visible;
                
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("HasPassed"));
                }
            }
        }
    }
}
