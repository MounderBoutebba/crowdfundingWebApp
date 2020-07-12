using Coiner.Business.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coiner.Controllers.ModelsDto
{
    public class BlockChainOfferDto
    {
        public int UserId { get; set; }

        public string PrivateKey { get; set; }

        public string ProductName { get; set; }

        public string Currency { get; set; }

        public decimal CurrencyQuantity { get; set; }

        public decimal ProductQuantity { get; set; }

        public OfferTypeEnum OfferType { get; set; }

        //commissionFees
        public decimal CommissionFees { get; set; } 

    }
}
