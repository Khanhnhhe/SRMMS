using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMMS.DTOs;
using SRMMS.Models;

[Route("api/catergory")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly SRMMSContext _context;

    public CategoryController(SRMMSContext context)
    {
        _context = context;
    }

    [HttpGet("list")]
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
                                           CatName = c.CatName,
                                           Description = c.Description
                                       }).ToListAsync();

        
        if (categories == null || !categories.Any())
        {
            return NotFound("No categories found.");
        }

        
        return Ok(categories);
    }

    [HttpPost("create")]
    public async Task<ActionResult<ProductCategoriesDTO>> AddCategory(ProductCategoriesDTO categoryDto)
    {
        
        if (await _context.Categories.AnyAsync(c => c.CatName == categoryDto.CatName))
        {
            return BadRequest("A category with this name already exists.");
        }

        
        var newCategory = new Category
        {
            CatName = categoryDto.CatName,
            Description = categoryDto.Description

        };

        
        _context.Categories.Add(newCategory);
        await _context.SaveChangesAsync();

        
        var categoryResult = new ProductCategoriesDTO
        {
            CatId = newCategory.CatId,
            CatName = newCategory.CatName,
            Description = newCategory.Description

        };

        
        return CreatedAtAction(nameof(GetCategoryById), new { catId = newCategory.CatId }, categoryResult);
    }

    [HttpGet("getCategoryById/{catId}")]
    public async Task<ActionResult<ProductCategoriesDTO>> GetCategoryById(int catId)
    {
        var category = await _context.Categories
            .Where(c => c.CatId == catId)
            .Select(c => new ProductCategoriesDTO
            {
                CatId = c.CatId,
                CatName = c.CatName,
                Description = c.Description
                
            })
            .FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpDelete("delete/{catId}")]
    public async Task<IActionResult> DeleteCategoryById(int catId)
    {
 
        var category = await _context.Categories.FindAsync(catId);


        if (category == null)
        {
            return NotFound("Category not found.");
        }

        try
        {
      
            var products = await _context.Products
                .Where(p => p.CatId == category.CatId)
                .ToListAsync();

           
            _context.Products.RemoveRange(products);

          
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            
            return BadRequest("Could not delete the category and its products.");
        }

        
        return NoContent();
    }



    [HttpGet("search")]
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
                                   CatName = c.CatName,
                                   Description = c.Description
                               }).ToListAsync();

        if (categories == null || !categories.Any())
        {
            return NotFound("No categories found.");
        }

        return Ok(categories);
    }

    [HttpPut("update/{catId}")]
    public async Task<IActionResult> UpdateCategory(int catId, ProductCategoriesDTO categoryDto)
    {
        
        var existingCategory = await _context.Categories.FindAsync(catId);
        if (existingCategory == null)
        {
            return NotFound("Category not found.");
        }

        if (!string.IsNullOrEmpty(categoryDto.CatName) &&
            existingCategory.CatName != categoryDto.CatName)
        {
            var categoryWithSameName = await _context.Categories
                .AnyAsync(c => c.CatName == categoryDto.CatName);

            if (categoryWithSameName)
            {
                return BadRequest("A category with this name already exists.");
            }

            existingCategory.CatName = categoryDto.CatName;
        }

        if (!string.IsNullOrEmpty(categoryDto.Description))
        {
            existingCategory.Description = categoryDto.Description;
        }

        
        _context.Entry(existingCategory).State = EntityState.Modified;

        try
        {
           
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
           
            if (!CategoryExists(catId))
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

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(c => c.CatId == id);
    }

}


