using Coiner.Business.Models.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coiner.Business.Models
{
    public class ProjectUpdate : BusinessEntityBase
    {
        public int ProjectId { get; set; }

        public string NewsContent { get; set; }

    }
}
