using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/// Documentation : https://www.rabbitmq.com/tutorials/tutorial-two-dotnet

/// By default, RabbitMQ will send each message to the next consumer, in sequence. 
/// On average every consumer will get the same number of messages. 
/// This way of distributing messages is called round-robin.

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "task_queue",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

/// Fair Dispatch - (prefetchCount: 1)
/// Don't dispatch a new message to a worker until it has processed and acknowledged the previous one. 7
/// Instead, it will dispatch it to the next worker that is not still busy.

channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

Console.WriteLine("Start listening to messages.");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, eventArgs) =>
{
    byte[] body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received : {message}");

    int dots = message.Split('.').Length - 1;
    Thread.Sleep(dots * 1000);

    Console.WriteLine("Completed");

    // If a consumer node is terminated while it was processing a message, nothing is lost.
    // Soon after the consumer node is terminated, all unacknowledged messages will be redelivered.
    channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
};

// Enabled Manual Message Acknowledgement - to make sure a message is never lost
channel.BasicConsume(queue: "task_queue",
                     autoAck: false,
                     consumer: consumer);

// With this line, we keep the program (event handler) running in the background
Console.ReadLine();