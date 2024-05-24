﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Backend.Helpers;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Backend.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersHelper _ordersHelper;

        public OrdersController(IOrdersHelper ordersHelper)
        {
            _ordersHelper = ordersHelper;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(OrderDTO OrderDTO)
        {
            var response = await _ordersHelper.ProcessOrderAsync(User.Identity!.Name!, OrderDTO.Remarks);
            if (response.WasSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);
        }
    }
}

