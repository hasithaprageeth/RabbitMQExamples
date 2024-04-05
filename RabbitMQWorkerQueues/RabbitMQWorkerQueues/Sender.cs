using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Queue Durability
channel.QueueDeclare(queue: "task_queue",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

for (int i = 0; i < 5; i++)
{
    var message = $"This is message{new string('.', (i + 1))}";
    var body = Encoding.UTF8.GetBytes(message);

    // Message Durability
    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;

    channel.BasicPublish(exchange: string.Empty,
                     routingKey: "task_queue",  // Queue Name
                     basicProperties: properties,
                     body: body);

    Console.WriteLine($"Sent : {message}");
    await Task.Delay(TimeSpan.FromSeconds(1));
}

Console.ReadLine();