using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using VideoAccess.Contracts;
using VideoAccess.Data;
using VideoAccess.Models.Entities;

namespace VideoAccess.Services
{
    public class VideoAccessService : VideosAccessService.VideosAccessServiceBase
    {
        private readonly ApplicationDbContext _dbContext;

        public VideoAccessService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<UploadVideoResponse> UploadVideo(
            UploadVideoRequest request,
            ServerCallContext context)
        {
            var videoMsg = request.Video;
            if (videoMsg == null ||
                string.IsNullOrEmpty(videoMsg.FileName) ||
                string.IsNullOrEmpty(videoMsg.FilePath))
            {
                return new UploadVideoResponse
                {
                    Error = new ErrorMessage
                    {
                        Code = "invalid_request",
                        Message = "Video or FileName/FilePath is missing."
                    }
                };
            }

            try
            {
                var video = new VideoEntity
                {
                    VideoId = Guid.TryParse(videoMsg.VideoId, out var vid) ? vid : Guid.NewGuid(),
                    FileName = videoMsg.FileName,
                    FilePath = videoMsg.FilePath,
                    FileSize = videoMsg.FileSize,
                    UserId = Guid.TryParse(videoMsg.UserId, out var uid) ? uid : null,
                    ProcessingStatus = (VideoProcessingStatus)videoMsg.Status
                };

                _dbContext.Videos.Add(video);
                await _dbContext.SaveChangesAsync();

                return new UploadVideoResponse { Ok = true };
            }
            catch (Exception ex)
            {
                return new UploadVideoResponse
                {
                    Error = new ErrorMessage
                    {
                        Code = "internal_error",
                        Message = ex.Message
                    }
                };
            }
        }

        public override async Task<GetVideoResponse> GetVideo(
            GetVideoRequest request,
            ServerCallContext context)
        {
            if (!Guid.TryParse(request.VideoId, out var videoId))
            {
                return new GetVideoResponse
                {
                    Error = new ErrorMessage
                    {
                        Code = "invalid_request",
                        Message = "VideoId is missing or invalid."
                    }
                };
            }

            var video = await _dbContext.Videos.FirstOrDefaultAsync(v => v.VideoId == videoId);
            if (video == null)
                return new GetVideoResponse
                {
                    Error = new ErrorMessage
                    {
                        Code = "not_found",
                        Message = "Video not found."
                    }
                };

            return new GetVideoResponse
            {
                Video = new VideoMessage
                {
                    VideoId = video.VideoId.ToString(),
                    FileName = video.FileName,
                    FilePath = video.FilePath,
                    FileSize = video.FileSize,
                    UserId = video.UserId.ToString(),
                    Status = (Contracts.VideoProcessingStatus)video.ProcessingStatus
                }
            };
        }

        public override async Task<ListVideosResponse> ListVideos(
            ListVideosRequest request,
            ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
            {
                return new ListVideosResponse
                {
                    Error = new ErrorMessage
                    {
                        Code = "invalid_request",
                        Message = "UserId is missing or invalid."
                    }
                };
            }

            var videos = await _dbContext.Videos
                .Where(v => v.UserId == userId)
                .Select(v => v.VideoId.ToString())
                .ToListAsync();

            return new ListVideosResponse
            {
                Videos = new VideoIdsMessage { VideoIds = { videos } }
            };
        }

        public override async Task<DeleteVideoResponse> DeleteVideo(
            DeleteVideoRequest request,
            ServerCallContext context)
        {
            if (!Guid.TryParse(request.VideoId, out var videoId))
            {
                return new DeleteVideoResponse
                {
                    Error = new ErrorMessage
                    {
                        Code = "invalid_request",
                        Message = "VideoId is missing or invalid."
                    }
                };
            }

            try
            {
                var video = await _dbContext.Videos.FirstOrDefaultAsync(v => v.VideoId == videoId);
                if (video == null)
                    return new DeleteVideoResponse
                    {
                        Error = new ErrorMessage
                        {
                            Code = "not_found",
                            Message = "Video not found."
                        }
                    };

                video.ProcessingStatus = VideoProcessingStatus.Deleted;

                await _dbContext.SaveChangesAsync();

                return new DeleteVideoResponse { Ok = true };
            }
            catch (Exception ex)
            {
                return new DeleteVideoResponse
                {
                    Error = new ErrorMessage
                    {
                        Code = "internal_error",
                        Message = ex.Message
                    }
                };
            }
        }


        public override async Task<UpdateVideoResponse> UpdateVideo(
            UpdateVideoRequest request,
            ServerCallContext context)
        {
            if (!Guid.TryParse(request.VideoId, out var videoId))
            {
                return new UpdateVideoResponse
                {
                    Error = new ErrorMessage
                    {
                        Code = "invalid_request",
                        Message = "VideoId is missing or invalid."
                    }
                };
            }

            var video = await _dbContext.Videos.FirstOrDefaultAsync(v => v.VideoId == videoId);
            if (video == null)
                return new UpdateVideoResponse
                {
                    Error = new ErrorMessage
                    {
                        Code = "not_found",
                        Message = "Video not found."
                    }
                };

            video.ProcessingStatus = (VideoProcessingStatus)request.Status;
            if (!string.IsNullOrEmpty(request.ErrorMessage))
                video.ErrorMessage = request.ErrorMessage;

            await _dbContext.SaveChangesAsync();

            return new UpdateVideoResponse { Ok = true };
        }
    }
}
