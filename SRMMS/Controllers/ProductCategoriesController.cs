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

    [HttpGet("listCategoriesOfProduct")]
    public async Task<ActionResult<IEnumerable<ProductCategoriesDTO>>> GetCategories(int pageNumber = 1, int pageSize = 10)
    {
        
        var skip = (pageNumber - 1) * pageSize;

        
        var categories = await _context.Categories
                                       .OrderBy(c => c.CatId) 
                                       .Skip(skip) 
                                       .Take(pageSize) 
                                       .Select(c => new ProductCategoriesDTO
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

    [HttpPost("addCategoriesOfProduct")]
    public async Task<ActionResult<ProductCategoriesDTO>> AddCategory(ProductCategoriesDTO categoryDto)
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

        
        var categoryResult = new ProductCategoriesDTO
        {
            CatId = newCategory.CatId,
            CatName = newCategory.CatName
        };

        
        return CreatedAtAction(nameof(GetCategoryById), new { catId = newCategory.CatId }, categoryResult);
    }

    [HttpGet("getProductCategoryById/{catId}")]
    public async Task<ActionResult<ProductCategoriesDTO>> GetCategoryById(int catId)
    {
        var category = await _context.Categories
            .Where(c => c.CatId == catId)
            .Select(c => new ProductCategoriesDTO
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

    [HttpDelete("deleteProductCategoryById/{catId}")]
    public async Task<IActionResult> DeleteCategoryById(int catId)
    {
        
        var category = await _context.Categories.FindAsync(catId);

        
        if (category == null)
        {
            return NotFound("Category not found.");
        }

        
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        
        return NoContent(); 
    }

    [HttpGet("searchProductCategoryByName")]
    public async Task<ActionResult<IEnumerable<ProductCategoriesDTO>>> SearchCategoryByName(string? categoryName = "", int pageNumber = 1, int pageSize = 10)
    {
        var skip = (pageNumber - 1) * pageSize;

        var query = _context.Categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            query = query.Where(c => c.CatName.Contains(categoryName));
        }

        var categories = await query
                               .OrderBy(c => c.CatId)
                               .Skip(skip)
                               .Take(pageSize)
                               .Select(c => new ProductCategoriesDTO
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


