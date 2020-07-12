using Coiner.Business.Heplers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coiner.Controllers.ModelsDto
{
    public class OfferDto
    {
        public int UserId { get; set; }

        public string PrivateKey { get; set; }

        public ProductOffer ProductOffer { get; set; }

        public string ProductName { get; set; }

        public decimal CommissionFees { get; set; } 
    }
}
