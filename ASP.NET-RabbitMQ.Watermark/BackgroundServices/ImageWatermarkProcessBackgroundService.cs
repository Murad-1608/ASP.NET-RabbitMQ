﻿using ASP.NET_RabbitMQ.Watermark.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace ASP.NET_RabbitMQ.Watermark.BackgroundServices
{
    public class ImageWatermarkProcessBackgroundService : BackgroundService
    {
        private readonly RabbitMQClientService _rabbitmqClientService;
        private readonly ILogger<ImageWatermarkProcessBackgroundService> _logger;
        private IModel _channel;
        public ImageWatermarkProcessBackgroundService(RabbitMQClientService rabbitmqClientService, ILogger<ImageWatermarkProcessBackgroundService> logger)
        {
            _rabbitmqClientService = rabbitmqClientService;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitmqClientService.Connect();
            _channel.BasicQos(0, 1, false);


            return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            _channel.BasicConsume(RabbitMQClientService.QueueName, false, consumer);

            consumer.Received += Consumer_Received;

            return Task.CompletedTask;
        }

        private Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {

            try
            {
                var imageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));


                var path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwrooot\images", imageCreatedEvent.ImageName);

                using var img = Image.FromFile(path);

                using var graphic = Graphics.FromImage(img);



                var font = new Font(FontFamily.GenericMonospace, 40, FontStyle.Bold, GraphicsUnit.Pixel);

                var textSize = graphic.MeasureString("www.mysite.com", font);

                var color = Color.FromArgb(128, 255, 255, 255);

                var brush = new SolidBrush(color);

                var position = new Point(img.Width - ((int)textSize.Width + 30), img.Height - ((int)textSize.Height + 30));

                graphic.DrawString("www.mysite.com", font, brush, position);

                img.Save(@"wwwroot\images\watermarks\" + imageCreatedEvent.ImageName);

                img.Dispose();
                graphic.Dispose();

                _channel.BasicAck(@event.DeliveryTag, false);
            }
            catch (Exception)
            {
                throw;
            }

            return Task.CompletedTask;


        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
