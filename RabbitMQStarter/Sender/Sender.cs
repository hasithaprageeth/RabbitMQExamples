using System.Text;
using RabbitMQ.Client;

/// Documentation : https://www.rabbitmq.com/tutorials/tutorial-one-dotnet

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

    /// Sender never sends any messages directly to a queue
    /// The empty string denotes the default or nameless exchange, which is a direct type

    channel.BasicPublish(exchange: string.Empty,
                     routingKey: "inputqueue",  // Queue Name
                     basicProperties: null,
                     body: body);

    Console.WriteLine($"Sent a message {i+1}");
    await Task.Delay(TimeSpan.FromSeconds(1));
}

Console.ReadLine();