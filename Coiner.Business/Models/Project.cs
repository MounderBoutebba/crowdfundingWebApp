using Coiner.Business.Models.Bases;
using Coiner.Business.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Coiner.Business.Models
{
    public class Project : BusinessEntityBase
    {
        public string ProjectName { get; set; }

        public string ProjectDescription { get; set; }

        public ProjectStatusEnum ProjectStatus { get; set; }

        public int FundraisingPeriod { get; set; }

        public ProjectTypeEnum ProjectType { get; set; }

        public string ActivityType { get; set; }

        public decimal FundingGoal { get; set; }

        public decimal ReceivedFunding { get; set; }

        public DateTime? BeginEstimatedDate { get; set; }

        public string ProjectAddress { get; set; }

        public string WebLink { get; set; }

        public decimal PercentageAsset { get; set; }

        // this is only for Project, Career, Product
        public string ProjectPresentation { get; set; }

        // this is only for Project, Career, Society
        public string BusinessPlan { get; set; }

        public DateTime? Product_SalesPercepective { get; set; }

        public int Career_EngagementYears { get; set; }

        public string Society_Name { get; set; }

        public string Society_LegaleIdentification { get; set; }

        public string Society_StructureType { get; set; }

        public DateTime? Society_CreationDate { get; set; }

        public DateTime? PublishDate { get; set; }

        public DateTime? ValidationDate { get; set; }

        public DateTime? BeginEnchereDate { get; set; }

        public DateTime? EndEnchereDate { get; set; }


        // product name in blockchain

        public string ProductName { get; set; }

        public decimal Product_TVA { get; set; }

        public int CommissionTokenStirblock { get; set; }

        public string Product_BillDescription { get; set; }
         
        public int UserId { get; set; }

        public User User { get; set; }

        public ICollection<ProjectImage> ProjectImages { get; set; }

        public ICollection<Document> Documents { get; set; }

        public ICollection<Coin> Coins { get; set; }

        public ICollection<ProjectUpdate> ProjectUpdates { get; set; }

        public ICollection<Discussion> Discussions { get; set; }

        public ICollection<Bill> Bills { get; set; }

        [NotMapped]
        public int Backers
        {
            get
            {
                if (Coins != null && Coins.Count != 0)
                {
                    var Backers = Coins.GroupBy(c => c.UserId).Select(c => c.FirstOrDefault()).Count();
                    return Backers;
                }
                else
                {
                    return 0;
                }
            }
        }

    }
}
