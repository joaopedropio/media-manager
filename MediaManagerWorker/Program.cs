using ContentManager;

namespace MediaManagerWorker
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = new Configuration();
            var factory = config.ConnectionFactory;
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var mm = new MediaManagerRabbitMQ(channel);
                mm.Configure(config.QueueExchange, config.QueueName);
                mm.Listen();
            }
        }
    }
}
