using Coiner.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coiner.Business.Models
{
    public class NotificationProduced
    {
        public int Id { get; set; }

        public int NotificationTemplateId { get; set; }

        public NotificationTemplate NotificationTemplate { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTimeOffset CreateDate { get; set; }

        public NotificationOutputTypeEnum NotificationOutputType { get; set; }

        public bool AppReadStatus { get; set; }

        public int ProjectId { get; set; }

        public Project project;

        public int UserId { get; set; }

        public User user;

        public DateTimeOffset AppReadTime { get; set; }

        public NotificationSent NotificationSent { get; set; }
    }
}
