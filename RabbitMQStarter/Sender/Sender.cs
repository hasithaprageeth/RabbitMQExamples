using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "inputqueue",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

for(int i = 0; i < 10; i++)
{
    var body = Encoding.UTF8.GetBytes($"This is message {i+1} from sender");

    channel.BasicPublish(exchange: string.Empty,
                     routingKey: "inputqueue",  // Queue Name
                     basicProperties: null,
                     body: body);

    Console.WriteLine($"Sent a message {i+1}");
    await Task.Delay(TimeSpan.FromSeconds(1));
}

Console.ReadLine();