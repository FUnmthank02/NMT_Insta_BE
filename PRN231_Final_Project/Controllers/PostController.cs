using AutoMapper;
using BusinessObjects.DTOs;
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using System.Net;
using Microsoft.AspNetCore.Builder.Extensions;
using FirebaseAdmin;
using Firebase.Storage;
using Google.Apis.Auth.OAuth2;

namespace PRN231_Final_Project.Controllers
{
    [Route("api/post")]
    [Authorize]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _repository;
        private readonly IMediaRepository _mediaRepo;
        private readonly ICommentRepository _commentRepo;
        private readonly IReactionRepository _reactionRepo;
        private readonly INotifyRepository _notifyRepo;
        private readonly IMapper _mapper;
        private readonly FirebaseApp _app;

        public PostController(IPostRepository repository, IMapper mapper, IMediaRepository mediaRepo, ICommentRepository commentRepo, IReactionRepository reactionRepo, INotifyRepository notifyRepo, FirebaseApp app)
        {
            _repository = repository;
            _mapper = mapper;
            _mediaRepo = mediaRepo;
            _commentRepo = commentRepo;
            _reactionRepo = reactionRepo;
            _notifyRepo = notifyRepo;
            _app = app;
        }

        [HttpGet("my-post")]
        public CustomResponse GetPostsByUserId(int userId)
        {
            try
            {
                List<Post> listPosts = _repository.GetPostsByUserId(userId);
                List<PostDTO> listPostDto = _mapper.Map<List<PostDTO>>(listPosts);

                return new CustomResponse
                {
                    Data = listPostDto,
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }
        
        [HttpGet("following-post")]
        public CustomResponse GetPostsFollowing(int userId)
        {
            try
            {
                List<Post> listPosts = _repository.GetPostsFollowing(userId);
                List<PostDTO> listPostDto = _mapper.Map<List<PostDTO>>(listPosts);

                return new CustomResponse
                {
                    Data = listPostDto,
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        [HttpPost]
        public CustomResponse CreatePost(PostCreateDTO pDto)
        {
            try
            {
                var localCreatedAt = TimeZoneInfo.ConvertTimeFromUtc(pDto.CreatedAt.Value, TimeZoneInfo.Local);
                pDto.CreatedAt = localCreatedAt;
                var post = _mapper.Map<Post>(pDto);
                if (_repository.CreatePost(post) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = {},
                        Message = "Create new post fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                // get the most recent post ( to get postId and return postId)
                Post newestPost = _repository.GetMostRecentPost();
                int postId = newestPost.PostId;

                return new CustomResponse
                {
                    Data = new { postId },
                    Message = "Create new post successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Success = false
                };
            }
        }

        [HttpPost("upload-media/{postId}")]
        [Consumes("multipart/form-data")]
        public async Task<CustomResponse> UploadFile([FromRoute] int postId, List<IFormFile> files)
        {
            try
            {
                if (files.Count != 0)
                {
                    List<string> mediaUrls = new List<string>();
                    List<MediumCreateDTO> listMedium = new List<MediumCreateDTO>();
                    foreach (var file in files)
                    {

                        if (file == null || file.Length == 0)
                        {
                            return new CustomResponse
                            {
                                Data = { },
                                Message = "No file uploaded",
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                Success = false
                            };
                        }

                        string fileType = Helper.Helper.GetFileType(file); // Validate and get type of file
                        if (fileType == "other")
                        {
                            return new CustomResponse
                            {
                                Data = { },
                                Message = "Only image or video is accepted",
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                Success = false
                            };
                        }

                        if (file.Length > Helper.Helper.MaxFileSize(fileType)) // Validate file size
                        {
                            return new CustomResponse
                            {
                                Data = { },
                                Message = "File size is too large, image's max size is 5MB, video's max size is 50MB",
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                Success = false
                            };
                        }

                        // Initialize Firebase Storage
                        var storage = new FirebaseStorage("nmt-insta.appspot.com");

                        // Generate a unique file name (e.g., using a GUID)
                        string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

                        // Upload the file to Firebase Storage
                        var path = $"media/{uniqueFileName}";
                        using (var stream = new MemoryStream())
                        {
                            await file.CopyToAsync(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            await storage.Child(path).PutAsync(stream);
                        }

                        // Return the URL of the uploaded file in the response
                        var mediaUrl = await storage.Child(path).GetDownloadUrlAsync();

                        // add to list mediaUrls
                        mediaUrls.Add(mediaUrl);

                        listMedium.Add(new MediumCreateDTO
                        {
                            PostId = postId,
                            MediaUrl = mediaUrl,
                            MediaType = fileType
                        });
                    }
                    if (HandleSaveMedia(listMedium) == 0) // save media to db
                    {
                        return new CustomResponse
                        {
                            Data = { },
                            Message = "Save to media fail",
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            Success = false
                        };
                    }
                    return new CustomResponse
                    {
                        Data = new { mediaUrls },
                        Message = $"Upload {(mediaUrls.Count() > 1 ? "files" : "file")} successfully",
                        StatusCode = (int)HttpStatusCode.OK,
                        Success = true
                    };
                }
                return new CustomResponse
                {
                    Data = { },
                    Message = "List files is empty",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }
        // handle save media to db
        private int HandleSaveMedia(List<MediumCreateDTO> listMedium)
        {
            int successNumber = 1;
            foreach(var mDto in listMedium)
            {
                var media = _mapper.Map<Medium>(mDto);
                if (_mediaRepo.CreateMedia(media) <= 0)
                    successNumber = 0;
            }
            if (successNumber == 0)
                return 0;
            return successNumber;
        }

        [HttpPut("{id}")]
        public CustomResponse UpdatePost(int id, PostUpdateDTO pDto)
        {
            try
            {
                var post = _repository.GetPostById(id);
                if (post == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Post does not exist",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }
                var p = _mapper.Map<Post>(pDto);
                if (_repository.UpdatePost(p) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Update post fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                return new CustomResponse
                {
                    Data = { },
                    Message = "Update post successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };

            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        [HttpDelete("{id}")]
        public CustomResponse DeletePost(int id)
        {
            try
            {
                var post = _repository.GetPostById(id);
                if (post == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Post does not exist",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                if (_repository.DeletePost(post) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Delete post fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                return new CustomResponse
                {
                    Data = { },
                    Message = "Delete post successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };

            }
            catch (Exception ex)
            {
                return new CustomResponse
                {
                    Data = { },
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }
    }
}
