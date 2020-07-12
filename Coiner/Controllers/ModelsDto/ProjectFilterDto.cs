using Coiner.Business.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coiner.Controllers.ModelsDto
{
    public class ProjectFilterDto
    {
        public List<string> ProjectActivityTypes { get; set; }

        public List<ProjectTypeEnum> ProjectTypes { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public ProjectFilterDto()
        {
            ProjectActivityTypes = new List<string>();
            ProjectTypes = new List<ProjectTypeEnum>();
        }
    }
}
