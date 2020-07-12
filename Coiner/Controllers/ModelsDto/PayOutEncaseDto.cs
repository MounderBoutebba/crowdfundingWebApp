using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coiner.Controllers.ModelsDto
{
    public class PayOutEncaseDto
    {
        public int UserId { get; set; }

        public int DebitedAmount { get; set; }

        public string PrivateKey { get; set; }
    }
}
