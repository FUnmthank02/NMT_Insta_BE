using AutoMapper;
using BusinessObjects.DTOs;
using BusinessObjects.Models;
using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using System.Net;

namespace PRN231_Final_Project.Controllers
{
    [Route("api/comment")]
    [Authorize]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _repository;
        private readonly IMapper _mapper;

        public CommentController(IMapper mapper, ICommentRepository repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpPost]
        public CustomResponse CreateComment(CommentCreateDTO pDto)
        {
            try
            {
                var localCreatedAt = TimeZoneInfo.ConvertTimeFromUtc(pDto.CreatedAt.Value, TimeZoneInfo.Local);
                pDto.CreatedAt = localCreatedAt;
                var comment = _mapper.Map<Comment>(pDto);
                if (_repository.CreateComment(comment) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Create new comment fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                return new CustomResponse
                {
                    Data = { },
                    Message = "Create new comment successfully",
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

        [HttpPut("{id}")]
        public CustomResponse UpdateComment(int id, CommentUpdateDTO pDto)
        {
            try
            {
                var post = _repository.GetCommentById(id);
                if (post == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Comment does not exist",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }
                var p = _mapper.Map<Comment>(pDto);
                if (_repository.UpdateComment(p) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Update comment fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                return new CustomResponse
                {
                    Data = { },
                    Message = "Update comment successfully",
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
        public CustomResponse DeleteComment(int id)
        {
            try
            {
                var comment = _repository.GetCommentById(id);
                if (comment == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Comment does not exist",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }
                
                if (_repository.DeleteComment(comment) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Delete comment fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                return new CustomResponse
                {
                    Data = { },
                    Message = "Delete comment successfully",
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
