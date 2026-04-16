using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

using Yee.Entitys.BaseData;
using Yee.Entitys.Common;

namespace Yee.Entitys.Production;

[Table("Base_Product")]
public class Base_Product : CommonData
{
    [MaxLength(200)] public string? Code { get; set; }
    [MaxLength(200)] public string? Name { get; set; }
    public int TypeId { get; set; }
    public DictionaryDetail? Type { get; set; }

    [MaxLength(200)] public string? Specification { get; set; }

    [MaxLength(200)] public string? PackPNRule { get; set; }

    [MaxLength(200)] public string? PackOutCodeRule { get; set; }

    public string? ModelRulesstr
    {
        get
        {
            if (this.ModelRules != null)
            {
                var ret = JsonConvert.SerializeObject(this.ModelRules);
                return ret;
            }
            else
            {
                return string.Empty;
            }
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                ModelRules = JsonConvert.DeserializeObject<List<Keyvalue>>(value);
            }
            else
            {
                ModelRules = new List<Keyvalue>();
            }
        }
    }

    [NotMapped] public List<Keyvalue> ModelRules { get; set; }
}

public class Keyvalue
{
    public object Key { get; set; }
    public string Value { get; set; }
}