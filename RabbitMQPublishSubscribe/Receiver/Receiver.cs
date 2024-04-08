using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

///  Documentation : https://www.rabbitmq.com/tutorials/tutorial-three-dotnet
///  Run more than one instance of the receiver to see the runtime behavior in RabbitMQ

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Create an exchange of fanout type
channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

// Temporary queues
// Let server to chosse the queue name - non-durable, exclusive, autodelete queue with a generated name
var queueName = channel.QueueDeclare().QueueName;
channel.QueueBind(queue: queueName,
                  exchange: "logs",
                  routingKey: string.Empty); // Roting key is ignored with fanout exchange

Console.WriteLine("Start listening to messages.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received {message}");
};

channel.BasicConsume(queue: queueName,
                     autoAck: true,
                     consumer: consumer);

// With this line, we keep the program (event handler) running in the background
Console.ReadLine();