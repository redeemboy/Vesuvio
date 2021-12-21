using Vesuvio.Domain.DTO;
using Vesuvio.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Vesuvio.API.Controllers
{

    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Route("getall")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetAll(DataTableRequest request)
        {
            DataTableResponse orderResponse = _orderService.GetOrdersResponse(request);

            if (orderResponse.Data.Count() > 0)
            {
                return Ok(orderResponse);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("getbyid/{Id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetById(Guid Id)
        {
            OrderDTO order = _orderService.GetOrderById(Id);

            if (order != null)
            {
                return Ok(order);
            }
            else return NotFound();
        }

        [HttpGet]
        [Route("getcurrentorder")]
        [AllowAnonymous]
        public IActionResult GetCurrentOrder()
        {
            OrderDTO currentOrder = _orderService.GetCurrentOrder();

            if (currentOrder != null)
            {
                return Ok(currentOrder);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Policy = "Customer")]
        public IActionResult Post(OrderRequestDTO orderDTO)
        {
            bool orderCreate = _orderService.CreateOrder(orderDTO);
            if (orderCreate) return Ok();
            else return BadRequest();
        }

        [HttpPatch("updateorderstatus")]
        [Authorize(Policy = "Admin")]
        public IActionResult UpdateOrderStatus([FromBody]OrderPropertyPatchDTO orderPatchObject)
        {
            var result = _orderService.UpdateOrderStatus((Guid)orderPatchObject.OrderId, orderPatchObject.OrderStatus);
            return Ok(result);
        }
    }
}