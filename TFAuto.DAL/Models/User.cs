using Microsoft.Azure.CosmosRepository.Attributes;
using Microsoft.Azure.CosmosRepository;

namespace TFAuto.DAL;
[PartitionKeyPath("/partitionKey")]
public class User : BaseEntity
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public override string PartitionKey { get; set; } = nameof(User);
}
