using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AsZero.Core.Entities
{
    public class FuncResource
    {
        private const char JoinerChar = ';';

        private static readonly Regex _regex = new Regex("t=(?<t>.*)[|]v=(?<v>.*)", RegexOptions.Compiled);

        public int Id { get; set; }

        public string UniqueName { get; set; } = "";


        public string Description { get; set; } = "";


        public int? ParentId { get; set; }

        public FuncResource Parent { get; set; }

        public IList<Claim> AllowedClaims { get; set; } = new List<Claim>();


        public bool Configurable { get; set; } = true;


        public bool IsDeleted { get; set; }

        public static string ClaimsToString(IList<Claim> claims)
        {
            IEnumerable<string> values = from c in claims
                                         where c != null
                                         select "t=" + c.Type + "|v=" + c.Value;
            return string.Join($"{';'}", values);
        }

        public static IList<Claim> StringToClaims(string str)
        {
            string[] array = str.Split(new char[1] { ';' });
            List<Claim> list = new List<Claim>();
            string[] array2 = array;
            foreach (string input in array2)
            {
                Match match = _regex.Match(input);
                if (!match.Success)
                {
                    return null;
                }

                string value = match.Groups["t"].Value;
                string value2 = match.Groups["v"].Value;
                list.Add(new Claim(value, value2));
            }

            return list;
        }
    }
}
