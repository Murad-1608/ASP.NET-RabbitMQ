using RabbitMQ.Client;
using System.Text;

DirectExchange();


static void DirectExchange()
{
    var factory = new ConnectionFactory();

    factory.Uri = new Uri(@"amqps://jfkwbydf:H1aKLn77-MY-ERbXzJt5eyPGBJLznH29@octopus.rmq3.cloudamqp.com/jfkwbydf");

    using var connection = factory.CreateConnection();

    var channel = connection.CreateModel();

    channel.ExchangeDeclare("direct-log-test", durable: true, type: ExchangeType.Direct);

    Enum.GetNames(typeof(LogNames)).ToList().ForEach(name =>
    {
        var routeKey = $"direct-{name}";
        var queueName = $"direct-queue-{name}";

        channel.QueueDeclare(queueName, true, false, false);

        channel.QueueBind(queueName, "direct-log-test", routeKey);
    });

    Enumerable.Range(1, 50).ToList().ForEach(x =>
    {
        var log = (LogNames)new Random().Next(1, 4);

        var randomRoute = $"direct-{log}";

        string message = $"Message {x} - {randomRoute}";

        var messageAsByte = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish("direct-log-test", randomRoute, null, messageAsByte);

        Console.WriteLine("Mesaj: " + message);
    });

}
enum LogNames
{
    Error = 1,
    Warning,
    Success,
}


