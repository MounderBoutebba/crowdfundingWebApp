using Coiner.Business.Models.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coiner.Business.Models
{
    public class UserImage : UploadedImage
    {
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
