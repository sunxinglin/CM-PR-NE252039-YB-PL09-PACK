using Microsoft.AspNetCore.Http;

using Yee.Entitys.DBEntity.Production;

namespace Yee.Entitys.DTOS.StationTaskDataDTOS
{
    public class TightenByImageDto
    {
        public int Id { get; set; }
        public string TaskName { get; set; } = "";
    }

    public class LayoutInfoDto
    {
        public int TaskId { get; set; } = 0;
        public string ImageUrl { get; set; } = "";
        public CanvasLayout? CanvasLayout { get; set; }
    }

    public class UploadImageDTO
    {
        public int TaskId { get; set; }
        public string? TaskName { get; set; }
        public IFormFile ImageFile { get; set; }
    }


}
