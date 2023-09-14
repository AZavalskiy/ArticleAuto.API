namespace TFAuto.Domain.Services;

public class PagingBase
{
    public int TotalItems { get; set; }

    public int Take { get; set; }

    public int Skip { get; set; }
}
