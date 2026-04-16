using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.FormulaExpressions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RogerTech.AuthService;
using RogerTech.AuthService.Models;
using RogerTech.Common;
using RogerTech.Tool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class WriteValueDialogViewModel : BindableBase, IDialogAware
    {
        private readonly RogerTech.AuthService.AuthService _authService;
        Group group;
        Tag tag;

        private string _TagName;
        public string TagValue
        {
            get => _TagName;
            set => SetProperty(ref _TagName, value);
        }
         

        public string Title => "修改PLC值";

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public WriteValueDialogViewModel(AuthService authService)
        {
            _authService = authService;
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));
        }
        private void Save()
        {
            var parameters = new DialogParameters
            {
                { "value", TagValue },
            };
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        { 
            tag = parameters.GetValue<Tag>("tag");
            if (tag != null&&tag.Result.Available==true)
            {
                TagValue = tag.GetValue().Value.ToString();
            }
        }
    }
}
