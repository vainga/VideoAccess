namespace VideoAccess.Models.Entities
{
    public class VideoEntity
    {
        public Guid VideoId { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public long FileSize { get; set; }

        /// <summary>
        /// Идентификатор пользователя (необязательный, для будущей аутентификации)
        /// </summary>
        public Guid? UserId { get; set; }
    }
}
