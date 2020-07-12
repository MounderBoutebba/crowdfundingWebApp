using Coiner.Business.Models.Bases;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coiner.Business.Models
{
    public class ProjectImage : UploadedImage
    {
        public int ProjectId { get; set; }

        public Project Project { get; set; }
    }
}
