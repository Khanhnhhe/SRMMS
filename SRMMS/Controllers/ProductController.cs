    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Http;
    using SRMMS.DTOs;
    using SRMMS.Models;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

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

       [HttpGet("getAllProducts")]
        public async Task<IActionResult> GetAllProducts(int pageNumber = 1, int pageSize = 10)
        {
            var totalProducts = await _context.Products.CountAsync(); 

            var products = await _context.Products
                .Include(p => p.Cat)
                .Skip((pageNumber - 1) * pageSize) 
                .Take(pageSize) 
                .Select(p => new ListProductDTO
                {
                    ProId = p.ProId,
                    ProName = p.ProName,
                    ProDiscription = p.ProDiscription,
                    ProWarning = p.ProWarning,
                    ProPrice = p.ProPrice,
                    CatName = p.Cat.CatName,
                    ProImg = p.ProImg,
                    ProCalories = p.ProCalories,
                    ProCookingTime = p.ProCookingTime.ToString(),
                    ProStatus = p.ProStatus
                })
                .ToListAsync();

            if (products == null || products.Count == 0)
            {
                return NotFound("No products found.");
            }

            
            var result = new
            {
                TotalProducts = totalProducts,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Products = products
            };

            return Ok(result);
        }



        
        [HttpPost("addNewProduct")]
        public async Task<ActionResult<addProductDTO>> AddProduct(addProductDTO productDto)
        {
            
            var categoryExists = await _context.Categories.AnyAsync(c => c.CatId == productDto.Category);
            if (!categoryExists)
            {
                return BadRequest("Category does not exist.");
            }

           
            var productExists = await _context.Products.AnyAsync(p => p.ProName == productDto.ProductName);
            if (productExists)
            {
                return BadRequest("A product with this name already exists.");
            }

          
            var newProduct = new Product
            {
                ProName = productDto.ProductName,
                ProDiscription = productDto.Description,
                ProWarning = productDto.Warning,
                ProPrice = productDto.Price,  
                CatId = productDto.Category, 
                ProImg = productDto.Image,
                ProCalories = productDto.Calories,
                ProCookingTime = productDto.CookingTime,
                ProStatus = productDto.Status  
            };

           
            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

           
            var productResult = new addProductDTO
            {
                ProductName = newProduct.ProName,
                Description = newProduct.ProDiscription,
                Warning = newProduct.ProWarning,
                Price = newProduct.ProPrice,
                Category = newProduct.CatId,
                Image = newProduct.ProImg,
                Calories = newProduct.ProCalories,
                CookingTime = newProduct.ProCookingTime,
                Status = newProduct.ProStatus
            };

           
            return CreatedAtAction(nameof(GetProductById), new { proId = newProduct.ProId }, productResult);
        }

        [HttpGet("getProductById/{proId}")]
        public async Task<ActionResult<ListProductDTO>> GetProductById(int proId)
        {
            var product = await _context.Products
        .Include(c => c.Cat)
        .Where(p => p.ProId == proId) 
        .Select(p => new ListProductDTO
        {
            ProId = p.ProId,
            ProName = p.ProName,
            ProDiscription = p.ProDiscription,
            ProWarning = p.ProWarning,
            ProPrice = p.ProPrice,
            CatName = p.Cat.CatName,
            ProImg = p.ProImg,
            ProCalories = p.ProCalories,
            ProCookingTime = p.ProCookingTime,
            ProStatus = p.ProStatus
        })
        .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);

        }

        [HttpPut("updateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id,updateProduct updateProductDto)
        {
           
            var existingProduct = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(p => p.ProId == id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            
            if (!string.IsNullOrEmpty(updateProductDto.ProductName))
            {
                var productWithSameName = await _context.Products
                    .Where(p => p.ProName == updateProductDto.ProductName && p.ProId != id) 
                    .FirstOrDefaultAsync();

                if (productWithSameName != null)
                {
                    return BadRequest("Product name already exists.");
                }

                existingProduct.ProName = updateProductDto.ProductName;
            }

            
            if (!string.IsNullOrEmpty(updateProductDto.Description))
            {
                existingProduct.ProDiscription = updateProductDto.Description;
            }

            if (!string.IsNullOrEmpty(updateProductDto.Warning))
            {
                existingProduct.ProWarning = updateProductDto.Warning;
            }

            if (updateProductDto.Price.HasValue)
            {
                existingProduct.ProPrice = updateProductDto.Price.Value;
            }

            if (!string.IsNullOrEmpty(updateProductDto.Image))
            {
                existingProduct.ProImg = updateProductDto.Image;
            }

            if (!string.IsNullOrEmpty(updateProductDto.Calories))
            {
                existingProduct.ProCalories = updateProductDto.Calories;
            }

            if (!string.IsNullOrEmpty(updateProductDto.CookingTime))
            {
                existingProduct.ProCookingTime = updateProductDto.CookingTime;
            }

            if (updateProductDto.Status.HasValue)
            {
                existingProduct.ProStatus = updateProductDto.Status.Value;
            }

            
            if (updateProductDto.Category.HasValue)
            {
                var category = await _context.Categories.FindAsync(updateProductDto.Category.Value);
                if (category != null)
                {
                    existingProduct.CatId = category.CatId;
                    existingProduct.Cat = category; 
                }
                else
                {
                    return BadRequest("Invalid category provided.");
                }
            }

            
            _context.Entry(existingProduct).State = EntityState.Modified;

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
        private bool ProductExists(int id)
        {
            return _context.Products.Any(p => p.ProId == id);
        }

        [HttpGet("filterByCategoryName/{categoryName}")]
        public async Task<IActionResult> FilterByCategoryName(string categoryName, int pageNumber = 1, int pageSize = 10)
        {
            
            var categories = await _context.Categories.ToListAsync();

            
            var category = categories
                .FirstOrDefault(c => c.CatName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            var totalProducts = await _context.Products.CountAsync(p => p.CatId == category.CatId);

            var products = await _context.Products
                .Include(p => p.Cat)
                .Where(p => p.CatId == category.CatId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ListProductDTO
                {
                    ProId = p.ProId,
                    ProName = p.ProName,
                    ProDiscription = p.ProDiscription,
                    ProWarning = p.ProWarning,
                    ProPrice = p.ProPrice,
                    CatName = p.Cat.CatName,
                    ProImg = p.ProImg,
                    ProCalories = p.ProCalories,
                    ProCookingTime = p.ProCookingTime.ToString(),
                    ProStatus = p.ProStatus
                })
                .ToListAsync();

            if (products == null || products.Count == 0)
            {
                return NotFound("No products found in this category.");
            }

            var result = new
            {
                TotalProducts = totalProducts,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Products = products
            };

            return Ok(result);
        }


        [HttpGet("/api/searchProductByProductName")]
        public async Task<ActionResult<IEnumerable<ListProductDTO>>> SearchByProductName(string? productName = "", int pageNumber = 1, int pageSize = 10)
        {
            
            var skip = (pageNumber - 1) * pageSize;

            
            var query = _context.Products.Include(p => p.Cat).AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(productName))
            {
                query = query.Where(p => p.ProName.Contains(productName));
            }

            
            var products = await query
                                 .Skip(skip)
                                 .Take(pageSize)
                                 .Select(p => new ListProductDTO
                                 {
                                     ProName = p.ProName,
                                     ProDiscription = p.ProDiscription,
                                     ProPrice = p.ProPrice,
                                     ProCalories = p.ProCalories,
                                     ProCookingTime = p.ProCookingTime,
                                     ProStatus = p.ProStatus,
                                     CatName = p.Cat.CatName
                                 }).ToListAsync();

            
            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }

            
            return Ok(products);
        }

        [HttpDelete("deleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            
            var product = await _context.Products.FindAsync(id);

            
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            
            _context.Products.Remove(product);

            
            await _context.SaveChangesAsync();

            
            return NoContent();
        }
    }
}
