using RabbitMQ.Client;
using System.Text;

/// Documentation : https://www.rabbitmq.com/tutorials/tutorial-four-dotnet

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Create a exchange of direct type
// Note : no queue is declared
channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

string[] severityStatus = ["info", "warning", "error"];
Random random = new();

for (int i = 0; i < 1000; i++)
{
    int randomIndex = random.Next(0, severityStatus.Length);
    var severity = severityStatus[randomIndex];
    var body = Encoding.UTF8.GetBytes($"This is message {i + 1} from sender");

    channel.BasicPublish(exchange: "direct_logs",
                         routingKey: severity, // Roting key is severity
                         basicProperties: null,
                         body: body);

    Console.WriteLine($"Sent a message {i + 1}");
    await Task.Delay(TimeSpan.FromSeconds(1));
}

Console.ReadLine();