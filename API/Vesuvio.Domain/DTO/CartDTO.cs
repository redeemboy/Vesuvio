using Vesuvio.Domain.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Vesuvio.Domain.DTO
{
    public class CartDTO
    {
        public Guid? Id { get; set; }
        [Required]
        public Guid? ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }

    public class CartRequestDTO : CartDTO
    {
        public Cart MapToCart(CartRequestDTO dto, Cart cart, Guid userId)
        {
            cart.Id = dto.Id.HasValue ? (Guid)dto.Id : cart.Id;
            cart.ProductId = (Guid)dto.ProductId;
            cart.Quantity = dto.Quantity;
            cart.UserId = userId;

            if (!dto.Id.HasValue)
            {
                cart.CreatedBy = userId;
                cart.CreatedDate = DateTime.Now;
            }
            else
            {
                cart.ModifiedDate = DateTime.Now;
                cart.ModifiedBy = userId;
            }

            return cart;
        }
    }

    public class CartResponseDTO : CartDTO
    {
        public ModelUser CreatedUser { get; set; }
        public ModelUser ModifiedUser { get; set; }
        public Guid? UserId { get; set; }

        public CartResponseDTO MapToCartDTO(Cart cart)
        {
            CartResponseDTO dto = new CartResponseDTO();

            dto.Id = cart.Id;
            dto.ProductId = cart.ProductId;
            dto.Quantity = cart.Quantity;
            dto.UserId = cart.UserId;


            dto.CreatedUser = new ModelUser { UserId = cart.CreatedBy, UserName = cart.CreatedByUser.UserName };

            if (cart.ModifiedByUser != null)
            {
                dto.ModifiedUser = new ModelUser { UserId = cart.ModifiedBy, UserName = cart.ModifiedByUser.UserName };
            }

            return dto;
        }
    }
}