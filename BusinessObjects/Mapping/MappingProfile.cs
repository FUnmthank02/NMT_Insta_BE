using AutoMapper.Execution;
using BusinessObjects.DTOs;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Mapping
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserCreateDTO>().ReverseMap();
            CreateMap<User, UserUpdateDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            
            CreateMap<Follower, FollowerCreateDTO>().ReverseMap();
            CreateMap<Follower, FollowerDTO>().ReverseMap();

            CreateMap<Post, PostDTO>().ReverseMap();
            CreateMap<Post, PostCreateDTO>().ReverseMap();
            CreateMap<Post, PostUpdateDTO>().ReverseMap();

            CreateMap<Notification, NotifyDTO>().ReverseMap();
            CreateMap<Notification, NotifyCreateDTO>().ReverseMap();
            CreateMap<Notification, NotifyUpdateDTO>().ReverseMap();

            CreateMap<Comment, CommentDTO>().ReverseMap();
            CreateMap<Comment, CommentCreateDTO>().ReverseMap();
            CreateMap<Comment, CommentUpdateDTO>().ReverseMap();

            CreateMap<Reaction, ReactionDTO>().ReverseMap();
            CreateMap<Reaction, ReactionCreateDTO>().ReverseMap();

            CreateMap<Medium, MediumDTO>().ReverseMap();
            CreateMap<Medium, MediumCreateDTO>().ReverseMap();

        }
    }
}
