using Coiner.Business.Models.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coiner.Business.Models
{
    public class Discussion : BusinessEntityBase
    {
        public string QuestionContent { get; set; }

        public string AnswerContent { get; set; }

        public DateTime QuestionCreation { get; set; }

        public DateTime AnswerCreation { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }
    }
}
