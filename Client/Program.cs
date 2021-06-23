using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace ProductionPlanListener
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HubConnection connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:8888/broadcast")
                .WithAutomaticReconnect()
                .Build();

            await connection.StartAsync();

            Console.WriteLine("Listening. Press enter to stop...");

            connection.On<string, string>("ReceiveMessage", (input, output) => {
                Console.WriteLine(input);
                Console.WriteLine();
                Console.WriteLine(output);
            });

            string message = Console.ReadLine();

        }

    }
}
