using Coiner.Business.Models.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coiner.Business.Models
{
    public class Bill : BusinessEntityBase
    {
        public string PDFPath { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }

        [NotMapped]
        public string Content { get; set; }
    }
}
