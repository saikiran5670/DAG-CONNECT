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
                .WithUrl("https://localhost:44300/NotificationHub")
                .Build();
                connection.StartAsync().Wait();
                //var mes = Console.ReadLine();
                _ = connection.InvokeCoreAsync("NotifyAlert", args: new[] { " Test name" });

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
