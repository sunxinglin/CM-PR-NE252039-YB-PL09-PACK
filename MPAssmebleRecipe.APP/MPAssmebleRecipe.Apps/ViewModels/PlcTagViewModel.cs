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

        private void LoadTags()
        {
            if (SelectedGroup != null)
            {
                Tags = new ObservableCollection<Tag>(SelectedGroup.Tags);
            }
            else
            {
                Tags = new ObservableCollection<Tag>();
            }
        }

        private void SaveTag()
        {
            if (SelectedGroup == null) return;
            // 不重新 LoadTags，直接使用当前 UI 中的 Tags 顺序写入文件，避免位置变化
            fileName = Path.Combine(configPath, SelectedGroup.GroupName + ".csv");
            WriteAllTagsToCsv(Tags, fileName);
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

                    // 在内存集合中插入：基于 SelectedTag（如果为空则追加到末尾）
                    int insertIndex = -1;
                    if (SelectedTag != null && Tags != null)
                    {
                        insertIndex = Tags.IndexOf(SelectedTag);
                    }

                    if (Tags == null) Tags = new ObservableCollection<Tag>();

                    if (insertIndex >= 0)
                    {
                        Tags.Insert(insertIndex + 1, newTag);
                    }
                    else
                    {
                        Tags.Add(newTag);
                    }

                    // 持久化到 CSV：在 SelectedTag 后插入（或末尾）
                    fileName = Path.Combine(configPath, SelectedGroup.GroupName + ".csv");
                    InsertLineInCsv(newTag, SelectedTag?.TagName, fileName);

                    OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Add, $"新增PLC点位，新增点位信息：[TagName:{newTag.TagName}]," +
                        $"[IP:{newTag.Connection?.IP}],[DataType:{newTag.DataType}],[DbNumber:{newTag.Dbnr}],[StartAddress:{newTag.StartAddress}],[Length:{newTag.DataLength}],[Bit:{newTag.DataBit}]");
                }
            });
        }

        private void EditTag(Tag tag)
        {
            if (tag == null) return;
            var originalTagName = tag.TagName;

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

                    // 在内存集合中替换同一索引，保持序号不变（避免移动到最下）
                    if (Tags != null)
                    {
                        int idx = Tags.IndexOf(tag);
                        if (idx >= 0)
                        {
                            Tags[idx] = newTag;
                        }
                    }

                    // 只更新 CSV 中匹配的那一行（按原始 TagName 匹配）
                    fileName = Path.Combine(configPath, SelectedGroup.GroupName + ".csv");
                    UpdateLineInCsv(originalTagName, newTag, fileName);

                    OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Update, $"编辑PLC点位，编辑点位信息：[TagName:{newTag.TagName}]," +
                        $"[IP:{newTag.Connection?.IP}],[DataType:{newTag.DataType}],[DbNumber:{newTag.Dbnr}],[StartAddress:{newTag.StartAddress}],[Length:{newTag.DataLength}],[Bit:{newTag.DataBit}]");
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

        #region CSV 内联实现（替代 CSVFileHelper）

        private void EnsureConfigPath()
        {
            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }
        }

        private readonly string[] _headerColumns = new[]
        {
            "TagName","Protocol","IP","DataType","DbNumber","StartAddress","Length","Bit","MesName","MesDataType","LowerLimit","UpperLimit","IsChecked","IsUpload"
            //"TagName","Protocol","IP","DataType","DbNumber","StartAddress","Length","Bit","IsChecked","LowerLimit","UpperLimit","IsUpload","MesName","MesDataType"
        };

        private void EnsureCsvFileWithHeader(string path)
        {
            EnsureConfigPath();
            if (!File.Exists(path))
            {
                File.WriteAllLines(path, new[] { string.Join(",", _headerColumns) }, Encoding.UTF8);
            }
            else
            {
                var fi = new FileInfo(path);
                if (fi.Length == 0)
                {
                    File.WriteAllLines(path, new[] { string.Join(",", _headerColumns) }, Encoding.UTF8);
                }
            }
        }

        private string EscapeCsv(string s)
        {
            if (s == null) return "";
            if (s.Contains("\""))
                s = s.Replace("\"", "\"\"");
            if (s.Contains(",") || s.Contains("\"") || s.Contains("\n") || s.Contains("\r"))
                return $"\"{s}\"";
            return s;
        }

        private string SerializeTagToCsvLine(Tag item)
        {
            var parts = new List<string>
            {
                EscapeCsv(item.TagName),
                EscapeCsv("SIEMENSS7"),
                EscapeCsv(item.Connection?.IP ?? ""),
                EscapeCsv(item.DataType.ToString()),
                EscapeCsv(item.Dbnr.ToString()),
                EscapeCsv(item.StartAddress.ToString()),
                EscapeCsv(item.DataLength.ToString()),
                EscapeCsv(item.DataBit.ToString()),
                EscapeCsv(item.MesName ?? ""),
                EscapeCsv(item.MesDataType.ToString()),
                EscapeCsv(item.LowerLimit.ToString()),
                EscapeCsv(item.UpperLimit.ToString()),
                EscapeCsv(item.IsChecked.ToString()),
                EscapeCsv(item.IsUpload.ToString())
            };
            return string.Join(",", parts);
        }

        // 支持双引号内含逗号的 CSV 分割
        private List<string> SplitCsvLine(string line)
        {
            var matches = System.Text.RegularExpressions.Regex.Matches(line,
                "(?:^|,)(?:\"(?<val>(?:[^\"]|\"\")*)\"|(?<val>[^,]*))",
                 System.Text.RegularExpressions. RegexOptions.Compiled);
            var result = new List<string>(matches.Count);
            foreach ( System.Text.RegularExpressions. Match m in matches)
            {
                var val = m.Groups["val"].Value;
                if (val != null && val.Contains("\"\""))
                    val = val.Replace("\"\"", "\"");
                result.Add(val);
            }
            return result;
        }

        private void WriteAllTagsToCsv(IEnumerable<Tag> tags, string path)
        {
            EnsureCsvFileWithHeader(path);
            var lines = new List<string> { string.Join(",", _headerColumns) };
            foreach (var t in tags)
            {
                lines.Add(SerializeTagToCsvLine(t));
            }
            File.WriteAllLines(path, lines, Encoding.UTF8);
        }

        private void UpdateLineInCsv(string originalTagName, Tag newTag, string path)
        {
            EnsureCsvFileWithHeader(path);
            var allLines = File.ReadAllLines(path, Encoding.UTF8).ToList();
            if (allLines.Count == 0)
            {
                allLines.Add(string.Join(",", _headerColumns));
            }

            var header = allLines.First();
            var dataLines = allLines.Skip(1).ToList();
            var updated = false;

            for (int i = 0; i < dataLines.Count; i++)
            {
                var cols = SplitCsvLine(dataLines[i]);
                if (cols.Count > 0 && string.Equals(cols[0], originalTagName, StringComparison.Ordinal))
                {
                    dataLines[i] = SerializeTagToCsvLine(newTag);
                    updated = true;
                    break;
                }
            }

            if (!updated)
            {
                // 备用匹配：newTag.TagName
                for (int i = 0; i < dataLines.Count; i++)
                {
                    var cols = SplitCsvLine(dataLines[i]);
                    if (cols.Count > 0 && string.Equals(cols[0], newTag.TagName, StringComparison.Ordinal))
                    {
                        dataLines[i] = SerializeTagToCsvLine(newTag);
                        updated = true;
                        break;
                    }
                }
            }

            if (!updated)
            {
                // 未匹配到则追加（按需可改）
                dataLines.Add(SerializeTagToCsvLine(newTag));
            }

            var outLines = new List<string> { header };
            outLines.AddRange(dataLines);
            File.WriteAllLines(path, outLines, Encoding.UTF8);
        }

        private void InsertLineInCsv(Tag newTag, string afterTagName, string path)
        {
            EnsureCsvFileWithHeader(path);
            var allLines = File.ReadAllLines(path, Encoding.UTF8).ToList();
            if (allLines.Count == 0)
            {
                allLines.Add(string.Join(",", _headerColumns));
            }

            var header = allLines.First();
            var dataLines = allLines.Skip(1).ToList();

            int insertIndex = dataLines.Count; // 默认为末尾
            if (!string.IsNullOrEmpty(afterTagName))
            {
                for (int i = 0; i < dataLines.Count; i++)
                {
                    var cols = SplitCsvLine(dataLines[i]);
                    if (cols.Count > 0 && string.Equals(cols[0], afterTagName, StringComparison.Ordinal))
                    {
                        insertIndex = i + 1;
                        break;
                    }
                }
            }

            // 移除已有同名项，避免重复
            dataLines = dataLines.Where(l =>
            {
                var cols = SplitCsvLine(l);
                return !(cols.Count > 0 && string.Equals(cols[0], newTag.TagName, StringComparison.Ordinal));
            }).ToList();

            if (insertIndex > dataLines.Count) insertIndex = dataLines.Count;
            dataLines.Insert(insertIndex, SerializeTagToCsvLine(newTag));

            var outLines = new List<string> { header };
            outLines.AddRange(dataLines);
            File.WriteAllLines(path, outLines, Encoding.UTF8);
        }

        #endregion
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