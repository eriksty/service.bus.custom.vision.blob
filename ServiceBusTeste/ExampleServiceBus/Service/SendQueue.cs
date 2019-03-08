﻿using ExampleServiceBus.Model;
using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExampleServiceBus.Service
{
    public class SendQueue
    {
        const string ServiceBusConnectionString = "Endpoint=sb://azure-semple-queue.servicebus.windows.net/;SharedAccessKeyName=bug-queue-acess;SharedAccessKey=nYyPNKtpRcv9KPmC30qSyhl5bZO5vfkxEKCVForsrbs=;";
        const string QueueName = "bug-queue";
        static IQueueClient queueClient;

        public SendQueue()
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
        }

        public static async Task SendMessagesAsync(Teste employee)
        {
            try
            {
                queueClient = new QueueClient(ServiceBusConnectionString, QueueName);


                var item = JsonConvert.SerializeObject(employee);
                var message = new Message(Encoding.UTF8.GetBytes(item));

                await queueClient.SendAsync(message);

            }
            catch (Exception exception)
            {
                await queueClient.CloseAsync();
                throw new Exception(exception.Message);

            }
        }

        public static async Task Finish()
        {
            await queueClient.CloseAsync();
        }
    }
}
