using System;
using System.Collections.Generic;
using System.Text;

namespace Coiner.Business.Heplers
{
    public class MangoPayTransfertObject
    {
        public string DebitedWalletID { get; set; }
        public string CreditedWalletId { get; set; } 
        public long TotalAmount { get; set; }
        public long FeesAmount { get; set; }
    }
}
