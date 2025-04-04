﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Shared;
using System.Text;
using System.Text.Json;

class subscriber
{
    static void Main(string[] args)
    {

        //****************** 4. Context Typeları Mesaj Olarak İletmek ***************//
        // 4.header
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqps://klihfdct:Tp5GHLQjqsmXgG_1544BvHhTgdnKTkNs@kebnekaise.lmq.cloudamqp.com/klihfdct");

        using var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.BasicQos(0, 1, false);

        var subscriber = new EventingBasicConsumer(channel);

        var queueName = channel.QueueDeclare().QueueName;
        Dictionary<string,object> headers = new Dictionary<string, object>();
        headers.Add("format", "pdf");
        headers.Add("shape", "a4");
        headers.Add("x-match", "any");
        channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);
        channel.QueueBind(queueName, "header-exchange", string.Empty,headers);
        channel.BasicConsume(queueName, false, subscriber);

        Console.WriteLine("loglar dinleniyor");

        subscriber.Received += (object? sender, BasicDeliverEventArgs e) =>
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            Product product = JsonSerializer.Deserialize<Product>(message);
            Thread.Sleep(1000);
            Console.WriteLine($"Gelen Mesaj : {product.Id }-{product.Name }-{product.Price }-{product.Stock }");
            channel.BasicAck(e.DeliveryTag, false);
        };

        Console.ReadLine();

    }

}

//*************** 1 VE 2. DERSLERDE YAZILAN KODLAR***************//
//var factory = new ConnectionFactory();
//factory.Uri = new Uri("amqps://klihfdct:Tp5GHLQjqsmXgG_1544BvHhTgdnKTkNs@kebnekaise.lmq.cloudamqp.com/klihfdct");

//using var connection = factory.CreateConnection();
//var channel = connection.CreateModel();

////Durable değerini true yaparsam rabbitmq de fiziksel olarak kaydedilir.
////Ama false yaparsan memoryde geçici yer edinir ve açıp kapadığımda silinir.
////channel.QueueDeclare("Hello-Queue", true, false, true);
//channel.BasicQos(0, 1, false);
////burda birden fazla subscriber çalıştırmak için windows komut isteminden
////subscriber dosyasının yolunu girip ordan her iki ekrandanda dotnet run
//// yapılırsa iki farklı subscriber ve channel olur.
//var subscriber = new EventingBasicConsumer(channel);
//channel.BasicConsume("Hello-Queue",false,subscriber);
//subscriber.Received += (object? sender, BasicDeliverEventArgs e)=> {
//    var message = Encoding.UTF8.GetString(e.Body.ToArray());
//    Thread.Sleep(1500);
//    Console.WriteLine(message);
//    channel.BasicAck(e.DeliveryTag, false);
//};

//Console.ReadLine();

//****************** 3. Ders Exchange Types ***************//
// 1.Fanout
//var factory = new ConnectionFactory();
//factory.Uri = new Uri("amqps://klihfdct:Tp5GHLQjqsmXgG_1544BvHhTgdnKTkNs@kebnekaise.lmq.cloudamqp.com/klihfdct");

//using var connection = factory.CreateConnection();
//var channel = connection.CreateModel();



//var randomQueueName = channel.QueueDeclare().QueueName; // burası random queue ismi oluşturmaya yarıyor.

////var randomQueueName = "log-database-save-queue"; // burdaki yaptığım değişiklik queuenin kalısı olmasını sağladı.
////channel.QueueDeclare(randomQueueName, true, false, false);
//// bu durumda queue lar kalıcı olmuyorlar bağlantıları koptuklarında gidiyolar.
////eğer kaydedilmesi isteniyorsa yukarıdaki yazılabilir
//channel.QueueBind(randomQueueName, "logs-fanout", "", null);
//channel.BasicQos(0, 1, false);

//var subscriber = new EventingBasicConsumer(channel);
//channel.BasicConsume(randomQueueName, false, subscriber);

//Console.WriteLine("loglar dinleniyor");

