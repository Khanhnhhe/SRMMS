using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using SRMMS.DTOs;
using SRMMS.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Account = CloudinaryDotNet.Account;


namespace SRMMS.Controllers
{
    [Route("api/product")]
    [ApiController]


    public class ProductController : ControllerBase
    {
        private readonly SRMMSContext _context;

        private readonly Cloudinary _clouddinary;



        public ProductController(SRMMSContext context)
        {
            Account account = new Account("dt92oc9xc", "548166873787419", "-8KA1HUjyTe6J4aHq4DGPXflJiw");
            _context = context;
            _clouddinary = new Cloudinary(account);

        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllProducts(int pageNumber = 1, int pageSize = 10, string? name = null, int? categoryId = null, string? minPrice = null, string? maxPrice = null)
        {
            var totalProductsQuery = _context.Products.AsQueryable();


            name = name?.Trim();

            if (!string.IsNullOrEmpty(name))
            {
                totalProductsQuery = totalProductsQuery.Where(p => p.ProName.Trim().Contains(name));
            }

            if (categoryId.HasValue)
            {
                totalProductsQuery = totalProductsQuery.Where(p => p.CatId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(minPrice) && IsNumeric(minPrice.Trim(), out decimal parsedMinPrice))
            {
                totalProductsQuery = totalProductsQuery.Where(p => p.ProPrice >= parsedMinPrice);
            }
            else if (!string.IsNullOrEmpty(minPrice))
            {
                return BadRequest("Giá tối thiểu không hợp lệ. Vui lòng nhập giá trị số.");
            }

            if (!string.IsNullOrEmpty(maxPrice) && IsNumeric(maxPrice.Trim(), out decimal parsedMaxPrice))
            {
                totalProductsQuery = totalProductsQuery.Where(p => p.ProPrice <= parsedMaxPrice);
            }
            else if (!string.IsNullOrEmpty(maxPrice))
            {
                return BadRequest("maxPrice không hợp lệ. Vui lòng nhập giá trị số.");
            }


            var totalProducts = await totalProductsQuery.CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var products = await totalProductsQuery
                .Include(p => p.Cat)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ListProductDTO
                {
                    ProductId = p.ProId,
                    ProductName = p.ProName,
                    Description = p.ProDiscription,
                    Price = p.ProPrice,
                    Category = p.Cat.CatName,
                    Image = p.ProImg,
                    Calories = p.ProCalories,
                    Status = p.ProStatus
                })
                .ToListAsync();

            if (products == null || products.Count == 0)
            {
                return Ok(new
                {
                    TotalProducts = totalProducts,
                    TotalPages = totalPages,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Products = new List<ListProductDTO>()
                });
            }

            var result = new
            {
                TotalProducts = totalProducts,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Products = products
            };

            return Ok(result);
        }


        private bool IsNumeric(string str, out decimal number)
        {
            return decimal.TryParse(str, out number);
        }



        [HttpPost("create")]
        public async Task<ActionResult<addProductDTO>> AddProduct([FromForm] addProductDTO productDto)
        {

            productDto.ProductName = productDto.ProductName?.Trim();
            productDto.Description = productDto.Description?.Trim();
            productDto.Calories = productDto.Calories?.Trim();

            var categoryExists = await _context.Categories.AnyAsync(c => c.CatId == productDto.Category);
            if (!categoryExists)
            {
                return BadRequest("Không có danh mục nào tồn tại.");
            }

            var productExists = await _context.Products.AnyAsync(p => p.ProName == productDto.ProductName);
            if (productExists)
            {
                return BadRequest("Một sản phẩm có tên này đã tồn tại.");
            }

            if (productDto.Image == null || productDto.Image.Length == 0)
            {
                return BadRequest("Không tìm thấy hình ảnh");
            }
            if (productDto.Price < 0)
            {
                return BadRequest("Giá không thể âm.");
            }

            if (productDto.Category.HasValue && productDto.Category < 0)
            {
                return BadRequest("Không có danh mục nào tồn tại.");
            }
            if (productDto.Price == 0)
            {
                return BadRequest("Giá là bắt buộc và phải lớn hơn 0.");
            }

            if (string.IsNullOrWhiteSpace(productDto.ProductName) &&
               string.IsNullOrWhiteSpace(productDto.Description) &&
               !productDto.Category.HasValue &&
                !productDto.Price.HasValue &&
                 productDto.Image == null &&
                 string.IsNullOrWhiteSpace(productDto.Calories))
            {
                return BadRequest("Không được để giá trị null ở tất cả các trường.");
            }

            var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(productDto.Image.FileName));


            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await productDto.Image.CopyToAsync(stream);
            }

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(tempFilePath),
                PublicId = Guid.NewGuid().ToString(),
            };

            var imageURL = await _clouddinary.UploadAsync(uploadParams);


            System.IO.File.Delete(tempFilePath);

