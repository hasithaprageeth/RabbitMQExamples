using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Create a exchange of fanout type
// Note : no queue is declared
channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

for (int i = 0; i < 1000; i++)
{
    var body = Encoding.UTF8.GetBytes($"This is message {i + 1} from sender");

    channel.BasicPublish(exchange: "logs",
                         routingKey: string.Empty, // Roting key is ignored with fanout exchange
                         basicProperties: null,
                         body: body);

    Console.WriteLine($"Sent a message {i + 1}");
    await Task.Delay(TimeSpan.FromSeconds(1));
}

Console.ReadLine();