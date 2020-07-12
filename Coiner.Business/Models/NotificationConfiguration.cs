using System;
using System.Collections.Generic;
using System.Text;

namespace Coiner.Business.Models
{
    public class NotificationConfiguration
    {
        public int Id { get; set; }

        public NotificationTemplate NotificationTemplate { get; set; }

        public NotificationUpdateFrequencyEnum NotificationUpdateFrequency { get; set; }

        public DateTimeOffset SendTime { get; set; }

        public DayOfWeek SendDay { get; set; }

        public NotificationOutputTypeEnum NotificationOutputType { get; set; }
    }
}
