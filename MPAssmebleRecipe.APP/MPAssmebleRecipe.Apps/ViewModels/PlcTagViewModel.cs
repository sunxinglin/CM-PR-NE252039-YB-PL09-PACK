using OfficeOpenXml.FormulaParsing.FormulaExpressions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RogerTech.AuthService.Models;
using RogerTech.Common;
using RogerTech.Common.AuthService.Model;
using RogerTech.Common.AuthService.Services;
using RogerTech.Tool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class PlcTagViewModel : BindableBase, IRegionMemberLifetime
    {
        //  public DelegateCommand AddGroupCommand { get; }
        public DelegateCommand AddTagCommand { get; }
        public DelegateCommand<Tag> EditTagCommand { get; }
        public DelegateCommand<Tag> WriteValueCommand { get; }
        public DelegateCommand<Tag> ReadValueCommand { get; }
        public DelegateCommand RefreshCommand { get; }

        public DelegateCommand SaveTagCommand { get; }
        private readonly IDialogService _dialogService;
        public Tag SelectedTag
        {
            get => _selectedTag;
            set => SetProperty(ref _selectedTag, value);
        }
        private ObservableCollection<Group> _plcGroups;
        public ObservableCollection<Group> PlcGroups
        {
            get => _plcGroups;
            set => SetProperty(ref _plcGroups, value);
        }       

        private ObservableCollection<Tag> _tags;
        private Group _selectedGroup;
        public Group SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (SetProperty(ref _selectedGroup, value))
                {
                    LoadTags();

                }
            }
        }

        private SolidColorBrush _selectedColor;
        public SolidColorBrush SelectedColor
        {
            get => _selectedColor;
            set => SetProperty(ref _selectedColor, value);
        }

        private Tag _selectedTag;
        public ObservableCollection<Tag> Tags
        {
            get => _tags;
            set => SetProperty(ref _tags, value);
        }

        public bool KeepAlive => false;

        string configPath = Path.Combine(Directory.GetCurrentDirectory(), "Config");
        string fileName = string.Empty;

        public PlcTagViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            LoadGroups();

            SaveTagCommand = new DelegateCommand(SaveTag);

            //   AddGroupCommand = new DelegateCommand(AddGroup);
            AddTagCommand = new DelegateCommand(AddTag);
            EditTagCommand = new DelegateCommand<Tag>(EditTag);
            WriteValueCommand = new DelegateCommand<Tag>(WriteValue);
            ReadValueCommand = new DelegateCommand<Tag>(ReadValue);
            RefreshCommand = new DelegateCommand(LoadGroups);
        }

        private void LoadGroups()
        {
            var core = AppManager.GetInstance().bussness;
            PlcGroups = new ObservableCollection<Group>(core.BussnessDic.PlcGroups);
        }


        private void SaveTag()
        {
            // SelectedGroup.GroupName
            if (SelectedGroup == null) return;
            LoadTags();
            List<TagAndConnEntry> lstEmtry = new List<TagAndConnEntry>();
            foreach (Tag item in Tags)
            {
                TagAndConnEntry emtry = new TagAndConnEntry
                {
                    TagName = item.TagName,
                    IP = item.Connection.IP,
                    DataType = item.DataType.ToString(),
                    DbNumber = item.Dbnr,
                    StartAddress = item.StartAddress,
                    Length = item.DataLength,
                    Bit = item.DataBit,
                    IsChecked = item.IsChecked,
                    LowerLimit = item.LowerLimit,
                    UpperLimit = item.UpperLimit,
                    IsUpload = item.IsUpload,
                    MesName = item.MesName,
                    MesDataType = item.MesDataType
                };
                lstEmtry.Add(emtry);
            }
            fileName = Path.Combine(configPath, SelectedGroup.GroupName + ".txt");
            CSVFileHelper.WriteFromList<TagAndConnEntry>(lstEmtry, fileName);

        }

        private void LoadTags()
        {
            if (SelectedGroup != null)
            {
                Tags = new ObservableCollection<Tag>(SelectedGroup.Tags);
            }
        }
        private void AddTag()
        {

            if (SelectedGroup == null) return;

            var parameters = new DialogParameters
            {
                { "group", SelectedGroup }
            };

            _dialogService.ShowDialog("AddPlcTagDialog", parameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    var newTag = result.Parameters.GetValue<Tag>("tag");
                    LoadTags();

                    List<TagAndConnEntry> lstEmtry = new List<TagAndConnEntry>();
                    foreach (Tag item in Tags)
                    {
                        TagAndConnEntry emtry = new TagAndConnEntry
                        {
                            TagName = item.TagName,
                            IP = item.Connection.IP,
                            DataType = item.DataType.ToString(),
                            DbNumber = item.Dbnr,
                            StartAddress = item.StartAddress,
                            Length = item.DataLength,
                            Bit = item.DataBit,
                            IsChecked = item.IsChecked,
                            LowerLimit = item.LowerLimit,
                            UpperLimit = item.UpperLimit,
                            IsUpload = item.IsUpload,
                            MesName = item.MesName,
                            MesDataType = item.MesDataType
                        };
                        lstEmtry.Add(emtry);
                    }

                    fileName = Path.Combine(configPath, SelectedGroup.GroupName + ".CSV");
                    CSVFileHelper.WriteFromList<TagAndConnEntry>(lstEmtry, fileName);
                    OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Add, $"新增PLC点位，新增点位信息：[TagName:{newTag.TagName}]," +
                        $"[IP:{newTag.Connection.IP}],[DataType:{newTag.DataType}],[DbNumber:{newTag.Dbnr}],[StartAddress:{newTag.StartAddress}],[Length:{newTag.DataLength}],[Bit:{newTag.DataBit}]");
                }
            });
        }
        private void EditTag(Tag tag)
        {
            if (tag == null) return;
            var parameters = new DialogParameters
            {
                { "tag", tag },
                { "group", SelectedGroup }
            };
            _dialogService.ShowDialog("AddPlcTagDialog", parameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    var newTag = result.Parameters.GetValue<Tag>("tag");
                    LoadTags();

                    List<TagAndConnEntry> lstEmtry = new List<TagAndConnEntry>();
                    foreach (Tag item in Tags)
                    {
                        TagAndConnEntry emtry = new TagAndConnEntry
                        {
                            TagName = item.TagName,
                            IP = item.Connection.IP,
                            DataType = item.DataType.ToString(),
                            DbNumber = item.Dbnr,
                            StartAddress = item.StartAddress,
                            Length = item.DataLength,
                            Bit = item.DataBit,
                            IsChecked = item.IsChecked,
                            LowerLimit = item.LowerLimit,
                            UpperLimit = item.UpperLimit,
                            IsUpload = item.IsUpload,
                            MesName = item.MesName,
                            MesDataType = item.MesDataType

                        };
                        lstEmtry.Add(emtry);
                    }

                    fileName = Path.Combine(configPath, SelectedGroup.GroupName + ".CSV");
                    CSVFileHelper.WriteFromList<TagAndConnEntry>(lstEmtry, fileName);
                    OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Update, $"编辑PLC点位，编辑点位信息：[TagName:{newTag.TagName}]," +
                        $"[IP:{newTag.Connection.IP}],[DataType:{newTag.DataType}],[DbNumber:{newTag.Dbnr}],[StartAddress:{newTag.StartAddress}],[Length:{newTag.DataLength}],[Bit:{newTag.DataBit}]");
                }
            });
        }

        private void WriteValue(Tag tag)
        {
            if (tag == null) return;

            var parameters = new DialogParameters
            {
                { "tag", tag }
            };
        }
        private void ReadValue(Tag tag)
        {
            if (tag == null) return;
        }


        private readonly Queue<string> _logQueue = new Queue<string>();
        public void FlushLogs(string log)
        {
            // 实际写入操作（示例）
            try
            {
                OperationService.OperationRecord(
                    Operation.Update,
                    string.Join(Environment.NewLine, log)
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"日志写入失败: {ex.Message}");
            }
        }
    }

    public class TagAndConnEntry
    {
        /// <summary>
        /// 点位名称
        /// </summary>
        public string TagName { get; set; }
        /// <summary>
        /// 通讯协议
        /// </summary>
        public string Protocol { get; set; } = "SIEMENSS7";
        /// <summary>
        /// PLC通讯IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// DB地址
        /// </summary>
        public int DbNumber { get; set; }
        /// <summary>
        /// 开始点位
        /// </summary>
        public int StartAddress { get; set; }
        /// <summary>
        /// 点位长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 数据位
        /// </summary>
        public byte Bit { get; set; }
        public string MesName { get; set; }

        public ParameterDataType MesDataType { get; set; }
        public double LowerLimit { get; set; }
        public double UpperLimit { get; set; }
        public bool IsChecked { get; set; }
        public bool IsUpload { get; set; }
    }
}

