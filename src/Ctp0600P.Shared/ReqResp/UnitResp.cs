namespace Ctp0600P.Shared
{

    public class UnitResp<TPayload>
    { 
        public int Code { get; set; }
        public string Msg { get; set; }
        public TPayload Payload { get; set; }
    }

    public class UnitResp : UnitResp<object>
    { 
    }
}
