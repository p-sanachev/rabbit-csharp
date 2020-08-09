using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xsolla.School.Common;

namespace Xsolla.School.Consumer
{
    class Program
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "messages", type: ExchangeType.Fanout);
                    channel.QueueDeclare(queue: "messages",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    channel.QueueBind(queue: "messages", exchange: "messages", routingKey: "");

                    Console.WriteLine(" [*] Waiting for message.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        byte[] body = ea.Body.ToArray();
                        var messageString = Encoding.UTF8.GetString(body);
                        var message = JsonConvert.DeserializeObject<MessageModel>(messageString);
                        HttpClient client = new HttpClient();
                        var responseMessage = client.GetAsync(message.Uri).GetAwaiter().GetResult();
                        Console.WriteLine(" [Response] {0}", responseMessage);
                    };
                    channel.BasicConsume(queue: "messages", autoAck: true, consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}