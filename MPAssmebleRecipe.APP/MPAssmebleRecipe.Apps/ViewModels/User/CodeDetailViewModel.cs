using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;
using System.Collections.ObjectModel;

namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class ErrorInfo
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public string Solution { get; set; }
    }

    public class CodeDetailViewModel : BindableBase, IDialogAware
    {
        public string Title => "日志详情"; // 对话框标题

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand CloseCommand { get; }

        private ObservableCollection<ErrorInfo> _errorInfos;
        public ObservableCollection<ErrorInfo> ErrorInfos
        {
            get => _errorInfos;
            set => SetProperty(ref _errorInfos, value);
        }

        public CodeDetailViewModel()
        {
            CloseCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.OK)));
            ErrorInfos = new ObservableCollection<ErrorInfo>();
        }

        private async void LoadData(int code)
        {
            try
            {
                ErrorInfos.Clear();
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MesError", "MES报错代码指南.xlsx");
                
                if (!File.Exists(filePath))
                {
                    ErrorInfos.Add(new ErrorInfo 
                    { 
                        Code = code,
                        Description = "错误代码文件不存在",
                        Solution = "请检查文件路径"
                    });
                    return;
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // 获取第一个工作表
                    
                    int rowCount = worksheet.Dimension.Rows;
                    
                    for (int row = 2; row <= rowCount; row++) // 假设第一行是标题
                    {
                        var codeCell = worksheet.Cells[row, 1].Value; // 假设错误代码在第一列
                        
                        if (codeCell != null && int.TryParse(codeCell.ToString(), out int excelCode))
                        {
                            if (excelCode == code)
                            {
                                ErrorInfos.Add(new ErrorInfo
                                {
                                    Code = excelCode,
                                    Description = worksheet.Cells[row, 2].Value?.ToString() ?? "无描述",
                                    Solution = worksheet.Cells[row, 3].Value?.ToString() ?? "无解决方案"
                                });
                                
                            }
                        }
                    }
                    
                    ErrorInfos.Add(new ErrorInfo
                    {
                        Code = code,
                        Description = $"未找到错误代码 {code} 的相关信息",
                        Solution = "请确认错误代码是否正确"
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorInfos.Add(new ErrorInfo
                {
                    Code = code,
                    Description = $"读取错误代码信息失败: {ex.Message}",
                    Solution = "请检查文件格式是否正确"
                });
            }
        }
        private int _code;
        public int Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            Code = parameters.GetValue<int>("logEntry");
            LoadData(Code);
        }

        public bool CanCloseDialog() => true; // 是否可以关闭对话框
        public void OnDialogClosed() { } // 对话框关闭时的逻辑
    }
}
