using Coiner.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coiner.Controllers.ModelsDto
{
    public class AddCoinsDto
    {
        public Coin Coin { get; set; }

        public string TransactionId { get; set; }

        public string UserWallerID { get; set; }
    }
}
