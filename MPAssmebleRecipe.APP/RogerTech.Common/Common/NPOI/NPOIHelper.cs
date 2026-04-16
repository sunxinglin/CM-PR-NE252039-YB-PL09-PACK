using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Web;

namespace RogerTech.Common
{
    public class NPOIHelper
    {
        /// <summary>  
        /// Excel导入DataTable
        /// </summary>  
        /// <param name="filePath">excel路径</param>  
        /// <param name="isColumnName">第一行是否是列名</param>  
        /// <returns>返回datatable</returns>  
        /// 
        public static DataTable ExcelToDataTable(string filePath, bool isColumnName)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 0;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // Excel2007以上版本  
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // Excel97-2003版本  
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet  
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数  
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行  
                                int cellCount = firstRow.LastCellNum;//列数  

                                //构建datatable的列  
                                if (isColumnName)
                                {
                                    startRow = 1;//如果第一行是列名，则从第二行开始读取  
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            if (cell.StringCellValue != null)
                                            {
                                                column = new DataColumn(cell.StringCellValue);
                                                dataTable.Columns.Add(column);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行  
                                for (int i = startRow; i <= rowCount; ++i)
                                {
                                    row = sheet.GetRow(i);
                                    bool emptyRow = true;
                                    if (row == null || row.PhysicalNumberOfCells == 0) continue;
                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)  
                                            switch (cell.CellType)
                                            {
                                                //case CellType.Blank:
                                                //    dataRow[j] = "";
                                                //    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理  
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                            }
                                        }
                                        if (dataRow[j] != null && !string.IsNullOrEmpty(dataRow[j].ToString().Trim()))
                                        {
                                            emptyRow = false;
                                        }
                                    }
                                    //非空数据行数据添加到DataTable
                                    if (!emptyRow)
                                    {
                                        dataTable.Rows.Add(dataRow);
                                    }
                                   
                                }
                            }
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
        }

        /// <summary>
        /// DataTable导出Excel
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="columns">自定义表头列</param>
        /// <param name="strSheetName">工作簿名</param>
        public static MemoryStream ExportExcel(DataTable dt, ArrayList columns, string strSheetName,string localFilePath="")
        {
            HSSFWorkbook book = new HSSFWorkbook();
            ICellStyle style = book.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            ICellStyle styleHead = book.CreateCellStyle();
            styleHead.Alignment = HorizontalAlignment.Center;
            styleHead.VerticalAlignment = VerticalAlignment.Center;
            IFont font = book.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;
            styleHead.SetFont(font);
            int sheetNum = 1;
            int tempIndex = 1;
            ISheet sheet = book.CreateSheet(strSheetName + sheetNum);
            IRow dataRow = null;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == 0)
                {
                    dataRow = sheet.CreateRow(0);
                    for (int k = 0; k < columns.Count; k++)
                    {
                        dataRow.CreateCell(k).SetCellValue(columns[k].ToString());
                        dataRow.GetCell(k).CellStyle = styleHead;
                    }
                }
                dataRow = sheet.CreateRow(tempIndex);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string ValueType = "";
                    string Value = "";
                    if (dt.Rows[i][j].ToString() != null)
                    {
                        ValueType = dt.Rows[i][j].GetType().ToString();
                        Value = dt.Rows[i][j].ToString();
                    }
                    switch (ValueType)
                    {
                        case "System.String"://字符串类型
                            dataRow.CreateCell(j).SetCellValue(Value);
                            break;

                        case "System.DateTime"://日期类型
                            System.DateTime dateV;
                            System.DateTime.TryParse(Value, out dateV);
                            dataRow.CreateCell(j).SetCellValue(dateV.ToString("yyyy-MM-dd HH:mm:ss"));
                            break;

                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(Value, out boolV);
                            dataRow.CreateCell(j).SetCellValue(boolV);
                            break;

                        case "System.Int16"://整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(Value, out intV);
                            dataRow.CreateCell(j).SetCellValue(intV);
                            break;

                        case "System.Decimal"://浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(Value, out doubV);
                            dataRow.CreateCell(j).SetCellValue(doubV);
                            break;

                        case "System.DBNull"://空值处理
                            dataRow.CreateCell(j).SetCellValue("");
                            break;

                        default:
                            dataRow.CreateCell(j).SetCellValue("");
                            break;
                    }
                    dataRow.GetCell(j).CellStyle = style;
                    //设置宽度
                    sheet.SetColumnWidth(j, (Value.Length + 10) * 256);
                }
                if (tempIndex == 65535)
                {
                    sheetNum++;
                    sheet = book.CreateSheet(strSheetName + sheetNum);
                    dataRow = sheet.CreateRow(0);
                    for (int k = 0; k < columns.Count; k++)
                    {
                        dataRow.CreateCell(k).SetCellValue(columns[k].ToString());
                        dataRow.GetCell(k).CellStyle = styleHead;
                    }
                    tempIndex = 0;
                }
                tempIndex++;
            }
            AutoColumnWidth(sheet, columns.Count);
            using (MemoryStream ms = new MemoryStream())
            {
                book.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //--------------------------
                if (!string.IsNullOrEmpty(localFilePath))
                {
                    var buf = ms.ToArray();
                    //保存为Excel文件  
                    using (FileStream fs = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(buf, 0, buf.Length);
                        fs.Flush();
                        fs.Close();
                    }
                }
                //--------------------------
               
                book.Close();
                return ms;
            }
        }

        /// <summary>
        /// DataTable导出Excel(用于OQC导出)
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="columns">自定义表头列</param>
        /// <param name="rowStartIndex">开始行</param>
        [Obsolete]
        public static MemoryStream ExportExcel(DataTable dt, ArrayList columns, IWorkbook book, ISheet sheet,int rowStartIndex)
        {
            ICellStyle style = book.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            ICellStyle styleHead = book.CreateCellStyle();
            styleHead.Alignment = HorizontalAlignment.Center;
            styleHead.VerticalAlignment = VerticalAlignment.Center;
            IFont font = book.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;
            styleHead.SetFont(font);
            IRow dataRow = sheet.CreateRow(rowStartIndex);
            for (int i = 0; i < columns.Count; i++)
            {
                dataRow.CreateCell(i).SetCellValue(columns[i].ToString());
                dataRow.GetCell(i).CellStyle = styleHead;
                if (i < 5)
                {
                    //设置自适应宽度
                    int columnWidth = (int)(sheet.GetColumnWidth(i) / 256);
                    for (int rowNum = 1; rowNum <= sheet.LastRowNum; rowNum++)
                    {
                        IRow currentRow = sheet.GetRow(rowNum);
                        if (currentRow.GetCell(i) != null)
                        {
                            ICell currentCell = currentRow.GetCell(i);
                            int length = Encoding.Default.GetBytes(currentCell.ToString()).Length;
                            if (columnWidth < length)
                            {
                                columnWidth = length;
                            }
                        }
                    }
                    sheet.SetColumnWidth(i, columnWidth * 256);
                }
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dataRow = sheet.CreateRow(i + rowStartIndex + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string ValueType = "";
                    string Value = "";
                    if (dt.Rows[i][j].ToString() != null)
                    {
                        ValueType = dt.Rows[i][j].GetType().ToString();
                        Value = dt.Rows[i][j].ToString();
                    }
                    switch (ValueType)
                    {
                        case "System.String"://字符串类型
                            dataRow.CreateCell(j).SetCellValue(Value);
                            break;

                        case "System.DateTime"://日期类型
                            System.DateTime dateV;
                            System.DateTime.TryParse(Value, out dateV);
                            dataRow.CreateCell(j).SetCellValue(dateV.ToString("yyyy-MM-dd HH:mm:ss"));
                            break;

                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(Value, out boolV);
                            dataRow.CreateCell(j).SetCellValue(boolV);
                            break;

                        case "System.Int16"://整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(Value, out intV);
                            dataRow.CreateCell(j).SetCellValue(intV);
                            break;

                        case "System.Decimal"://浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(Value, out doubV);
                            dataRow.CreateCell(j).SetCellValue(doubV);
                            break;

                        case "System.DBNull"://空值处理
                            dataRow.CreateCell(j).SetCellValue("");
                            break;

                        default:
                            dataRow.CreateCell(j).SetCellValue("");
                            break;
                    }
                    dataRow.GetCell(j).CellStyle = style;
                    //设置宽度
                    sheet.SetColumnWidth(j, (Value.Length + 10) * 256);
                }
            }

            using (MemoryStream ms = new MemoryStream())
            {
                book.Write(ms);
                ms.Flush();
                ms.Position = 0;
                book.Close();
                return ms;
            }
        }

        /// <summary>
        /// 用于Web导出
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">文件名</param>
        [Obsolete]
        public static void ExportByWeb(DataTable dtSource, ArrayList columns, string strFileName,string strSheetName)
        {
            HttpContext curContext = HttpContext.Current;

            // 设置编码和附件格式
            curContext.Response.ContentType = "application/vnd.ms-excel";
            curContext.Response.ContentEncoding = Encoding.UTF8;
            curContext.Response.Charset = "";
            curContext.Response.AppendHeader("Content-Disposition",
                "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));

            curContext.Response.BinaryWrite(ExportExcel(dtSource, columns, strSheetName).GetBuffer());
            curContext.Response.End();
        }

        /// <summary>
        /// 列宽自适应
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="cols"></param>
        private static void AutoColumnWidth(ISheet sheet, int cols)
        {
            for (int col = 0; col <= cols; col++)
            {
                sheet.AutoSizeColumn(col);//自适应宽度，但是其实还是比实际文本要宽
                int columnWidth = (int)(sheet.GetColumnWidth(col) / 256);//获取当前列宽度
                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    if (row.GetCell(cols) != null)
                    {
                        ICell cell = row.GetCell(col);
                        int contextLength = Encoding.UTF8.GetBytes(cell.ToString()).Length;//获取当前单元格的内容宽度
                        columnWidth = columnWidth < contextLength ? contextLength : columnWidth;
                    }
                }
                sheet.SetColumnWidth(col, columnWidth * 256);
            }
        }
    }
}