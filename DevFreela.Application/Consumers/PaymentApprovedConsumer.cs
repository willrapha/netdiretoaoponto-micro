using DevFreela.Core.IntegrationEvents;
using DevFreela.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DevFreela.Application.Consumers
{
    public class PaymentApprovedConsumer : BackgroundService
    {
        private const string PAYMENT_APPROVED_QUEUE = "PaymentsApproved";
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public PaymentApprovedConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var factory = new ConnectionFactory() 
            { 
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection(); // Cria conexão com o RabbitMQ
            _channel = _connection.CreateModel(); // Cria canal de comunicação com o RabbitMQ
            _channel.QueueDeclare(queue: PAYMENT_APPROVED_QUEUE, // Declara a fila
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, eventArgs) =>
            {
                var paymentApprovedBytes = eventArgs.Body.ToArray();
                var paymentApprovedJson = Encoding.UTF8.GetString(paymentApprovedBytes);
                
                var paymentApprovedEvent = JsonSerializer.Deserialize<PaymentApprovedIntegrationEvent>(paymentApprovedJson);
                await FinishProject(paymentApprovedEvent.IdProject);

                _channel.BasicAck(eventArgs.DeliveryTag, multiple: false); // Confirma a mensagem recebida
            };

            _channel.BasicConsume(queue: PAYMENT_APPROVED_QUEUE, // Consome a fila
                                    autoAck: false,
                                    consumer: consumer);

            return Task.CompletedTask;
        }

        public async Task FinishProject(int id)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var projectRepository = scope.ServiceProvider.GetService<IProjectRepository>();
                var project = await projectRepository.GetByIdAsync(id);
                project.Finish();
                await projectRepository.SaveChangesAsync();
            }
        }
    }
}
