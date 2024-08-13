using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();

factory.Uri = new Uri(@"amqps://jfkwbydf:H1aKLn77-MY-ERbXzJt5eyPGBJLznH29@octopus.rmq3.cloudamqp.com/jfkwbydf");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.QueueDeclare("hello-queue", true, false, false);

string message = "Hello world";

var messageAsByte = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(string.Empty, "hello-queue", null, messageAsByte);

Console.WriteLine("Message send completed");