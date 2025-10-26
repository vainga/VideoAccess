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
            if (string.IsNullOrEmpty(request.FileName) || string.IsNullOrEmpty(request.FilePath))
            {
                return new UploadVideoResponse
                {
                    Error = new ErrorMessage
                    {
                        Code = "invalid_request",
                        Message = "FileName or FilePath is missing."
                    }
                };
            }

            try
            {
                var video = new VideoEntity
                {
                    VideoId = Guid.Parse(request.VideoId),
                    FileName = request.FileName,
                    FilePath = request.FilePath,
                    FileSize = request.FileSize,
                    UserId = Guid.TryParse(request.UserId, out var uid) ? uid : null,
                    ProcessingStatus = (VideoProcessingStatus)request.Status
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

        public override async Task<UpdateVideoResponse> UpdateVideo(
            UpdateVideoRequest request,
            ServerCallContext context)
        {
            if (!Guid.TryParse(context.GetHttpContext()?.Request.RouteValues["video_id"]?.ToString(), out var videoId))
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

            try
            {
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
            catch (Exception ex)
            {
                return new UpdateVideoResponse
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

            try
            {
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
                    StorageUrl = video.FilePath,
                    Status = (Contracts.VideoProcessingStatus)video.ProcessingStatus
                };
            }
            catch (Exception ex)
            {
                return new GetVideoResponse
                {
                    Error = new ErrorMessage
                    {
                        Code = "internal_error",
                        Message = ex.Message
                    }
                };
            }
        }

        public override async Task<ListVideosResponse> ListVideos(
            ListVideosRequest request,
            ServerCallContext context)
        {
            try
            {
                var query = _dbContext.Videos.AsQueryable();

                if (!string.IsNullOrEmpty(request.UserId) && Guid.TryParse(request.UserId, out var userId))
                    query = query.Where(v => v.UserId == userId);

                var videoIds = await query.Select(v => v.VideoId.ToString()).ToListAsync();

                return new ListVideosResponse
                {
                    Videos = new VideoIdsMessage { VideoIds = { videoIds } }
                };
            }
            catch (Exception ex)
            {
                return new ListVideosResponse
                {
                    Error = new ErrorMessage
                    {
                        Code = "internal_error",
                        Message = ex.Message
                    }
                };
            }
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

                _dbContext.Videos.Remove(video);
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
    }
}
