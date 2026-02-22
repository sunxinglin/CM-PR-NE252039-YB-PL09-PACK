using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogerTech.Common.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MPAssmebleRecipe.Apps.ViewModels.BlockManage
{
    public class UcAddtionBlockDialogViewModel : BindableBase, IDialogAware
    {
        public string Title { get; set; } = "";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            //throw new NotImplementedException();
            return true;
        }
        Template_Pack pack = null;
        Template_Pack packTemplete = null;
        Template_Pack resultPack = null;
        private bool station1Select;


        private ObservableCollection<string> blockGroups;

        public ObservableCollection<string> BlockGroups
        {
            get { return blockGroups; }
            set { blockGroups = value;
                RaisePropertyChanged();
            }
        }

        public bool Station1Select
        {
            get { return station1Select; }
            set { station1Select = value;
                RaisePropertyChanged();
            }
        }

        private string selectGroup;

        public string SelectGroup
        {
            get { return selectGroup; }
            set
            {
                selectGroup = value;
                RaisePropertyChanged();
            }
        }

        private bool station2Select;

        public bool Station2Select
        {
            get { return station2Select; }
            set
            {
                station2Select = value;
                RaisePropertyChanged();
            }
        }
        public void OnDialogClosed()
        {
            // throw new NotImplementedException();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            //throw new NotImplementedException();
            packTemplete = parameters.GetValue<Template_Pack>("pack");
            if (packTemplete == null)
            {
                throw new ArgumentNullException();
            }
            Station1Select = true;
            Station2Select = true;
            BlockGroups = new ObservableCollection<string>(packTemplete.BlockItems.Select(x => x.BlockName));

            //初始化命令
            AddCommand = new DelegateCommand(ExcuteAddCommand);
            DeleteCommand = new DelegateCommand(ExcuteDeleteCommand);
            SaveCommand = new DelegateCommand(ExcuteSaveCommand);
        }

        private void ExcuteSaveCommand()
        {
            IDialogParameters dialogParameters = new DialogParameters();
            dialogParameters.Add("pack", pack);
            dialogParameters.Add("station1", station1Select);
            dialogParameters.Add("station2", station2Select);
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, dialogParameters));
        }

        /// <summary>
        ///添加对应的group删除
        ///</summary>
        private void ExcuteDeleteCommand()
        {
            //throw new NotImplementedException();
            if(pack == null)
            {
                return;
            }
            else
            {
                if (SelectedBlockGroup != null)
                {
                    BlocksGroupItem.Remove(SelectedBlockGroup);

                }
            }
        }
        /// <summary>
        ///添加对应的group
        /// </summary>
        private void ExcuteAddCommand()
        {
            //throw new NotImplementedException();
            if (pack == null)
            {
                pack = new Template_Pack()
                {
                    PackName = packTemplete.PackName,
                    PackPn = packTemplete.PackPn
                };
                pack.BlockItems = new List<Template_Block>();
            }
            var blockGroup = packTemplete.BlockItems.Where(x => x.BlockName == selectGroup).FirstOrDefault();
            if (blockGroup == null)
            {
                return;
            }
            pack.BlockItems.Add(blockGroup);
            BlocksGroupItem = new ObservableCollection<Template_Block>(pack.BlockItems);
        }

        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; set; }

        public DelegateCommand SaveCommand { get; private set; }

        private ObservableCollection<Template_Block>  blocksGroupItem;

        public ObservableCollection<Template_Block> BlocksGroupItem
        {
            get { return blocksGroupItem; }
            set { blocksGroupItem = value;
                RaisePropertyChanged();
            }
        }

        private Template_Block selectedBlockGroup;

        public Template_Block SelectedBlockGroup
        {
            get { return selectedBlockGroup; }
            set { selectedBlockGroup = value;
                RaisePropertyChanged();
            }
        }


    }
}
