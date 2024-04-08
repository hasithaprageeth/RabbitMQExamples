using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/// Documentation : https://www.rabbitmq.com/tutorials/tutorial-one-dotnet

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Use default direct exchange
channel.QueueDeclare(queue: "inputqueue",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

Console.WriteLine("Start listening to messages.");

var consumer = new EventingBasicConsumer(channel);

// Consuming messages happen asynchronously, so we provide a callback. EventingBasicConsumer.Received event handler
consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received {message}");
};

// Enabled Auto Message Acknowledgement
channel.BasicConsume(queue: "inputqueue",
                     autoAck: true,
                     consumer: consumer);

// With this line, we keep the program (event handler) running in the background
Console.ReadLine();