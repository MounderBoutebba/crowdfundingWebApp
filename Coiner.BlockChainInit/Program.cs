using LucidOcean.MultiChain;
using System;
using System.Text;

namespace Coiner.BlockChainInit
{
    class Program
    {
        static void Main(string[] args)
        {
            MultiChainConnection connection = new MultiChainConnection()
            {
                Hostname = "192.168.50.1",
                Port = 4798,
                Username = "multichainrpc",
                Password = "AUFwxEjB2Uia1bTS5oG3Q8vW8cDAC2P1Qg5Q9QQiLn3w",
                ChainName = "chain1",
            };
            MultiChainClient _Client = new MultiChainClient(connection);

            //CreateNewAsset(_Client, "restaurant_stocks", 500);
            //CreateNewAsset(_Client, "Euro", 5000);
            //CreateUsers(_Client, 5);
        }

        private static void CreateNewAsset(MultiChainClient client, string assetName, int quantity)
        {
            //var newAddress = client.Address.CreateKeyPairsAsync(1).Result.Result[0].Address;
            //client.Wallet.ImportAddress(newAddress, null, false);
            var newAddress = client.Wallet.GetNewAddress().Result;
            client.Permission.Grant(newAddress, LucidOcean.MultiChain.API.Enums.BlockChainPermission.Send);
            client.Permission.Grant(newAddress, LucidOcean.MultiChain.API.Enums.BlockChainPermission.Receive);
            client.Asset.IssueAsync(newAddress, assetName, quantity, 1m);
        }

        private static void CreateUsers(MultiChainClient client, int usersNumber)
        {
            for (int i = 1; i < usersNumber + 1; i++)
            {
                // create new address with pub/priv keys
                //var clientAddress = client.Address.CreateKeyPairsAsync(1).Result.Result[0].Address;
                //client.Wallet.ImportAddress(clientAddress, null, false);
                var userAddress = client.Wallet.GetNewAddress().Result;
                //grant address some permissions
                client.Permission.Grant(userAddress, LucidOcean.MultiChain.API.Enums.BlockChainPermission.Send);
                client.Permission.Grant(userAddress, LucidOcean.MultiChain.API.Enums.BlockChainPermission.Receive);

                byte[] label = Encoding.ASCII.GetBytes($"user{i}");
                client.Stream.PublishFrom(userAddress, "root", "", label);
            }
        }

        private static void DistributeAssetToUsers(MultiChainClient client, string fromAddress, string adminAddress, string assetName, decimal quantity)
        {
            var addresses = client.Wallet.ListAddresses(true).Result;
            foreach (var address in addresses)
            {
                if (address.Address != fromAddress && address.Address != adminAddress)
                {
                    //var dictionnary = new Dictionary<string, object>();
                    //dictionnary.Add(address.Address, new { restaurant_stocks = 100 });
                    //var TxHex = client.Transaction.CreateRawSendFromAsync(productMarkertAddress, dictionnary).Result.TxHex.Result;
                    var response = client.Asset.SendAssetFromAsync(fromAddress, address.Address, assetName, quantity).Result;
                }
            }
        }

    }
}
