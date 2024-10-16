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
            public async Task<IActionResult> GetAllProducts()
            {
                var products = await _context.Products
                    .Include(p => p.Cat)
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

                return Ok(products);
            }

           
        

    }
}
