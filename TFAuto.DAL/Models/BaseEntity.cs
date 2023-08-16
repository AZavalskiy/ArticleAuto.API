using Microsoft.Azure.CosmosRepository;

namespace TFAuto.DAL;

public class BaseEntity : FullItem
{
    public virtual string PartitionKey { get; set; }
    protected override string GetPartitionKeyValue() => PartitionKey;
}
