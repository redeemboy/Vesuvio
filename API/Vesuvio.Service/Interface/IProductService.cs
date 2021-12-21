using System;
using System.Collections.Generic;
using Vesuvio.Domain.DTO;

namespace Vesuvio.Service
{
    public interface IProductService
    {
        DataTableResponse GetProductsResponse(DataTableRequest request);

        List<ProductResponseDTO> GetAllProducts(DataTableRequest request);

        ProductResponseDTO GetProductById(Guid id);

        bool CreateProduct(ProductDTO dto);

        bool UpdateProduct(ProductDTO dto);

        bool DeleteProducts(List<string> idArray);

        int GetAllCount();

    }
}
