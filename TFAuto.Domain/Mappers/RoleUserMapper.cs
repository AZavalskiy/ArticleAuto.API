using AutoMapper;
using TFAuto.DAL.Entities;
using TFAuto.Domain.Repository.Roles.DTO;

namespace TFAuto.Domain;

public class RoleUserMapper : Profile
{
    public RoleUserMapper()
    {

        CreateMap<RoleListDTO, Role>();
        CreateMap<Role, RoleListDTO>();

        CreateMap<RoleCreateDTO, Role>();
        CreateMap<Role, RoleCreateDTO>();

        CreateMap<RoleUpdateDTO, Role>();
        CreateMap<Role, RoleUpdateDTO>();

    }
}