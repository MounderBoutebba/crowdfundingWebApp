using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Coiner.Business.Heplers;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using MangoPay.SDK;
using MangoPay.SDK.Entities.POST;
using MangoPay.SDK.Core.Enumerations;
using Coiner.Business.Models;
using MangoPay.SDK.Entities;
using MangoPay.SDK.Entities.GET;
using Coiner.Business.Context;
using System.Linq;
using Coiner.Business.Models.Enums;

namespace Coiner.Business.Services
{
    public class MangoPayService
    {
        private readonly IMangoPayApi _mangoPayApi;
        private IConfiguration config;
        private CoinerContext _context = new CoinerContext();
        private double _mangoPayConstantFees;
        private double _mangoPayPercentageAmountFees;
        private double _stirblockPercentageFees;
        private string _stirblockCantonmentWalletId;
        private string _stirblockCommisionWalletId;


        public MangoPayService()
        {
            config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
            var MangoPayConfiguration = config.GetSection("MangoPayConfiguration");

            _mangoPayApi = new MangoPayApi();

            _mangoPayApi.Config.BaseUrl = MangoPayConfiguration["BaseUrl"]; // or the prod url
            _mangoPayApi.Config.ClientId = MangoPayConfiguration["ClientId"];
            _mangoPayApi.Config.ClientPassword = MangoPayConfiguration["ApiKey"];
            _mangoPayApi.Config.ApiVersion = MangoPayConfiguration["ApiVersion"];
            _mangoPayApi.Config.Timeout = 0;

            _mangoPayConstantFees = Convert.ToDouble(config.GetSection("MangoPayConfiguration")["MangoPayConstantFees"]);
            _mangoPayPercentageAmountFees = Convert.ToDouble(config.GetSection("MangoPayConfiguration")["MangoPayPercentageAmountFees"]);
            _stirblockPercentageFees = Convert.ToDouble(config.GetSection("MangoPayConfiguration")["StirblockPercentageFees"]);

            _stirblockCantonmentWalletId = Convert.ToString(config.GetSection("MangoPayConfiguration")["StirblockCantonmentWalletId"]);
            _stirblockCommisionWalletId = Convert.ToString(config.GetSection("MangoPayConfiguration")["StirblockCommisionWalletId"]);

        }

        #region public methods
        public async Task<string> CreateUser(User user)
        {
            UserNaturalPostDTO naturalUser = new UserNaturalPostDTO(user.Email, user.FirstName, user.LastName, new DateTime(1975, 12, 21, 0, 0, 0), CountryIso.FR, CountryIso.FR);
            var createdUser = await _mangoPayApi.Users.Create(naturalUser);
            return await CreateWallet(createdUser.Id);
        }

        public async Task<string> CreateWallet(string userId)
        {
            List<string> owners = new List<string>();
            owners.Add(userId);
            WalletPostDTO wallet = new WalletPostDTO(owners, Constants.walletDescription, CurrencyIso.EUR);

            var CreatedWallet = await _mangoPayApi.Wallets.Create(wallet);
            if (CreatedWallet != null)
            {
                return CreatedWallet.Id;
            }
            else return null;
        }

        public async Task<PayInCardWebDTO> CreateCashInCryptoCurrency(string walletId, int CurrencyQuantity, int projectId, RedirectPageEnum redirectPage)
        {
            string returnUrl = Constants.BaseUrl;
            switch (redirectPage)
            {
                case RedirectPageEnum.Project:
                     returnUrl = $"{Constants.BaseUrl}/details-projet/" + projectId;
                    break;
                case RedirectPageEnum.Product:
                     returnUrl = $"{Constants.BaseUrl}/details-produit/" + projectId;
                    break;
                case RedirectPageEnum.Wallet:
                     returnUrl = $"{Constants.BaseUrl}/dashboard?page=wallet";
                    break;
                default:
                    break;
            }
            var totalAmount = (CurrencyQuantity * 100) + CalculateMangoPayFees(CurrencyQuantity);
            var fees = 0;
            var payInObject = await CreateCashIn(walletId, totalAmount, fees, returnUrl);
            return payInObject;
        }

