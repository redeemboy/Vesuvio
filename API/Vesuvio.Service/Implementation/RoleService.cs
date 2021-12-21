using LinqKit;
using Microsoft.Data.SqlClient;
using Vesuvio.Core;
using Vesuvio.Domain.DTO;
using Vesuvio.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Vesuvio.Service
{
    public class RoleService : IRoleService
    {
        private IUnitOfWork _uow;

        public RoleService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public List<RoleDTO> GetRoles()
        {
            List<ApplicationRole> role = _uow.GetRepository<ApplicationRole>().GetAll().ToList();

            RoleDTO dto = new RoleDTO();

            return dto.MapToRolesDTO(role);
        }

        public List<RoleDTO> GetAllRoles(DataTableRequest request)
        {
            /////

            if (request != null)
            {
                Expression<Func<ApplicationRole, bool>> queryUser = null;

                if (request.Search != null)
                {
                    if (request.Search.Value != string.Empty)
                    {
                        queryUser = PredicateBuilder.New<ApplicationRole>(true);

                        string searchValue = request.Search.Value;

                        queryUser.And(q => q.Name == searchValue || q.NormalizedName == searchValue);

                    }
                }


                Dictionary<Expression<Func<ApplicationRole, object>>, SortOrder> orderByDictionary = new Dictionary<Expression<Func<ApplicationRole, object>>, SortOrder>();

                if (request.Order.Count() > 0)
                {
                    foreach (var order in request.Order)
                    {

                        // as of now generic property binding is giving error therefore we are doing it manually
                        /*PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(ApplicationUser)).Find(order.Column, true);


                        //Expression<Func<ApplicationUser, object>> orderBy = q => q.GetType().GetProperty(order.Column).GetValue(q);
                        Expression<Func<ApplicationUser, object>> orderBy = q => prop.GetValue(q);*/

                        Expression<Func<ApplicationRole, object>> orderBy = q => q.Name;

                        if (order.Column.ToLower() == "normalizedname")
                        {
                            orderBy = q => q.NormalizedName;
                        }

                        if (order.Dir == "desc")
                        {
                            orderByDictionary.Add(orderBy, SortOrder.Descending);
                        }
                        else
                        {
                            orderByDictionary.Add(orderBy, SortOrder.Ascending);
                        }
                    }

                }

                var roles = _uow.GetRepository<ApplicationRole>().Get(queryUser, orderByDictionary, request.PageNumber, request.PageSize).ToList();

                RoleDTO dto = new RoleDTO();

                return dto.MapToRolesDTO(roles);
            }
            /////
            ///

            return null;

        }

        public bool CreateUpdateRole(RoleDTO dto, string operation)
        {

            ApplicationRole role = new ApplicationRole();

            IGenericRepository<ApplicationRole> roleRepository = _uow.GetRepository<ApplicationRole>();

            if (operation == Constants.Operation.Insert)
            {
                role = dto.MapToRole(dto);

                roleRepository.Add(role);

            }
            else
            {

                var updateEntity = roleRepository.Get(x => x.Id == dto.Id).FirstOrDefault();

                dto.MapToRole(dto, updateEntity);

                roleRepository.Update(updateEntity);

            }

            _uow.SaveChanges();

            return true;

        }

        public bool DeleteRole(Guid id)
        {

            IGenericRepository<ApplicationRole> roleRepository = _uow.GetRepository<ApplicationRole>();

            ApplicationRole appRole = roleRepository.GetById(id);
            roleRepository.Delete(appRole);

            _uow.SaveChanges();

            return true;

        }
    }
}
