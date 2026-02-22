using FutureTech.Mvvm;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;


namespace Ctp0600P.Client.ViewModels.StationTaskViewModels
{
    public class TaskViewModelBase : ViewModelBase
    {
        public delegate void CompleteTaskEventHandle(StationTaskDTO dto);
        public event CompleteTaskEventHandle CompleteTaskHandle;

        public TaskViewModelBase()
        {
            this.CompleteTaskHandle -= RealtimePage_CompleteTask;
            this.CompleteTaskHandle += RealtimePage_CompleteTask;
            
        }

        public virtual void OnCompleteTask(StationTaskDTO msg)
        {
            if (this.CompleteTaskHandle != null)
                this.CompleteTaskHandle(msg);
        }

        public async void RealtimePage_CompleteTask(StationTaskDTO dto)
        {
            await App._RealtimePage.RealtimePage_CompleteTask(dto);
        }
    }
}
