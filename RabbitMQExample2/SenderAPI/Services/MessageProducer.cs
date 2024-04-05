using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace SenderAPI.Services
{
    public class MessageProducer : IMessageProducer
    {
        public void SendMessage<T>(T message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare("bookings", durable: true, exclusive: false);

            var jsonBody = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonBody);

            channel.BasicPublish(string.Empty, "bookings", body: body);
        }
    }
}
