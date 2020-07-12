using Coiner.Business.Models.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coiner.Business.Models
{
    public class UserWallet : BusinessEntityBase
    {
        public int UnusedCoinsNumber { get; set; }

        public int RemovedCoinsNumber { get; set; }

        public int FirstUsedCoinsNumber { get; set; }

        public int SecondUsedCoinsNumber { get; set; }

        public int ThirdUsedCoinsNumber { get; set; }

        public int DisappeardCoinsNumber { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
