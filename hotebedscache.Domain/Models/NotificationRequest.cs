﻿namespace Hotebedscache.Domain.Models
{
    public class NotificationRequest
    {
        public string? Token { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
    }
}
