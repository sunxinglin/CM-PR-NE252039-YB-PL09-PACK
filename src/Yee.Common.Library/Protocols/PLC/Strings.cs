using System.Runtime.InteropServices;

namespace Yee.Common.Library.Protocols
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class TuJiao_Content
    {
        public byte TotalLength;
        public byte EffectiveLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string Content;

        public String EffectiveContent
        {
            get => this.Content?.Substring(0, Math.Min(this.EffectiveLength, this.Content.Length));
            set
            {
                var v = String.IsNullOrEmpty(value) ? String.Empty : value;
                this.Content = v;
                this.TotalLength = 30;
                this.EffectiveLength = (byte)v.Length;
            }
        }
    }
}
