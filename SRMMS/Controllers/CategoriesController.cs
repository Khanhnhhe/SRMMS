using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMMS.DTOs;
using SRMMS.Models;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly SRMMSContext _context;

    public CategoryController(SRMMSContext context)
    {
        _context = context;
    }

    [HttpGet("/api/listCategories")]
    public async Task<ActionResult<IEnumerable<CategoriesDTO>>> GetCategories()
    {
        
        var categories = await _context.Categories
                                       .Select(c => new CategoriesDTO
                                       {
                                           CatId = c.CatId,
                                           CatName = c.CatName
                                       }).ToListAsync();

        
        if (categories == null || !categories.Any())
        {
            return NotFound("No categories found.");
        }

        
        return Ok(categories);
    }
}
