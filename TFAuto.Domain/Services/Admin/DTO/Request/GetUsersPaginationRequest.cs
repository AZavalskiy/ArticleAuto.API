using System.ComponentModel;

namespace TFAuto.Domain.Services.Admin.DTO.Request
{
    public class GetUsersPaginationRequest
    {
        [DefaultValue(0)]
        public int Skip { get; set; }

        [DefaultValue(25)]
        public int Take { get; set; }

        public SortOrderUsers SortBy { get; set; } = SortOrderUsers.UserNameAscending;
    }
}