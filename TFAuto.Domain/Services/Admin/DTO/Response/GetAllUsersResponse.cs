namespace TFAuto.Domain.Services.Admin.DTO.Response
{
    public class GetAllUsersResponse : BasePagination
    {
        public List<GetUserResponse> Users { get; set; } = new();

        public SortOrderUsers OrderBy { get; set; }
    }
}