namespace TFAuto.DAL.Entities.Article;

public class Tag : BaseEntity
{
    public string Name { get; set; }
    public List<string> ArticleIds { get; set; } = new List<string>();
    public override string PartitionKey { get; set; } = nameof(Tag);
}

