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
    [Route("api/notification")]
    [Authorize]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly INotifyRepository _repository;
        private readonly IMapper _mapper;

        public NotifyController(IMapper mapper, INotifyRepository repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public CustomResponse GetNotificationsByUserId([FromRoute]int id)
        {
            try
            {
                List<Notification> listNotifications = _repository.GetNotificationsByUserId(id);
                List<NotifyDTO> listNotificationsDto = _mapper.Map<List<NotifyDTO>>(listNotifications);
                return new CustomResponse
                {
                    Data = listNotificationsDto,
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
        public CustomResponse CreateNotification(NotifyCreateDTO pDto)
        {
            try
            {
                var localCreatedAt = TimeZoneInfo.ConvertTimeFromUtc(pDto.CreatedAt.Value, TimeZoneInfo.Local);
                pDto.CreatedAt = localCreatedAt;
                var comment = _mapper.Map<Notification>(pDto);
                if (_repository.CreateNotification(comment) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Create new notification fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                return new CustomResponse
                {
                    Data = { },
                    Message = "Create new notification successfully",
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

        [HttpPut("{id}")]
        public CustomResponse UpdateNotification([FromRoute]int id, NotifyUpdateDTO pDto)
        {
            try
            {
                var notification = _repository.GetSingleNotification(id);
                if (notification == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Notification does not exist",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }
                var p = _mapper.Map<Notification>(pDto);
                if (_repository.UpdateNotification(p) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Update notification fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                return new CustomResponse
                {
                    Data = { },
                    Message = "Update notification successfully",
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
        
        [HttpPut("update-all/{userId}")]
        public CustomResponse UpdateAllNotificationByUserId(int userId)
        {
            try
            {
                
                if (_repository.UpdateAllNotification(userId) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Update all notification fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                return new CustomResponse
                {
                    Data = { },
                    Message = "Update all notification successfully",
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
        public CustomResponse DeleteNotification(int id)
        {
            try
            {
                var notification = _repository.GetSingleNotification(id);
                if (notification == null)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Notification does not exist",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }
                
                if (_repository.DeleteNotification(notification) <= 0)
                {
                    return new CustomResponse
                    {
                        Data = { },
                        Message = "Delete notification fail",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                return new CustomResponse
                {
                    Data = { },
                    Message = "Delete notification successfully",
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
