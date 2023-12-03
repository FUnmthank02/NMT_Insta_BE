using AutoMapper;
using BusinessObjects.DTOs;
using BusinessObjects.Models;
using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using System.Net;
using Repositories.Repositories;

namespace PRN231_Final_Project.Controllers
{
    [Route("api/follow")]
    [Authorize]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly IFollowRepository _repository;
        private readonly IMapper _mapper;

        public FollowController(IMapper mapper, IFollowRepository repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("get-all-followers/{userId}")]
        public CustomResponse GetAllFollowerByUserId([FromRoute]int userId)
        {
            try
            {
                List<Follower> listFollowers = _repository.GetAllFollowerByUserId(userId);
                List<FollowerDTO> listFollowersDto = _mapper.Map<List<FollowerDTO>>(listFollowers);

                return new CustomResponse
                {
                    Data = listFollowersDto,
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

        [HttpGet("get-all-followings/{userId}")]
        public CustomResponse GetAllFollowingByUserId([FromRoute]int userId)
        {
            try
            {
                List<Follower> listFollowings = _repository.GetAllFollowingByUserId(userId);
                List<FollowerDTO> listFollowingsDto = _mapper.Map<List<FollowerDTO>>(listFollowings);

                return new CustomResponse
                {
                    Data = listFollowingsDto,
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
        public CustomResponse CreateFollow(FollowerCreateDTO pDto)
        {
            try
            {
                var localCreatedAt = TimeZoneInfo.ConvertTimeFromUtc(pDto.CreatedAt.Value, TimeZoneInfo.Local);
                pDto.CreatedAt = localCreatedAt;
                var follow = _mapper.Map<Follower>(pDto);
                if (_repository.CreateFollow(follow) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Create new follow fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                return new CustomResponse
                {
                    Data = { },
                    Message = "Create new follow successfully",
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
        public CustomResponse DeleteFollow(int userId, int followingId)
        {
            try
            {
                var follow = _repository.GetSingleFollowing(userId, followingId);
                if (follow == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Follow does not exist",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }
                
                if (_repository.DeleteFollow(follow) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Delete follow fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                return new CustomResponse
                {
                    Data = { },
                    Message = "Delete follow successfully",
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
