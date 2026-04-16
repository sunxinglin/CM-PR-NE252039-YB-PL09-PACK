namespace Ctp0600P.Client.Protocols.AnyLoad_Wifi;

public class AnyLoadApi : IAnyLoadApi
{
    private readonly AnyLoadSendMessage _sendMessage;
    private readonly AnyLoadMgr _anyLoadMgr;
    public AnyLoadApi(AnyLoadSendMessage sendMessage, AnyLoadMgr anyLoadMgr)
    {
        _anyLoadMgr = anyLoadMgr;
        _sendMessage = sendMessage;
    }

    public async Task ReadCurrentWeightData()
    {
        var message = _sendMessage.ReadCurrentWeightData();
        await _anyLoadMgr._AnyLoadCtrl.ReadWeightData(message);
    }
}