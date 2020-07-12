using Coiner.Business.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coiner.Controllers.ModelsDto
{
    public class ProductFilterDto
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

    }
}
