using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        //*************** 1 VE 2. DERSLERDE YAZILAN KODLAR***************//
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqps://klihfdct:Tp5GHLQjqsmXgG_1544BvHhTgdnKTkNs@kebnekaise.lmq.cloudamqp.com/klihfdct");

        using var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        //Durable değerini true yaparsam rabbitmq de fiziksel olarak kaydedilir.
        //Ama false yaparsan memoryde geçici yer edinir ve açıp kapadığımda silinir.
        //channel.QueueDeclare("Hello-Queue", true, false, true);
        channel.BasicQos(0, 1, false);
        //burda birden fazla subscriber çalıştırmak için windows komut isteminden
        //subscriber dosyasının yolunu girip ordan her iki ekrandanda dotnet run
        // yapılırsa iki farklı subscriber ve channel olur.
        var subscriber = new EventingBasicConsumer(channel);
        channel.BasicConsume("Hello-Queue", false, subscriber);
        subscriber.Received += (object? sender, BasicDeliverEventArgs e) =>
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            Thread.Sleep(1500);
            Console.WriteLine(message);
            channel.BasicAck(e.DeliveryTag, false);
        };

        Console.ReadLine();

    }

}