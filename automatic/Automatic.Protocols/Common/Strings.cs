using System.Runtime.InteropServices;


namespace Automatic.Protocols.Common
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class String30
    {
        private const int MAX_SIZE = 30;

        public byte TotalLength;
        public byte EffectiveLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SIZE)]
        public string Content;

        public string EffectiveContent
        {
            get => Content?.Substring(0, Math.Min(EffectiveLength, Content.Length));
            set
            {
                var v = string.IsNullOrEmpty(value) ? string.Empty : value;
                Content = v;
                TotalLength = MAX_SIZE;
                EffectiveLength = (byte)v.Length;
            }
        }

        public static String30 New(string content)
        {
            var len = content.Length;
            if (len > MAX_SIZE)
            {
                throw new ArgumentOutOfRangeException($"字符串{content}长度={len}，大于最大值({MAX_SIZE})");
            }

            return new String30
            {
                TotalLength = MAX_SIZE,
                EffectiveLength = (byte)content.Length,
                Content = content,
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class String40
    {
        private const int MAX_SIZE = 40;

        public byte TotalLength;
        public byte EffectiveLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SIZE)]
        public string Content;

        public string EffectiveContent
        {
            get => Content?.Substring(0, Math.Min(EffectiveLength, Content.Length));
            set
            {
                var v = string.IsNullOrEmpty(value) ? string.Empty : value;
                Content = v;
                TotalLength = MAX_SIZE;
                EffectiveLength = (byte)v.Length;
            }
        }

        public static String40 New(string content)
        {
            var len = content.Length;
            if (len > MAX_SIZE)
            {
                throw new ArgumentOutOfRangeException($"字符串{content}长度={len}，大于最大值({MAX_SIZE})");
            }

            return new String40
            {
                TotalLength = MAX_SIZE,
                EffectiveLength = (byte)content.Length,
                Content = content,
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class String64
    {
        private const int MAX_SIZE = 64;

        public byte TotalLength;
        public byte EffectiveLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SIZE)]
        public string Content;

        public string EffectiveContent
        {
            get => Content?.Substring(0, Math.Min(EffectiveLength, Content.Length));
            set
            {
                var v = string.IsNullOrEmpty(value) ? string.Empty : value;
                Content = v;
                TotalLength = MAX_SIZE;
                EffectiveLength = (byte)v.Length;
            }
        }

        public static String64 New(string content)
        {
            var len = content.Length;
            if (len > MAX_SIZE)
            {
                throw new ArgumentOutOfRangeException($"字符串{content}长度={len}，大于最大值({MAX_SIZE})");
            }

            return new String64
            {
                TotalLength = MAX_SIZE,
                EffectiveLength = (byte)content.Length,
                Content = content,
            };
        }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class String82
    {
        private const int MAX_SIZE = 82;

        public byte TotalLength;
        public byte EffectiveLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SIZE)]
        public string Content;

        public string EffectiveContent
        {
            get => Content?.Substring(0, Math.Min(EffectiveLength, Content.Length));
            set
            {
                var v = string.IsNullOrEmpty(value) ? string.Empty : value;
                Content = v;
                TotalLength = MAX_SIZE;
                EffectiveLength = (byte)v.Length;
            }
        }

        public static String82 New(string content)
        {
            var len = content.Length;
            if (len > MAX_SIZE)
            {
                throw new ArgumentOutOfRangeException($"字符串{content}长度={len}，大于最大值({MAX_SIZE})");
            }

            return new String82
            {
                TotalLength = MAX_SIZE,
                EffectiveLength = (byte)content.Length,
                Content = content,
            };
        }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class String126
    {
        private const int MAX_SIZE = 126;

        public byte TotalLength;
        public byte EffectiveLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SIZE)]
        public string Content;

        public string EffectiveContent
        {
            get => Content?.Substring(0, Math.Min(EffectiveLength, Content.Length));
            set
            {
                var v = string.IsNullOrEmpty(value) ? string.Empty : value;
                Content = v;
                TotalLength = MAX_SIZE;
                EffectiveLength = (byte)v.Length;
            }
        }

        public static String126 New(string content)
        {
            var len = content.Length;
            if (len > MAX_SIZE)
            {
                throw new ArgumentOutOfRangeException($"字符串{content}长度={len}，大于最大值({MAX_SIZE})");
            }

            return new String126
            {
                TotalLength = MAX_SIZE,
                EffectiveLength = (byte)content.Length,
                Content = content,
            };
        }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class String254
    {
        private const int MAX_SIZE = 254;

        public byte TotalLength;
        public byte EffectiveLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SIZE)]
        public string Content;

        public string EffectiveContent
        {
            get => Content?.Substring(0, Math.Min(EffectiveLength, Content.Length));
            set
            {
                var v = string.IsNullOrEmpty(value) ? string.Empty : value;
                Content = v;
                TotalLength = MAX_SIZE;
                EffectiveLength = (byte)v.Length;
            }
        }

        public static String254 New(string content)
        {
            var len = content.Length;
            if (len > MAX_SIZE)
            {
                throw new ArgumentOutOfRangeException($"字符串{content}长度={len}，大于最大值({MAX_SIZE})");
            }

            return new String254
            {
                TotalLength = MAX_SIZE,
                EffectiveLength = (byte)content.Length,
                Content = content,
            };
        }
    }
}
