using AutoMapper;
using TFAuto.DAL.Entities;
using TFAuto.Domain.Repository.Roles.DTO;

namespace TFAuto.Domain;

public class RoleUserMapper : Profile
{
    public RoleUserMapper()
    {
        CreateMap<Role, RoleListResponse>();

        CreateMap<RoleCreateRequest, Role>();
        CreateMap<Role, RoleCreateResponse>();

        CreateMap<RoleUpdateRequest, Role>();
        CreateMap<Role, RoleUpdateResponse>();
    }
}