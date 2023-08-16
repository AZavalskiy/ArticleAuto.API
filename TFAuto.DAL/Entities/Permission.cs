namespace TFAuto.DAL.Entities
{
    public class Permission : BaseEntity
    {
        public string PermissionName { get; set; }

        public string RoleId { get; set; }

        public override string PartitionKey { get; set; } = nameof(Permission);

    }
}
