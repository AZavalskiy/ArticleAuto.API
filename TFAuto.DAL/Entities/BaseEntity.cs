using Microsoft.Azure.CosmosRepository;

namespace TFAuto.DAL.Entities;

public class BaseEntity : FullItem
{
    public virtual string PartitionKey { get; set; }
    protected override string GetPartitionKeyValue() => PartitionKey;
}
