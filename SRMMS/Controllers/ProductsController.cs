using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMMS.DTOs;
using SRMMS.Models;

namespace SRMMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly SRMMSContext _context;

        public ProductController(SRMMSContext context)
        {
            _context = context;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductGetDTO>>> GetProducts()
        {
            return await _context.Products
                .Include(p => p.Cat)
                .Select(p => new ProductGetDTO
                {
                    ProId = p.ProId,
                    ProName = p.ProName,
                    ProDescription = p.ProDiscription,
                    ProPrice = p.ProPrice,
                    CatName = p.Cat.CatName
                })
                .ToListAsync();
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductGetDTO>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Cat)
                .Where(p => p.ProId == id)
                .Select(p => new ProductGetDTO
                {
                    ProId = p.ProId,
                    ProName = p.ProName,
                    ProDescription = p.ProDiscription,
                    ProPrice = p.ProPrice,
                    CatName = p.Cat.CatName
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // POST: api/Product
        [HttpPost]
        public async Task<ActionResult<ProductGetDTO>> PostProduct(ProductCreateDTO productDTO)
        {
            // Kiểm tra xem Category có tồn tại không
            var categoryExists = await _context.Categories.AnyAsync(c => c.CatId == productDTO.CatId);
            if (!categoryExists)
            {
                return BadRequest($"Category with ID {productDTO.CatId} does not exist.");
            }

            // Tạo đối tượng Product từ ProductCreateDTO
            var product = new Product
            {
                ProName = productDTO.ProName,
                ProDiscription = productDTO.ProDiscription,
                ProWarning = productDTO.ProWarning,
                ProPrice = productDTO.ProPrice,
                CatId = productDTO.CatId,
                ProImg = productDTO.ProImg,
                ProCalories = productDTO.ProCalories,
                ProCookingTime = productDTO.ProCookingTime,
                ProStatus = productDTO.ProStatus
            };

            // Thêm sản phẩm vào database
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Tạo ProductGetDTO từ đối tượng product đã được lưu
            var createdProductDTO = new ProductGetDTO
            {
                ProId = product.ProId,
                ProName = product.ProName,
                ProDescription = product.ProDiscription,
                ProPrice = product.ProPrice,
                CatName = (await _context.Categories.FindAsync(product.CatId))!.CatName,
                ProWarning = product.ProWarning,
                ProImg = product.ProImg,
                ProCalories = product.ProCalories,
                ProCookingTime = product.ProCookingTime,
                ProStatus = product.ProStatus
            };

            // Trả về kết quả CreatedAtAction với dữ liệu sản phẩm vừa được tạo
            return CreatedAtAction(nameof(GetProduct), new { id = product.ProId }, createdProductDTO);
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductUpdateDTO productDTO)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Kiểm tra xem Category có tồn tại không
            var categoryExists = await _context.Categories.AnyAsync(c => c.CatId == productDTO.CatId);
            if (!categoryExists)
            {
                return BadRequest($"Category with ID {productDTO.CatId} does not exist.");
            }

            // Cập nhật các thuộc tính của sản phẩm
            product.ProName = productDTO.ProName;
            product.ProDiscription = productDTO.ProDiscription;
            product.ProWarning = productDTO.ProWarning;
            product.ProPrice = productDTO.ProPrice;
            product.CatId = productDTO.CatId; // Cập nhật CatId
            product.ProImg = productDTO.ProImg;
            product.ProCalories = productDTO.ProCalories;
            product.ProCookingTime = productDTO.ProCookingTime;
            product.ProStatus = productDTO.ProStatus;

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // GET: api/Product/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductGetDTO>>> SearchProducts([FromQuery] ProductSearchDTO searchDTO)
        {
            var query = _context.Products.AsQueryable();

            // Tìm kiếm theo tên sản phẩm
            if (!string.IsNullOrEmpty(searchDTO.Name))
            {
                query = query.Where(p => p.ProName.Contains(searchDTO.Name));
            }

            // Lọc theo giá
            if (searchDTO.MinPrice.HasValue)
            {
                query = query.Where(p => p.ProPrice >= searchDTO.MinPrice.Value);
            }

            if (searchDTO.MaxPrice.HasValue)
            {
                query = query.Where(p => p.ProPrice <= searchDTO.MaxPrice.Value);
            }

            // Lọc theo danh mục
            if (searchDTO.CategoryId.HasValue)
            {
                query = query.Where(p => p.CatId == searchDTO.CategoryId.Value);
            }

            // Thực hiện truy vấn và chuyển đổi kết quả thành ProductGetDTO
            var products = await query
                .Include(p => p.Cat)
                .Select(p => new ProductGetDTO
                {
                    ProId = p.ProId,
                    ProName = p.ProName,
                    ProDescription = p.ProDiscription,
                    ProPrice = p.ProPrice,
                    CatName = p.Cat.CatName
                })
                .ToListAsync();

            return Ok(products);
        }



        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProId == id);
        }
    }
}
