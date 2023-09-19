namespace TFAuto.Domain.Services;

public class BasePagination
{
    public int TotalItems { get; set; }

    public int Take { get; set; }

    public int Skip { get; set; }
}
