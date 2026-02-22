using System.Collections.Generic;

namespace Ctp0600P.Shared
{

    public class ColumnDescription
    {
        /// <summary>
        /// 键值
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 键的描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 前端是否显示
        /// </summary>
        public bool Browsable { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string Type { get; set; }
    }


    /// <summary>
    /// 返回确定类型的表格数据，可以为swagger提供精准的注释
    /// </summary>
    public class TableResp<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 操作消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 总记录条数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 数据内容
        /// </summary>
        public List<T> Data { get; set; }

        /// <summary>
        ///  返回的列表头信息
        /// </summary>
        public List<ColumnDescription> ColumnHeaders { get; set; }

        public TableResp()
        {
            Code = 200;
            Msg = "加载成功";
            ColumnHeaders = new List<ColumnDescription>();
        }
    }

}
