using Autofac;
using MediaManager;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace MediaManagerWorker
{
    public class MediaManagerRabbitMQ
    {
        private IModel channel;

        private static AutoResetEvent waitHandle = new AutoResetEvent(false);

        public MediaManagerRabbitMQ(IConnectionFactory factory)
        {
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                this.channel = channel;
            }
        }

        public MediaManagerRabbitMQ(IModel model)
        {
            this.channel = model;
        }
        public void OnDelivery(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);

            var container = ApplicationFactory.Configure();

            using (var scope = container.BeginLifetimeScope())
            {
                var app = scope.Resolve<IMediaManager>();
            }

            Console.WriteLine(" [x] Done");

            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }

        public void ShutdownGracefully(object obj, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Saindo...");

            // Libera a continuação da thread principal
            waitHandle.Set();
        }

        public void Configure(string queueExchange, string queueName)
        {
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            channel.ExchangeDeclare(exchange: queueExchange, type: ExchangeType.Direct);

            channel.QueueBind(queueName, queueExchange, "*");

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += this.OnDelivery;
            channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);
        }

        public void Listen()
        {
            Console.CancelKeyPress += ShutdownGracefully;

            // Aguarda que o evento CancelKeyPress ocorra
            waitHandle.WaitOne();
        }
    }
}
