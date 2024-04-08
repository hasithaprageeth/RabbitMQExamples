using RabbitMQ.Client;
using System.Text;

/// Documentation : https://www.rabbitmq.com/tutorials/tutorial-five-dotnet
/// Sender - dotnet run

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Create a exchange of topic type
// Note : no queue is declared
channel.ExchangeDeclare(exchange: "topic_logs", type: ExchangeType.Topic);

string[] facility = ["auth", "cron", "kern"];
string[] severityStatus = ["info", "warning", "error"];
Random random1 = new();
Random random2 = new();

for (int i = 0; i < 1000; i++)
{
    int randomIndex1 = random1.Next(0, facility.Length);
    int randomIndex2 = random1.Next(0, severityStatus.Length);
    var routingKey = $"{facility[randomIndex1]}.{severityStatus[randomIndex2]}";
    var body = Encoding.UTF8.GetBytes($"This is message {i + 1} from sender");

    channel.BasicPublish(exchange: "topic_logs",
                         routingKey: routingKey, // Roting key is <facility>.<severity>
                         basicProperties: null,
                         body: body);

    Console.WriteLine($"Sent a message {i + 1}");
    await Task.Delay(TimeSpan.FromSeconds(1));
}

Console.ReadLine();