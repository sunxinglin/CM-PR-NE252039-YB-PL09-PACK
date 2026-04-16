namespace Ctp0600P.Client.PLC.Common;

public enum RequestFlag : ushort
{
    None = 0,
    Req = 1 << 0,
}

public class RequestFlagBuilder : FlagsBuilder<RequestFlag>
{
    public RequestFlagBuilder(RequestFlag wCmd) : base(wCmd)
    {

    }

    public RequestFlagBuilder Req(bool isOk)
    {
        SetOnOff(RequestFlag.Req, isOk).Build();
        return this;
    }

}