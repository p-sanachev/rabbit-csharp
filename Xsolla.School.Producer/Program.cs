using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Xsolla.School.Common;

namespace Xsolla.School.Producer
{
    class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "messages", type: ExchangeType.Fanout);

                    var message = GetMessage("https://google.com");
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                    channel.BasicPublish(exchange: "messages", routingKey: "", basicProperties: null, body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                }

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static MessageModel GetMessage(string uri)
        {
            var message = new MessageModel {Uri = uri};
            return message;
        }
    }
}