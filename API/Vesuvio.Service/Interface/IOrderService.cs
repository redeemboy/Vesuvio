using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vesuvio.Domain;
using Vesuvio.Domain.DTO;
using Vesuvio.Domain.Model;
namespace Vesuvio.Service
{
    public interface IOrderService
    {
        DataTableResponse GetOrdersResponse(DataTableRequest request);
        List<OrderDTO> GetAllOrders(DataTableRequest request);
        //List<OrderDTO> GetOrdersByUserId(Guid userId);
        OrderDTO GetOrderById(Guid id);
        OrderDTO GetCurrentOrder();
        Guid GetOrderIdFromOrderNumber(string orderNumber);
        bool CreateOrder(OrderRequestDTO dto);
        bool UpdateOrderStatus(Guid orderId, string orderStatus);
    }
}
