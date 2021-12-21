using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Vesuvio.Domain.Model;

namespace Vesuvio.Domain.DTO
{
    public class OrderModel
    {

        public Guid? Id { get; set; }

        [Required]
        public string OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public string Caption { get; set; }

        public string Status { get; set; }
    }

    public class OrderProductDTO
    {
        public Guid? Id { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }


        public OrderProduct MapToOrderProduct(OrderProductDTO dto, OrderProduct orderProduct, Guid userId, Guid orderId)
        {
            orderProduct.Id = dto.Id.HasValue ? (Guid)dto.Id : orderProduct.Id;

            orderProduct.ProductId = dto.ProductId;
            orderProduct.OrderId = orderId;
            orderProduct.Quantity = dto.Quantity;
            orderProduct.Price = dto.Price;

            if (!dto.Id.HasValue)
            {
                orderProduct.CreatedBy = userId;
                orderProduct.CreatedDate = DateTime.Now;
            }
            else
            {
                orderProduct.ModifiedDate = DateTime.Now;
                orderProduct.ModifiedBy = userId;
            }

            return orderProduct;
        }

        public OrderProductDTO MapToOrderProductDTO(OrderProduct orderProduct)
        {
            OrderProductDTO dto = new OrderProductDTO();

            dto.Id = orderProduct.Id;

            dto.Quantity = orderProduct.Quantity;
            dto.Price = orderProduct.Price;
            dto.ProductId = (Guid)orderProduct.ProductId;

            return dto;
        }
    }

    public class OrderRequestDTO : OrderModel
    {
        public List<OrderProductDTO> Product { get; set; }

        public Order MapToOrder(OrderRequestDTO dto, Order order, Guid userId)
        {
            order.Id = dto.Id.HasValue ? (Guid)dto.Id : order.Id;
            order.BuyerId = userId;

            order.OrderNumber = dto.OrderNumber;
            order.OrderDate = dto.OrderDate;
            order.Caption = dto.Caption;
            order.Status = dto.Status;


            if (!dto.Id.HasValue)
            {
                order.CreatedBy = userId;
                order.CreatedDate = DateTime.Now;
            }
            else
            {
                order.ModifiedDate = DateTime.Now;
                order.ModifiedBy = userId;
            }


            return order;

        }
    }

    public class OrderDTO : OrderModel
    {

        public ModelUser Buyer { get; set; }

        public List<OrderProductDTO> OrderProducts { get; set; }

        public ModelUser CreatedUser { get; set; }

        public Order MapToOrder(OrderDTO dto, Order order, Guid userId)
        {
            order.Id = dto.Id.HasValue ? (Guid)dto.Id : order.Id;
            order.BuyerId = userId;

            order.OrderNumber = dto.OrderNumber;
            order.Caption = dto.Caption;

            order.Status = dto.Status;

            if (!dto.Id.HasValue)
            {
                order.CreatedBy = userId;
                order.CreatedDate = DateTime.Now;
            }
            else
            {
                order.ModifiedDate = DateTime.Now;
                order.ModifiedBy = userId;
            }


            return order;
        }

        public OrderDTO MapToOrderDTO(Order order)
        {
            OrderDTO dto = new OrderDTO();

            dto.Id = order.Id;
            dto.OrderNumber = order.OrderNumber;

            dto.Caption = order.Caption;
            dto.Status = order.Status;
            dto.OrderDate = order.CreatedDate;

            dto.CreatedUser = new ModelUser { UserId = order.CreatedBy, UserName = order.CreatedByUser.UserName };

            return dto;
        }

        public OrderDTO MapToExtendedOrderDTO(Order order, List<OrderProduct> orderProducts)
        {
            OrderDTO dto = this.MapToOrderDTO(order);

            dto.Buyer = new ModelUser { UserId = order.CreatedBy, UserName = order.CreatedByUser.UserName };

            dto.OrderProducts = orderProducts.Select(orderProduct => new OrderProductDTO().MapToOrderProductDTO(orderProduct)).ToList();

            return dto;
        }
    }

    public class OrderPropertyPatchDTO
    {
        public Guid? OrderId { get; set; }
        public string OrderStatus { get; set; }
    }
}
