using HomeTrack.Domain;
using HomeTrack.Domain.Enum;
using HomeTrack.Api.Models.Entities;

namespace HomeTrack.Domain
{
    public class Subscription
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public int PackageId { get; set; } // Khóa ngoại đến Package
        public virtual Package Package { get; set; } = null!; // Navigation property

        public SubscriptionStatus Status { get; set; }
        public DateTime StartsAt { get; set; } // Thời gian bắt đầu hiệu lực
        public DateTime EndsAt { get; set; }   // Thời gian kết thúc hiệu lực
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}