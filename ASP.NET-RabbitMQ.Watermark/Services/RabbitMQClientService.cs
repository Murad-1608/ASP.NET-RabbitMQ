﻿using RabbitMQ.Client;

namespace ASP.NET_RabbitMQ.Watermark.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWatermark = "watermark-route-image";
        public static string QueueName = "queue-watermark-image";
        private readonly ILogger<RabbitMQClientService> _logger;
        public RabbitMQClientService(ConnectionFactory connectionFactory,
                                     ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();

            if (_channel is { IsClosed: true })
            {
                return _channel;
            }

            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, type: ExchangeType.Direct, true, false);

            _channel.QueueDeclare(QueueName, true, false, false, null);

            _channel.QueueBind(QueueName, ExchangeName, routingKey: RoutingWatermark);

            _logger.LogInformation("RabbitMQ ile elaqe quruldu");

            return _channel;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();

            _logger.LogInformation("RabbitMQ ile elaqe kesildi");
        }
    }
}
