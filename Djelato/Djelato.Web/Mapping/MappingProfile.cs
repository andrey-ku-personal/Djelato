using AutoMapper;
using Djelato.DataAccess.Entity;
using Djelato.Services.Models;
using Djelato.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Djelato.Web.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDTO, UserModel>()
                .ForMember(x => x.Name, x => x.MapFrom(src => src.Name))
                .ForMember(x => x.Email, x => x.MapFrom(src => src.Email.ToLower().Trim()))
                .ForMember(x => x.Role, x => x.MapFrom(src => src.Role))
                .ForMember(x => x.Password, x => x.MapFrom(src => src.Password))
                .ForMember(x => x.EmailConfirmed, x => x.MapFrom(src => src.EmailConfirmed));

            CreateMap<User, UserModel>()
                .ForMember(x => x.Name, x => x.MapFrom(src => src.Name))
                .ForMember(x => x.Email, x => x.MapFrom(src => src.Email))
                .ForMember(x => x.Role, x => x.MapFrom(src => src.Role));

            CreateMap<UserModel, User>()
               .ForMember(x => x.Name, x => x.MapFrom(src => src.Name))
               .ForMember(x => x.Email, x => x.MapFrom(src => src.Email))
               .ForMember(x => x.Role, x => x.MapFrom(src => src.Role));
        }
    }
}
