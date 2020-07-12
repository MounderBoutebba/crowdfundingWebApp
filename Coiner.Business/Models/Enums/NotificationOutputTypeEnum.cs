using System;

namespace Coiner.Business.Models
{
    [Flags]
    public enum NotificationOutputTypeEnum
    {
        Application = 0,
        Email       = 1,
        SMS         = 0
    }
}