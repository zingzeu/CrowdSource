using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using System;
using System.Threading.Tasks;
using Zezo.Core.Configuration;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Client
{
    public class Program
    {

        private static readonly string SimpleProject = @"
            <Project Id=""test"">
                <Project.Pipeline>
                    <Sequence Id=""seq"">
                        <Sequence.Children>
                            <DummyStep Id=""dummy1"" />
                            <DummyStep Id=""dummy2"" />
                        </Sequence.Children>
                    </Sequence>
                </Project.Pipeline>
            </Project>
        ";
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await ConnectClient())
                {
                    await DoClientWork(client);
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.Message} \n\n {e.StackTrace}");
                Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<IClusterClient> ConnectClient()
        {
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "Zezo";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

        private static async Task DoClientWork(IClusterClient client)
        {
            // example of calling grains from the initialized client
            var friend = client.GetGrain<IHello>(0);
            var response = await friend.SayHello("Good morning, HelloGrain!");
            Console.WriteLine("\n\n{0}\n\n", response);


            var parser = new Parser();
            var config = parser.ParseXmlString(SimpleProject) as ProjectNode;
            var project = client.GetGrain<IProjectGrain>(Guid.NewGuid());
            Console.WriteLine("Loading config...");

            await project.LoadConfig(config);
            Console.WriteLine("Creating entity");
            var e1K = await project.CreateEntity(1);
            Console.WriteLine($"Entity {e1K} created.");
            var e1 = client.GetGrain<IEntityGrain>(e1K);
            Console.WriteLine("Starting entity");
            await e1.Start();
            Console.WriteLine("Started entity");
        }
    }
}