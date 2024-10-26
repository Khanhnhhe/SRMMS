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
                ComboStatus = comboDto.ComboStatus
            };

            _context.Combos.Add(newCombo);
            await _context.SaveChangesAsync();


            foreach (var productName in comboDto.ProductNames)
            {
                var product = _context.Products.FirstOrDefault(p => p.ProName == productName);
                if (product != null)
                {
                    var comboDetail = new ComboDetail
                    {
                        ComboId = newCombo.ComboId,
                        ProId = product.ProId
                    };

                    _context.ComboDetails.Add(comboDetail);
                }
                else
                {
                    return NotFound($"Sản phẩm '{productName}' không tìm thấy trong cơ sở dữ liệu.");
                }
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
        public async Task<IActionResult> GetAllCombos(int pageNumber = 1, int pageSize = 10, string? CbName = null)
        {

            var query = _context.Combos.AsQueryable();


            if (!string.IsNullOrWhiteSpace(CbName))
            {
                query = query.Where(c => c.ComboName.Contains(CbName));
            }


            var totalCombos = await query.CountAsync();


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
                ComboName = combo.ComboName,
                ComboDescription = combo.ComboDiscription,
                ComboImg = combo.ComboImg,
                ComboMoney = combo.ComboMoney,
                ComboStatus = combo.ComboStatus
            }).ToList();


            return Ok(new
            {
                TotalCount = totalCombos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Combos = comboDtos
            });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCombo(int id, [FromForm] UpdateComboProductDTO updateDto)
        {
            // Tìm combo theo ID
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
                if (product != null)
                {
                    var comboDetail = new ComboDetail
                    {
                        ComboId = existingCombo.ComboId,
                        ProId = product.ProId
                    };
                    _context.ComboDetails.Add(comboDetail);
                }
                else
                {
                    return NotFound($"Sản phẩm '{productName}' không tìm thấy trong cơ sở dữ liệu.");
                }
            }


            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Combo '{updateDto.ComboName}' đã được cập nhật thành công.",
                ComboId = existingCombo.ComboId
            });
        }

    }
}
