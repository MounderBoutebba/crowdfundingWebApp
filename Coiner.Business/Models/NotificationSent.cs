using System;
using System.Collections.Generic;
using System.Text;

namespace Coiner.Business.Models
{
    public class NotificationSent
    {
        public int Id { get; set; }

        public int NotificationProducedId { get; set; }

        public NotificationProduced NotificationProduced { get; set; }

        public bool SendStatus { get; set; }

        public string SendEmail { get; set; }

        public string SendPhone { get; set; }

        public DateTimeOffset SendTime { get; set; }
    }
}
