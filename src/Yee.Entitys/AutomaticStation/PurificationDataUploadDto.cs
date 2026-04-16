namespace Yee.Entitys.AutomaticStation
{
    public class PurificationDataUploadDto
    {
        public string BlockCode { get; set; } = string.Empty;

        public Dictionary<string, object> Datas { get; set; } = null!;
    }
}
