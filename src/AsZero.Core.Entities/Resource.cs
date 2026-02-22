using FutureTech.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace AsZero.Core.Entities
{

    /// <summary>
    /// 资源
    /// </summary>
    public class Resource : FutureBaseEntity<int>
    {
        /// <summary>
        /// 短名
        /// </summary>
        public string UniqueName { get; set; }


        /// <summary>
        /// 资源描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 父节点ID，如果是根节点，则为NULL
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 父节点，如果是根节点，则为NULL
        /// </summary>
        public Resource Parent { get; set; }

        /// <summary>
        /// 允许的Claim
        /// </summary>
        public IList<Claim> AllowedClaims { get; set; }

        /// <summary>
        /// 可配置?
        /// </summary>
        public bool Configurable { get; set; } = true;

        private const char JoinerChar = ';';

        /// <summary>
        /// the input must not be null
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        internal static string ClaimsToString(IList<Claim> claims)
        {
            var list = claims
                .Where(c => c != null)
                .Select(c => $"t={c.Type}|v={c.Value}");

            return string.Join($"{JoinerChar}", list);
        }

        private static readonly Regex _regex = new("t=(?<t>.*)[|]v=(?<v>.*)", RegexOptions.Compiled);

        /// <summary>
        /// the input must not be null
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static IList<Claim> StringToCliams(string str)
        {
            var pairstrs = str.Split(JoinerChar);

            var list = new List<Claim>();
            foreach (var pair in pairstrs)
            {
                var r = _regex.Match(pair);
                if (!r.Success)
                {
                    return null;
                }
                var t = r.Groups["t"].Value;
                var v = r.Groups["v"].Value;
                list.Add(new Claim(t, v));
            }
            return list;
        }

    }

    internal class ResouceConfiguration : IEntityTypeConfiguration<Resource>
    {

        public void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.HasIndex(r =>  r.UniqueName);

            builder.Property(r => r.AllowedClaims)
                .HasConversion(
                    p => p == null ? null : Resource.ClaimsToString(p),
                    s => s == null ? null : Resource.StringToCliams(s)
                );
        }
    }

}
