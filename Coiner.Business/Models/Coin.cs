using System;
using System.Collections.Generic;
using System.Text;
using Coiner.Business.Models.Bases;
using Coiner.Business.Models.Enums;

namespace Coiner.Business.Models
{
    public class Coin : BusinessEntityBase
    {
        public decimal CoinValue { get; set; }

        public CoinStatusEnum CoinStatus { get; set; }

        public int CoinsNumber { get; set; }

        public int UsedNumber { get; set; }

        public decimal CoinPrice { get; set; }

        public int CoinsMonetizedNumber { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }
    }
}
