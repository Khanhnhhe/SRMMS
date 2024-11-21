using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using SRMMS.DTOs;
using SRMMS.Models;

namespace SRMMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TableController : ControllerBase
    {
        private readonly SRMMSContext _context;

        public TableController(SRMMSContext context)
        {
            _context = context;
            
        }

        [HttpPost("/api/table/create")]
        public async Task<IActionResult> CreateTable([FromBody] TableDTO model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Table_Name))
            {
                return BadRequest("Invalid table data.");
            }

            var existingTable = await _context.Tables.FirstOrDefaultAsync(t => t.TableName == model.Table_Name);
            if (existingTable != null)
            {
                return Conflict("Table name already exists.");
            }

            var table = new SRMMS.Models.Table
            {
                TableName = model.Table_Name,
                TableOfPeople = model.TableOfPeople,
                StatusId = 1 
            };

            _context.Tables.Add(table);
            await _context.SaveChangesAsync();

            var result = new TableDTO
            {
                Table_Id = table.TableId,
                Table_Name = table.TableName,
                TableOfPeople = table.TableOfPeople 
            };

            return Created("Table created successfully.", result);
        }

        [HttpPut("/api/table/update/{id}")]
        public async Task<IActionResult> UpdateTable(int id, [FromBody] TableDTO model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Table_Name))
            {
                return BadRequest("BadRequest");
            }

            var table = await _context.Tables.FirstOrDefaultAsync(t => t.TableId == id);
            //if (table == null)
            //{
            //    return NotFound(".");
            //}

            table.TableName = model.Table_Name;
            table.TableOfPeople = model.TableOfPeople;
            table.StatusId = model.StatusId;


            _context.Tables.Update(table);
            await _context.SaveChangesAsync();

            var result = new TableDTO
            {
                Table_Id = table.TableId,
                Table_Name = table.TableName,
                TableOfPeople = table.TableOfPeople,
                StatusId = table.StatusId
            };

            return Ok("Update successfully");
        }

        [HttpGet("/api/table/{id}")]
        public async Task<IActionResult> GetTableById(int id)
        {
            var table = await _context.Tables
                .Where(t => t.TableId == id)
                .Select(t => new ListTableDTO
                {
                    TableId = t.TableId,
                    TableName = t.TableName,
                    StatusName = t.StatusId != null
                        ? _context.StatusTables
                            .Where(s => s.StatusId == t.StatusId)
                            .Select(s => s.StatusName)
                            .FirstOrDefault()
                        : null,
                    BookingId = t.BookingId,
                    TableOfPeople = t.TableOfPeople
                })
                .FirstOrDefaultAsync();

            //if (table == null)
            //{
            //    return NotFound("not found table");
            //}

            return Ok(table);
        }



        //[HttpGet("/api/table/list")]
        //public async Task<IActionResult> GetTables(int? statusId = null, int? tableOfPeople = null, int pageNumber = 1, int pageSize = 10)
        //{
       
        //    var query = _context.Tables.AsQueryable();

        //    if (statusId.HasValue)
        //    {
        //        query = query.Where(t => t.StatusId == statusId.Value);
        //    }

        //    if (tableOfPeople.HasValue)
        //    {
        //        query = query.Where(t => t.TableOfPeople == tableOfPeople.Value);
        //    }

        //    var totalTables = await query.CountAsync();

        //    var skip = (pageNumber - 1) * pageSize;

        //    var tables = await query
        //        .Skip(skip)
        //        .Take(pageSize)
        //        .Select(t => new ListTableDTO
        //        {
        //            TableId = t.TableId,
        //            TableName = t.TableName,
        //            StatusName = t.StatusId != null
        //                ? _context.StatusTables
        //                    .Where(s => s.StatusId == t.StatusId)
        //                    .Select(s => s.StatusName)
        //                    .FirstOrDefault()
        //                : null,
        //            BookingId = t.BookingId,
        //            TableOfPeople = t.TableOfPeople
        //        })
        //        .ToListAsync();

        //    return Ok(new
        //    {
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        TotalTables = totalTables,
        //        Tables = tables
        //    });
        //}

        [HttpGet("/api/table/list")]
        public async Task<IActionResult> GetTables()
        {
            var tables = await _context.Tables
                .Select(t => new ListTableDTO
                {
                    TableId = t.TableId,
                    TableName = t.TableName,
                    StatusId = t.StatusId,
                    StatusName = t.StatusId != null
                        ? _context.StatusTables
                            .Where(s => s.StatusId == t.StatusId)
                            .Select(s => s.StatusName)
                            .FirstOrDefault()
                        : null,

                    BookingId = t.BookingId,
                    TableOfPeople = t.TableOfPeople
                })
                .ToListAsync();

            return Ok(tables);
        }

        [HttpGet("/api/status/list")]
        public async Task<IActionResult> GetStatusList()
        {
            
            var statuses = await _context.StatusTables
                .Select(s => new
                {
                    StatusId = s.StatusId,
                    StatusName = s.StatusName
                })
                .ToListAsync();

            
            return Ok(statuses);
        }


        [HttpDelete("/api/table/delete/{id}")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var table = await _context.Tables.FirstOrDefaultAsync(t => t.TableId == id);
            //if (table == null)
            //{
            //    return NotFound("not found");
            //}

            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();

            return Ok("delete successfully");
        }












    }
}

