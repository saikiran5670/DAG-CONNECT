using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace SignalRClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            try
            {
                var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/NotificationHub")
                .Build();

                //var connection = new HubConnectionBuilder()
                // .WithUrl($"https://api.dev1.ct2.atos.net/NotificationHub", (opts) =>
                // {
                //     opts.HttpMessageHandlerFactory = (message) =>
                //     {
                //         if (message is HttpClientHandler clientHandler)
                //              bypass SSL certificate
                //             clientHandler.ServerCertificateCustomValidationCallback +=
                //                 (sender, certificate, chain, sslPolicyErrors) => { return true; };
                //         return message;
                //     };
                // }).Build();	

                connection.StartAsync().Wait();
                //var mes = Console.ReadLine();
                _ = connection.InvokeCoreAsync("ReadKafkaMessages", args: new[] { " Test name" });

            connection.On<string>("NotifyAlertResponse",
                (string message) =>
                {
                    Console.WriteLine();
                    Console.WriteLine($"Message is : {message}");
                    Console.WriteLine();
                });
                connection.On<string>("askServerResponse",
                (string message) =>
                {
                    Console.WriteLine();
                    Console.WriteLine($"Error is : {message}");
                    Console.WriteLine();
                });
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
            }
            
            Console.ReadKey();
            Console.WriteLine("Program end");
        }

    }
}
