using Vesuvio.Core;
using Vesuvio.DatabaseMigration;
using Vesuvio.Domain.DTO;
using Vesuvio.Domain.Model;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Vesuvio.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;
        private readonly ClaimsPrincipal _claimsPrincipal;
        private readonly Guid _userId;

        public OrderService(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _claimsPrincipal = httpContextAccessor.HttpContext.User;
            if (_claimsPrincipal.Claims.Where(x => x.Type == "id").Count() > 0)
            {
                _userId = Guid.Parse(_claimsPrincipal.Claims.Where(x => x.Type == "id").FirstOrDefault().Value);
            }
        }

        public DataTableResponse GetOrdersResponse(DataTableRequest request)
        {
            List<OrderDTO> items = GetAllOrders(request);

            return new DataTableResponse
            {
                RecordsTotal = ServiceHelper.GetTotalPageCount<Order>(request, _uow.GetRepository<Order>()),
                Data = items.ToArray()
            };
        }

        public List<OrderDTO> GetAllOrders(DataTableRequest request)
        {
            if (request != null)
            {
                Expression<Func<Order, bool>> queryUser = ServiceHelper.GetSearchQuery<Order>(request);
                Dictionary<Expression<Func<Order, object>>, SortOrder> orderByDictionary = ServiceHelper.OrderBy<Order>(request);
                IQueryable<Order> orders = _uow.GetRepository<Order>().Get(queryUser, orderByDictionary, request.PageNumber, request.PageSize);

                List<OrderDTO> finalOrders = orders.ToList().Select(order => new OrderDTO().MapToOrderDTO(order)).ToList();
                return finalOrders;
            }

            return null;
        }

        public OrderDTO GetOrderById(Guid id)
        {
            Order order = _uow.GetRepository<Order>().GetById(id);

            OrderDTO dto = new OrderDTO();

            List<OrderProduct> orderItems = _uow.GetRepository<OrderProduct>().Get(q => q.OrderId == id && q.DeletedDate == null).ToList();

            return dto.MapToExtendedOrderDTO(order, orderItems);
        }

        public OrderDTO GetCurrentOrder()
        {
            Order order = _uow.GetRepository<Order>().GetQuerable(q => q.BuyerId == _userId && q.DeletedDate == null)
                         .OrderByDescending(q => q.CreatedDate).FirstOrDefault();

            if (order != null)
            {
                OrderDTO dto = new OrderDTO();
                dto = dto.MapToOrderDTO(order);

                return dto;
            }

            return null;
        }

        public Guid GetOrderIdFromOrderNumber(string orderNumber)
        {
            return _uow.GetRepository<Order>().GetQuerable(q => q.BuyerId == _userId && q.DeletedDate == null && q.OrderNumber == orderNumber)
                .FirstOrDefault().Id;
        }

        public bool CreateOrder(OrderRequestDTO dto)
        {
            IGenericRepository<Order> repository = _uow.GetRepository<Order>();

            Order order = dto.MapToOrder(dto, new Order(), _userId);
            decimal total = 0;

            total = dto.Product.Select(q => q.Price * q.Quantity).Sum();
            order.TotalAmount = total;

            order.OrderProduct = new List<OrderProduct>();

            dto.Product.ForEach(orderItemDto =>
            {
                OrderProduct orderItem = orderItemDto.MapToOrderProduct(orderItemDto, new OrderProduct(), _userId, order.Id);
                orderItemDto.Price = orderItem.Price = orderItemDto.Price;

                order.OrderProduct.Add(orderItem);

            });

            repository.Add(order);

            return _uow.SaveInTransaction();
        }

        public bool UpdateOrderStatus(Guid orderId, string orderStatus)
        {
            IGenericRepository<Order> repository = _uow.GetRepository<Order>();
            Order order = repository.Get(q => q.Id == orderId && q.DeletedDate == null).FirstOrDefault();

            if (order != null)
            {
                order.Status = orderStatus;
                order.ModifiedDate = DateTime.Now;
                order.ModifiedBy = _userId;

                if (_uow.SaveChanges() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
    }
}
