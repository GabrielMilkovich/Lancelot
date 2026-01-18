namespace Resurrection.Ressurrection.Domain.Base
{
    public class BaseResponseDatas
    {
        public string Data { get; private set; }

        public void AtribuiData(string data)
            => Data = data;
    }
}
