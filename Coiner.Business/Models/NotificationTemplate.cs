using System;
using System.Collections.Generic;
using System.Text;

namespace Coiner.Business.Models
{
    public class NotificationTemplate
    {
        public int Id { get; set; }

        public int NotificationConfigurationId { get; set; }

        public NotificationConfiguration NotificationConfiguration { get; set; }

        public NotificationCategoryEnum NotificationCategory { get; set; }

        public NotificationTypeEnum NotificationType { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public ICollection<NotificationProduced> NotificationsProduced { get; set; }
    }
}
