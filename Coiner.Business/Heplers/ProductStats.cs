using System;
using System.Collections.Generic;
using System.Text;

namespace Coiner.Business.Heplers
{
    class ProductStats
    {
        public string productName;
        public decimal totalBuy;
        public decimal totalSell;
        public int projectId;

        public ProductStats(string pName)
        {
            productName = pName;
            projectId = 0;
            totalBuy = 0;
            totalSell = 0;
        }

    }
    
}
