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
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _uow;
        private readonly Guid _userId;
        private readonly ClaimsPrincipal _claimsPrincipal;

        public CartService(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor)
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
            return _uow.GetRepository<Cart>().Count();
        }

        public DataTableResponse GetCartsResponse(DataTableRequest request)
        {
            List<CartResponseDTO> carts = GetAllCarts(request);

            return new DataTableResponse
            {
                RecordsTotal = ServiceHelper.GetTotalPageCount<Cart>(request, _uow.GetRepository<Cart>()),
                Data = carts.ToArray()
            };
        }

        public List<CartResponseDTO> GetAllCarts(DataTableRequest request)
        {
            if (request != null)
            {
                Expression<Func<Cart, bool>> queryUser = ServiceHelper.GetSearchQuery<Cart>(request);
                Dictionary<Expression<Func<Cart, object>>, SortOrder> orderByDictionary = ServiceHelper.OrderBy<Cart>(request);
                IQueryable<Cart> carts = _uow.GetRepository<Cart>().Get(queryUser, orderByDictionary, request.PageNumber, request.PageSize);

                List<CartResponseDTO> finalCarts = carts.ToList().Select(cart => new CartResponseDTO().MapToCartDTO(cart)).ToList();
                return finalCarts;
            }

            return null;
        }

        public CartResponseDTO GetCartById(Guid id)
        {
            Cart cart = _uow.GetRepository<Cart>().GetById(id);

            CartResponseDTO dto = new CartResponseDTO();

            return dto.MapToCartDTO(cart);
        }

        public List<CartResponseDTO> GetUserCart()
        {
            List<Cart> carts = _uow.GetRepository<Cart>().Get(c=> c.UserId == _userId && c.DeletedDate == null).ToList();

            List<CartResponseDTO> finalCarts = carts.Select(cart => new CartResponseDTO().MapToCartDTO(cart)).ToList();

            return finalCarts;
        }

        public bool CreateCart(CartRequestDTO dto)
        {
            IGenericRepository<Cart> repository = _uow.GetRepository<Cart>();

            Cart cart = dto.MapToCart(dto, new Cart(), _userId);

            repository.Add(cart);

            if (_uow.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateCart(CartRequestDTO dto)
        {
            IGenericRepository<Cart> repository = _uow.GetRepository<Cart>();

            Cart cart = repository.GetById(dto.Id);

            cart = dto.MapToCart(dto, cart, _userId);

            repository.Update(cart);

            if (_uow.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteCarts(List<string> idArray)
        {
            IGenericRepository<Cart> repository = _uow.GetRepository<Cart>();

            foreach (string id in idArray)
            {
                Guid cartId = Guid.Parse(id);
                Cart cart = repository.GetById(cartId);

                if (cart != null)
                {

                    cart.DeletedDate = DateTime.Now;
                    cart.DeletedBy = _userId;

                    repository.Update(cart);
                }
            }

            return _uow.SaveInTransaction();
        }


    }
}
