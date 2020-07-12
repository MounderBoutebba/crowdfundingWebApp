using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coiner.Controllers.ModelsDto
{
    public class UserCheckTokenDto
    {
        public int UserId { get; set; }

        public string Token { get; set; }
    }
}
