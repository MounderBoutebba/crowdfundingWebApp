using Coiner.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coiner.Controllers.ModelsDto
{
    public class InvestCurrencyDto
    {
        public int UserId { get; set; }

        public int CurrencyQuantity { get; set; }

        public string WalletId { get; set; }

        public string ProjectId { get; set; }
    }
}
