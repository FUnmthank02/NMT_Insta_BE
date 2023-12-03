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
    [Route("api/reaction")]
    [Authorize]
    [ApiController]
    public class ReactionController : ControllerBase
    {
        private readonly IReactionRepository _repository;
        private readonly IMapper _mapper;

        public ReactionController(IMapper mapper, IReactionRepository repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpPost]
        public CustomResponse CreateReaction(ReactionCreateDTO pDto)
        {
            try
            {
                var localCreatedAt = TimeZoneInfo.ConvertTimeFromUtc(pDto.CreatedAt.Value, TimeZoneInfo.Local);
                pDto.CreatedAt = localCreatedAt;
                var reaction = _mapper.Map<Reaction>(pDto);
                if (_repository.CreateReaction(reaction) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Create new reaction fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                return new CustomResponse
                {
                    Data = { },
                    Message = "Create new reaction successfully",
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

        [HttpDelete]
        public CustomResponse DeleteReaction(int postId, int userId)
        {
            try
            {
                var reaction = _repository.GetReactionByPostIdUserId(postId, userId);
                if (reaction == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Reaction does not exist",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }
                
                if (_repository.DeleteReaction(reaction) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Delete reaction fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                return new CustomResponse
                {
                    Data = { },
                    Message = "Delete reaction successfully",
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
