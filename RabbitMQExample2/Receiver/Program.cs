using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine($"Start listening.");

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    UserName = "guest",
    Password = "guest",
    VirtualHost = "/",
};

var connection = factory.CreateConnection();
var channel = connection.CreateModel();

channel.QueueDeclare("bookings", durable: true, exclusive: false);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Received : {message}");
};

channel.BasicConsume("bookings", true, consumer);

Console.ReadLine();