namespace SRMMS.DTOs
{
    public class ProductSearchDTO
    {
        public string? Name { get; set; } // Tên sản phẩm
        public decimal? MinPrice { get; set; } // Giá tối thiểu
        public decimal? MaxPrice { get; set; } // Giá tối đa
        public int? CategoryId { get; set; } // Danh mục
    }
}
