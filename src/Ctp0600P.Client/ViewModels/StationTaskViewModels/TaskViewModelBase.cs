using FutureTech.Mvvm;

using Yee.Entitys.DTOS;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels;

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
        this.CompleteTaskHandle?.Invoke(msg);
    }

    private async void RealtimePage_CompleteTask(StationTaskDTO dto)
    {
        await App._RealtimePage.RealtimePage_CompleteTask(dto);
    }
}