using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

///  Documentation : https://www.rabbitmq.com/tutorials/tutorial-six-dotnet

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var rpcQueueName = "rpc_queue";

// Use default direct exchange
// Declares RPC queue, non-durable, non-exclusive, manual delete queue
channel.QueueDeclare(queue: rpcQueueName,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

// Fair Dispatch
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

var consumer = new EventingBasicConsumer(channel);

Console.WriteLine("Awaiting RPC requests");

consumer.Received += (model, eventArgs) =>
{
    string response = string.Empty;

    var body = eventArgs.Body.ToArray();
    var props = eventArgs.BasicProperties;
    var replyProps = channel.CreateBasicProperties();
    replyProps.CorrelationId = props.CorrelationId;

    try
    {
        var message = Encoding.UTF8.GetString(body);
        int n = int.Parse(message);
        Console.WriteLine($"Calculating : Fib({message})");
        response = Fib(n).ToString();
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error : {e.Message}");
        response = string.Empty;
    }
    finally
    {
        var responseBytes = Encoding.UTF8.GetBytes(response);
        channel.BasicPublish(exchange: string.Empty,
                             routingKey: props.ReplyTo,  // Reply queue
                             basicProperties: replyProps,
                             body: responseBytes);

        channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
    }
};

channel.BasicConsume(queue: rpcQueueName,
                     autoAck: false,
                     consumer: consumer);

// With this line, we keep the program (event handler) running in the background
Console.ReadLine();

// Assumes only valid positive integer input.
// Don't expect this one to work for big numbers, and it's probably the slowest recursive implementation possible.
static int Fib(int n)
{
    if (n is 0 or 1)
    {
        return n;
    }

    return Fib(n - 1) + Fib(n - 2);
}
