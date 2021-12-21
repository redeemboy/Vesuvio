using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Vesuvio.Domain.DTO;
using Vesuvio.Service;

namespace Vesuvio.API.Controllers
{

    public class CartController : BaseController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost]
        [Route("getall")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetAll(DataTableRequest request)
        {
             DataTableResponse cartsResponse = _cartService.GetCartsResponse(request);

            if (cartsResponse.Data.Count() > 0)
            {
                return Ok(cartsResponse);
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
            CartResponseDTO cart = _cartService.GetCartById(Id);

            if (cart != null)
            {
                return Ok(cart);
            }
            else return NotFound();
        }

        [HttpGet]
        [Route("getusercart")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetUserCart()
        {
            List<CartResponseDTO> cart = _cartService.GetUserCart();

            if (cart != null)
            {
                return Ok(cart);
            }
            else return NotFound();
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Policy = "Admin")]
        public IActionResult Post(CartRequestDTO cartDTO)
        {
            bool cartCreate = _cartService.CreateCart(cartDTO);
            if (cartCreate) return Ok();
            else return BadRequest();
        }

        [HttpPut]
        [Route("update")]
        [Authorize(Policy = "Admin")]
        public IActionResult Put(CartRequestDTO cartDTO)
        {
            bool cartUpdate = _cartService.UpdateCart(cartDTO);
            if (cartUpdate)
            {
                return Ok();
            }
            else return BadRequest();
        }

        [HttpPatch("delete")]
        [Authorize(Policy = "Admin")]
        public IActionResult Delete([FromBody] List<string> ids)
        {
            bool result = _cartService.DeleteCarts(ids);

            if (result)
            {
                return Ok();
            }
            else return BadRequest();
        }

    }
}