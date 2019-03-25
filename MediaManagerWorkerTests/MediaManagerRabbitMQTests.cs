using MediaManagerWorker;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManagerWorkerTests
{
    public class MediaManagerRabbitMQTests
    {
        [Test]
        public void ReceiveMessagesOnQueue()
        {
            var rabbitServer = new RabbitServer();

            var queueName = "my_queue";
            var queueExchage = "my_exchange";
            var message = "hello_world";

            ConfigureQueueBinding(rabbitServer, queueExchage, queueName);
            SendMessage(rabbitServer, queueExchage, message);

            //var connectionFactory = new FakeConnectionFactory(rabbitServer);
            //using (var connection = connectionFactory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    // First message
            //    var message = channel.BasicGet("my_queue", autoAck: false);

            //    Assert.That(message, Is.Not.Null);
            //    var messageBody = Encoding.ASCII.GetString(message.Body);

            //    Assert.That(messageBody, Is.EqualTo("hello_world"));

            //    channel.BasicAck(message.DeliveryTag, multiple: false);
            //}


            var connectionFactory = new FakeConnectionFactory(rabbitServer);
            var mm = new MediaManagerRabbitMQ(connectionFactory);
            mm.Configure(queueExchage, queueName);
        }





        private static void SendMessage(RabbitServer rabbitServer, string exchange, string message, IBasicProperties basicProperties = null)
        {
            var connectionFactory = new FakeConnectionFactory(rabbitServer);

            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var messageBody = Encoding.ASCII.GetBytes(message);
                channel.BasicPublish(exchange: exchange, routingKey: null, mandatory: false, basicProperties: basicProperties, body: messageBody);
            }
        }

        private void ConfigureQueueBinding(RabbitServer rabbitServer, string exchangeName, string queueName)
        {
            var connectionFactory = new FakeConnectionFactory(rabbitServer);
            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

                channel.QueueBind(queueName, exchangeName, null);
            }
        }
    }
}
