using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMMS.DTOs;
using SRMMS.Models;
namespace SRMMS.Controllers
{
    [Route("api")]
    [ApiController]
    public class InformationRestaurantController : Controller
    {
        private readonly SRMMSContext _context;
        public InformationRestaurantController(SRMMSContext context)
        {
            _context = context;
        }


        [HttpGet("ShowInformationRestaurant")]
        public IActionResult GetRestaurantInformation()
        {
            var restaurantInfo = _context.RestaurantInformations.FirstOrDefault();

            if (restaurantInfo == null)
            {
                return NotFound(new { message = "Restaurant information not found." });
            }

            return Ok(restaurantInfo);
        }


        [HttpPut("UpdateInformationRestaurant")]
        public IActionResult UpdateRestaurantInformation([FromBody] RestaurantInformation updatedInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var restaurantInfo = _context.RestaurantInformations.FirstOrDefault();

            if (restaurantInfo == null)
            {
                return NotFound(new { message = "Restaurant information not found." });
            }

            restaurantInfo.ResName = updatedInfo.ResName;
            restaurantInfo.ResAdress = updatedInfo.ResAdress;
            restaurantInfo.ResFacebook = updatedInfo.ResFacebook;
            restaurantInfo.ResEmail = updatedInfo.ResEmail;
            restaurantInfo.ResPhone = updatedInfo.ResPhone;

            _context.SaveChanges();

            return Ok(new { message = "Restaurant information updated successfully." });
        }

    }
}

