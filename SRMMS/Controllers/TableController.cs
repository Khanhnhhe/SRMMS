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

            var table = new Table
            {
                TableName = model.Table_Name
            };

            _context.Tables.Add(table);
            await _context.SaveChangesAsync();

            var result = new TableDTO
            {
                Table_Id = table.TableId,
                Table_Name = table.TableName
            };

            return Created("Table created successfully.", result);
        }


        [HttpPost("/api/table/List")]

        public ActionResult<IEnumerable<ListTableDTO>> GetTables()
        {
            var tables = _context.Tables
                .Select(t => new ListTableDTO
                {
                    Table_Id = t.TableId,
                    Table_Name = t.TableName, 
                    QR_Code = t.QrCode, 
                    Acc_Id = t.AccId, 
                    Time_Booking = t.TimeBooking, 
                    Status = t.Status 
                })
                .ToList();

            return Ok(tables);
        }

    }
}

