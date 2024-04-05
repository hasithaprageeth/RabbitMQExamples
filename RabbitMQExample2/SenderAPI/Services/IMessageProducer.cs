namespace SenderAPI.Services
{
    public interface IMessageProducer
    {
        public void SendMessage<T>(T message);
    }
}