            var newProduct = new Product
            {
                ProName = productDto.ProductName,
                ProDiscription = productDto.Description,
                ProPrice = productDto.Price ?? 0,
                CatId = productDto.Category.Value,
                ProImg = imageURL.Url.ToString(),
                ProCalories = productDto.Calories,
                ProStatus = productDto.Status ?? true
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { proId = newProduct.ProId }, newProduct);
        }




        [HttpGet("getProductById/{proId}")]
        public async Task<ActionResult<ListProductDTO>> GetProductById(int proId)
        {

            if (proId == null)
            {
                return BadRequest("Tham số 'proId' là bắt buộc.");
            }
            var product = await _context.Products
        .Include(c => c.Cat)
        .Where(p => p.ProId == proId)
        .Select(p => new ListProductDTO
        {
            ProductId = p.ProId,
            ProductName = p.ProName,
            Description = p.ProDiscription,
            Price = p.ProPrice,
            Category = p.Cat.CatName,
            Image = p.ProImg,
            Calories = p.ProCalories,
            Status = p.ProStatus
        })
        .FirstOrDefaultAsync();

            if (product == null)
            {
                return Ok(new { Message = $"Không tìm thấy sản phẩm có ID {proId}." });
            }

            return Ok(product);

        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] updateProduct updateProductDto)
        {
            var existingProduct = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(p => p.ProId == id);

            if (existingProduct == null)
            {
                return Ok(new { Message = $"Không tìm thấy sản phẩm." });
            }

            if (!string.IsNullOrEmpty(updateProductDto.ProductName))
            {
                var productWithSameName = await _context.Products
                    .Where(p => p.ProName == updateProductDto.ProductName && p.ProId != id)
                    .FirstOrDefaultAsync();

                if (productWithSameName != null)
                {
                    return BadRequest("Tên sản phẩm đã tồn tại.");
                }

                existingProduct.ProName = updateProductDto.ProductName;
            }

            if (!string.IsNullOrEmpty(updateProductDto.Calories))
            {
                if (int.TryParse(updateProductDto.Calories, out var calories))
                {
                    if (calories <= 0)
                    {
                        return BadRequest("Lượng calo không thể âm.");
                    }
                    existingProduct.ProCalories = calories.ToString();
                }
                else
                {
                    return BadRequest("Giá trị calo không hợp lệ. Vui lòng cung cấp số hợp lệ.");
                }
            }

            if (updateProductDto.Price.HasValue)
            {

                if (updateProductDto.Price.Value <= 0)
                {
                    return BadRequest("Giá không hợp lệ. Vui lòng cung cấp giá hợp lệ.");
                }
                existingProduct.ProPrice = updateProductDto.Price.Value;
            }
            if (updateProductDto.Category.HasValue)
            {

                if (updateProductDto.Category.Value < 0)
                {
                    return BadRequest("Thể loại không hợp lệ. Thể loại không thể là số âm.");
                }

                var category = await _context.Categories.FindAsync(updateProductDto.Category.Value);
                if (category != null)
                {
                    existingProduct.CatId = category.CatId;
                    existingProduct.Cat = category;
                }
                else
                {
                    return BadRequest("Danh mục không hợp lệ.");
                }
            }

            if (updateProductDto.Price.HasValue)
            {
                existingProduct.ProPrice = updateProductDto.Price.Value;
            }

            if (updateProductDto.Image != null)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(updateProductDto.Image.FileName, updateProductDto.Image.OpenReadStream()),
                    PublicId = System.Guid.NewGuid().ToString(),
                };

                var imageResult = await _clouddinary.UploadAsync(uploadParams);
                existingProduct.ProImg = imageResult.Url.ToString();
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
                    return BadRequest("Danh mục không hợp lệ.");
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
                    return Ok(new { Message = $"Không tìm thấy sản phẩm." });
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



        [HttpGet("filter/{categoryName}")]
        public async Task<IActionResult> FilterByCategoryName(string categoryName, int pageNumber = 1, int pageSize = 10)
        {

            var categories = await _context.Categories.ToListAsync();


            var category = categories
                .FirstOrDefault(c => c.CatName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            if (category == null)
            {
                return Ok(new { Message = $"Không tìm thấy danh mục." });
            }

            var totalProducts = await _context.Products.CountAsync(p => p.CatId == category.CatId);

            var products = await _context.Products
                .Include(p => p.Cat)
                .Where(p => p.CatId == category.CatId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ListProductDTO
                {
                    ProductId = p.ProId,
                    ProductName = p.ProName,
                    Description = p.ProDiscription,
                    Price = p.ProPrice,
                    Category = p.Cat.CatName,
                    Image = p.ProImg,
                    Calories = p.ProCalories,
                    Status = p.ProStatus
                })
                .ToListAsync();

            if (products == null || products.Count == 0)
            {

                return Ok(new { Message = $"Không tìm thấy sản phẩm nào trong danh mục này." });
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


        [HttpGet("searchProductName")]
        public async Task<ActionResult<IEnumerable<ListProductDTO>>> SearchByProductName(string? productName = "", int pageNumber = 1, int pageSize = 10)
        {
            var skip = (pageNumber - 1) * pageSize;


            var query = _context.Products.Include(p => p.Cat).AsQueryable();


            if (!string.IsNullOrWhiteSpace(productName))
            {

                query = query.Where(p => p.ProName.ToLower().Contains(productName.ToLower().Trim()));
            }


            var products = await query
                                     .Skip(skip)
                                     .Take(pageSize)
                                     .Select(p => new ListProductDTO
                                     {
                                         ProductId = p.ProId,
                                         ProductName = p.ProName,
                                         Image = p.ProImg,
                                         Description = p.ProDiscription,
                                         Price = p.ProPrice,
                                         Calories = p.ProCalories,
                                         Status = p.ProStatus,
                                         Category = p.Cat.CatName
                                     }).ToListAsync();


            if (products == null || !products.Any())
            {
                return Ok(new { Message = $"Product not found." });
            }


            return Ok(products);
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    var product = await _context.Products.FindAsync(id);
                    if (product == null)
                    {
                        return Ok(new { Message = $"Product not found." });
                    }


                    product.ProStatus = false;


                    var comboDetails = _context.ComboDetails.Where(cd => cd.ProId == id).ToList();
                    if (comboDetails.Any())
                    {
                        _context.ComboDetails.RemoveRange(comboDetails);
                    }

                    var orderDetails = _context.OrderDetails
                     .Include(od => od.Order)
                     .Where(od => od.ProId == id)
                      .ToList();

                    if (orderDetails.Any())
                    {
                        foreach (var orderDetail in orderDetails)
                        {
                            if (orderDetail.Order?.Status == false)
                            {

                                var productInOrder = await _context.Products.FindAsync(orderDetail.ProId);
                                if (productInOrder != null)
                                {
                                    productInOrder.ProStatus = false;
                                    _context.Products.Update(productInOrder);
                                }


                                var comboDetail = _context.ComboDetails.FirstOrDefault(cd => cd.ProId == orderDetail.ProId);
                                if (comboDetail != null)
                                {
                                    var combo = await _context.Combos.FindAsync(comboDetail.ComboId);
                                    if (combo != null)
                                    {
                                        combo.ComboStatus = false;
                                        _context.Combos.Update(combo);
                                    }
                                }
                            }

                        }

                        await _context.SaveChangesAsync();
                    }



                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new { Message = "Trạng thái sản phẩm đã được cập nhật thành sai thành công." });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                    return StatusCode(500, new { Message = "An error occurred.", Details = innerExceptionMessage });
                }
            }
        }






        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetProductDetail(int id)
        {

            if (id <= 0)
            {
                return BadRequest("ID sản phẩm không hợp lệ. ID phải lớn hơn 0.");
            }


            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(p => p.ProId == id);

            if (product == null)
            {
                return Ok(new { Message = $"Không tìm thấy sản phẩm." });
            }


            var relatedProducts = await _context.Products
                .Where(p => p.CatId == product.CatId && p.ProId != id)
                .Select(p => new ListProductDTO
                {
                    ProductId = p.ProId,
                    ProductName = p.ProName,
                    Description = p.ProDiscription,
                    Price = p.ProPrice,
                    Category = p.Cat.CatName,
                    Image = p.ProImg,
                    Calories = p.ProCalories,
                    Status = p.ProStatus
                })
                .ToListAsync();


            var result = new
            {
                ProductDetail = new
                {
                    ProductId = product.ProId,
                    ProductName = product.ProName,
                    Description = product.ProDiscription,
                    Price = product.ProPrice,
                    Category = product.Cat.CatName,
                    Image = product.ProImg,
                    Calories = product.ProCalories,
                    Status = product.ProStatus
                },
                RelatedProducts = relatedProducts
            };

            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> CountPro()
        {
            try
            {

                var totalPro = await _context.Products.CountAsync();

                return Ok(new { TotalCount = totalPro });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("list-combo")]
        public async Task<IActionResult> GetAllProductsForCombo()
        {
            var products = await _context.Products
                .Include(p => p.Cat)
                .Select(p => new ListProductDTO
                {
                    ProductId = p.ProId,
                    ProductName = p.ProName,
                    Description = p.ProDiscription,
                    Price = p.ProPrice,
                    Category = p.Cat.CatName,
                    Image = p.ProImg,
                    Calories = p.ProCalories,
                    Status = p.ProStatus
                })
                .ToListAsync();

            if (products == null || products.Count == 0)
            {
                return Ok(new List<ListProductDTO>());
            }

            return Ok(products);
        }

    }
}
