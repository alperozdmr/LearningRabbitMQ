using RabbitMQ.Client;
using RabbitMQ.Shared;
using System.Text;
using System.Text.Json;

class publisher
{
    //Direct types
    public enum LogNames
    {
        Critical =1,
        Error=2,
        Warning=3,
        Info =4
    }
    static void Main(string[] args)
    {

        //****************** 4. Context Typeları Mesaj Olarak İletmek ***************//
        //4.Header
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqps://klihfdct:Tp5GHLQjqsmXgG_1544BvHhTgdnKTkNs@kebnekaise.lmq.cloudamqp.com/klihfdct");

        using var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

        Dictionary<string,object> headers = new Dictionary<string, object>();
        headers.Add("format", "pdf");
        headers.Add("shape2", "a4");

        var properties = channel.CreateBasicProperties();
        properties.Headers = headers;
        properties.Persistent = true;// Mesajları kalıcı hale getirir

        var product = new Product { Id=1 ,Name="Kalem",Price=100,Stock=10};
        var productJsonString = JsonSerializer.Serialize(product);

        channel.BasicPublish("header-exchange", string.Empty, properties,
            Encoding.UTF8.GetBytes(productJsonString));

        Console.WriteLine("Mesaj gönderilmişit");
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
//channel.QueueDeclare("Hello-Queue",true,false,false);
//// rabbit mq mesajlar byte dizisi olarak gider bu yüzden her şey gönderilebilir.

//Enumerable.Range(1, 50).ToList().ForEach(x =>
//{//Burası 50 tane mesaj göndermeye yarıyor
//    string message = $"Message {x}";
//    var messageBody = Encoding.UTF8.GetBytes(message);
//    channel.BasicPublish(string.Empty, "Hello-Queue", null, messageBody);

//    Console.WriteLine($"Mesaj gönderildi : {message}");
//});
////string message = "hello worl";
////var messageBody = Encoding.UTF8.GetBytes(message);
////channel.BasicPublish(string.Empty, "Hello-Queue", null, messageBody);

////Console.WriteLine("Mesaj gönderildi");
//Console.ReadLine();


//****************** 3. Ders Exchange Types ***************//
//1. Fanout 
//var factory = new ConnectionFactory();
//factory.Uri = new Uri("amqps://klihfdct:Tp5GHLQjqsmXgG_1544BvHhTgdnKTkNs@kebnekaise.lmq.cloudamqp.com/klihfdct");

//using var connection = factory.CreateConnection();
//var channel = connection.CreateModel();

//channel.ExchangeDeclare("logs-fanout",durable:true,type:ExchangeType.Fanout);

//Enumerable.Range(1, 50).ToList().ForEach(x =>
//{
//    string message = $"log {x}";
//    var messageBody = Encoding.UTF8.GetBytes(message);
//    channel.BasicPublish("logs-fanout","", null, messageBody);

//    Console.WriteLine($"Mesaj gönderildi : {message}");
//});
//Console.ReadLine();

/*************************************************************/
//2.Direct
//var factory = new ConnectionFactory();
//factory.Uri = new Uri("amqps://klihfdct:Tp5GHLQjqsmXgG_1544BvHhTgdnKTkNs@kebnekaise.lmq.cloudamqp.com/klihfdct");

//using var connection = factory.CreateConnection();
//var channel = connection.CreateModel();

//channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);

//Enum.GetNames(typeof(LogNames)).ToList().ForEach(x => {
//    var queueName = $"direct-queuq-{x}";
//    var rootKey = $"route-{x}";
//    channel.QueueDeclare(queueName, true, false, false);
//    channel.QueueBind(queueName, "logs-direct", rootKey, null);
//});

//Enumerable.Range(1, 50).ToList().ForEach(x =>
//{
//    LogNames log = (LogNames)new Random().Next(1, 5);
//    string message = $"log-type :{log}";

//    var rootKey = $"route-{log}";

//    var messageBody = Encoding.UTF8.GetBytes(message);
//    channel.BasicPublish("logs-direct", rootKey, null, messageBody);

//    Console.WriteLine($"log gönderildi : {message}");
//});
//Console.ReadLine();

/**********************************************************/
//3.Topic
//var factory = new ConnectionFactory();
//factory.Uri = new Uri("amqps://klihfdct:Tp5GHLQjqsmXgG_1544BvHhTgdnKTkNs@kebnekaise.lmq.cloudamqp.com/klihfdct");

//using var connection = factory.CreateConnection();
//var channel = connection.CreateModel();

//channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);

//Random rnd = new Random();

//Enumerable.Range(1, 50).ToList().ForEach(x =>
//{
//    LogNames log1 = (LogNames)rnd.Next(1, 5);
//    LogNames log2 = (LogNames)rnd.Next(1, 5);
//    LogNames log3 = (LogNames)rnd.Next(1, 5);

//    var rootKey = $"{log1}.{log2}.{log3}";
//    string message = $"log-type :{log1}.{log2}.{log3}";
//    var messageBody = Encoding.UTF8.GetBytes(message);
//    channel.BasicPublish("logs-topic", rootKey, null, messageBody);

//    Console.WriteLine($"log gönderildi : {message}");
//});
//Console.ReadLine();

/***************************************************************/
//4.Header
//var factory = new ConnectionFactory();
//factory.Uri = new Uri("amqps://klihfdct:Tp5GHLQjqsmXgG_1544BvHhTgdnKTkNs@kebnekaise.lmq.cloudamqp.com/klihfdct");

//using var connection = factory.CreateConnection();
//var channel = connection.CreateModel();

//channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

//Dictionary<string, object> headers = new Dictionary<string, object>();
//headers.Add("format", "pdf");
//headers.Add("shape2", "a4");

//var properties = channel.CreateBasicProperties();
//properties.Headers = headers;
//properties.Persistent = true;// Mesajları kalıcı hale getirir
//channel.BasicPublish("header-exchange", string.Empty, properties,
//    Encoding.UTF8.GetBytes("header mesajım"));

//Console.WriteLine("Mesaj gönderilmişit");
//Console.ReadLine();