using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coiner.Business.Models.Bases
{
    public abstract class UploadedImage : BusinessEntityBase
    {
        public string Path { get; set; }

        public bool IsDefault { get; set; }

        [NotMapped]
        public string Content { get; set; }
    }
}
