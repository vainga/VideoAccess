using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoAccess.Models.Entities;

namespace VideoAccess.Data
{

    public class VideoRepository : IVideoRepository
    {
        private readonly ApplicationDbContext _context;

        public VideoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<VideoEntity> CreateAsync(VideoEntity video)
        {
            if (video == null)
                throw new ArgumentNullException(nameof(video));

            video.VideoId = Guid.NewGuid();
            video.UploadedAt = DateTime.UtcNow;

            await _context.Videos.AddAsync(video);
            await _context.SaveChangesAsync();

            return video;
        }

        public async Task<VideoEntity?> GetByIdAsync(Guid videoId)
        {
            return await _context.Videos
                .FirstOrDefaultAsync(v => v.VideoId == videoId);
        }

        public async Task<List<VideoEntity>> GetAllAsync()
        {
            return await _context.Videos
                .OrderByDescending(v => v.UploadedAt)
                .ToListAsync();
        }

        public async Task<List<VideoEntity>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Videos
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.UploadedAt)
                .ToListAsync();
        }

        public async Task<VideoEntity?> GetByFileNameAsync(string fileName)
        {
            return await _context.Videos
                .FirstOrDefaultAsync(v => v.FileName == fileName);
        }

        public async Task<VideoEntity?> UpdateAsync(VideoEntity video)
        {
            if (video == null)
                throw new ArgumentNullException(nameof(video));

            var existingVideo = await _context.Videos
                .FirstOrDefaultAsync(v => v.VideoId == video.VideoId);

            if (existingVideo == null)
                return null;

            existingVideo.FileName = video.FileName;
            existingVideo.FilePath = video.FilePath;
            existingVideo.FileSize = video.FileSize;
            existingVideo.UserId = video.UserId;

            await _context.SaveChangesAsync();

            return existingVideo;
        }

        public async Task<VideoEntity?> UpdateFileNameAsync(Guid videoId, string newFileName)
        {
            var video = await _context.Videos
                .FirstOrDefaultAsync(v => v.VideoId == videoId);

            if (video == null)
                return null;

            video.FileName = newFileName;
            await _context.SaveChangesAsync();

            return video;
        }

        public async Task<bool> DeleteAsync(Guid videoId)
        {
            var video = await _context.Videos
                .FirstOrDefaultAsync(v => v.VideoId == videoId);

            if (video == null)
                return false;

            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> DeleteByUserIdAsync(Guid userId)
        {
            var userVideos = await _context.Videos
                .Where(v => v.UserId == userId)
                .ToListAsync();

            _context.Videos.RemoveRange(userVideos);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid videoId)
        {
            return await _context.Videos
                .AnyAsync(v => v.VideoId == videoId);
        }

        public async Task<(List<VideoEntity> Videos, int TotalCount)> GetPaginatedAsync(int page, int pageSize)
        {
            var query = _context.Videos.OrderByDescending(v => v.UploadedAt);

            var totalCount = await query.CountAsync();
            var videos = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (videos, totalCount);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Videos.CountAsync();
        }

        public async Task<int> GetCountByUserIdAsync(Guid userId)
        {
            return await _context.Videos
                .CountAsync(v => v.UserId == userId);
        }
    }
}