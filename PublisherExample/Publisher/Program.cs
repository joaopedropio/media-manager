using RabbitMQ.Client;
using System;
using System.Text;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueName = "my_queue";
            var exchangeName = "my_exchange";

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

                channel.QueueBind(queueName, exchangeName, "*");

                string message = "Agora vai essa bosta!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: exchangeName, routingKey: "*", mandatory: false, body: body);

                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}