using VideoAccess.Contracts;

namespace VideoAccess.Models.Entities
{
    public class VideoEntity
    {
        public Guid VideoId { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public long FileSize { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public VideoProcessingStatus ProcessingStatus { get; set; } = VideoProcessingStatus.Created;

        /// <summary>
        /// Идентификатор пользователя (необязательный, для будущей аутентификации)
        /// </summary>
        public Guid? UserId { get; set; }
    }
}
