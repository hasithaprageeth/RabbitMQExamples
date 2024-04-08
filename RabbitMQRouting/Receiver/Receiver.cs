using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

///  Documentation : https://www.rabbitmq.com/tutorials/tutorial-four-dotnet
///  Run more than one instance of the receiver to see the runtime behavior in RabbitMQ
///  First Receiver - dotnet run info warning
///  Second Receiver - dotnet run error

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Create an exchange of direct type
channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

// Temporary queues
// Let server to choose the queue name - non-durable, exclusive, autodelete queue with a generated name
var queueName = channel.QueueDeclare().QueueName;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
                            Environment.GetCommandLineArgs()[0]);
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
    Environment.ExitCode = 1;
    return;
}

foreach (var severity in args)
{
    channel.QueueBind(queue: queueName,
                      exchange: "direct_logs",
                      routingKey: severity); // Roting key is set to severity
}

Console.WriteLine("Start listening to messages.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received {eventArgs.RoutingKey} : {message}");
};

channel.BasicConsume(queue: queueName,
                     autoAck: true,
                     consumer: consumer);

// With this line, we keep the program (event handler) running in the background
Console.ReadLine();