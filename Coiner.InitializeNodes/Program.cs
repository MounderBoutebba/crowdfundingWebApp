using LucidOcean.MultiChain;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Coiner.InitializeNodes
{
    class Program
    {
        static private IConfiguration config;

        static private MultiChainConnection connection;

        static private MultiChainClient multiChainClient;

        static private List<MultiChainClient> otherNodes = new List<MultiChainClient>();

        static private string adminAddress;

        static void Main(string[] args)
        {
            try
            {
                config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", true, true)
                           .Build();

                var MultiChainConnectionConfig = config.GetSection("MultiChainConnection");

                connection = new MultiChainConnection()
                {
                    Hostname = MultiChainConnectionConfig["Hostname"],
                    Port = Convert.ToInt32(MultiChainConnectionConfig["Port"]),
                    Username = MultiChainConnectionConfig["Username"],
                    Password = MultiChainConnectionConfig["Password"],
                    ChainName = MultiChainConnectionConfig["ChainName"],
                };

                multiChainClient = new MultiChainClient(connection);

                adminAddress = MultiChainConnectionConfig["AdminAddress"];

                var otherNodesConfigurationCount = config.GetSection("OtherNodes").GetChildren().ToList().Count;

                for (int i = 0; i < otherNodesConfigurationCount; i++)
                {
                    var nodeConfiguration = config.GetSection($"OtherNodes:{i}");
                    var connection = new MultiChainConnection()
                    {
                        Hostname = nodeConfiguration["Hostname"],
                        Port = Convert.ToInt32(nodeConfiguration["Port"]),
                        Username = nodeConfiguration["Username"],
                        Password = nodeConfiguration["Password"],
                        ChainName = MultiChainConnectionConfig["ChainName"],
                    };

                    var node = new MultiChainClient(connection);

                    otherNodes.Add(node);
                }

                ImportAddresses().GetAwaiter().GetResult();
                SubscribeToStreams().GetAwaiter().GetResult();

                Console.WriteLine("initialize nodes service finished successfully, Press any key to exit");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"initialize nodes service stopped because : {ex.Message}");
                Console.ReadLine();
            }

        }

        static async private Task ImportAddresses()
        {
            try
            {
                Console.WriteLine("Start import addresses in other Nodes...");
                var addresses = await multiChainClient.Wallet.GetAddressesAsync();
                var mainNodeAddresses = addresses.Result.ToList();

                foreach (var node in otherNodes)
                {
                    var nodeInfo = await node.Utility.GetInfoAsync();
                    Console.WriteLine($"Start import addresses in node {nodeInfo.Result.NodeAddress}");
                    var otherNodeAddresses = await node.Wallet.GetAddressesAsync();
                    var otherNodeAllAddresses = otherNodeAddresses.Result.ToList();

                    foreach (var address in mainNodeAddresses)
                    {
                        if (!otherNodeAllAddresses.Contains(address) && address != adminAddress)
                        {
                            await node.Wallet.ImportAddressAsync(address, null, false);
                            Console.WriteLine($"Import address {address} in node {nodeInfo.Result.NodeAddress}");
                        }
                    }
                }

                Console.WriteLine($"Import addresses finished successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"initialize nodes service stopped because : {ex.Message}");
                Console.ReadLine();
            }

        }

        static async private Task SubscribeToStreams()
        {
            try
            {
                foreach (var node in otherNodes)
                {
                    Console.WriteLine("Start subscribe to streams from other Nodes...");
                    var nodeInfo = await node.Utility.GetInfoAsync();
                    Console.WriteLine($"Start subscribe to streams from node {nodeInfo.Result.NodeAddress}");
                    var nodeStreams = await node.Stream.ListStreamsAsync();
                    var streams= nodeStreams.Result.ToList();

                    foreach (var stream in streams)
                    {
                        if (!stream.Subscribed)
                        {
                            await node.Asset.SubscribeAsync(stream.Name);
                            Console.WriteLine($"Subscribe to stream {stream.Name} from node {nodeInfo.Result.NodeAddress}");
                        }
                    }
                }

                Console.WriteLine($"Subscribe to streams finished successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"initialize nodes service stopped because : {ex.Message}");
                Console.ReadLine();
            }

        }

    }
}
