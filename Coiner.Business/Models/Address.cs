using Coiner.Business.Models.Bases;

namespace Coiner.Business.Models
{
    public class Address : BusinessEntityBase
    {
        public string Country { get; set; }

        public string Town { get; set; }

        public string ZipCode { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
