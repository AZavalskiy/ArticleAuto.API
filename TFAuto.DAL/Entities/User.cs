using TFAuto.DAL.Entities;

namespace TFAuto.TFAuto.DAL.Entities;

public class User : BaseEntity
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string RoleId { get; set; }

    public override string PartitionKey { get; set; } = nameof(User);
}
