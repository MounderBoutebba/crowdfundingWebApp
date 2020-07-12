using Coiner.Business.Models;
using Coiner.Business.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coiner.Controllers.ModelsDto
{
    public class CreditCurrencyDto
    {
        public int UserId { get; set; }

        public int CurrencyQuantity { get; set; }

        public string WalletId { get; set; }

        public int ProjectId { get; set; }

        public RedirectPageEnum RedirectPage { get; set; }
    }
}
