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
    }
}
