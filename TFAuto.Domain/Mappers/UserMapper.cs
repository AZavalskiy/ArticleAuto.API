﻿using AutoMapper;
using TFAuto.TFAuto.DAL.Entities;

namespace TFAuto.Domain;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserRegistrationRequestModel, User>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom(scr => BCrypt.Net.BCrypt.HashPassword(scr.Password)));
        CreateMap<User, UserRegistrationResponseModel>();
    }
}