        public async Task<PayInCardWebDTO> CreateCashInInvestment(string walletId, int CurrencyQuantity, int projectId)
        {
            var returnUrl = $"{Constants.BaseUrl}/details-projet/" + projectId;
            var totalAmount = (CurrencyQuantity * 100) + CalculateMangoPayFees(CurrencyQuantity);
            var fees = 0;
            var payInObject = await CreateCashIn(walletId, totalAmount, fees, returnUrl);
            return payInObject;
        }

        public async Task<bool> SendMoneyFromContonmentToOwnerProject(string creditedWalletId, int coinsNumber, long feesAmount)
        {
            var tranfertObject = await CreateTransfertMoney(_stirblockCantonmentWalletId, creditedWalletId, GetTotalAmountInvestment(coinsNumber), feesAmount);
            return tranfertObject.Status == TransactionStatus.SUCCEEDED ? true : false;
        }

        public long GetTotalAmountInvestment(int coinsNumber)
        {
            long totalAmount = 0;
            return totalAmount = (coinsNumber * 100);
        }

        public async Task<int> GetCryptoCurrencyAmount(string transactionId)
        {
            var payInObject = await _mangoPayApi.PayIns.Get(transactionId);
            var cryptoCurrencyQuantity = 0;
            long transactionTotalAmount = 0;
            var contonmentAmount = 0;
            var mangoPayFeesAmount = 0;

            if (payInObject.Status == TransactionStatus.SUCCEEDED)
            {
                transactionTotalAmount = payInObject.DebitedFunds.Amount;

                contonmentAmount = CalculateContonmentTotalAmount(transactionTotalAmount) * 100;

                var contonmentTransfertObject = new MangoPayTransfertObject
                {
                    DebitedWalletID = payInObject.CreditedWalletId,
                    CreditedWalletId = _stirblockCantonmentWalletId,
                    TotalAmount = contonmentAmount,
                    FeesAmount = 0
                };

                mangoPayFeesAmount = Convert.ToInt32(transactionTotalAmount) - contonmentAmount;

                var commissionTransfertObject = new MangoPayTransfertObject
                {
                    DebitedWalletID = payInObject.CreditedWalletId,
                    CreditedWalletId = _stirblockCommisionWalletId,
                    TotalAmount = mangoPayFeesAmount,
                    FeesAmount = 0
                };

                var transfertToContonmentDone = await TransfertMoneyToCantonmentWallet(contonmentTransfertObject.DebitedWalletID, contonmentTransfertObject.CreditedWalletId, contonmentTransfertObject.TotalAmount, contonmentTransfertObject.FeesAmount);
                var transfertToCommissionDone = await TransfertMangoPayFeesToCommissionWallet(commissionTransfertObject.DebitedWalletID, commissionTransfertObject.CreditedWalletId, commissionTransfertObject.TotalAmount, commissionTransfertObject.FeesAmount);

                if (transfertToContonmentDone && transfertToCommissionDone)
                {
                    cryptoCurrencyQuantity = CalculateCryptoCurrency(transactionTotalAmount);
                    return cryptoCurrencyQuantity;
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

        public async Task<bool> GetInvestmentAmount(string transactionId, string ProjecOwnertWalletID)
        {
            var payInObject = await _mangoPayApi.PayIns.Get(transactionId);
            long transactionTotalAmount = 0;
            var projectOwnerAmount = 0;
            var mangoPayFeesAmount = 0;

            if (payInObject.Status == TransactionStatus.SUCCEEDED)
            {
                transactionTotalAmount = payInObject.DebitedFunds.Amount;

                projectOwnerAmount = CalculateProjectOwnerTotalAmount(transactionTotalAmount) * 100;
                var projectwnerTransfertObject = new MangoPayTransfertObject
                {
                    DebitedWalletID = payInObject.CreditedWalletId,
                    CreditedWalletId = ProjecOwnertWalletID,
                    TotalAmount = projectOwnerAmount,
                    FeesAmount = 0
                };

                mangoPayFeesAmount = Convert.ToInt32(transactionTotalAmount) - projectOwnerAmount;

                var commissionTransfertObject = new MangoPayTransfertObject
                {
                    DebitedWalletID = payInObject.CreditedWalletId,
                    CreditedWalletId = _stirblockCommisionWalletId,
                    TotalAmount = mangoPayFeesAmount,
                    FeesAmount = 0
                };

                var transfertToProjectOwnerDone = await TransfertMoneyToCantonmentWallet(projectwnerTransfertObject.DebitedWalletID, projectwnerTransfertObject.CreditedWalletId, projectwnerTransfertObject.TotalAmount, projectwnerTransfertObject.FeesAmount);
                var transfertToCommissionDone = await TransfertMangoPayFeesToCommissionWallet(commissionTransfertObject.DebitedWalletID, commissionTransfertObject.CreditedWalletId, commissionTransfertObject.TotalAmount, commissionTransfertObject.FeesAmount);

                if(transfertToProjectOwnerDone && transfertToCommissionDone)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RefundUsers(string projectOwnerWalletId, long totalAmountToRefund)
        {
            var trasfertObject = new MangoPayTransfertObject
            {
                DebitedWalletID = projectOwnerWalletId,
                CreditedWalletId = _stirblockCantonmentWalletId,
                TotalAmount = totalAmountToRefund * 100,
                FeesAmount = 0
            };

            var tranfertObject = await CreateTransfertMoney(trasfertObject.DebitedWalletID, trasfertObject.CreditedWalletId, trasfertObject.TotalAmount, trasfertObject.FeesAmount);
            return tranfertObject.Status == TransactionStatus.SUCCEEDED ? true : false;
        }

        #endregion

        #region private methods
        private async Task<PayInCardWebDTO> CreatePayInCardWeb(string authorId, string creditedWalletId, Money debitedFunds, Money fees, string returnURL, CultureCode culture, CardType cardType)
        {
            PayInCardWebPostDTO payIn = new PayInCardWebPostDTO(authorId, debitedFunds, fees, creditedWalletId, returnURL, culture, cardType)
            {
                SecureMode = SecureMode.FORCE
            };
            var payInCardWeb = await _mangoPayApi.PayIns.CreateCardWeb(payIn);
            return payInCardWeb;
        }

        private async Task<PayInCardWebDTO> CreateCashIn(string walletId, long totalAmount, long feesAmount, string returnUrl)
        {
            var wallet = await _mangoPayApi.Wallets.Get(walletId);
            var naturalUserId = wallet.Owners[0];

            var debitedFunds = new Money
            {
                Currency = CurrencyIso.EUR,
                Amount = totalAmount
            };

            var fees = new Money
            {
                Currency = CurrencyIso.EUR,
                Amount = feesAmount
            };

            var payInObject = await CreatePayInCardWeb(naturalUserId, walletId, debitedFunds, fees, returnUrl, CultureCode.FR, CardType.CB_VISA_MASTERCARD);
            return payInObject;
        }

        private async Task<TransferDTO> CreateTransfertMoney(string debitedWalletID, string creditedWalletId, long totalAmount, long feesAmount)
        {
            var wallet = await _mangoPayApi.Wallets.Get(debitedWalletID);
            var naturalUserId = wallet.Owners[0];

            var debitedFunds = new Money()
            {
                Currency = CurrencyIso.EUR,
                Amount = totalAmount
            };

            var fees = new Money()
            {
                Currency = CurrencyIso.EUR,
                Amount = feesAmount
            };

            TransferPostDTO transferPostDTO = new TransferPostDTO(naturalUserId, naturalUserId, debitedFunds, fees, debitedWalletID, creditedWalletId);
            var transferObject = await _mangoPayApi.Transfers.Create(transferPostDTO);
            return transferObject;
        }

        private async Task<bool> TransfertMoneyToCantonmentWallet(string debitedWalletID, string creditedWalletId, long totalAmount, long feesAmount)
        {
            var tranfertObject = await CreateTransfertMoney(debitedWalletID, creditedWalletId, totalAmount, feesAmount);
            return tranfertObject.Status == TransactionStatus.SUCCEEDED ? true : false;
        }

        private async Task<bool> TransfertMangoPayFeesToCommissionWallet(string debitedWalletID, string creditedWalletId, long totalAmount, long feesAmount)
        {
            var tranfertObject = await CreateTransfertMoney(debitedWalletID, creditedWalletId, totalAmount, feesAmount);
            return tranfertObject.Status == TransactionStatus.SUCCEEDED ? true : false;
        }

        private long CalculateMangoPayFees(long amount)
        {
            var fees = Convert.ToInt64((_mangoPayConstantFees + ((_mangoPayPercentageAmountFees / 100) * amount)) * 100);
            return fees;
        }

        private int CalculateContonmentTotalAmount(long totalAmount)
        {
            var amount = Convert.ToDouble(totalAmount) / 100;
            var cryptoCurrency = Math.Round((amount - _mangoPayConstantFees) / (1 + (_mangoPayPercentageAmountFees / 100)), 2);
            return Convert.ToInt32(cryptoCurrency);
        }

        private int CalculateProjectOwnerTotalAmount(long totalAmount)
        {
            var amount = Convert.ToDouble(totalAmount) / 100;
            var cryptoCurrency = Math.Round((amount - _mangoPayConstantFees) / (1 + (_mangoPayPercentageAmountFees / 100)), 2);
            return Convert.ToInt32(cryptoCurrency);
        }

        private int CalculateCryptoCurrency(long totalAmount)
        {
            var amount = Convert.ToDouble(totalAmount) / 100;
            var cryptoCurrency = Math.Round((amount - _mangoPayConstantFees) / (1 + (_mangoPayPercentageAmountFees / 100)), 2);
            return Convert.ToInt32(cryptoCurrency);
        }

        private int GetMangoPayFees(long totalAmount)
        {
            var amount = Convert.ToDouble(totalAmount) / 100;
            var cryptoCurrency = Math.Round((amount - _mangoPayConstantFees) / (1 + (_mangoPayPercentageAmountFees / 100)), 2);
            return Convert.ToInt32(cryptoCurrency);
        }
        #endregion
        public async Task<PayOutBankWireDTO> CreatePayOut(int userId, long debited)
        {
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
            string authorId;
            string debitedWalletId;
            Money debitedFunds = new Money();
            Money fees = new Money();
            string bankAccountId;
            string bankWireRef;
            debitedWalletId = user.WalletId;
            var wallet = await _mangoPayApi.Wallets.Get(debitedWalletId);
            authorId = wallet.Owners[0];


            //-------------------------------------------------//////////////
            // temporaire creation d'un compte bancaire
            var bankAccounts = await _mangoPayApi.Users.GetBankAccounts(authorId);
            if (bankAccounts.Count == 0)
            {
                MangoPay.SDK.Entities.Address adress = new MangoPay.SDK.Entities.Address();
                adress.AddressLine1 = "1 Mangopay Street ";
                adress.AddressLine2 = "The Loop";
                adress.City = "Paris";
                adress.Country = CountryIso.FR;
                adress.Region = "Ile de France";
                adress.PostalCode = "75001";
                string iban = "FR7630004000031234567890143";
                BankAccountIbanPostDTO bankacount = new BankAccountIbanPostDTO("John doe", adress, iban);
                var Bankid = await _mangoPayApi.Users.CreateBankAccountIban(authorId, bankacount);
                bankAccountId = Bankid.Id;
            }
            else
            {
                bankAccountId = bankAccounts[0].Id;
            }
            // ---------------------------------------- ////////

            debitedFunds.Amount = debited * 100;
            debitedFunds.Currency = CurrencyIso.EUR;
            fees.Amount = 0;
            fees.Currency = CurrencyIso.EUR;
            
            bankWireRef = "testAUTO";
            PayOutBankWirePostDTO payOut = new PayOutBankWirePostDTO(authorId, debitedWalletId, debitedFunds, fees, bankAccountId, bankWireRef);
           
            var transferObject = await CreateTransfertMoney(_stirblockCantonmentWalletId, debitedWalletId, debitedFunds.Amount, fees.Amount);

            var payOutResult = await _mangoPayApi.PayOuts.CreateBankWire(payOut);
            return payOutResult;
        }
    }
}
