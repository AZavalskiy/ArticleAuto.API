using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFAuto.DAL;
using BCrypt.Net;

namespace TFAuto.Domain;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserRegistrationRequest, User>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom(scr => BCrypt.Net.BCrypt.HashPassword(scr.Password)));
        CreateMap<User, UserRegistrationResponse>();
    }
}