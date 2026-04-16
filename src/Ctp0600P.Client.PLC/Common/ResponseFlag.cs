namespace Ctp0600P.Client.PLC.Common;

public enum ResponseFlag : ushort
{
    None = 0,
    Ack = 1 << 0,
    OK = 1 << 1,
    NG = 1 << 2,
}

public class ResponseFlagBuilder : FlagsBuilder<ResponseFlag>
{
    public ResponseFlagBuilder(ResponseFlag wCmd) : base(wCmd)
    {

    }

    public ResponseFlagBuilder Ack(bool isOk)
    {
        SetOnOff(ResponseFlag.Ack, true)
            .SetOnOff(ResponseFlag.OK, isOk)
            .SetOnOff(ResponseFlag.NG, !isOk)
            .Build();
        return this;
    }

    public ResponseFlagBuilder Reset()
    {
        SetOnOff(ResponseFlag.Ack, false)
            .SetOnOff(ResponseFlag.OK, false)
            .SetOnOff(ResponseFlag.NG, false)
            .Build();
        return this;
    }
}