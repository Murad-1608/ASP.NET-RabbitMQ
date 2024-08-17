using RabbitMQ.Client;
using System.Text;

TopicExchange();


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

static void TopicExchange()
{
    var factory = new ConnectionFactory();

    factory.Uri = new Uri(@"amqps://jfkwbydf:H1aKLn77-MY-ERbXzJt5eyPGBJLznH29@octopus.rmq3.cloudamqp.com/jfkwbydf");

    using var connection = factory.CreateConnection();

    var channel = connection.CreateModel();

    channel.ExchangeDeclare("log-topic", durable: true, type: ExchangeType.Topic);

    Enumerable.Range(1, 50).ToList().ForEach(x =>
    {
        var log1 = (LogNames)new Random().Next(1, 4);
        var log2 = (LogNames)new Random().Next(1, 4);
        var log3 = (LogNames)new Random().Next(1, 4);


        var routeKey = $"{log1}.{log2}.{log3}";
        var message = $"{routeKey} - {x}";

        var messageBody = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish("log-topic", routeKey, null, messageBody);

        Console.WriteLine("Mesaj: " + message);

        Thread.Sleep(1500);

    });



}
enum LogNames
{
    Error = 1,
    Warning,
    Success,
}


