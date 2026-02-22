using FutureTech.OpResults;

namespace HJZK.IOController
{
    public interface IIOBoxApi
    {
        public Task<bool[]> GetDoAsync(IOBoxConfig box);
        public Task<bool[]> GetDIAsync(IOBoxConfig box);
        public Task PutMultipleDoAsync(IOBoxConfig box, bool[] statues);
        public Task PutSingleDoAsync(IOBoxConfig box, ushort adress, bool statue);
        public void PutMultipleCoil(IOBoxConfig box, bool[] statues);
        public void PutMultipleCoil(IOBoxConfig box,ushort startadress, bool[] statues);
        public void PutsingleCoil(IOBoxConfig box, ushort adress, bool statue);
        public Task<OpResult<object>> OnRedLed(DoStatusEnum status);
        public Task<OpResult<object>> OnGreenLed(DoStatusEnum status);
        public Task<OpResult<object>> OnYellowLed(DoStatusEnum status);
        public Task<OpResult<object>> StartAction(DoStatusEnum status);
        public Task<OpResult<object>> ResetAction(DoStatusEnum status);
        public Task<OpResult<object>> OnBuzzer(DoStatusEnum status);
        public Task<OpResult<object>> StartSpecialAction(DoStatusEnum status);
        public Task<OpResult<object>> OnRedLedSpecial(DoStatusEnum status);

        public Task<OpResult<object>> OnTaoTongLed(int taoTongNo, DoStatusEnum status);
        public Task PutDoAsync(IOBoxConfig box);
    }
}
