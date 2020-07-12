using Coiner.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coiner.Business.Heplers
{
    public class Product
    {
        public string ProductName { get; set; }

        public decimal LastTransaction { get; set; }
     
        public decimal TotalCapitalization { get; set; }

        public List<decimal> Transactions { get; set; }

        public Project Project { get; set; }

        public decimal TransactionVariation { get; set; }

        public decimal CoinValue { get; set; }

        public decimal TotalCoinNumber { get; set; }

        public string MinBuyValue { get; set; }

        public string MaxSellValue { get; set; }

        public Product()
        {
            Transactions = new List<decimal>();
        }
      
    }
}
