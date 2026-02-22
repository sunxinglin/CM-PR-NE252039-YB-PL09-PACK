using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yee.Common.Library.CommonEnum;

namespace Yee.Entitys
{
    public class ReWorkData
    {
        public int Id { get; set; }

        public int StationTaskId { get; set; }

        public string WorkName { get; set; } = "";

        public int Sequence { get; set; } = 1;

        public StationTaskTypeEnum StationTaskType { get; set; }

        public StationTaskStatusEnum Statue { get; set; }
        public List<WorkRecord> WorkRecords { get; set; } = new List<WorkRecord>();

    }
    public class WorkRecord :INotifyPropertyChanged
    {
        public int Id { get; set; }

        public string WorkName { get; set; } = string.Empty;

        public int CurNo { get; set; }
        public List<int>? ReworkNums { get; set; }

        public int ReworkNum { get; set; } = 1;

        public StationTaskStatusEnum Statue { get; set; }
        public StationTaskTypeEnum StationTaskType { get; set; }

        private bool _IsChecked = false;
        public bool IsChecked
        {
            get => _IsChecked;
            set
            {
                _IsChecked = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
            }
        } 
        public bool IsNeedRemoveAll { get; set; } = true;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
