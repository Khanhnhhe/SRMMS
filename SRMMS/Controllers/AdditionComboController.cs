using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using SRMMS.DTOs;
using SRMMS.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Account = CloudinaryDotNet.Account;
using Microsoft.EntityFrameworkCore;

namespace SRMMS.Controllers
{
    [ApiController]
    [Route("api/comboProduct")]
    public class AdditionComboController : ControllerBase
    {
        private readonly SRMMSContext _context;
        private readonly Cloudinary _cloudinary;

        public AdditionComboController(SRMMSContext context)
        {
            Account account = new Account("dt92oc9xc", "548166873787419", "-8KA1HUjyTe6J4aHq4DGPXflJiw");
            _context = context;
            _cloudinary = new Cloudinary(account);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateComboWithProducts([FromForm] AdditionComboProductDTO comboDto)
        {

            if (string.IsNullOrWhiteSpace(comboDto.ComboName) || comboDto.ProductNames == null || !comboDto.ProductNames.Any())
            {
                return BadRequest("Tên combo và danh sách sản phẩm là bắt buộc.");
            }

            if (comboDto.ComboImg == null || comboDto.ComboImg.Length == 0)
            {
                return BadRequest("Hình ảnh không tìm thấy.");
            }


            var existingCombo = await _context.Combos.FirstOrDefaultAsync(c => c.ComboName == comboDto.ComboName);
            if (existingCombo != null)
            {
                return Conflict($"Combo với tên '{comboDto.ComboName}' đã tồn tại.");
            }


            var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(comboDto.ComboImg.FileName));

            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await comboDto.ComboImg.CopyToAsync(stream);
            }

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(tempFilePath),
                PublicId = $"combo/{Guid.NewGuid()}"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);


            var newCombo = new Combo
            {
                ComboName = comboDto.ComboName,
                ComboDiscription = comboDto.ComboDescription,
                ComboImg = uploadResult.SecureUrl.ToString(),
                ComboMoney = comboDto.ComboMoney,
                ComboStatus = comboDto.ComboStatus ?? true
            };


