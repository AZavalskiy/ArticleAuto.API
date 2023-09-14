using AutoMapper;
using TFAuto.DAL.Entities;
using TFAuto.Domain.Services.CommentService.DTO;

namespace TFAuto.Domain.Mappers
{
    public class CommentMapper : Profile
    {
        public CommentMapper()
        {
            CreateMap<CreateCommentRequest, Comment>();
            CreateMap<Comment, CreateCommentResponse>();

            CreateMap<UpdateCommentRequest, Comment>();
            CreateMap<Comment, UpdateCommentResponse>();

            CreateMap<Comment, GetCommentResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ArticleId, opt => opt.MapFrom(src => src.ArticleId))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.LikesCount));
        }
    }
}