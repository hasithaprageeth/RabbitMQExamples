using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

///  Documentation : https://www.rabbitmq.com/tutorials/tutorial-five-dotnet
///  Run more than one instance of the receiver to see the runtime behavior in RabbitMQ
///  First Receiver - dotnet run "#" - To receive all the logs
///  Second Receiver - dotnet run "kern.*"  - To receive all logs from the facility "kern"
///  Third Receiver - dotnet run "*.error"  - To receive all logs from the severity "error"
///  Or use dotnet run "kern.*" "*.error"

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Create an exchange of topic type
channel.ExchangeDeclare(exchange: "topic_logs", type: ExchangeType.Topic);

// Temporary queues
// Let server to choose the queue name - non-durable, exclusive, autodelete queue with a generated name
var queueName = channel.QueueDeclare().QueueName;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: {0} [binding_key...]",
                            Environment.GetCommandLineArgs()[0]);
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
    Environment.ExitCode = 1;
    return;
}

foreach (var severity in args)
{
    channel.QueueBind(queue: queueName,
                      exchange: "topic_logs",
                      routingKey: severity); // Roting key is <facility>.<severity>
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