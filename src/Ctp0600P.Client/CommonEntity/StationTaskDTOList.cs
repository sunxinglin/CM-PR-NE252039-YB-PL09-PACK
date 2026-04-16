using System.Collections.Generic;
using System.Windows.Media;

namespace Ctp0600P.Client.CommonEntity
{
    public class StationTaskDTOList
    {
        /// <summary>
        /// 完成颜色
        /// </summary>
        internal Brush FinishColor { get; }

        /// <summary>
        /// 进行中颜色
        /// </summary>
        internal Brush DoingColor { get; }

        /// <summary>
        /// 未完成颜色
        /// </summary>
        internal Brush UnFinishColor { get; }
        /// <summary>
        /// 信息
        /// </summary>
        internal List<StationTaskLeftTagData> Infos { get; }
        public StationTaskDTOList(List<StationTaskLeftTagData> infos, Brush finishColor, Brush doingColor, Brush unFinishColor)
        {
            this.Infos = infos;
            this.FinishColor = finishColor;
            this.UnFinishColor = unFinishColor;
            this.DoingColor = doingColor;
            this.Count = infos.Count;
        }
        /// <summary>
        /// 总数
        /// </summary>
        internal int Count { get; }


    }
}
