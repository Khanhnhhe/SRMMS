using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMMS.DTOs;
using SRMMS.Models;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly SRMMSContext _context;

    public CategoryController(SRMMSContext context)
    {
        _context = context;
    }

    [HttpGet("listCategories")]
    public async Task<ActionResult<IEnumerable<CategoriesDTO>>> GetCategories(int pageNumber = 1, int pageSize = 10)
    {
        
        var skip = (pageNumber - 1) * pageSize;

        
        var categories = await _context.Categories
                                       .OrderBy(c => c.CatId) 
                                       .Skip(skip) 
                                       .Take(pageSize) 
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

    [HttpPost("addCategory")]
    public async Task<ActionResult<CategoriesDTO>> AddCategory(CategoriesDTO categoryDto)
    {
        
        if (await _context.Categories.AnyAsync(c => c.CatName == categoryDto.CatName))
        {
            return BadRequest("A category with this name already exists.");
        }

        
        var newCategory = new Category
        {
            CatName = categoryDto.CatName
        };

        
        _context.Categories.Add(newCategory);
        await _context.SaveChangesAsync();

        
        var categoryResult = new CategoriesDTO
        {
            CatId = newCategory.CatId,
            CatName = newCategory.CatName
        };

        
        return CreatedAtAction(nameof(GetCategoryById), new { catId = newCategory.CatId }, categoryResult);
    }

    [HttpGet("getCategoryById/{catId}")]
    public async Task<ActionResult<CategoriesDTO>> GetCategoryById(int catId)
    {
        var category = await _context.Categories
            .Where(c => c.CatId == catId)
            .Select(c => new CategoriesDTO
            {
                CatId = c.CatId,
                CatName = c.CatName
            })
            .FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

}


