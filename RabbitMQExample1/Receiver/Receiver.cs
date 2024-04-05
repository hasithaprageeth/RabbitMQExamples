using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    Uri = new Uri("amqp://guest:guest@localhost:5672"),
    ClientProvidedName = "Receiver" // Any preferred name
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

// Fair Dispatch
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

Console.WriteLine("Start listening to messages.");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received {message}");

    channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
};

var consumerTag = channel.BasicConsume(queue: "inputqueue",
                                       autoAck: false,
                                       consumer: consumer);
Console.ReadLine();

// Cancel/deletes the consumer
channel.BasicCancel(consumerTag);

channel.Close();
connection.Close();