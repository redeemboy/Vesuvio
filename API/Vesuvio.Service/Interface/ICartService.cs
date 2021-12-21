using System;
using System.Collections.Generic;
using Vesuvio.Domain.DTO;

namespace Vesuvio.Service
{
    public interface ICartService
    {
        int GetAllCount();

        DataTableResponse GetCartsResponse(DataTableRequest request);

        List<CartResponseDTO> GetAllCarts(DataTableRequest request);

        CartResponseDTO GetCartById(Guid id);

        List<CartResponseDTO> GetUserCart();

        bool CreateCart(CartRequestDTO dto);

        bool UpdateCart(CartRequestDTO dto);

        bool DeleteCarts(List<string> idArray);
    }
}