            var productIds = new List<int>();
            foreach (var productName in comboDto.ProductNames)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProName == productName);
                if (product != null)
                {
                    productIds.Add(product.ProId);
                }
                else
                {
                    return NotFound($"Sản phẩm '{productName}' không tìm thấy trong cơ sở dữ liệu.");
                }
            }


            _context.Combos.Add(newCombo);
            await _context.SaveChangesAsync();


            foreach (var proId in productIds)
            {
                var comboDetail = new ComboDetail
                {
                    ComboId = newCombo.ComboId,
                    ProId = proId
                };
                _context.ComboDetails.Add(comboDetail);
            }


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi lưu combo: {ex.Message}");
            }

            return Ok(new
            {
                Message = $"Combo '{comboDto.ComboName}' đã được tạo với các sản phẩm.",
                ComboId = newCombo.ComboId,
                ProductNames = comboDto.ProductNames
            });
        }



        [HttpGet("list")]
        public async Task<IActionResult> GetAllCombos(int pageNumber = 1, int pageSize = 10, string? cbName = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var query = _context.Combos.AsQueryable();


            if (!string.IsNullOrWhiteSpace(cbName))
            {
                query = query.Where(c => c.ComboName.Contains(cbName));
            }

            // Tìm kiếm theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(c => c.ComboMoney >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.ComboMoney <= maxPrice.Value);
            }

            var totalCombos = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCombos / pageSize);

            var combos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (combos == null || !combos.Any())
            {
                return NotFound("Không tìm thấy combo nào.");
            }

            var comboDtos = combos.Select(combo => new ListComboProductDTO
            {
                ComboId = combo.ComboId,
                ComboName = combo.ComboName,
                ComboDescription = combo.ComboDiscription,
                ComboImg = combo.ComboImg,
                ComboMoney = combo.ComboMoney,
                ComboStatus = combo.ComboStatus
            }).ToList();

            return Ok(new
            {
                TotalCount = totalCombos,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Combos = comboDtos
            });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCombo(int id, [FromForm] UpdateComboProductDTO updateDto)
        {

            var existingCombo = await _context.Combos.FindAsync(id);
            if (existingCombo == null)
            {
                return NotFound($"Combo với ID '{id}' không tìm thấy.");
            }

            if (!string.IsNullOrWhiteSpace(updateDto.ComboName) &&
                existingCombo.ComboName != updateDto.ComboName)
            {
                var comboExists = await _context.Combos
                    .AnyAsync(c => c.ComboName == updateDto.ComboName);
                if (comboExists)
                {
                    return BadRequest("Tên combo đã tồn tại.");
                }
            }


            var missingProducts = new List<string>();
            foreach (var productName in updateDto.ProductNames)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProName == productName);
                if (product == null)
                {
                    missingProducts.Add(productName);
                }
            }


            if (missingProducts.Any())
            {
                return NotFound($"Các sản phẩm không tìm thấy: {string.Join(", ", missingProducts)}.");
            }


            existingCombo.ComboName = updateDto.ComboName;
            existingCombo.ComboDiscription = updateDto.ComboDescription;
            existingCombo.ComboMoney = updateDto.ComboMoney;
            existingCombo.ComboStatus = updateDto.ComboStatus;


            if (updateDto.ComboImg != null && updateDto.ComboImg.Length > 0)
            {
                var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(updateDto.ComboImg.FileName));
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await updateDto.ComboImg.CopyToAsync(stream);
                }

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(tempFilePath),
                    PublicId = $"combo/{Guid.NewGuid()}"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                existingCombo.ComboImg = uploadResult.SecureUrl.ToString();
            }


            var currentProductDetails = await _context.ComboDetails
                .Where(cd => cd.ComboId == existingCombo.ComboId)
                .ToListAsync();

            _context.ComboDetails.RemoveRange(currentProductDetails);


            foreach (var productName in updateDto.ProductNames)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProName == productName);
                var comboDetail = new ComboDetail
                {
                    ComboId = existingCombo.ComboId,
                    ProId = product.ProId
                };
                _context.ComboDetails.Add(comboDetail);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Combo '{updateDto.ComboName}' đã được cập nhật thành công.",
                ComboId = existingCombo.ComboId
            });
        }

        [HttpDelete("{comboId}")]
        public IActionResult DeleteCombo(int comboId)
        {

            var combo = _context.Combos
                .Include(c => c.ComboDetails)
                .FirstOrDefault(c => c.ComboId == comboId);

            if (combo == null)
            {
                return NotFound();
            }


            if (combo.ComboDetails != null && combo.ComboDetails.Any())
            {
                _context.ComboDetails.RemoveRange(combo.ComboDetails);
            }


            _context.Combos.Remove(combo);


            _context.SaveChanges();

            return NoContent();
        }

        [HttpGet("detail/{comboId}")]
        public async Task<IActionResult> GetComboDetails(int comboId)
        {
            var combo = await _context.Combos
                .Include(c => c.ComboDetails)
                    .ThenInclude(cd => cd.Pro).
                    ThenInclude(p => p.Cat)
                .FirstOrDefaultAsync(c => c.ComboId == comboId);

            if (combo == null)
            {
                return NotFound($"Combo với ID '{comboId}' không tìm thấy.");
            }

            var comboDetailDto = new ComboProductDetailDTO
            {
                ComboId = combo.ComboId,
                ComboName = combo.ComboName,
                ComboDescription = combo.ComboDiscription,
                Products = combo.ComboDetails
                    .Where(cd => cd.Pro != null)
                    .Select(cd => new ProductDetailDTO
                    {
                        ProductId = cd.Pro?.ProId,
                        ProductName = cd.Pro.ProName,
                        Description = cd.Pro.ProDiscription,
                        Price = cd.Pro.ProPrice,
                        Category = cd.Pro.Cat.CatName,
                        Image = cd.Pro.ProImg,
                        Calories = cd.Pro.ProCalories,
                    }).ToList()
            };


            Console.WriteLine($"Số lượng sản phẩm trong combo: {combo.ComboDetails.Count()}");

            return Ok(comboDetailDto);
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchCombos(int pageNumber = 1, int pageSize = 10, string? cbName = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var query = _context.Combos.AsQueryable();


            if (!string.IsNullOrWhiteSpace(cbName))
            {
                cbName = cbName.Trim();
                query = query.Where(c => c.ComboName.Trim().Contains(cbName));
            }

            // Tìm kiếm theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(c => c.ComboMoney >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.ComboMoney <= maxPrice.Value);
            }

            var totalCombos = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCombos / pageSize);

            var combos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (combos == null || !combos.Any())
            {
                return NotFound("Không tìm thấy combo nào.");
            }

            var comboDtos = combos.Select(combo => new ListComboProductDTO
            {
                ComboId = combo.ComboId,
                ComboName = combo.ComboName,
                ComboDescription = combo.ComboDiscription,
                ComboImg = combo.ComboImg,
                ComboMoney = combo.ComboMoney,
                ComboStatus = combo.ComboStatus
            }).ToList();

            return Ok(new
            {
                TotalCount = totalCombos,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Combos = comboDtos
            });
        }

        [HttpGet("count")]
        public async Task<IActionResult> CountCombos()
        {
            try
            {
                
                var totalCombos = await _context.Combos.CountAsync();

                return Ok(new { TotalCount = totalCombos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
