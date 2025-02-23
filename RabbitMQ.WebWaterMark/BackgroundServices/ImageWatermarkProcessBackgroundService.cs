using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.WebWaterMark.Services;
//using System.Drawing;

using System.Drawing.Text;
using System.Text;
using System.Text.Json;
using System.Drawing;
using SkiaSharp;

namespace RabbitMQ.WebWaterMark.BackgroundServices
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
            _channel.BasicQos(0,1,false);

            return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMQClientService.QueueName,false, consumer);
            consumer.Received += Consumer_Receiver;
            return Task.CompletedTask;
        }

        private  Task Consumer_Receiver(object sender, BasicDeliverEventArgs @event)
        {
            //try
            //{
            //    var productImageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>
            //   (Encoding.UTF8.GetString(@event.Body.ToArray()));
            //    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ımages",
            //        productImageCreatedEvent.ImageName);
            //    var name = "www.mysite.com";
            //    using var img = Image.FromFile(path);
            //    using var graphic = Graphics.FromImage(img);
            //    var font = new Font(FontFamily.GenericMonospace, 32, FontStyle.Bold, GraphicsUnit.Pixel);

            //    var textSize = graphic.MeasureString(name, font);
            //    var color = Color.FromArgb(128, 255, 255, 255);
            //    var brush = new SolidBrush(color);
            //    var position = new Point(img.Width - ((int)textSize.Width - 30), img.Height - ((int)textSize.Height + 30));

            //    graphic.DrawString(name, font, brush, position);
            //    img.Save("wwwroot/ımages/watermarks/" + productImageCreatedEvent.ImageName);
            //    img.Dispose();
            //    graphic.Dispose();
            //    _channel.BasicAck(@event.DeliveryTag, false);

            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex.Message);
            //}
            //return Task.CompletedTask;
            try
            {
                var productImageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>
                    (Encoding.UTF8.GetString(@event.Body.ToArray()));

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ımages",
                    productImageCreatedEvent.ImageName);
                var name = "www.mysite.com";

                using var inputStream = File.OpenRead(path);
                using var bitmap = SKBitmap.Decode(inputStream);
                using var image = SKImage.FromBitmap(bitmap);
                using var canvas = new SKCanvas(bitmap);
                using var paint = new SKPaint
                {
                    Color = new SKColor(255, 255, 255, 128), // White with transparency
                    TextSize = 32,
                    IsAntialias = true,
                    Typeface = SKTypeface.Default
                };

                var textBounds = new SKRect();
                paint.MeasureText(name, ref textBounds);

                float x = bitmap.Width - textBounds.Width - 30;
                float y = bitmap.Height - textBounds.Height - 30;

                canvas.DrawText(name, x, y, paint);

                using var outputStream = File.OpenWrite(Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/ımages/watermarks", productImageCreatedEvent.ImageName));

                bitmap.Encode(outputStream, SKEncodedImageFormat.Png, 100);

                _channel.BasicAck(@event.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
