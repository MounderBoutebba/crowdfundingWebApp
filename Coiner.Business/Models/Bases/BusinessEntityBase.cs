using System;

namespace Coiner.Business.Models.Bases
{
    public class BusinessEntityBase
    {
        public BusinessEntityBase()
        {
            CreationDate = DateTime.Now;
            UpdateDate = DateTime.Now;
        }

        public int Id { get; set; }

        private DateTime _creationDate;
        public DateTime CreationDate
        {
            get
            {
                return _creationDate;
            }
            set
            {
                if (value.Kind == DateTimeKind.Unspecified)
                {
                    _creationDate = new DateTime(value.Ticks, DateTimeKind.Local);
                }
                else
                {
                    _creationDate = value.ToLocalTime();
                }
            }
        }

        private DateTime _updateDate;
        public DateTime UpdateDate
        {
            get
            {
                return _updateDate;
            }
            set
            {
                if (value.Kind == DateTimeKind.Unspecified)
                {
                    _updateDate = new DateTime(value.Ticks, DateTimeKind.Local);
                }
                else
                {
                    _updateDate = value.ToLocalTime();
                }
            }
        }
    }
}
