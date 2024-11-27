using SRMMS.DTOs;

public class ProductDetailResponseDTO
{
    public ProductDetailDTO ProductDetail { get; set; }
    public List<ListProductDTO> RelatedProducts { get; set; }
}