//subscriber.Received += (object? sender, BasicDeliverEventArgs e) => {
//    var message = Encoding.UTF8.GetString(e.Body.ToArray());
//    Thread.Sleep(1000);
//    Console.WriteLine(message);
//    channel.BasicAck(e.DeliveryTag, false);
//};

//Console.ReadLine();

/******************************************************/
// 2.direct
//var factory = new ConnectionFactory();
//factory.Uri = new Uri("amqps://klihfdct:Tp5GHLQjqsmXgG_1544BvHhTgdnKTkNs@kebnekaise.lmq.cloudamqp.com/klihfdct");

//using var connection = factory.CreateConnection();
//var channel = connection.CreateModel();

//channel.BasicQos(0, 1, false);

//var subscriber = new EventingBasicConsumer(channel);


////string arg = args[0];
////var queueName = $"direct-queuq-{arg}";
////linux üzerinden çalıştırılırsa birden fazla pencerede birden fazla
////direct type ı görebilmek için kullanıllır

//var queueName = "direct-queuq-Error";
//channel.BasicConsume(queueName, false, subscriber);

//Console.WriteLine("loglar dinleniyor");

//subscriber.Received += (object? sender, BasicDeliverEventArgs e) =>
//{
//    var message = Encoding.UTF8.GetString(e.Body.ToArray());
//    Thread.Sleep(1000);
//    Console.WriteLine(message);

//    File.AppendAllText("log-critical.txt", message + "\n");
//    channel.BasicAck(e.DeliveryTag, false);
//};

//Console.ReadLine();

/*******************************************************/
// 3.topic
//var factory = new ConnectionFactory();
//factory.Uri = new Uri("amqps://klihfdct:Tp5GHLQjqsmXgG_1544BvHhTgdnKTkNs@kebnekaise.lmq.cloudamqp.com/klihfdct");

//using var connection = factory.CreateConnection();
//var channel = connection.CreateModel();

//channel.BasicQos(0, 1, false);

//var subscriber = new EventingBasicConsumer(channel);

//var queueName = channel.QueueDeclare().QueueName;
//var routeKey = "*.Error.*";
//var routeKey2 = "Info.#"; // burdaki '#' bu sembol sonrası ne olursa olsun anlamnında
//                          // '*' sadece bir tanesi için '#' birden fazla için kullanılıyor
//var routeKey3 = "*.*.Warning";

//channel.QueueBind(queueName, "logs-topic", routeKey2);
//channel.BasicConsume(queueName, false, subscriber);

//Console.WriteLine("loglar dinleniyor");

//subscriber.Received += (object? sender, BasicDeliverEventArgs e) =>
//{
//    var message = Encoding.UTF8.GetString(e.Body.ToArray());
//    Thread.Sleep(1000);
//    Console.WriteLine(message);

//    File.AppendAllText("log-critical.txt", message + "\n");
//    channel.BasicAck(e.DeliveryTag, false);
//};

//Console.ReadLine();

/********************************************************/
// 4.header
//var factory = new ConnectionFactory();
//factory.Uri = new Uri("amqps://klihfdct:Tp5GHLQjqsmXgG_1544BvHhTgdnKTkNs@kebnekaise.lmq.cloudamqp.com/klihfdct");

//using var connection = factory.CreateConnection();
//var channel = connection.CreateModel();

//channel.BasicQos(0, 1, false);

//var subscriber = new EventingBasicConsumer(channel);

//var queueName = channel.QueueDeclare().QueueName;
//Dictionary<string, object> headers = new Dictionary<string, object>();
//headers.Add("format", "pdf");
//headers.Add("shape", "a4");
//headers.Add("x-match", "any");
//channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);
//channel.QueueBind(queueName, "header-exchange", string.Empty, headers);
//channel.BasicConsume(queueName, false, subscriber);

//Console.WriteLine("loglar dinleniyor");

//subscriber.Received += (object? sender, BasicDeliverEventArgs e) =>
//{
//    var message = Encoding.UTF8.GetString(e.Body.ToArray());
//    Thread.Sleep(1000);
//    Console.WriteLine(message);
//    channel.BasicAck(e.DeliveryTag, false);
//};

//Console.ReadLine();
