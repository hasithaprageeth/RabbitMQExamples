using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory
{
    Uri = new Uri("amqp://guest:guest@localhost:5672"),
    ClientProvidedName = "Sender" // Any preferred name
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var exchangeName = "inputExchange";
var routingKey = "inputqueue";
var queueName = "inputqueue";

// Exchange routes the messages directly
channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queue: queueName,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

// Bind queue to the exchange
channel.QueueBind(queueName, exchangeName, routingKey);

for (int i = 0; i < 10; i++)
{
    var body = Encoding.UTF8.GetBytes($"This is message {i + 1} from sender");

    channel.BasicPublish(exchange: string.Empty,
                     routingKey: routingKey,
                     basicProperties: null,
                     body: body);

    Console.WriteLine($"Sent a message {i + 1}");
    await Task.Delay(TimeSpan.FromSeconds(1));
}

Console.ReadLine();

channel.Close();
connection.Close();