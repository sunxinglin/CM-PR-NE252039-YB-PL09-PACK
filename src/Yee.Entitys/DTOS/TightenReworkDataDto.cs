using Yee.Entitys.DTOS.StationTaskDataDTOS;

namespace Yee.Entitys.DTOS;

public  class TightenReworkDataDto:CommonDataDto
{
    public string TaskName { get; set; } = "";

    public int OrderNo { get; set; }

    public bool ResultOk { get; set; }

    public int ProgramNo { get; set; }
    public decimal TorqueValue { get; set; }

    public decimal AngleValue { get; set; }
        
    public decimal TorqueMin { get; set; }

    public decimal TorqueMax { get; set; }
        
    public decimal AngleMin { get; set; }
        
    public decimal AngleMax { get; set; }

    public string UpMesCode { get; set; } = "";
    public string UpMesCodeJD { get; set; } = "";
        
    public int Operator { get; set; }
}