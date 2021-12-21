using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using Vesuvio.Core;
using Vesuvio.Domain.DTO;
using Vesuvio.Domain.Model;

namespace Vesuvio.Service
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly Guid _userId;
        private readonly ClaimsPrincipal _claimsPrincipal;

        public ProductService(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _claimsPrincipal = httpContextAccessor.HttpContext.User;
            if (_claimsPrincipal.Claims.Where(x => x.Type == "id").Count() > 0)
            {
                _userId = Guid.Parse(_claimsPrincipal.Claims.Where(x => x.Type == "id").FirstOrDefault().Value);
            }
        }

        public int GetAllCount()
        {
            return _uow.GetRepository<Product>().Count();
        }

        public DataTableResponse GetProductsResponse(DataTableRequest request)
        {
            List<ProductResponseDTO> products = GetAllProducts(request);

            return new DataTableResponse
            {
                RecordsTotal = ServiceHelper.GetTotalPageCount<Product>(request, _uow.GetRepository<Product>()),
                Data = products.ToArray()
            };
        }

        public List<ProductResponseDTO> GetAllProducts(DataTableRequest request)
        {
            if (request != null)
            {
                Expression<Func<Product, bool>> queryUser = ServiceHelper.GetSearchQuery<Product>(request);
                Dictionary<Expression<Func<Product, object>>, SortOrder> orderByDictionary = ServiceHelper.OrderBy<Product>(request);
                IQueryable<Product> products = _uow.GetRepository<Product>().Get(queryUser, orderByDictionary, request.PageNumber, request.PageSize);

                List<ProductResponseDTO> finalProducts = products.ToList().Select(product => new ProductResponseDTO().MapToProductDTO(product)).ToList();
                return finalProducts;
            }

            return null;
        }

        public ProductResponseDTO GetProductById(Guid id)
        {
            Product product = _uow.GetRepository<Product>().GetById(id);

            ProductResponseDTO dto = new ProductResponseDTO();

            return dto.MapToProductDTO(product);
        }

        public bool CreateProduct(ProductDTO dto)
        {
            IGenericRepository<Product> repository = _uow.GetRepository<Product>();

            Product product = dto.MapToProduct(dto, new Product(), _userId);

            repository.Add(product);

            if (_uow.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateProduct(ProductDTO dto)
        {
            IGenericRepository<Product> repository = _uow.GetRepository<Product>();

            Product product = repository.GetById(dto.Id);

            product = dto.MapToProduct(dto, product, _userId);

            repository.Update(product);

            if (_uow.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteProducts(List<string> idArray)
        {
            IGenericRepository<Product> repository = _uow.GetRepository<Product>();

            foreach (string id in idArray)
            {
                Guid productId = Guid.Parse(id);
                Product product = repository.GetById(productId);

                if (product != null)
                {

                    product.DeletedDate = DateTime.Now;
                    product.DeletedBy = _userId;

                    repository.Update(product);
                }
            }

            return _uow.SaveInTransaction();
        }


    }
}
