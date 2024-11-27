using SRMMS.DTOs;

public class ProductFilterResultDTO
{
    public int TotalProducts { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public List<ListProductDTO> Products { get; set; }
}
