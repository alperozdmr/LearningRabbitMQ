using RabbitMQ.Client;
using System.Text;

class publisher
{
    static void Main(string[] args)
    {
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
    }
}