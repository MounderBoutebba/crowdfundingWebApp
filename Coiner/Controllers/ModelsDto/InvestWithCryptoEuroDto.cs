using Coiner.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coiner.Controllers.ModelsDto
{
    public class InvestWithCryptoEuroDto
    {
        public Coin Coin { get; set; }

        public int UserId { get; set; }

        public int CryptoEuroQuantity { get; set; }

        public string PrivateKey { get; set; }

        public string OwnerProjectWalletId { get; set; }

    }
}
