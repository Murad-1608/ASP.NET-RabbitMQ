using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();

factory.Uri = new Uri(@"amqps://jfkwbydf:H1aKLn77-MY-ERbXzJt5eyPGBJLznH29@octopus.rmq3.cloudamqp.com/jfkwbydf");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

var queueName = "Test-queue";
var routeKey = "*.Error.*";

channel.QueueDeclare(queueName, true, false, false);


channel.QueueBind(queueName, "log-topic", routeKey, null);

var consumer = new EventingBasicConsumer(channel);

channel.BasicConsume(queueName, false, consumer);

Console.WriteLine("Loglar başladılır....");

consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    try
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        Thread.Sleep(1000);
        Console.WriteLine("Gelen mesaj: " + message);

        channel.BasicAck(e.DeliveryTag, false);
    }
    catch (Exception)
    {
        throw;
    }

};