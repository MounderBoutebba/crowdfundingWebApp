using Coiner.Business.Models.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coiner.Business.Models
{
    public class Document : BusinessEntityBase
    {
        public string Path { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Extention { get; set; }

        [NotMapped]
        public string Content { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }

    }
}
