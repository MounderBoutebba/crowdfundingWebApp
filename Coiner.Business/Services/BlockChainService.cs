using Coiner.Business.Context;
using Coiner.Business.Helpers;
using Coiner.Business.Heplers;
using Coiner.Business.Models;
using Coiner.Business.Models.Enums;
using LucidOcean.MultiChain;
using LucidOcean.MultiChain.API.Enums;
using LucidOcean.MultiChain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Coiner.Business.Services
{
    public class BlockChainService
    {
        private IConfiguration config;

        private CoinerContext _context = new CoinerContext();

        private EmailService _emailService = new EmailService();

        private NotificationsService _notificationsService = new NotificationsService();

        private MultiChainConnection connection;

        private MultiChainClient multiChainClient;

        private List<MultiChainClient> otherNodes = new List<MultiChainClient>();

        private string adminAddress;
        private string burnAddress;
        private string comissionAddress;

        private bool _enableBlockhainLock;

        public BlockChainService()
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
            burnAddress = MultiChainConnectionConfig["BurnAddress"];
            comissionAddress = MultiChainConnectionConfig["ComissionAddress"];
            _enableBlockhainLock = Convert.ToBoolean(MultiChainConnectionConfig["EnableBlockhainLock"]);
            multiChainClient.Permission.GrantFromAsync(adminAddress, new List<string>() { burnAddress}, BlockChainPermission.Receive);
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

        }

        public async Task CreateUsers(Project project)
        {
            var addresses = new List<string>();

            foreach (var coin in project.Coins)
            {
                var currentUser = _context.Users.Where(u => u.Id == coin.UserId).FirstOrDefault();
                await CreateUserInBlockChain(currentUser, addresses);
            }

            //grant addresses send & recieve permissions
            if (addresses.Count != 0)
            {
                multiChainClient.Permission.GrantFrom(adminAddress, addresses, BlockChainPermission.Send);
                multiChainClient.Permission.GrantFrom(adminAddress, addresses, BlockChainPermission.Receive);
                _context.SaveChanges();
            }
        }

        public async void CreateNewAssetForProject(Project project)
        {
            var assetName = project.ProductName; //$"asset_{project.Id}";

            var assetQuantity = project.Coins.Sum(c => c.CoinsNumber);
            //var metadata = new
            //{
            //    projectName = project.ProjectName,
            //    projectType = project.ProjectType,
            //};
            Decimal units = 1;
            object data = new { coinVal = Constants.coinValue.ToString(), sellPercentage = Math.Round(project.PercentageAsset, 2).ToString() };
            //create asset
            await multiChainClient.Asset.IssueFromAsync(adminAddress, adminAddress, assetName, assetQuantity, units, data);

            // create init streams
            await CreateStreamsForProject(assetName.Replace(" ", "_"));

            // distribute asset to users
            await DistributeAssetToUsers(project, assetName);

            await CreateCurrencyAsset();
        }

        public async Task<object> CreateNewOffer(int userId, string privateKey, string productName, decimal productQuantity, string currency, decimal currencyQuantity, OfferTypeEnum offerType, decimal commissionFees)
        {
            string offerAsset;
            decimal offerQuantity;
            string askAsset;
            decimal askQuantity;
            string streamName;

            if (offerType == OfferTypeEnum.Buy)
            {
                offerAsset = Constants.CurrencyAssetName;
                offerQuantity = currencyQuantity;
                askAsset = productName;
                askQuantity = productQuantity;
                streamName = $"STREAM_ACHAT_{productName.Replace(" ", "_")}";
            }
            else
            {
                offerAsset = productName;
                offerQuantity = productQuantity;
                askAsset = Constants.CurrencyAssetName;
                askQuantity = currencyQuantity;
                streamName = $"STREAM_VENTE_{productName.Replace(" ", "_")}";
            }
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();

            IDictionary<string, object> offerItems = new Dictionary<string, object>();
            offerItems.Add(offerAsset, offerQuantity);

            Dictionary<string, object> outputs1 = new Dictionary<string, object>()
            {
                { user.BlockChainAddress, offerItems},
            };

            var InfosPrivateKey = await multiChainClient.Transaction.validateaddress(privateKey);

            //Begin check private key is valid
            if (!InfosPrivateKey.Result.isvalid)
            {
                var errorResponse = JsonConvert.DeserializeObject<JsonRpcErrorResponse>("{\"result\":null,\"error\":{\"code\":-5,\"message\":\"Invalid private key\"},\"id\":0}\n");
                throw new JsonRpcException(errorResponse.Error);
            }
            //End check private key is valid

            var offerValid = true;
            var lockMode = RawTransactionAction.Lock;

            if (!_enableBlockhainLock)
            {
                lockMode = RawTransactionAction.Default;

                var userOffersStreamIsEmpty = await CheckUserOffersStream(user);

                if (!userOffersStreamIsEmpty)
                {
                    offerValid = await CheckUserBalances(offerAsset, offerQuantity, user);
                }
            }

            if (offerValid)
            {
                var BLOB1 = await multiChainClient.Transaction.CreateRawSendFromAsync(user.BlockChainAddress, outputs1, null, lockMode);
                string[] privatekeys = new string[] { privateKey };
                var LargeBLOB1 = await multiChainClient.Transaction.SignRawTransactionAsync(BLOB1.TxHex.Result, privatekeys);
                var TXID1 = await multiChainClient.Transaction.SendRawTransactionAsync(LargeBLOB1.Result.Hex);

                Dictionary<string, object> inputs = new Dictionary<string, object>()
            {
                { "txid", TXID1.Result },
                { "vout", 0 }
            };

                IDictionary<string, object> askItems = new Dictionary<string, object>();
                askItems.Add(askAsset, askQuantity);

                Dictionary<string, object> outputs2 = new Dictionary<string, object>()
            {
               { user.BlockChainAddress, askItems},
            };

                var BLOB3 = await multiChainClient.Transaction.CreateRawTransactionAsync(new object[] { inputs }, outputs2, null, lockMode);

                var HEXBLOB = await multiChainClient.Transaction.SignRawTransactionAsync(BLOB3.TxHex.Result, privatekeys, "SINGLE|ANYONECANPAY");

                var transactionDetails = await multiChainClient.Transaction.DecodeRawTransactionAsync(HEXBLOB.Result.Hex);

                // publish to stream 

                decimal pxCoin = (offerType == OfferTypeEnum.Buy) ?
                        Math.Round((decimal)(offerQuantity / askQuantity), 2) :
                        Math.Round((decimal)(askQuantity / offerQuantity), 2);

                var productOffer = new ProductOffer()
                {
                    OfferAssetName = offerAsset,
                    OfferQuantity = offerQuantity,
                    AskAssetName = askAsset,
                    AskQuantity = askQuantity,
                    PxCoin = pxCoin,
                    FromAddress = user.BlockChainAddress,
                    TxId = TXID1.Result,
                    HexBlob = HEXBLOB.Result.Hex,
                    Operation = offerType
                };

                byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(productOffer));
                string reducedUserBlockChainAddress = user.BlockChainAddress.Substring(0, 20);
                await multiChainClient.Stream.PublishFromAsync(adminAddress, streamName, "hexablob", data);
                await multiChainClient.Stream.PublishFromAsync(adminAddress, $"USO_{reducedUserBlockChainAddress}", "hexablob", data);

                // send transaction fees to commission stirblock (admin in our case we can change it after)
                if (offerType == OfferTypeEnum.Buy)
                {
                    await SendCommissionFeesToAdmin(user.BlockChainAddress, privateKey, commissionFees, lockMode);
                }
                // send transaction fees to commission stirblock (admin in our case we can change it after)

                dynamic productOffers = await GetProductOffersFromStreams(productName);
                string MaxSellValue = GetMaxSellValue(productOffers.sellOffers);
                string MinBuyValue = GetMinBuyValue(productOffers.buyOffers);
                return new { productOffers, MaxSellValue, MinBuyValue };
            }
            else
            {
                throw new JsonRpcException("99");
            }

        }

        public async Task<object> AcceptOffer(int userId, string productName, string privateKey, string askAssetName, decimal askAssetQauntity, string offerAssetName, decimal offerAssetQuantity, string HexBlob, decimal pxCoin, string txId, string fromAddress, string zoom, decimal commissionFees)
        {
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();

            IDictionary<string, object> askItems = new Dictionary<string, object>();
            askItems.Add(askAssetName, askAssetQauntity);

            Dictionary<string, object> outputs1 = new Dictionary<string, object>()
            {
                { user.BlockChainAddress, askItems},
            };

            var InfosPrivateKey = await multiChainClient.Transaction.validateaddress(privateKey);

            if (!InfosPrivateKey.Result.isvalid)
            {
                var errorResponse = JsonConvert.DeserializeObject<JsonRpcErrorResponse>("{\"result\":null,\"error\":{\"code\":-5,\"message\":\"Invalid private key\"},\"id\":0}\n");
                throw new JsonRpcException(errorResponse.Error);
            }

            var BLOB5 = await multiChainClient.Transaction.CreateRawSendFromAsync(user.BlockChainAddress, outputs1, null, RawTransactionAction.Lock);
            string[] privatekeys = new string[] { privateKey };
            var LargeBLOB2 = await multiChainClient.Transaction.SignRawTransactionAsync(BLOB5.TxHex.Result, privatekeys);
            var TXID2 = await multiChainClient.Transaction.SendRawTransactionAsync(LargeBLOB2.Result.Hex);

            Dictionary<string, object> inputs = new Dictionary<string, object>()
            {
                { "txid", TXID2.Result },
                { "vout", 0 }
            };

            IDictionary<string, object> offerItems = new Dictionary<string, object>();
            offerItems.Add(offerAssetName, offerAssetQuantity);

            Dictionary<string, object> outputs2 = new Dictionary<string, object>()
            {
               { user.BlockChainAddress, offerItems},
            };

            var BLOB6 = await multiChainClient.Transaction.AppendRawTransactionAsync(HexBlob, new object[] { inputs }, outputs2, null, RawTransactionAction.Lock);

            var HEXBLOB2 = await multiChainClient.Transaction.SignRawTransactionAsync(BLOB6.TxHex.Result, privatekeys);

            var TXID3 = await multiChainClient.Transaction.SendRawTransactionAsync(HEXBLOB2.Result.Hex);

            var transactionDetails = await multiChainClient.Transaction.DecodeRawTransactionAsync(HEXBLOB2.Result.Hex);


            //publish to stream

            //decimal pxCoin = (offerType == OfferTypeEnum.Buy) ?
            //        Math.Round((decimal)(offerQuantity / askQuantity), 2) :
            //        Math.Round((decimal)(askQuantity / offerQuantity), 2);

            var productOfferTrade = new ProductOffer()
            {
                OfferAssetName = offerAssetName,
                OfferQuantity = offerAssetQuantity,
                AskAssetName = askAssetName,
                AskQuantity = askAssetQauntity,
                PxCoin = pxCoin,
                FromAddress = user.BlockChainAddress,
                TxId = TXID3.Result,
                HexBlob = HEXBLOB2.Result.Hex
            };

            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(productOfferTrade));

            await multiChainClient.Stream.PublishFromAsync(adminAddress, $"STREAM_TRADE_{productName.Replace(" ", "_")}", "hexablob", data);

            var useroffer = _context.Users.Where(u => u.BlockChainAddress == fromAddress).FirstOrDefaultAsync();

            var productOffer = new ProductOffer()
            {
                OfferAssetName = offerAssetName,
                OfferQuantity = offerAssetQuantity,
                AskAssetName = askAssetName,
                AskQuantity = askAssetQauntity,
                PxCoin = pxCoin,
                FromAddress = user.BlockChainAddress,
                TxId = txId,
                Operation = (askAssetName == productName) ? OfferTypeEnum.Buy : OfferTypeEnum.Sell
                //HexBlob = HEXBLOB2.Result.Hex
            };

            byte[] dataOffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(productOffer));

            //var streamName = (askAssetName == productName) ? $"STREAM_ACHAT_{productName}" : $"STREAM_VENTE_{productName}";
            await multiChainClient.Stream.PublishFromAsync(adminAddress, $"STREAM_OPERATIONS_{productName.Replace(" ", "_")}", "hexablob", dataOffer);
            string reducedAskerUserBlockChainAddress = useroffer.Result.BlockChainAddress.Substring(0, 20);

            ProductOffer askerProduct = productOffer;
            askerProduct.FromAddress = useroffer.Result.BlockChainAddress;
            byte[] askerProductByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(askerProduct));
            await multiChainClient.Stream.PublishFromAsync(adminAddress, $"UST_{reducedAskerUserBlockChainAddress}", "hexablob", askerProductByte);

            //await multiChainClient.Stream.PublishFromAsync(adminAddress, $"USER_STREAM_TRADES_{userId}", "hexablob", dataOffer);

            var validatorProduct = new ProductOffer()
            {
                OfferAssetName = askAssetName,
                OfferQuantity = askAssetQauntity,
                AskAssetName = offerAssetName,
                AskQuantity = offerAssetQuantity,
                PxCoin = pxCoin,
                FromAddress = user.BlockChainAddress,
                TxId = txId,
                Operation = (askAssetName == productName) ? OfferTypeEnum.Sell : OfferTypeEnum.Buy
                //HexBlob = HEXBLOB2.Result.Hex
            };
            byte[] validatorProductByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(validatorProduct));

            string reducedUserBlockChainAddress = user.BlockChainAddress.Substring(0, 20);

            await multiChainClient.Stream.PublishFromAsync(adminAddress, $"UST_{reducedUserBlockChainAddress}", "hexablob", validatorProductByte);

            if(askAssetName == productName)
            {
                SendBuyCompleteEmail(useroffer.Result.Id, productName, askAssetQauntity.ToString(), offerAssetQuantity.ToString());
            }
            else
            {
                SendSaleCompleteEmail(useroffer.Result.Id, productName, askAssetQauntity.ToString(), offerAssetQuantity.ToString());
            }
            dynamic productOffers = await GetProductOffersFromStreams(productName);
            string MaxSellValue = GetMaxSellValue(productOffers.sellOffers);
            string MinBuyValue = GetMinBuyValue(productOffers.buyOffers);
            // send transaction fees to commission stirblock (admin in our case we can change it after)
            await SendCommissionFeesToAdmin(user.BlockChainAddress, privateKey, commissionFees, RawTransactionAction.Lock);
            // send transaction fees to commission stirblock (admin in our case we can change it after)
            var stats = await GetStatsForProduct(productName, zoom);

            var tradeTransactions = new List<ProductOffer>();
            var tradeStreamForProduct = await multiChainClient.Stream.ListStreamItemsAsync($"STREAM_TRADE_{productName.Replace(" ", "_")}", false, 999999999, 0);

            foreach (var offer in tradeStreamForProduct.Result)
            {
                var tempData = HextoString(offer.Data.ToString());
                ProductOffer prodOffer = JsonConvert.DeserializeObject<ProductOffer>(tempData);
                tradeTransactions.Add(prodOffer);
            }
            List<decimal> Transactions = new List<decimal>();
            foreach (var tradeOffer in tradeTransactions)
            {
                Transactions.Add(tradeOffer.PxCoin);
            }
            var assetInformations = await multiChainClient.Asset.ListAssetsAsync(productName);

            var TotalCoinNumber = assetInformations.Result.FirstOrDefault().IssueQty;
            AssetDetails assetDetails = JsonConvert.DeserializeObject<AssetDetails>(assetInformations.Result[0].Details.ToString());

            decimal totalCapitalization;
            decimal lastTransaction;
            try
            {
                lastTransaction = Transactions[Transactions.Count - 1];
                totalCapitalization = Math.Round((100 / decimal.Parse(assetDetails.SellPercentage)) * TotalCoinNumber * lastTransaction);
            }
            catch
            {
                totalCapitalization = Math.Round((100 / decimal.Parse(assetDetails.SellPercentage)) * TotalCoinNumber * decimal.Parse(assetDetails.CoinVal));
                lastTransaction = decimal.Parse(assetDetails.CoinVal);
            }
            decimal secondLastTransaction;
            try
            {
                secondLastTransaction = Transactions[Transactions.Count - 2];
            }
            catch { secondLastTransaction = 0; }
            decimal transactionVariation;
            try
            {
                if (secondLastTransaction != 0)
                {
                    transactionVariation = Math.Round(((lastTransaction - secondLastTransaction) / lastTransaction) * 100, 2);
                }
                else
                    transactionVariation = Math.Round(((lastTransaction - decimal.Parse(assetDetails.CoinVal)) / lastTransaction) * 100, 2);
            }
            catch { transactionVariation = 0; }

            return new { productOffers, stats, lastTransaction, transactionVariation, totalCapitalization, MinBuyValue, MaxSellValue };
        }

        public async Task<object> CancelOffer(int userId, string productName, string privateKey, string askAssetName, decimal askAssetQauntity, string offerAssetName, decimal offerAssetQuantity, string HexBlob, decimal pxCoin, string txId)
        {
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();

            string[] privatekeys = new string[] { privateKey };
            Dictionary<string, object> inputs = new Dictionary<string, object>()
            {
                { "txid", txId },
                { "vout", 0 }
            };

            IDictionary<string, object> offerItems = new Dictionary<string, object>();
            offerItems.Add(offerAssetName, offerAssetQuantity);

            Dictionary<string, object> outputs2 = new Dictionary<string, object>()
            {
               { user.BlockChainAddress, offerItems},
            };

            var InfosPrivateKey = await multiChainClient.Transaction.validateaddress(privateKey);

            if (!InfosPrivateKey.Result.isvalid)
            {
                var errorResponse = JsonConvert.DeserializeObject<JsonRpcErrorResponse>("{\"result\":null,\"error\":{\"code\":-5,\"message\":\"Invalid private key\"},\"id\":0}\n");
                throw new JsonRpcException(errorResponse.Error);
            }

            var lockMode = (_enableBlockhainLock) ? RawTransactionAction.Lock : RawTransactionAction.Default;

            var BLOB = await multiChainClient.Transaction.CreateRawTransactionAsync(new object[] { inputs }, outputs2, null, lockMode);

            var HEXBLOB = await multiChainClient.Transaction.SignRawTransactionAsync(BLOB.TxHex.Result, privatekeys);

            if (_enableBlockhainLock)
            {
                var TXID = await multiChainClient.Transaction.SendRawTransactionAsync(HEXBLOB.Result.Hex);
            }

            var transactionDetails = await multiChainClient.Transaction.DecodeRawTransactionAsync(HEXBLOB.Result.Hex);

            var productOffer = new ProductOffer()
            {
                OfferAssetName = offerAssetName,
                OfferQuantity = offerAssetQuantity,
                AskAssetName = askAssetName,
                AskQuantity = askAssetQauntity,
                PxCoin = pxCoin,
                FromAddress = user.BlockChainAddress,
                TxId = txId,
                //HexBlob = HEXBLOB.Result.Hex
                Operation = OfferTypeEnum.Cancel
            };

            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(productOffer));
            string reducedUserBlockChainAddress = user.BlockChainAddress.Substring(0, 20);

            await multiChainClient.Stream.PublishFromAsync(adminAddress, $"STREAM_OPERATIONS_{productName.Replace(" ", "_")}", "hexablob", data);
            await multiChainClient.Stream.PublishFromAsync(adminAddress, $"USO_{reducedUserBlockChainAddress}", "hexablob", data);
            dynamic productOffers = await GetProductOffersFromStreams(productName);
            string MaxSellValue = GetMaxSellValue(productOffers.sellOffers);
            string MinBuyValue = GetMinBuyValue(productOffers.buyOffers);
            return new { productOffers, MaxSellValue, MinBuyValue };
        }

        public async Task<object> GetProductOffersFromStreams(string productName)
        {
            var buyOffersList = new List<ProductOffer>();
            var sellOffersList = new List<ProductOffer>();
            var operationsOffersList = new List<ProductOffer>();
            var buyOffersListFinal = new List<ProductOffer>();
            var sellOffersListFinal = new List<ProductOffer>();

            var buyOffersStream = await multiChainClient.Stream.ListStreamItemsAsync($"STREAM_ACHAT_{productName.Replace(" ", "_")}", false, 999999999, 0);
            var sellOffersStream = await multiChainClient.Stream.ListStreamItemsAsync($"STREAM_VENTE_{productName.Replace(" ", "_")}", false, 999999999, 0);
            var operationsOffersStream = await multiChainClient.Stream.ListStreamItemsAsync($"STREAM_OPERATIONS_{productName.Replace(" ", "_")}", false, 999999999, 0);

            foreach (var offer in buyOffersStream.Result)
            {
                var data = HextoString(offer.Data.ToString());
                ProductOffer productOffer = JsonConvert.DeserializeObject<ProductOffer>(data);
                buyOffersList.Add(productOffer);
            }

            foreach (var offer in sellOffersStream.Result)
            {
                var data = HextoString(offer.Data.ToString());
                ProductOffer productOffer = JsonConvert.DeserializeObject<ProductOffer>(data);
                sellOffersList.Add(productOffer);
            }

            foreach (var offer in operationsOffersStream.Result)
            {
                var data = HextoString(offer.Data.ToString());
                ProductOffer productOffer = JsonConvert.DeserializeObject<ProductOffer>(data);
                operationsOffersList.Add(productOffer);
            }

            //filter duplicates 
            if (operationsOffersList.Count != 0)
            {
                foreach (var item in buyOffersList)
                {
                    var search = operationsOffersList.Where(o => o.TxId == item.TxId && o.Operation == OfferTypeEnum.Buy ||
                                                            (o.TxId == item.TxId && o.Operation == OfferTypeEnum.Cancel))
                                                                           .FirstOrDefault();
                    if (search == null)
                    {
                        buyOffersListFinal.Add(item);
                    }
                }

                foreach (var item in sellOffersList)
                {
                    var search = operationsOffersList.Where(o => o.TxId == item.TxId && o.Operation == OfferTypeEnum.Sell ||
                                                            (o.TxId == item.TxId && o.Operation == OfferTypeEnum.Cancel))
                                                                           .FirstOrDefault();
                    if (search == null)
                    {
                        sellOffersListFinal.Add(item);
                    }
                }
            }
            else
            {
                buyOffersListFinal = buyOffersList;
                sellOffersListFinal = sellOffersList;
            }




            return new
            {
                buyOffers = buyOffersListFinal.ToList(),
                sellOffers = sellOffersListFinal.ToList()
            };
        }

        public async Task<User> CreditCurrency(int userId, int currencyQuantity)
        {
            await CreateCurrencyAsset();
            var addresses = new List<string>();

            var currentUser = _context.Users.Where(u => u.Id == userId)
                                                   //        .Include(u => u.Address)
                                                   .Include(u => u.UserImage)
                                                   .FirstOrDefault();

            await CreateUserInBlockChain(currentUser, addresses);
            //grant addresses send & recieve permissions
            if (addresses.Count != 0)
            {
                multiChainClient.Permission.GrantFrom(adminAddress, addresses, BlockChainPermission.Send);
                multiChainClient.Permission.GrantFrom(adminAddress, addresses, BlockChainPermission.Receive);
                _context.SaveChanges();
            }

            //credit currency to EUR asset
            await multiChainClient.Asset.IssueMoreFromAsync(adminAddress, adminAddress, Constants.CurrencyAssetName, currencyQuantity);
            
            //send user the currency quantity
            await multiChainClient.Asset.SendAssetFromAsync(adminAddress, currentUser.BlockChainAddress, Constants.CurrencyAssetName, currencyQuantity);

            return currentUser;
        }
    
        private async Task CreateStreamsForProject(string assetName)
        {
            await multiChainClient.Stream.CreateFromAsync(adminAddress, $"STREAM_VENTE_{assetName}", false, new { });
            await multiChainClient.Stream.CreateFromAsync(adminAddress, $"STREAM_ACHAT_{assetName}", false, new { });
            await multiChainClient.Stream.CreateFromAsync(adminAddress, $"STREAM_TRADE_{assetName}", false, new { });
            await multiChainClient.Stream.CreateFromAsync(adminAddress, $"STREAM_OPERATIONS_{assetName}", false, new { });

            // subscribe to streams done auto by the blockchain in configuration

            //await multiChainClient.Asset.SubscribeAsync($"STREAM_VENTE_{assetName}", false);
            //await multiChainClient.Asset.SubscribeAsync($"STREAM_ACHAT_{assetName}", false);
            //await multiChainClient.Asset.SubscribeAsync($"STREAM_TRADE_{assetName}", false);
            //await multiChainClient.Asset.SubscribeAsync($"STREAM_OPERATIONS_{assetName}", false);

            //if (otherNodes.Count != 0)
            //{
            //    foreach (var node in otherNodes)
            //    {
            //        await node.Asset.SubscribeAsync($"STREAM_VENTE_{assetName}", false);
            //        await node.Asset.SubscribeAsync($"STREAM_ACHAT_{assetName}", false);
            //        await node.Asset.SubscribeAsync($"STREAM_TRADE_{assetName}", false);
            //        await node.Asset.SubscribeAsync($"STREAM_OPERATIONS_{assetName}", false);
            //        await node.Asset.SubscribeAsync($"STREAM_OPERATIONS_{assetName}", false);
            //    }
            //}

        }

        private async Task DistributeAssetToUsers(Project project, string assetName)
        {
            foreach (var coin in project.Coins)
            {
                var currentUser = _context.Users.Where(u => u.Id == coin.UserId).FirstOrDefault();
                await multiChainClient.Asset.SendAssetFromAsync(adminAddress, currentUser.BlockChainAddress, assetName, coin.CoinsNumber);
            }
        }

        private void SendPrivKeyToUser(User user, string privKey, string publicKey, string Adress)
        {
            _notificationsService.SendPivateKeyNotification(user, privKey, publicKey, Adress);
            var content = EmailTemplate.EmailContent(BlockChainPrivKeyEmailTemplate.Path)
                                       .Replace(EmailTemplate.UserName, user.FirstName)
                                       .Replace(EmailTemplate.UserPrivKey, privKey)
                                       .Replace(EmailTemplate.UserPubKey, publicKey)
                                       .Replace(EmailTemplate.UserAdress, Adress);

            _emailService.SendEmail(user.Email, BlockChainPrivKeyEmailTemplate.Subject, content);
        }

        private async Task CreateCurrencyAsset()
        {
            var assetName = Constants.CurrencyAssetName;
            var asset = new { name = assetName, open = true };

            Decimal units = 0.01m;
            //create asset
            var assets = await multiChainClient.Asset.ListAssetsAsync();
            var currencyAsset = assets.Result.Where(a => a.Name == assetName).FirstOrDefault();

            if (currencyAsset == null)
            {
                await multiChainClient.Asset.IssueFromAsync(adminAddress, adminAddress, asset, 10000, units, null);
            }

            // distribute asset to users (it was only for testing when mangopay not exisitng )
            //foreach (var coin in project.Coins)
            //{
            //    var currentUser = _context.Users.Where(u => u.Id == coin.UserId).FirstOrDefault();
            //    await multiChainClient.Asset.SendAssetFromAsync(adminAddress, currentUser.BlockChainAddress, assetName, 100);
            //}
        }

        private async Task CreateUserInBlockChain(User currentUser, List<string> addresses)
        {
            if (currentUser.BlockChainAddress == null && currentUser.BlockChainPublicKey == null)
            {
                // create new address with pub/priv keys
                var blockChainAddress = multiChainClient.Address.CreateKeyPairsAsync(1).Result.Result[0];

                // store address & pubKey in Database 
                currentUser.BlockChainAddress = blockChainAddress.Address;
                currentUser.BlockChainPublicKey = blockChainAddress.PubKey;
                // send privKey to user with email
                SendPrivKeyToUser(currentUser, blockChainAddress.PrivKey, blockChainAddress.PubKey, blockChainAddress.Address);

                // import address
                await multiChainClient.Wallet.ImportAddressAsync(blockChainAddress.Address, null, false);

                if (otherNodes.Count != 0)
                {
                    foreach (var node in otherNodes)
                    {
                        await node.Wallet.ImportAddressAsync(blockChainAddress.Address, null, false);
                    }
                }

                addresses.Add(blockChainAddress.Address);

                string reducedUserBlockChainAddress = currentUser.BlockChainAddress.Substring(0, 20);

                await CreateUserStreams(reducedUserBlockChainAddress);


            }
        }

        public async Task CreateUserStreams(string userAdress)
        {
            await multiChainClient.Stream.CreateFromAsync(adminAddress, $"USO_{userAdress}", false, new { });
            await multiChainClient.Stream.CreateFromAsync(adminAddress, $"UST_{userAdress}", false, new { });

            //await multiChainClient.Asset.SubscribeAsync($"USO_{userAdress}", false);
            //await multiChainClient.Asset.SubscribeAsync($"UST_{userAdress}", false);

            //if (otherNodes.Count != 0)
            //{
            //    foreach (var node in otherNodes)
            //    {
            //        await node.Asset.SubscribeAsync($"USO_{userAdress}", false);
            //        await node.Asset.SubscribeAsync($"UST_{userAdress}", false);
            //    }
            //}
        }

        public static string HextoString(string InputText)
        {

            byte[] bb = Enumerable.Range(0, InputText.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(InputText.Substring(x, 2), 16))
                             .ToArray();
            return Encoding.UTF8.GetString(bb);
        }

        public async Task<List<Product>> GetListProducts()
        {
            decimal secondLastTransaction;
            var allAssets = await multiChainClient.Asset.ListAssetsAsync();

            var productsList = new List<Product>();
            var listAssets = new List<Asset>();

            foreach (var asset in allAssets.Result)
            {
                if (asset.Name != "EUR")//.Split("_")[0] == "asset")
                {
                    var newAsset = new Asset()
                    {
                        //ProjectId = Convert.ToInt32(asset.Name.Split("_")[1]),
                        AssetName = asset.Name
                    };
                    listAssets.Add(newAsset);
                }
            }

            foreach (var asset in listAssets)
            {
                try
                {
                    var newProduct = new Product();
                    var tradeOffers = new List<ProductOffer>();

                    newProduct.ProductName = asset.AssetName;
                    var currentProject = _context.Projects.Where(p => p.ProductName == asset.AssetName)//p.Id == asset.ProjectId)
                                                           .Include(p => p.User)
                                                           .Include(p => p.ProjectImages)
                                                           .Include(p => p.Documents)
                                                           .Include(p => p.Coins)
                                                           .Include(p => p.ProjectUpdates)
                                                           .Include(p => p.Discussions)
                                                           .ThenInclude(d => d.User)
                                                           .ThenInclude(d => d.UserImage)
                                                           .FirstOrDefault();
                    newProduct.Project = currentProject;

                    var tradeStreamForProduct = await multiChainClient.Stream.ListStreamItemsAsync($"STREAM_TRADE_{asset.AssetName.Replace(" ", "_")}", false, 999999999, 0);

                    foreach (var offer in tradeStreamForProduct.Result)
                    {
                        var data = HextoString(offer.Data.ToString());
                        ProductOffer productOffer = JsonConvert.DeserializeObject<ProductOffer>(data);
                        tradeOffers.Add(productOffer);
                    }

                    foreach (var tradeOffer in tradeOffers)
                    {
                        newProduct.Transactions.Add(tradeOffer.PxCoin);
                        var quantity = (tradeOffer.OfferAssetName == newProduct.ProductName) ?
                                                                    tradeOffer.OfferQuantity :
                                                                    tradeOffer.AskQuantity;

                        //newProduct.TotalCapitalization += (tradeOffer.PxCoin * quantity);
                    }

                    await GetInitTransactionsForProduct(newProduct.Transactions, newProduct.Transactions.Count, asset.AssetName);

                    var lastTransactionInStream = tradeStreamForProduct.Result.OrderByDescending(s => s.BlockTime).FirstOrDefault();
                    var assetInformations = await multiChainClient.Asset.ListAssetsAsync(asset.AssetName);

                    newProduct.TotalCoinNumber = assetInformations.Result.FirstOrDefault().IssueQty;
                    AssetDetails assetDetails = JsonConvert.DeserializeObject<AssetDetails>(assetInformations.Result[0].Details.ToString());

                    if (lastTransactionInStream != null)
                    {
                        var lastData = HextoString(lastTransactionInStream.Data.ToString());
                        ProductOffer lastOffer = JsonConvert.DeserializeObject<ProductOffer>(lastData);
                        newProduct.LastTransaction = lastOffer.PxCoin;
                        newProduct.TotalCapitalization = Math.Round((100 / decimal.Parse(assetDetails.SellPercentage)) * newProduct.TotalCoinNumber * newProduct.LastTransaction);
                    }
                    else
                    {
                        newProduct.TotalCapitalization = Math.Round((100 / decimal.Parse(assetDetails.SellPercentage)) * newProduct.TotalCoinNumber * decimal.Parse(assetDetails.CoinVal));
                        newProduct.LastTransaction = decimal.Parse(assetDetails.CoinVal);
                    }
                    try
                    {
                        var secondLastTransationInStream = tradeStreamForProduct.Result.OrderByDescending(s => s.BlockTime).ElementAt(1);
                        var secondLastData = HextoString(secondLastTransationInStream.Data.ToString());
                        ProductOffer secondLastOffer = JsonConvert.DeserializeObject<ProductOffer>(secondLastData);
                        secondLastTransaction = secondLastOffer.PxCoin;

                    }
                    catch (Exception ex)
                    {
                        secondLastTransaction = 0;
                    }


                    if (lastTransactionInStream == null)
                    {
                        newProduct.TransactionVariation = 0;
                    }
                    else if (lastTransactionInStream != null && secondLastTransaction == 0)
                    {
                        newProduct.TransactionVariation = Math.Round(((newProduct.LastTransaction - decimal.Parse(assetDetails.CoinVal)) / newProduct.LastTransaction) * 100, 2);
                    }
                    else if (lastTransactionInStream != null && secondLastTransaction != 0)
                    {
                        newProduct.TransactionVariation = Math.Round(((newProduct.LastTransaction - secondLastTransaction) / newProduct.LastTransaction) * 100, 2);
                    }
                    dynamic productOffers = await GetProductOffersFromStreams(newProduct.ProductName.Replace(" ", "_"));
                    newProduct.MaxSellValue = GetMaxSellValue(productOffers.sellOffers);
                    newProduct.MinBuyValue = GetMinBuyValue(productOffers.buyOffers);
                    productsList.Add(newProduct);
                }
                catch (Exception ex)
                {
                    continue;
                }

            }
            productsList.Reverse();
            return productsList;
        }

        public async Task<Product> GetProductFromUrl(string productName)
        {
            decimal secondLastTransaction;
            var newAsset = new Asset();
            var newProduct = new Product();
            if (productName != "EUR")//.Split("_")[0] == "asset")
            {
                try
                {
                    //newAsset.ProjectId = Convert.ToInt32(productName.Split("_")[1]);
                    //newAsset.AssetName = productName;

                    var tradeOffers = new List<ProductOffer>();

                    
                    var currentProject = _context.Projects.Where(p => p.Id == Convert.ToInt32(productName))//p.Id == newAsset.ProjectId)
                                                           .Include(p => p.User)
                                                           .Include(p => p.ProjectImages)
                                                           .Include(p => p.Documents)
                                                           .Include(p => p.Coins)
                                                           .Include(p => p.ProjectUpdates)
                                                           .Include(p => p.Discussions)
                                                           .ThenInclude(d => d.User)
                                                           .ThenInclude(d => d.UserImage)
                                                           .FirstOrDefault();
                    newProduct.Project = currentProject;
                    newAsset.AssetName = currentProject.ProductName;
                    newProduct.ProductName = currentProject.ProductName;
                    var tradeStreamForProduct = await multiChainClient.Stream.ListStreamItemsAsync($"STREAM_TRADE_{newAsset.AssetName.Replace(" ", "_")}", false, 999999999, 0);

                    foreach (var offer in tradeStreamForProduct.Result)
                    {
                        var data = HextoString(offer.Data.ToString());
                        ProductOffer productOffer = JsonConvert.DeserializeObject<ProductOffer>(data);
                        tradeOffers.Add(productOffer);
                    }

                    foreach (var tradeOffer in tradeOffers)
                    {
                        newProduct.Transactions.Add(tradeOffer.PxCoin);
                        var quantity = (tradeOffer.OfferAssetName == newProduct.ProductName) ?
                                                                    tradeOffer.OfferQuantity :
                                                                    tradeOffer.AskQuantity;

                        //newProduct.TotalCapitalization += (tradeOffer.PxCoin * quantity);
                    }

                    await GetInitTransactionsForProduct(newProduct.Transactions, newProduct.Transactions.Count, newAsset.AssetName);

                    var lastTransactionInStream = tradeStreamForProduct.Result.OrderByDescending(s => s.BlockTime).FirstOrDefault();

                    var assetInformations = await multiChainClient.Asset.ListAssetsAsync(newAsset.AssetName);
                    newProduct.TotalCoinNumber = assetInformations.Result.FirstOrDefault().IssueQty;
                    AssetDetails assetDetails = JsonConvert.DeserializeObject<AssetDetails>(assetInformations.Result[0].Details.ToString());

                    if (lastTransactionInStream != null)
                    {
                        var lastData = HextoString(lastTransactionInStream.Data.ToString());
                        ProductOffer lastOffer = JsonConvert.DeserializeObject<ProductOffer>(lastData);
                        newProduct.LastTransaction = lastOffer.PxCoin;
                        newProduct.TotalCapitalization = Math.Round((100 / decimal.Parse(assetDetails.SellPercentage)) * newProduct.TotalCoinNumber * newProduct.LastTransaction);
                    }
                    else
                    {
                        newProduct.TotalCapitalization = Math.Round((100 / decimal.Parse(assetDetails.SellPercentage)) * newProduct.TotalCoinNumber * decimal.Parse(assetDetails.CoinVal));
                        newProduct.LastTransaction = decimal.Parse(assetDetails.CoinVal);
                    }

                    try
                    {
                        var secondLastTransationInStream = tradeStreamForProduct.Result.OrderByDescending(s => s.BlockTime).ElementAt(1);
                        var secondLastData = HextoString(secondLastTransationInStream.Data.ToString());
                        ProductOffer secondLastOffer = JsonConvert.DeserializeObject<ProductOffer>(secondLastData);
                        secondLastTransaction = secondLastOffer.PxCoin;
                    }
                    catch (Exception ex)
                    {
                        secondLastTransaction = 0;
                    }

                    if (lastTransactionInStream == null)
                    {
                        newProduct.TransactionVariation = 0;
                    }
                    else if (lastTransactionInStream != null && secondLastTransaction == 0)
                    {
                        newProduct.TransactionVariation = Math.Round(((newProduct.LastTransaction - decimal.Parse(assetDetails.CoinVal)) / newProduct.LastTransaction) * 100, 2);
                    }
                    else if (lastTransactionInStream != null && secondLastTransaction != 0)
                    {
                        newProduct.TransactionVariation = Math.Round(((newProduct.LastTransaction - secondLastTransaction) / newProduct.LastTransaction) * 100, 2);
                    }
                    dynamic productOffers = await GetProductOffersFromStreams(newAsset.AssetName);
                    newProduct.MaxSellValue = GetMaxSellValue(productOffers.sellOffers);
                    newProduct.MinBuyValue = GetMinBuyValue(productOffers.buyOffers);


                }
                catch (Exception ex)
                {
                    return null;
                }


                return newProduct;
            }
            else
            {
                return null;
            }
        }

        public string GetMinBuyValue(List<ProductOffer> productOffersBuy)
        {
            decimal maxValue = 0;

            foreach (var productOffer in productOffersBuy)
            {
                maxValue = Math.Max(maxValue, productOffer.PxCoin);
            }

            return ((maxValue == 0) ? "-" : maxValue.ToString() + "€");
        }

        public string GetMaxSellValue(List<ProductOffer> productOffersSell)
        {
            decimal maxValue = productOffersSell.Count == 0 ? 0 : productOffersSell.ElementAt(0).PxCoin;

            foreach (var productOffer in productOffersSell)
            {
                maxValue = Math.Min(maxValue, productOffer.PxCoin);
            }
            return ((maxValue == 0) ? "-" : maxValue.ToString() + "€");
        }

        public async Task<int> GetAllProductsCount()
        {
            var allProducts = await GetListProducts();
            return allProducts.Count;
        }

        public async Task<List<Product>> GetFilteredProducts(int pageIndex, int pageSize)
        {
            var products = await GetListProducts();
            return products.Skip(pageIndex).Take(pageSize).ToList();
        }

        public async Task<List<decimal>> GetStatsForProduct(string productName, string zoom)
        {
            var stats = new List<decimal>();
            var endDate = DateTime.UtcNow;
            var beginDate = new DateTime();

            switch (zoom)
            {
                case "year":
                    beginDate = endDate.AddDays(-365);
                    break;
                case "month":
                    beginDate = endDate.AddMonths(-1);
                    break;
                case "week":
                    beginDate = endDate.AddDays(-7);
                    break;
            }
            var tradeStreamForProduct = await multiChainClient.Stream.ListStreamItemsAsync($"STREAM_TRADE_{productName.Replace(" ", "_")}", false, 999999999, 0);

            foreach (var offer in tradeStreamForProduct.Result)
            {
                var offerDatetime = UnixTimestampToDateTime(offer.BlockTime).Date;
                if (offerDatetime >= beginDate.Date && offerDatetime <= endDate.Date)
                {
                    var data = HextoString(offer.Data.ToString());
                    ProductOffer productOffer = JsonConvert.DeserializeObject<ProductOffer>(data);
                    stats.Add(productOffer.PxCoin);
                }
            }

            await GetInitTransactionsForProduct(stats, stats.Count, productName.Replace("_", " "));

            return stats;
        }

        private async Task GetInitTransactionsForProduct(List<decimal> productTransactions, int productTransactionsCount, string assetName)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", true, true)
                      .Build();

            var transactionsCountInit = Convert.ToInt32(config["TransactionsCountInit"]);

            var assetInformations = await multiChainClient.Asset.ListAssetsAsync(assetName);

            AssetDetails assetDetails = JsonConvert.DeserializeObject<AssetDetails>(assetInformations.Result[0].Details.ToString());

            if (productTransactionsCount < transactionsCountInit)
            {
                for (int i = 0; i < (transactionsCountInit - productTransactionsCount); i++)
                {
                    productTransactions.Insert(0, Convert.ToDecimal(assetDetails.CoinVal) * 10);
                }
            }
        }

        private DateTime UnixTimestampToDateTime(long unixTime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTime).ToLocalTime();
            return dtDateTime;
        }

        public class Asset
        {
            //public int ProjectId { get; set; }

            public string AssetName { get; set; }
        }

        private void SendBuyCompleteEmail(int userId, string productName, string qte, string totalValue)
        {
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
            var content = EmailTemplate.EmailContent(BuyEndEmailTemplate.Path)
                                           .Replace(EmailTemplate.UserName, user.FirstName)
                                           .Replace(EmailTemplate.ProductName, productName)
                                           .Replace(EmailTemplate.Qte, qte)
                                           .Replace(EmailTemplate.TotalValue, totalValue);

            _emailService.SendEmail(user.Email, BuyEndEmailTemplate.Subject, content);
        }

        private void SendSaleCompleteEmail(int userId, string productName, string qte, string totalValue)
        {
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
            var content = EmailTemplate.EmailContent(SaleEndEmailTemplate.Path)
                                           .Replace(EmailTemplate.UserName, user.FirstName)
                                           .Replace(EmailTemplate.ProductName, productName)
                                           .Replace(EmailTemplate.Qte, qte)
                                           .Replace(EmailTemplate.TotalValue, totalValue);

            _emailService.SendEmail(user.Email, SaleEndEmailTemplate.Subject, content);
        }

        public async Task<object> GetUserProductsBuyList(int userId)
        {
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user != null)
            {
                var buyList = new List<ProductOffer>();
                var sellList = new List<ProductOffer>();
                var buyHistoryList = new List<ProductOffer>();
                var sellHistoryList = new List<ProductOffer>();
                var userOperationsList = new List<ProductOffer>();
                var userTradesList = new List<ProductOffer>();

                //var productsList = new List<Product>();
                //productsList = await GetListProducts();
                string reducedUserBlockChainAddress = user.BlockChainAddress.Substring(0, 20);

                var userOperationsStream = await multiChainClient.Stream.ListStreamItemsAsync($"USO_{reducedUserBlockChainAddress}", false, 999999999, 0);
                var userTradesStream = await multiChainClient.Stream.ListStreamItemsAsync($"UST_{reducedUserBlockChainAddress}", false, 999999999, 0);

                foreach (var offer in userOperationsStream.Result)
                {
                    var data = HextoString(offer.Data.ToString());
                    ProductOffer productOffer = JsonConvert.DeserializeObject<ProductOffer>(data);
                    productOffer.Date = UnixTimestampToDateTime(offer.BlockTime).ToShortDateString();
                    userOperationsList.Add(productOffer);
                }
                foreach (var offer in userTradesStream.Result)
                {
                    var data = HextoString(offer.Data.ToString());
                    ProductOffer productOffer = JsonConvert.DeserializeObject<ProductOffer>(data);
                    productOffer.Date = UnixTimestampToDateTime(offer.BlockTime).ToShortDateString();
                    userTradesList.Add(productOffer);
                }

                foreach (var item in userOperationsList)
                {
                    var searchDeleted = userOperationsList.Where(o => o.TxId == item.TxId && o.Operation == OfferTypeEnum.Cancel).FirstOrDefault();
                    if (searchDeleted == null)
                    {
                        var searchValidated = userTradesList.Where(o => o.TxId == item.TxId).FirstOrDefault();
                        if (searchValidated == null)
                        {
                            if (item.Operation == OfferTypeEnum.Buy)
                            {
                                //var projetcId = Convert.ToInt32(item.AskAssetName.Split("_")[1]);
                                var project = _context.Projects.Where(p => p.ProductName == item.AskAssetName).FirstOrDefault();
                                item.AskAssetName = project.ProductName;
                                item.OfferAssetName = project.Id.ToString();
                                buyList.Add(item);
                            }
                            else
                            {
                                //var projetcId = Convert.ToInt32(item.OfferAssetName.Split("_")[1]);
                                var project = _context.Projects.Where(p => p.ProductName == item.OfferAssetName).FirstOrDefault();
                                item.OfferAssetName = project.ProductName;
                                item.AskAssetName = project.Id.ToString();
                                sellList.Add(item);
                            }
                        }
                    }
                }

                foreach (var item in userTradesList)
                {
                    if (item.Operation == OfferTypeEnum.Buy)
                    {
                        //var projetcId = Convert.ToInt32(item.AskAssetName.Split("_")[1]);
                        var project = _context.Projects.Where(p => p.ProductName == item.AskAssetName).FirstOrDefault();
                        item.AskAssetName = project.ProductName;
                        item.OfferAssetName = project.Id.ToString();
                        buyHistoryList.Add(item);
                    }
                    else
                    {
                        //var projetcId = Convert.ToInt32(item.OfferAssetName.Split("_")[1]);
                        var project = _context.Projects.Where(p => p.ProductName == item.OfferAssetName).FirstOrDefault();
                        item.OfferAssetName = project.ProductName;
                        item.AskAssetName = project.Id.ToString();
                        sellHistoryList.Add(item);

                    }
                }
                return new
                {
                    buyList,
                    sellList,
                    buyHistoryList,
                    sellHistoryList
                };
            }
            else
            {
                throw new System.ArgumentException("Error, UserId is invalid");
            }
        }

        public async Task<int> GetUserEuroBalance(int userId)
        {
            await CreateCurrencyAsset();
            var currentUser = _context.Users.Where(u => u.Id == userId)
                                                   //      .Include(u => u.Address)
                                                   .Include(u => u.UserImage)
                                                   .FirstOrDefault();

            if (currentUser.BlockChainAddress != null && currentUser.BlockChainPublicKey != null)
            {
                string[] addresses = new string[] { currentUser.BlockChainAddress };
                string[] assets = new string[] { "EUR" };

                var balances = await multiChainClient.Wallet.GetAddressBalancesAsync(currentUser.BlockChainAddress, 1, true);
                var currencyAsset = balances.Result.Where(a => a.Name == Constants.CurrencyAssetName).FirstOrDefault();
                if (currencyAsset != null)
                {
                    return Convert.ToInt32(currencyAsset.Qty);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public async Task<object> GetUserStatsDetails(int userId)
        {
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
            var userCoins = _context.Coins.Where(u => u.UserId == userId);
            var returnedList = new List<ProductStats>();
            if (user != null)
            {
                //var buyList = new List<ProductOffer>();
                //var sellList = new List<ProductOffer>();
                string reducedUserBlockChainAddress = user.BlockChainAddress.Substring(0, 20);

                var userTradesList = new List<ProductOffer>();
                var userTradesStream = await multiChainClient.Stream.ListStreamItemsAsync($"UST_{reducedUserBlockChainAddress}", false, 999999999, 0);

                foreach (var offer in userTradesStream.Result)
                {
                    var data = HextoString(offer.Data.ToString());
                    ProductOffer productOffer = JsonConvert.DeserializeObject<ProductOffer>(data);
                    productOffer.Date = UnixTimestampToDateTime(offer.BlockTime).ToShortDateString();
                    userTradesList.Add(productOffer);
                }

                Boolean found;

                foreach (var item in userTradesList)
                {
                    found = false;
                    if (returnedList != null)
                        foreach (var product in returnedList)
                        {
                            if (item.AskAssetName == product.productName)
                            {
                                found = true;
                                product.totalBuy += item.OfferQuantity;

                            }
                            else if (item.OfferAssetName == product.productName)
                            {
                                found = true;
                                product.totalSell += item.AskQuantity;
                            }

                        }
                    if (!found)
                    {
                        if (item.AskAssetName == "EUR")
                        {

                            ProductStats newProductInList = new ProductStats(item.OfferAssetName);
                            newProductInList.totalSell += item.AskQuantity;
                            returnedList.Add(newProductInList);
                        }
                        else
                        {

                            ProductStats newProductInList = new ProductStats(item.AskAssetName);
                            newProductInList.totalBuy += item.OfferQuantity;
                            returnedList.Add(newProductInList);
                        }

                    }

                }
                foreach (var item in returnedList)
                {
                    //var projetcId = Convert.ToInt32(item.productName.Split("_")[1]);
                    var project = _context.Projects.Where(p => p.ProductName == item.productName).FirstOrDefault();
                    item.projectId = project.Id;
                    item.productName = project.ProductName;
                }
                //investements
                if (userCoins != null)
                {
                    foreach (var item in userCoins)
                    {
                        var project = _context.Projects.Where(p => p.Id == item.ProjectId).FirstOrDefault();
                        found = false;
                        if (returnedList != null)
                            foreach (var product in returnedList)
                            {
                                if (project.ProductName == product.productName)
                                {
                                    found = true;
                                    product.totalBuy += item.CoinsNumber;
                                }
                            }
                        if (!found)
                        {
                            ProductStats newProductInList = new ProductStats(project.ProductName);
                            newProductInList.totalBuy += item.CoinsNumber;
                            returnedList.Add(newProductInList);
                        }
                    }
                }
            }
            else
            {
                throw new System.ArgumentException("Error, UserId is invalid");
            }

            return returnedList;
        }

        public async Task<int> GetUserAssetBalance(string assetName, string blockchainAddress)
        {
            string[] addresses = new string[] { blockchainAddress };
            string[] assets = new string[] { assetName };

            var balances = await multiChainClient.Wallet.GetAddressBalancesAsync(blockchainAddress, 1, false);
            var currencyAsset = balances.Result.Where(a => a.Name == assetName).FirstOrDefault();

            return Convert.ToInt32(currencyAsset.Qty);
        }

        private async Task<bool> CheckUserOffersStream(User user)
        {
            var userOffersStream = await multiChainClient.Stream.ListStreamItemsAsync($"USO_{user.BlockChainAddress.Substring(0, 20)}", false, 999999999, 0);
            return (userOffersStream.Result.Count == 0) ? true : false;
        }
        public async Task<int> GetUserProductAssetBalance(string assetName, int userId)
        {
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
            var BlockchainAddress = user.BlockChainAddress;
            //var userOffersStreamIsEmpty = await CheckUserOffersStream(user);

           
                string[] addresses = new string[] { BlockchainAddress };
                string[] assets = new string[] { assetName };

                var balances = await multiChainClient.Wallet.GetAddressBalancesAsync(BlockchainAddress, 1, false);
                var currencyAsset = balances.Result.Where(a => a.Name == assetName).FirstOrDefault();
                if(currencyAsset != null)
                return Convert.ToInt32(currencyAsset.Qty);
            return 0;
        }

        public async Task<Boolean> SendCryptoToBurnAddress(int userId, string userPrivateKey, decimal debitedAmount)
        {
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
            var userBlockChainAddress = user.BlockChainAddress;
            var lockMode = RawTransactionAction.Lock;
            if (!_enableBlockhainLock)
            {
                lockMode = RawTransactionAction.Default;
            }
            IDictionary<string, object> PayOutItems = new Dictionary<string, object>();
            PayOutItems.Add(Constants.CurrencyAssetName, debitedAmount);
            Dictionary<string, object> payOutOutput = new Dictionary<string, object>()
                {
                    { burnAddress, PayOutItems},
                };
            var payOutBLOB = await multiChainClient.Transaction.CreateRawSendFromAsync(userBlockChainAddress, payOutOutput, null, lockMode);
            string[] privateKey = new string[] { userPrivateKey };
            var payOutLargeBLOB = await multiChainClient.Transaction.SignRawTransactionAsync(payOutBLOB.TxHex.Result, privateKey);
            var payOutTxid = await multiChainClient.Transaction.SendRawTransactionAsync(payOutLargeBLOB.Result.Hex);
            if (payOutTxid.Result != null) return true;
            return false;
        }

        private async Task<bool> CheckUserBalances(string offerAsset, decimal offerQuantity, User user)
        {
            //get user stream wallet
            var userofferAssetTotal = await GetUserAssetBalance(offerAsset, user.BlockChainAddress);

            var userOffersStream = await multiChainClient.Stream.ListStreamItemsAsync($"USO_{user.BlockChainAddress.Substring(0, 20)}", false, 999999999, 0);
            var userTradeStream = await multiChainClient.Stream.ListStreamItemsAsync($"UST_{user.BlockChainAddress.Substring(0, 20)}", false, 999999999, 0);

            var userOffersList = new List<ProductOffer>();
            var userTradeList = new List<ProductOffer>();

            foreach (var offer in userOffersStream.Result)
            {
                var data = HextoString(offer.Data.ToString());
                ProductOffer productOffer = JsonConvert.DeserializeObject<ProductOffer>(data);

                userOffersList.Add(productOffer);
            }

            foreach (var offer in userTradeStream.Result)
            {
                var data = HextoString(offer.Data.ToString());
                ProductOffer productOffer = JsonConvert.DeserializeObject<ProductOffer>(data);

                userTradeList.Add(productOffer);
            }

            decimal lockedAssetCreated = 0;
            decimal lockedAssetCanceled = 0;
            decimal lockedAssetTrade = 0;
            decimal lockedAsset = 0;

            foreach (var offer in userOffersList)
            {
                if (offer.OfferAssetName == offerAsset && offer.Operation != OfferTypeEnum.Cancel)
                {
                    lockedAssetCreated = lockedAssetCreated + offer.OfferQuantity;
                }
            }

            foreach (var offer in userOffersList)
            { 
                if (offer.OfferAssetName == offerAsset && offer.Operation == OfferTypeEnum.Cancel)
                {
                    lockedAssetCanceled = lockedAssetCanceled + offer.OfferQuantity;
                }
            }

            foreach (var offer in userTradeList)
            {
                if (offer.OfferAssetName == offerAsset)
                {
                    lockedAssetTrade = lockedAssetTrade + offer.OfferQuantity;
                }
            }

            lockedAsset = lockedAssetCreated - (lockedAssetCanceled + lockedAssetTrade);

            return (lockedAsset + offerQuantity <= userofferAssetTotal) ? true : false;
        }

        private async Task SendCommissionFeesToAdmin(string userBlockchainAddress, string userPrivateKey, decimal commissionFees, RawTransactionAction lockMode)
        {
            IDictionary<string, object> commissionFeesItems = new Dictionary<string, object>();
            commissionFeesItems.Add(Constants.CurrencyAssetName, commissionFees);

            Dictionary<string, object> commissionFeesOutput = new Dictionary<string, object>()
                {
                    { comissionAddress, commissionFeesItems},
                };
            var commissionFeesBLOB = await multiChainClient.Transaction.CreateRawSendFromAsync(userBlockchainAddress, commissionFeesOutput, null, lockMode);
            string[] privateKey = new string[] { userPrivateKey };
            var commissionFeesLargeBLOB = await multiChainClient.Transaction.SignRawTransactionAsync(commissionFeesBLOB.TxHex.Result, privateKey);
            var commissionFeesTxid = await multiChainClient.Transaction.SendRawTransactionAsync(commissionFeesLargeBLOB.Result.Hex);
        }
    }
}
